using Antlr4.Runtime.Tree;
using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Domain.CodeInfo.MethodSystem;
using Infrastructure.Builders;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static Antlr4.Runtime.Atn.SemanticContext;

namespace Infrastructure.Mediators
{
    public class AntlrMediator : IMediator
    {
        private string _currentNamespace;
        private string _currentClassName;
        private string _currentClassNameWithoutDot => _currentClassName.Substring(0, _currentClassName.Length - 1);
        private string _currentMethodName;
        /// <summary>
        /// Used to know when there are instances with their defined type used in a method and we must identify it
        /// </summary>
        private Dictionary<string, AbstractInstance> _knownInstancesDeclaredInCurrentMethodAnalysis = new Dictionary<string, AbstractInstance>();
        /// This stores the properties declared in a class and refills the _knownInstancesDeclaredInCurrentMethodAnalysis 
        /// everytime it is cleared when a methodAnalysisEnd is reached
        /// </summary>
        private Dictionary<string, AbstractInstance> _propertiesDeclared = new Dictionary<string, AbstractInstance>();
        /// <summary>
        /// Used to set the proprety of the MethodInstance to know the namespaces used and be 
        /// able to dissambiguate between classes with the same name
        /// </summary>
        private List<string> _usedNamespaces = new List<string>();
        private readonly string paramIdentifier = "<p>";

        public void DefineUndefinedMethodInstances()
        {
            // After having the analysis of the entire codebase done, we traverse the methodInstancesWithUndefinedCallsite to be able to define them to start making the diagrams
            for (int j = 0, maxTries = 0; j < MethodInstance.methodInstancesWithUndefinedCallsite.Count && maxTries < 10; j++)
            {
                var methodInstance = MethodInstance.methodInstancesWithUndefinedCallsite[j];
                methodInstance.SolveTypesOfAliases();
                if (j >= MethodInstance.methodInstancesWithUndefinedCallsite.Count - 1 && MethodInstance.methodInstancesWithUndefinedCallsite.Count > 0)
                {
                    j = -1;
                    maxTries++;
                }
            }
        }

        public void ReceiveClassEntityBuilders(List<AbstractBuilder<ClassEntity>> builders)
        {
            // After the code analysis of all files is complete, we start building the ClassEntities and Methods(which are built within the ClassEntityBuilder)
            foreach (var builder in builders)
            {
                builder.Build();
            }
        }
        public void ReceiveMethodBuilders(List<AbstractBuilder<Method>> builders)
        {
        }   

