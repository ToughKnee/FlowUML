using Antlr4.Runtime.Tree;
using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Domain.CodeInfo.MethodSystem;
using Infrastructure.Builders;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

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

        public void DefineUndefinedMethodInstances()
        {
            // After having the analysis of the entire codebase done, we traverse the methodInstancesWithUndefinedCallsite to be able to define them to start making the diagrams
            for (int j = 0, maxTries = 0; j < MethodInstance.methodInstancesWithUndefinedCallsite.Count && maxTries < 10; j++)
            {
                var methodInstance = MethodInstance.methodInstancesWithUndefinedCallsite[j];
                methodInstance.SolveTypesOfAliases();
                if (j == MethodInstance.methodInstancesWithUndefinedCallsite.Count - 1 && MethodInstance.methodInstancesWithUndefinedCallsite.Count > 0)
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
            _knownInstancesDeclaredInCurrentMethodAnalysis.Add(identifier, parameterInstance);
        }
        public void ReceiveLocalVariableDeclaration(string assignee, string? assigner, List<MethodCallData> methodCallAssigner)
        {
            // Create the instance assignee to be defined
            Instance instanceAssignee;

            // If the assigner is a method call...
            if (assigner.Contains("("))
            {
                // Create the instance and assign the type of the new instance the return type of the methodCall
                instanceAssignee = new Instance(assignee);
                var returnType = new StringWrapper();
                var methodInstanceAssigner = GenerateMethodInstance(methodCallAssigner[0]);
                methodInstanceAssigner.refType = returnType;
                instanceAssignee.refType = methodInstanceAssigner.refType;
            }
            // IF the declaration is simple
            else
            {
                // Make the new instance and link it to an existing instance
                instanceAssignee = new Instance(assignee);

                // Check in the known instances the assigner and make the link between these 2 instances
                if (_knownInstancesDeclaredInCurrentMethodAnalysis.TryGetValue(assigner, out AbstractInstance knownAssignerInstance))
                {
                    instanceAssignee.refType.data = knownAssignerInstance.type;
                }
                // If unknown, then the assigner must be a property from a parent class, and then we must add this assignment to the instancesDictionary
                else
                {
                    // Creating the Instance of the unknown assigner
                    var unknownAssignerInstance = new Instance(_currentNamespace + _currentClassName + _currentMethodName + assigner);
                }

                // Add the new instance to the known instances dictionary and the instancesDictionary
                //_knsownInstancesDeclaredInCurrentMethodAnalysis.Add(assignee, instanceAssignee);
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

        public void ReceiveMethodCall(List<MethodCallData> methodCallData)
        {
            GenerateMethodInstance(methodCallData[0]);
        }

        public void ReceiveUsedNamespaces(List<string>? usedNamespaces)
        {
            _usedNamespaces = (usedNamespaces is null) ? (_usedNamespaces) : (usedNamespaces);
        }
 
        public MethodInstance GenerateMethodInstance(MethodCallData callData)
        {
            var (calledClassName, calledMethodName, calledParameters, linkedMethodBuilder, isConstructor) = callData;
            //===========================  Get the components of this methodCall(methodName, className, properties) and get the linked instances for the components
            AbstractInstance? linkedClassOrParameterInstance = null;
            List<AbstractInstance> linkedParametersNameInstance = new();
            KindOfInstance instanceKind = KindOfInstance.Normal;

            // Adding at the end the className instance
            if (calledParameters is null)
            {
                calledParameters = new List<string> { calledClassName };
            }
            else
            {
                calledParameters.Add(calledClassName);
            }

            // Process each parameter and class name according to what it is(based on its position in the List, all but the last one are parameters, the last element is the class name)
            int calledParametersCount2 = (calledParameters is null) ? (0) : (calledParameters.Count);
            for (int i = 0; i < calledParametersCount2; i++)
            {
                string currentStringInstance = calledParameters[i];

                linkedClassOrParameterInstance = new Instance(currentStringInstance);
                linkedClassOrParameterInstance.inheritedClasses = null;
                //===========================  Make the analysis just as usual
                // TODO: Make the case when the class name or parameter is another MethodCall
                // If this is the class name and it is a constructor, then mark the kind of the MethodInstance and set the data to match the actual Method later
                if (i == calledParametersCount2 - 1 && isConstructor)
                {
                    linkedClassOrParameterInstance.kind = KindOfInstance.IsConstructor;
                    instanceKind = KindOfInstance.IsConstructor;
                    break;
                }
                // If we already registered an instance with the same name of the className or parameter, then we link that instance to this method
                if (_knownInstancesDeclaredInCurrentMethodAnalysis.TryGetValue(currentStringInstance, out AbstractInstance knownClassInstance))
                {
                    linkedClassOrParameterInstance = knownClassInstance;
                }
                // If that component wasn't in that dictionary, isn't empty and isn't the "this" nor "base" keyword, then this instance may come from a property of a parent class or is a static method and we must set that state using the HasClassNameStaticOrParentProperty enum
                else if (!String.IsNullOrEmpty(currentStringInstance) && currentStringInstance != "this" && currentStringInstance != "base")
                {
                    linkedClassOrParameterInstance.kind = KindOfInstance.HasClassNameStaticOrParentProperty;
                    // We set the inheritedClasses of the component to the inheritance of this current class we are analyzing since an inherited class may contain this component as its property
                    linkedClassOrParameterInstance.inheritedClasses = InheritanceDictionaryManager.instance.inheritanceDictionary[_currentClassNameWithoutDot];
                    instanceKind = KindOfInstance.HasClassNameStaticOrParentProperty;
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
                    instanceKind = KindOfInstance.IsInheritedOrInThisClass;
                }

                // If this iteration covers the properties then add the instance to the parameters list of the MethodInstance to be created
                if (i < calledParametersCount2 - 1 && linkedClassOrParameterInstance is not null)
                {
                    linkedParametersNameInstance.Add(linkedClassOrParameterInstance);
                }

                //======

            }
            //======

            // Make the callsite for the method
            var callsite = new Callsite(null);
            linkedMethodBuilder.AddCallsite(callsite);

            // Put the MethodInstance created in a property to be passed to the ReceiveLocalVariableDeclaration
            var methodInstance = new MethodInstance(linkedClassOrParameterInstance, calledMethodName, linkedParametersNameInstance, callsite, instanceKind, _usedNamespaces);
            // If this methodCall is inherited or in this class, then we set the inheritedClass of this component since it needs the inheritance of the current class to know where this came from
            if (instanceKind == KindOfInstance.IsInheritedOrInThisClass)
            {
                methodInstance.inheritedClasses = InheritanceDictionaryManager.instance.inheritanceDictionary[_currentClassNameWithoutDot];
            }
         
            return methodInstance;
        }
    }
}   
