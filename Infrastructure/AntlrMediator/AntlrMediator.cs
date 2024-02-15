using Antlr4.Runtime.Tree;
using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Infrastructure.Builders;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
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
        /// This is used first by the method ReceiveMethodCall, which creates the MethodInstance 
        /// AND Callsite made form the method currently analyzed, and sets this property with 
        /// the MethodInstance created, which is then read by the ReceiveLocalVariableDeclaration, 
        /// to be able to manage it and set it null IF the method was used in an assignment and send 
        /// it to the instancesManager, if the methodCall did not assign anything to any variable, 
        /// then ReceiveLocalVariableDeclaration won't read this property and must be 
        /// managed the next time ReceiveMethodCall is called
        /// </summary>
        private MethodInstance? _currentMethodCallInstance = null;

        public void CheckMethodInstanceWasHandled()
        {
            if (_currentMethodCallInstance is not null)
            {
                // Send the MethodInstance to the instancesDictionary
                InstancesDictionaryManager.instance.AddMethodCallInstance(_currentMethodCallInstance);
                _currentMethodCallInstance = null;
            }
        }

        public void ReceiveClassEntityBuilders(List<AbstractBuilder<ClassEntity>> builders)
        {
        }
        public void ReceiveMethodBuilders(List<AbstractBuilder<Method>> builders)
        {
        }

        public void ReceiveNamespace(string? belongingNamespace)
        {
            _currentNamespace = (belongingNamespace == null) ? ("") : (belongingNamespace + ".");
        }
        public void ReceiveClassNameAndInheritance(string? classAndInheritanceNames)
        {
            CheckMethodInstanceWasHandled();

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
            string propertyInstanceId = _currentNamespace + _currentClassName + identifier;

            // Create the instance of the property and add it to both of the dictionaries
            var propertyInstance = new Instance(propertyInstanceId, type);

            // Since this is a property, we must fill its inheritanceList to be able to recognize the usage of this property in child classes
            // If the received class has inheritance, pass the inheritance info to the instance
            propertyInstance.inheritedClasses = InheritanceDictionaryManager.instance.inheritanceDictionary[_currentClassNameWithoutDot ];

            InstancesDictionaryManager.instance.AddInstanceWithDefinedType(propertyInstance);
            _knownInstancesDeclaredInCurrentMethodAnalysis.Add(identifier, propertyInstance);
            _propertiesDeclared.Add(identifier, propertyInstance);
        }
        public void ReceiveParameters(string type, string identifier)
        {
            var parameterInstance = new Instance(identifier, type);
            // Send the parameter to the instancesManager
            // InstancesDictionaryManager.instance.AddInstanceWithDefinedType(parameterInstance);

            // Add the parameter Instance with its identifier inside this method to link with future instances
            // _knownInstancesDeclaredInCurrentMethodAnalysis.Add(identifier, parameterInstance);
        }
        public void ReceiveLocalVariableDeclaration(string assignee, string assigner)
        {
            // Create the instance assignee to be defined
            Instance instanceAssignee;

            // If the assigner is a method call...
            if (assigner.Contains("("))
            {
                // Get the _currentMethodCallInstance which contains the methodCall assigner already processed and add the assignment to the instanceDictionary
                if(_currentMethodCallInstance is null)
                {
                    throw new NullReferenceException("The property '_currentMethodCallInstance' is null when it should never be null in this case");
                }

                // Create the instance and assign the type of the new instance the return type of the methodCall
                instanceAssignee = new Instance(assignee);
                instanceAssignee.type = _currentMethodCallInstance.returnType;
                // InstancesDictionaryManager.instance.AddMethodAssignment(instanceAssignee, _currentMethodCallInstance);
             
                // After handling the methodCall instance, we clean the property
                _currentMethodCallInstance = null;
            }
            // IF the declaration is simple
            else
            {
                // Make the new instance and link it to an existing instance
                instanceAssignee = new Instance(assignee);

                // Check in the known instances the assigner and make the link between these 2 instances
                if (_knownInstancesDeclaredInCurrentMethodAnalysis.TryGetValue(assigner, out AbstractInstance knownAssignerInstance))
                {
                    instanceAssignee.type = knownAssignerInstance.type;
                }
                // If unknown, then the assigner must be a property from a parent class, and then we must add this assignment to the instancesDictionary
                else
                {
                    // Creating the Instance of the unknown assigner
                    var unknownAssignerInstance = new Instance(_currentNamespace + _currentClassName + _currentMethodName + assigner);
                    unknownAssignerInstance.inheritedClasses = InheritanceDictionaryManager.instance.inheritanceDictionary[_currentClassNameWithoutDot ];

                    // Handle to the instancesManager the unknown assignment 
                    InstancesDictionaryManager.instance.AddSimpleAssignment(instanceAssignee, unknownAssignerInstance);
                }

                // Add the new instance to the known instances dictionary and the instancesDictionary
                _knownInstancesDeclaredInCurrentMethodAnalysis.Add(assignee, instanceAssignee);
            }

            // Add the new instance to the known instances dictionary
            _knownInstancesDeclaredInCurrentMethodAnalysis.Add(assignee, instanceAssignee);

            return;
        }
        public void ReceiveMethodAnalysisEnd()
        {
            CheckMethodInstanceWasHandled();

            // Refilling the common dictionary with the custom identifiers while erasing the ones that are not needed anymore
            _knownInstancesDeclaredInCurrentMethodAnalysis.Clear();
            foreach (var property in _propertiesDeclared)
            {
                _knownInstancesDeclaredInCurrentMethodAnalysis.Add(property.Key, property.Value);
            }
        }
        public void ReceiveMethodCall(string calledClassName, string calledMethodName, List<string>? calledParameters, MethodBuilder linkedMethodBuilder)
        {
            // Check the property which contains the MethodInstance this method will put in there, if this is not null, then it means the RecieveLocalVariableDeclaration did not catch that since this was a pure methodCall without assigning any variable anything, and we must manage it ourselves, we must then put this MethodInstance to the InstancesManager to let that MethodCall be identifiable if it isn't identifiable
            CheckMethodInstanceWasHandled();

            //===========================  Get the components of this methodCall(methodName, className, properties) and get the linked instances for the components
            AbstractInstance? linkedClassNameInstance = null;
            List<AbstractInstance> linkedParametersNameInstance = new();

            // If we already registered an instance with the same name of the className, then we link that instance to this method
            if (_knownInstancesDeclaredInCurrentMethodAnalysis.TryGetValue(calledClassName, out AbstractInstance classInstance))
            {
                linkedClassNameInstance = classInstance;
            }
            // If that component wasn't in that dictionary AND it isn't emtpy, then this must be a property from this class or a inherited clas and we must add the data to later find out the linked instance and its type
            else if (!String.IsNullOrEmpty(calledClassName))
            {
                linkedClassNameInstance = new Instance(_currentNamespace + _currentClassName + _currentMethodName + calledClassName);
                linkedClassNameInstance.inheritedClasses = null;
            }

            int calledParametersCount = (calledParameters is null) ? (0) : (calledParameters.Count);
            // Now we pass through each parameter and do the same process
            for (global::System.Int32 j = 0; j < calledParametersCount; j++)
            {
                string parameterAlias = calledParameters[j];
                if (_knownInstancesDeclaredInCurrentMethodAnalysis.TryGetValue(parameterAlias, out AbstractInstance knownInstance))
                {
                    linkedParametersNameInstance.Add(knownInstance);
                }
                else
                {
                    var linkedParameterInstance = new Instance(_currentNamespace + _currentClassName + _currentMethodName + parameterAlias);
                    linkedParameterInstance.inheritedClasses = null;
                    linkedParametersNameInstance.Add(linkedParameterInstance);
                }
            }
            //======

            // Make the callsite for the method
            var callsite = new Callsite(null);
            linkedMethodBuilder.AddCallsite(callsite);

            // Put the MethodInstance created in a property to be passed to the ReceiveLocalVariableDeclaration
            _currentMethodCallInstance = new MethodInstance(linkedClassNameInstance, calledMethodName, linkedParametersNameInstance, callsite, true);
            _currentMethodCallInstance.inheritedClasses = InheritanceDictionaryManager.instance.inheritanceDictionary[_currentClassNameWithoutDot ];
        }

        public void ReceiveUsedNamespaces(List<string>? usedNamespaces)
        {
        }
    }
}   