        public void ReceiveNamespace(string? belongingNamespace)
        {
            _currentNamespace = (belongingNamespace == null) ? ("") : (belongingNamespace + ".");
            _usedNamespaces.Add(belongingNamespace);
        }
        public void ReceiveClassNameAndInheritance(string? classAndInheritanceNames)
        {
            _knownInstancesDeclaredInCurrentMethodAnalysis.Clear();
            _propertiesDeclared.Clear();
            string[] classAndInheritanceNamesArray = { classAndInheritanceNames };
            // If the received parameter contains a ",", it means this class has inheritance and we must make Instances inside this class match this info and send this inheritance to the inheritance manager
            if (classAndInheritanceNames.Contains(","))
            {
                classAndInheritanceNamesArray = classAndInheritanceNames.Split(',');
                classAndInheritanceNames = classAndInheritanceNamesArray[0];
            }
            // Send the class to the inheritanceDictionaryManager
            InheritanceDictionaryManager.instance.ReceiveClassDeclaration(classAndInheritanceNamesArray);

            _currentClassName = (classAndInheritanceNames == null) ? ("") : (classAndInheritanceNames + ".");
        }
        public void ReceiveProperties(string type, string identifier)
        {
            type = type.Replace("?", "");
            string propertyInstanceId = identifier;

            // Create the instance of the property and add it to both of the dictionaries
            var propertyInstance = new Instance(propertyInstanceId, type);

            _knownInstancesDeclaredInCurrentMethodAnalysis.Add(identifier, propertyInstance);
            _propertiesDeclared.Add(identifier, propertyInstance);
        }
        public void ReceiveParameters(string type, string identifier)
        {
            var parameterInstance = new Instance(identifier, type);

            // Add the parameter Instance with its identifier inside this method to link with future instances
            // This also adds a string to the identfier to prevent ambiguity when there are porperties with the same identifier
            _knownInstancesDeclaredInCurrentMethodAnalysis.Add(paramIdentifier + identifier, parameterInstance);
        }
        public void ReceiveLocalVariableDeclaration(string assignee, string? assigner, AbstractBuilder<MethodInstance> methodCallAssigner)
        {
            // Create the instance assignee to be defined
            Instance instanceAssignee;

            // If the assigner is a method call, then create the instances and link them
            if (assigner.Contains("("))
            {
                // Create the instance and assign the type of the new instance the return type of the methodCall
                instanceAssignee = new Instance(assignee);
                var returnType = new StringWrapper();
                var methodInstanceAssigner = methodCallAssigner.Build();
                methodInstanceAssigner.refType = returnType;
                instanceAssignee.refType = methodInstanceAssigner.refType;
            }
            // If the assigner is the element of an indexed collection, create the instance and assign the kind of element from a collection
            else if (assigner.Contains("["))
            {
                // instanceAssignee.MakeInstanceInformationBasedFromString(assigner);
                string assignerWithoutBrackets = assigner.Substring(0, assigner.IndexOf("["));
                instanceAssignee = new Instance(assignee);
                instanceAssignee.kind = KindOfInstance.IsElementFromCollection;

                // Check in the known instances the assigner without the brackets and make the link between these 2 instances
                AbstractInstance knownAssignerInstance = _knownInstancesDeclaredInCurrentMethodAnalysis[assignerWithoutBrackets];
                instanceAssignee.refType = knownAssignerInstance.refType;
            }
            // If the declaration is simple
            else
            {
                // Make the new instance and link it to an existing instance
                instanceAssignee = new Instance(assignee);

                // Check in the known instances the assigner and make the link between these 2 instances
                if (_knownInstancesDeclaredInCurrentMethodAnalysis.TryGetValue(assigner, out AbstractInstance knownAssignerInstance))
                {
                    instanceAssignee.refType = knownAssignerInstance.refType;
                }
                // If unknown, then the assigner must be a property from a parent class, and then we must add this assignment to the instancesDictionary
                else
                {
                    // Creating the Instance of the unknown assigner
                    var unknownAssignerInstance = new Instance(_currentNamespace + _currentClassName + _currentMethodName + assigner);
                }

            }

            // Add the new instance to the known instances dictionary
            _knownInstancesDeclaredInCurrentMethodAnalysis.Add(assignee, instanceAssignee);

            return;
        }
        public void ReceiveMethodAnalysisEnd()
        {
            // Refilling the common dictionary with the custom identifiers while erasing the ones that are not needed anymore
            _knownInstancesDeclaredInCurrentMethodAnalysis.Clear();
            foreach (var property in _propertiesDeclared)
            {
                _knownInstancesDeclaredInCurrentMethodAnalysis.Add(property.Key, property.Value);
            }
        }

        public void ReceiveMethodCall(AbstractBuilder<MethodInstance> methodCallBuilder)
        {
            methodCallBuilder.Build();
        }
        public void ReceiveUsedNamespaces(List<string>? usedNamespaces)
        {
            _usedNamespaces = (usedNamespaces is null) ? (_usedNamespaces) : (usedNamespaces);
        }
        /// <summary>
        /// Generate the whole propertyChain from the provided strings, and it avoids to put inside 
        /// the propertyChain the class that owns the propertyChain
        /// </summary>
        /// <param name="ownerName">The name of the Instance that will be the
        /// head of this propertyChain</param>
        /// <param name="propertyChainString">The text that represents the whole propertyChain as 
        /// a raw string, which will be converted into multiple chained AbstractInstances</param>
        /// <returns>The first Instance that has all the other properties chained</returns>
        public AbstractInstance GeneratePropertyChain(string ownerName, string propertyChainString)
        {
            var methodInstancePropertyChainString = propertyChainString.Split(".");
            AbstractInstance previousInstance = null;
            AbstractInstance firstChainedInstance = null;
            foreach (string componentString in methodInstancePropertyChainString)
            {
                // If the current component isn't the caller class, then it is part of the propertyChain
                if (componentString != ownerName)
                {
                    var component = new Instance(componentString);
                    component.kind = KindOfInstance.IsPropertyFromInheritanceOrInThisClass;
                    // If the previous component is null, then this is the first element of the propertyChain which will be received by the MethodInstance constructor
                    if (previousInstance is null)
                        firstChainedInstance = component;
                    // If not then we need to chain all the properties properly
                    else
                        previousInstance.chainedInstance = component;
                    previousInstance = component;
                }
            }
            return firstChainedInstance;
        }

