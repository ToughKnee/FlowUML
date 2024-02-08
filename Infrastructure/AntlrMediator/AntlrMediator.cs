using Antlr4.Runtime.Tree;
using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Infrastructure.Builders;
using System;

namespace Infrastructure.Mediators
{
    public class AntlrMediator : IMediator
    {
        private string _currentNamespace;
        private string _currentClassName;
        private string _currentMethodName;
        /// <summary>
        /// Used to know when there are instances with their defined type used in a method and we must identify it
        /// </summary>
        private Dictionary<string, string> _knownInstancesDeclaredInCurrentMethodAnalysis = new Dictionary<string, string>();
        /// This stores the properties declared in a class and refills the _knownInstancesDeclaredInCurrentMethodAnalysis 
        /// everytime it is cleared when a methodAnalysisEnd is reached
        /// </summary>
        private Dictionary<string, string> _propertiesDeclared = new Dictionary<string, string>();
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

        public void ReceiveClassEntityBuilders(List<AbstractBuilder<ClassEntity>> builders)
        {
        }
        public void ReceiveMethodBuilders(List<AbstractBuilder<Method>> builders)
        {
        }
        public void ReceiveMethodDeclaration(string? belongingNamespace, string? ownerClass, string name, string parametersType, string returnType)
        {
            _currentMethodName = name + ".";

            // TODO: Add this method declaration to the instancesDictionary
        }

        public void ReceiveNamespace(string? belongingNamespace)
        {
            _currentNamespace = (belongingNamespace == null) ? ("") : (belongingNamespace + ".");
        }
        public void ReceiveClassName(string? className)
        {
            _knownInstancesDeclaredInCurrentMethodAnalysis.Clear();
            _propertiesDeclared.Clear();
            _currentClassName = (className == null) ? ("") : (className + ".");
        }
        public void ReceiveProperties(string type, string identifier)
        {
            string propertyInstanceId = _currentNamespace + _currentClassName + identifier;
            InstancesDictionaryManager.instance.AddAssignment(propertyInstanceId, type, true);
            _knownInstancesDeclaredInCurrentMethodAnalysis.Add(identifier, propertyInstanceId);
        }
        public void ReceiveParameters(string type, string identifier)
        {
            string parameterInstanceId = _currentNamespace + _currentClassName + _currentMethodName + identifier;

            // Send the parameter to the instancesManager
            InstancesDictionaryManager.instance.AddAssignment(parameterInstanceId, type, true);

            // Add the parameter with its custom identifier to apply this identifier to future usages of this instance
            _knownInstancesDeclaredInCurrentMethodAnalysis.Add(identifier, parameterInstanceId);
        }
        public void ReceiveLocalVariableDeclaration(string assignee, string assigner)
        {
            // If the assigner is a method call...
            if(assigner.Contains("("))
            {
                // Get the _currentMethodCallInstance which contains the methodCall assigner already processed and add the assignment to the instanceDictionary
                if(_currentMethodCallInstance is null)
                {
                    throw new NullReferenceException("The property '_currentMethodCallInstance' is null when it should never be null in this case");
                }
                InstancesDictionaryManager.instance.AddMethodAssignment(assignee, _currentMethodCallInstance);
                _currentMethodCallInstance = null;
            }

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

        public void ReceiveMethodCall(string calledClassName, string calledMethodName, List<string>? calledParameters, MethodBuilder linkedMethodBuilder)
        {
            // Check the property which contains the MethodInstance this method will put in there, if this is not null, then it means the RecieveLocalVariableDeclaration did not catch that since this was a pure methodCall without assigning any variable anything, and we must manage it ourselves, we must then put this MethodInstance to the InstancesManager to let that MethodCall be identifiable if it isn't identifiable
            if(_currentMethodCallInstance is not null)
            {
                // Send the MethodInstance to the instancesDictionary
                InstancesDictionaryManager.instance.AddMethodInstance(_currentMethodCallInstance);
            }

            //===========================  Get the components of this methodCall(methodName, className, properties) and make the custom identifier for the components that require identification
            // If the namespaceAndClass component has a period, it means that it has already the identifier we were about to give it and we just keep as is
            // If not AND the className has an identification already made in the _parameterAndPropertyDeclaredInCurrentMethodAnalysis, then we rename it
            if (!calledClassName.Contains(".") 
                && _knownInstancesDeclaredInCurrentMethodAnalysis.TryGetValue(calledClassName, out string? namespaceAndClassIdentifier) 
                )
            {
                calledClassName = namespaceAndClassIdentifier;
            }
            // If that component wasn't in that dictionary AND it isn't emtpy, then we must add the custom identifier ourselves
            else if (!String.IsNullOrEmpty(calledClassName))
            {
                calledClassName = _currentNamespace + _currentClassName + _currentMethodName + calledClassName;
            }

            // Now we pass through each parameter and do the same process
            for (global::System.Int32 j = 0; j < calledParameters.Count; j++)
            {
                string parameterAlias = calledParameters[j];
                if (_knownInstancesDeclaredInCurrentMethodAnalysis.TryGetValue(parameterAlias, out string? parameterAliasIdentifier))
                {
                    calledParameters[j] = parameterAliasIdentifier;
                }
                else
                {
                    calledParameters[j] = _currentNamespace + _currentClassName + _currentMethodName + parameterAlias;
                }
            }
            //======

            // Make the callsite for the method
            Callsite callsite = new Callsite(null);
            MethodInstance methodInstance = new MethodInstance(calledClassName, calledMethodName, calledParameters, callsite, true);

            // Put the MethodInstance created in a property to be passed to the RecieveLocalVariableDeclaration
            _currentMethodCallInstance = methodInstance;
        }

        public void ReceiveUsedNamespaces(List<string>? usedNamespaces)
        {
        }
    }
}   
