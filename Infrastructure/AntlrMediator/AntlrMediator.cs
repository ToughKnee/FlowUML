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
        /// <summary>
        /// Used to know when there is a parameter or property used in a method and we must identify it
        /// </summary>
        private Dictionary<string, string> _parameterAndPropertyDeclaredInCurrentMethodAnalysis = new Dictionary<string, string>();
        /// <summary>
        /// This stores the properties declared in a class and refills the _parameterAndPropertyDeclaredInCurrentMethodAnalysis 
        /// everytime it is cleared when a methodAnalysisEnd is reached
        /// </summary>
        private Dictionary<string, string> _propertiesDeclared = new Dictionary<string, string>();

        public void ReceiveClassEntityBuilders(List<AbstractBuilder<ClassEntity>> builders)
        {
            throw new NotImplementedException();
        }
        public void ReceiveMethodBuilders(List<AbstractBuilder<Method>> builders)
        {
            throw new NotImplementedException();
        }
        public void ReceiveMethodDeclaration(string belongingNamespace, string ownerClass, string name, string parametersType, string returnType)
        {
            throw new NotImplementedException();
        }

        public void ReceiveNamespace(string? belongingNamespace)
        {
            _currentNamespace = (belongingNamespace == null) ? ("") : (belongingNamespace);
        }
        public void ReceiveClassName(string? className)
        {
            _currentNamespace = (className == null) ? ("") : (className);
        }
        public void ReceiveProperties(List<string> properties)
        {
            // Clear all the dicts since this is a new class we received
            _parameterAndPropertyDeclaredInCurrentMethodAnalysis.Clear();
            _propertiesDeclared.Clear();
            
            // Go through all the properties
            foreach (var property in properties)
            {
                // Split the properties between its type and identifier 
                string[] propertiesArray = property.Split("-");
                string propertyInstanceId = _currentNamespace + _currentNamespace + propertiesArray[1];

                // Send the identifier and type to the instancesManager
                InstancesDictionaryManager.instance.AddAssignment(propertyInstanceId, propertiesArray[0], true);

                // Add the property with its custom identifier to apply this identifier to future usages of this instance
                _propertiesDeclared.Add(propertiesArray[1], propertyInstanceId);
                _parameterAndPropertyDeclaredInCurrentMethodAnalysis.Add(propertiesArray[1], propertyInstanceId);
            }
        }
        public void ReceiveParameters(string type, string identifier)
        {
            string parameterInstanceId = _currentNamespace + _currentClassName + identifier;

            // Send the parameter to the instancesManager
            InstancesDictionaryManager.instance.AddAssignment(parameterInstanceId, type, true);

            // Add the parameter with its custom identifier to apply this identifier to future usages of this instance
            _parameterAndPropertyDeclaredInCurrentMethodAnalysis.Add(identifier, parameterInstanceId);
        }
        // TODO: =================  CHECK the "assigner" parameter, and if it is a methodCall, it has NO WHITESPACES
        public void ReceiveLocalVariableDeclaration(string assignee, string assigner)
        {
            // If the assigner is a method call...
            if(assigner.Contains("("))
            {
                //===========================  Getting the components of the method called
                string functionSignature = assigner;
                var lastPeriodIndex = functionSignature.LastIndexOf('.');
                Console.WriteLine("lastPeriodIndex: " + lastPeriodIndex.ToString());
                var methodName = functionSignature.Substring(
                    (lastPeriodIndex != -1) ? (lastPeriodIndex + 1) : (0)
                    );
                var namespaceAndClass = (lastPeriodIndex != -1) ? (functionSignature.Substring(0, lastPeriodIndex)) : ("");
                var openParenIndex = methodName.IndexOf('(');
                var closeParenIndex = methodName.IndexOf(')');
                var parameters = methodName.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1).Split(',');
                List<string> parameterList = null;
                if (!String.IsNullOrEmpty(parameters[0]))
                {
                    parameterList = new List<string>(parameters);
                }
                methodName = methodName.Substring(0, openParenIndex);

                //===========================  Check if parameters or properties were used here to replace them with the custom identifiers
                // If not, make the identifier of the components

                // If the namespaceAndClass component has a period, it means that it has already the identifier we were about to give it and we just keep as is
                // If not AND there is a 
                if(!namespaceAndClass.Contains(".") && _parameterAndPropertyDeclaredInCurrentMethodAnalysis.TryGetValue(namespaceAndClass, out string? namespaceAndClassIdentifier))
                {
                    namespaceAndClass = namespaceAndClassIdentifier;
                }
                // If that component wasn't in that dictionary, then we must add the custom identifier ourselves
                else
                {
                    namespaceAndClass = _currentNamespace + _currentClassName + namespaceAndClass;
                }

                // Now we pass through each parameter and do the same process
                for (global::System.Int32 j = 0; j < parameterList.Count; j++)
                {
                    string parameterAlias = parameterList[j];
                    if (_parameterAndPropertyDeclaredInCurrentMethodAnalysis.TryGetValue(parameterAlias, out string? parameterAliasIdentifier))
                    {
                        parameterList[j] = parameterAliasIdentifier;
                    }
                    else
                    {
                        parameterList[j] = _currentNamespace + _currentClassName + parameterAlias;
                    }
                }

                // //===========================  TODO: Check a property of type list in this class that should ALWAYS contain the MethodInstance that should be generated and is assigning a variable, this list must be filled by te method "ReceiveMethodCall", since this method is called BEFORE this one

                // Finally, we pass the MethodInstance to the instancesManager
                InstancesDictionaryManager.instance.AddMethodAssignment(assigner, assigner);

            }

            return;
        }
        public void ReceiveMethodAnalysisEnd()
        {
            throw new NotImplementedException();
        }

        public void ReceiveMethodCall(string calledClassName, string calledMethodName, List<string>? calledParameters, MethodBuilder linkedMethodBuilder)
        {
            _parameterAndPropertyDeclaredInCurrentMethodAnalysis.Clear();
            // Refilling the common dictionary with the custom identifiers while erasing the ones that are not needed anymore
            foreach (var property in _propertiesDeclared)
            {
                _parameterAndPropertyDeclaredInCurrentMethodAnalysis.Add(property.Key, property.Value);
            }

            throw new NotImplementedException();
        }

        public void ReceiveUsedNamespaces(List<string>? usedNamespaces)
        {
            throw new NotImplementedException();
        }
    }
}   