        /// <summary>
        /// Create a MethodInstance according to the MethodCallData
        /// If the MethodInstance to be created is part of a MethodCallChain, then we can
        /// link the respective MethodInstance caller with the called MethodInstance(which 
        /// is the MehtodInstance to be created)
        /// </summary>
        /// <param name="callData">The MethodCall data to be used to generate the MethodInstnace</param>
        /// <param name="callerMethodInstance">The MethodInstance that has called the 
        /// MethodInstance that is going to be generated</param>
        /// <returns></returns>
        public MethodInstance GenerateMethodInstance(MethodCallData callData, AbstractInstance? callerMethodInstance = null)
        {
            var (calledClassName, calledMethodName, calledParameters, propertyChainString, linkedMethodBuilder, isConstructor) = callData;
            AbstractInstance? linkedClassOrParameterInstance = null;
            List<AbstractInstance> calledParametersInstances = new();
            KindOfInstance methodInstanceKind = KindOfInstance.Normal;
            AbstractInstance classCallerChainedInstance = null;
            AbstractInstance methodInstanceChainedInstance = null;

            // If the calledClassName has "." or there is a linkedMethodCaller, then this caller class has a property chain and we must separate it from the starting class and all the other components in this chain, and for each component we create an Instance of kind PropertyFromInheritanceOrThisClass
            if ((calledClassName != null && calledClassName.Contains(".")) || callerMethodInstance is not null)
            {
                // If there is a callerMethodInstance, then the calledClass is part of the propertyChain and it must be specified to the GeneratePropertyChain function, otherwise then it is not part of the chain
                string classOwner = calledClassName.Split(".")[0];
                if (callerMethodInstance is not null)
                    classOwner = "";

                classCallerChainedInstance = GeneratePropertyChain(classOwner, calledClassName);
                calledClassName = classOwner;
            }

            // Generate the propertyChain of the MehtodInstance
            methodInstanceChainedInstance = GeneratePropertyChain("", propertyChainString);

            // Adding at the end the className instance if there is no callerMethodInstance, if there is then we must not add the calledClassName since it actually is the callerMethodInstance
            if (callerMethodInstance is null)
            {
                calledParameters.Add(calledClassName);
            }

            // Process each parameter and class name according to what it is(based on its position in the List, all but the last one are parameters, the last element is the class name)
            int calledParametersCount = calledParameters.Count;
            for (int i = 0; i < calledParametersCount; i++)
            {
                if (calledParameters[i] is string)
                {
                    string currentStringInstance = (string)calledParameters[i];
                    string currentStringInstancePropertyChain = "";

                    // Check if there are parameters that are a propertyChain, and if so, mark its kind and set the information for the MethodInstance to solve their types
                    if (currentStringInstance.Contains("."))
                    {
                        currentStringInstancePropertyChain = currentStringInstance;
                        currentStringInstance = currentStringInstance.Split(".")[0];
                    }

                    linkedClassOrParameterInstance = new Instance(currentStringInstance);
                    linkedClassOrParameterInstance.inheritedClasses = null;
                    //===========================  Make the analysis just as usual
                    // If this is the class name and it is a constructor, then mark the kind of the MethodInstance and set the data to match the actual Method later
                    if (i == calledParametersCount - 1 && isConstructor)
                    {
                        linkedClassOrParameterInstance.kind = KindOfInstance.IsConstructor;
                        methodInstanceKind = KindOfInstance.IsConstructor;
                        break;
                    }
                    // If the current instance is an element from an indexed list, then remove the part with brackets and look for the collection type in the known instances
                    else if (currentStringInstance.Contains("["))
                    {
                        currentStringInstance = currentStringInstance.Substring(0, currentStringInstance.IndexOf("["));
                        if (_knownInstancesDeclaredInCurrentMethodAnalysis.TryGetValue(currentStringInstance, out AbstractInstance knownClassInstance))
                        {
                        linkedClassOrParameterInstance.refType = knownClassInstance.refType;
                        linkedClassOrParameterInstance.kind = KindOfInstance.IsElementFromCollection;
                        }
                    }
                    // If we already registered an instance with the same name of the className or parameter, then we link that instance to this method
                    else if (_knownInstancesDeclaredInCurrentMethodAnalysis.TryGetValue(currentStringInstance, out AbstractInstance knownClassInstance))
                    {
                        linkedClassOrParameterInstance.refType = knownClassInstance.refType;
                        linkedClassOrParameterInstance.kind = knownClassInstance.kind;
                    }
                    // Check again but adding the parameter identifier if this instance was a parameter
                    else if (_knownInstancesDeclaredInCurrentMethodAnalysis.TryGetValue(paramIdentifier + currentStringInstance, out AbstractInstance knownClassInstanceParam))
                    {
                        linkedClassOrParameterInstance.refType = knownClassInstanceParam.refType;
                        linkedClassOrParameterInstance.kind = knownClassInstanceParam.kind;
                    }
                    // If that component wasn't in that dictionary, isn't empty and isn't the "this" nor "base" keyword, then this instance may come from a property of a parent class or is a static property and we must set that state using the HasClassNameStaticOrParentProperty kind
                    else if (!String.IsNullOrEmpty(currentStringInstance) && currentStringInstance != "this" && currentStringInstance != "base")
                    {
                        linkedClassOrParameterInstance.kind = KindOfInstance.IsPropertyFromInheritanceOrInThisClass;
                        // We set the inheritedClasses of the component to the inheritance of this current class we are analyzing since an inherited class may contain this component as its property
                        linkedClassOrParameterInstance.inheritedClasses = InheritanceDictionaryManager.instance.inheritanceDictionary[_currentClassNameWithoutDot];
                        methodInstanceKind = KindOfInstance.HasClassNameStatic;
                    }
                    // If this is not empty and is the "this" and isn't the "base" keyword, then this component is of type of the class we are analyzing
                    else if (!String.IsNullOrEmpty(currentStringInstance) && currentStringInstance == "this")
                    {
                        linkedClassOrParameterInstance.refType.data = _currentClassNameWithoutDot;
                    }
                    // If this is the called class name component, is empty or is the "this" or "base" keyword, then this method uses inheritance to get its type and we set this inheritance information
                    else if (String.IsNullOrEmpty(currentStringInstance) || currentStringInstance == "this" || currentStringInstance == "base")
                    {
                        linkedClassOrParameterInstance.kind = KindOfInstance.IsInheritedOrInThisClass;
                        methodInstanceKind = KindOfInstance.IsInheritedOrInThisClass;
                        linkedClassOrParameterInstance.refType.data = _currentClassNameWithoutDot;
                    }

                    // If this iteration covers the properties(i. e. isn't the last element, which is the caller class, but if the caller class is another MethodCall then all the elements are parameters) then add the instance to the parameters list of the MethodInstance to be created
                    if ((i < calledParametersCount - 1 || callerMethodInstance is not null) && linkedClassOrParameterInstance is not null)
                    {
                        calledParametersInstances.Add(linkedClassOrParameterInstance);
                    }
                    // After the current Instance has been properly set, then generate its propertyChain
                    if (!String.IsNullOrEmpty(currentStringInstancePropertyChain))
                        linkedClassOrParameterInstance.chainedInstance = GeneratePropertyChain(currentStringInstance, currentStringInstancePropertyChain);
                }
                // If the parameter to manage is another MethodCall, then generate the Data and link the generated MethodInstance to this MethodCall
                else if (calledParameters[i] is MethodCallData)
                {
                    var parameterMethodCall = GenerateMethodInstance((MethodCallData)calledParameters[i]);
                    calledParametersInstances.Add(parameterMethodCall);
                }
                else
                {
                    throw new Exception("The instance received isn't either a string nor a MethodCallData");
                }
            }

            // If there is a linked methodCall caller(which means that this MethodInstance caller is another MethodInstance), then we link the data accordingly
            if (callerMethodInstance is not null)
                linkedClassOrParameterInstance = callerMethodInstance;

            // Adding the propertyChain to the class caller IF the caller class is not another MethodInstance
            if (callerMethodInstance is null)
                linkedClassOrParameterInstance.chainedInstance = classCallerChainedInstance;

            // Make the callsite for the method
            var callsite = new Callsite(null);
            linkedMethodBuilder.AddCallsite(callsite);

            // Put the MethodInstance created in a property to be passed to the ReceiveLocalVariableDeclaration
            var methodInstance = new MethodInstance(linkedClassOrParameterInstance, methodInstanceChainedInstance, calledMethodName, calledParametersInstances, callsite, methodInstanceKind, _usedNamespaces);
            // If this methodCall is inherited or in this class, then we set the inheritedClass of this component since it needs the inheritance of the current class to know where this came from
            methodInstance.SetInheritance(InheritanceDictionaryManager.instance.inheritanceDictionary[_currentClassNameWithoutDot]);
         
            return methodInstance;
        }
        public ReadOnlyDictionary<string, AbstractInstance> GetKnownInstancesDeclaredInCurrentMethodAnalysis()
        {
            return this._knownInstancesDeclaredInCurrentMethodAnalysis.AsReadOnly();
        }
        public string GetCurrentAnalyzedClassName()
        {
            return _currentClassNameWithoutDot;
        }
        public List<string> GetUsedNamespaces()
        {
            return _usedNamespaces;
        }

    }
}   
