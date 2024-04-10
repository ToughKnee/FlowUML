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
