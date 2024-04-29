using Antlr4.Runtime.Tree;
using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Domain.CodeInfo.MethodSystem;
using Infrastructure.Builders;
using System;
using System.Collections.ObjectModel;

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
                methodInstance.SolveTypesOfComponents();
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
        public void ReceiveLocalVariableDefinition(string assignee, string? assigner, AbstractBuilder<AbstractInstance>? instanceAssignerBuilder)
        {
            // Create the instance assignee to be defined
            AbstractInstance instanceAssignee;

            // If the assigner is a method call, then create the instances and link them
            if (assigner.Contains("("))
            {
                // Create the instance and assign the type of the new instance the return type of the methodCall
                instanceAssignee = new Instance(assignee);
                var returnType = new StringWrapper();
                var methodInstanceAssigner = ((MethodInstanceBuilder)instanceAssignerBuilder).Build();
                methodInstanceAssigner.refType = returnType;
                instanceAssignee.refType = methodInstanceAssigner.refType;

                // This is done to add the actual Instance in the knownInstances dictionary
                instanceAssignee = methodInstanceAssigner;
            }
            // If the declaration is simple
            else
            {
                // Check first if the assigner is another variable and we already processed it, if so then just make this assignee also point to the actual instnace of the assginer
                if (_knownInstancesDeclaredInCurrentMethodAnalysis.TryGetValue(assigner, out AbstractInstance knownAssignerInstance) && knownAssignerInstance is not MethodInstance)
                {
                    instanceAssignee = (Instance)knownAssignerInstance;
                }
                // Make the new instance 
                else
                {
                    instanceAssignee = (Instance)instanceAssignerBuilder.Build();
                }
            }

            // Add the new instance to the known instances dictionary
            if(!_knownInstancesDeclaredInCurrentMethodAnalysis.ContainsKey(assignee))
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
        public void ReceiveMethodCall(AbstractBuilder<AbstractInstance> methodCallBuilder)
        {
            ((MethodInstanceBuilder)methodCallBuilder).Build();
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
