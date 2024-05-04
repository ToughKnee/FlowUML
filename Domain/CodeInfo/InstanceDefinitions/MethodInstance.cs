using System;
using System.ComponentModel;
using Domain.CodeInfo.MethodSystem;

namespace Domain.CodeInfo.InstanceDefinitions
{
    /// <summary>
    /// Special class for the instances that are return types from methods
    /// At first it will store the copmonents of a method(owner class, name, parameters and chainedInstances)
    /// This is the part where, when we got the information from the code analysis and 
    /// their respective linking from the Mediator, then we start using the 
    /// information in this class and start discovering the information in order to
    /// discover the actual Method this class represents, the callsite, and its type for other
    /// MethodInstances to use and also discover their actual Methods
    /// MethodInstances are in charge of descovering the types of their components
    /// ONLY IF they are normal Instances, and NOT other MethodInstances
    /// </summary>
    public class MethodInstance : AbstractInstance
    {
        /// <summary>
        /// This helps in identifying the namespaces under which this method was called, if 
        /// there are 2 classes with the same name in the entire code but different namespaces
        /// </summary>
        public List<string> candidateNamespaces = new List<string>();
        /// <summary>
        /// This helps in traversing all the MehtodInstances which don't know the 
        /// return type of the method they hold
        /// </summary>
        public static List<MethodInstance> methodInstancesWithUndefinedCallsite = new List<MethodInstance>();

        public override string name => GetIdentifier();
        /// <summary>
        /// This makes this MethodInstance know which Callsite it is linked to,
        /// Because the callsite must be generated alongside the instances, and when
        /// the method is unknown, we must define it later and make the callsite created
        /// know the method when it is known, thus we don't need to 
        /// </summary>
        public Callsite? linkedCallsite { get; set; }
        /// <summary>
        /// Caller class of this MethodCall
        /// The refType of this property is NEVER modified unless it is not a normal Instance,
        /// if this MethodInstance is of a special kind like static or constrctor, then we can modify the 
        /// refType of this property
        /// </summary>
        public AbstractInstance? callerClass;
        /// <summary>
        /// Name of the method which does not have alias
        /// </summary>
        public string methodName;
        /// <summary>
        /// The parameters of the method used in the method call 
        /// </summary>
        public List<AbstractInstance> calledParameters { get; private set; } = new List<AbstractInstance>();
        /// <summary>
        /// The identifier to be used when this Method must be added to the methodDictionary
        /// </summary>
        public MethodIdentifier methodIdentifier { get; private set; }

        /// <summary>
        /// Constructor for methodCall instances inside methods
        /// Every value is normal except for 2 components of the methodCall, namely the class name and parameters,
        /// because since they are linked directly to this MethodInstance, they must share the inheritanceList 
        /// with the one this MethodInstance has unless the instance already has inheritance
        /// </summary>
        /// <param name="calledClass"></param>
        /// <param name="methodName"></param>
        /// <param name="aliasParams"></param>
        /// <param name="linkedCallsite"></param>
        /// <param name="kind"></param>
        public MethodInstance(AbstractInstance? calledClass, AbstractInstance? chainedInstance, string methodName, List<AbstractInstance> aliasParams, Callsite linkedCallsite, KindOfInstance kind, List<string> usedNamespaces)
        {
            this.refType = new StringWrapper();
            this.callerClass = calledClass;
            this.chainedInstance = chainedInstance;

            this.methodName = methodName;
            this.calledParameters = aliasParams;
            this.linkedCallsite = linkedCallsite;
            this.kind = kind;
            RegisterToTheMethodInstancesList(this);
            this.candidateNamespaces = usedNamespaces;

        }
        public MethodInstance(Method method)
        {
            this.linkedCallsite = new Callsite(method);
            RegisterToTheMethodInstancesList(this);
        }
        public static void RegisterToTheMethodInstancesList(MethodInstance methodInstance)
        {
            MethodInstance.methodInstancesWithUndefinedCallsite.Add(methodInstance);
        }
        private void HandleActualMethod(Method actualMethod)
        {
            this.refType.data = actualMethod.returnType;
            this.linkedCallsite.calledMethod = actualMethod;
            // Check if the actualMethod or if the owner class of the actualMethod has typename parameters, if so, then look for the correct type used when instantiated
            if(actualMethod.typenames is not null || (actualMethod.ownerClass is not null && actualMethod.ownerClass.typenames is not null))
            {
                bool typenameFound = false;
                var callerClassTypenameParameters = Typename.GetTypenameList(callerClass.type);
                // Look for the typename parameter in the actualMethod typename, if the typename was there, then set the real return type
                for (int i = 0; actualMethod.typenames != null && i < actualMethod.typenames.Count; i++)
                {
                    if (refType.data == actualMethod.typenames[i].name)
                    {
                        refType.data = callerClassTypenameParameters[i].name;
                        typenameFound = true;
                        break;
                    }
                }
                // If the typename parameter wasn't found in the actualMethod, then look for it in the owner class
                if (typenameFound == false)
                for (int i = 0; i < actualMethod.ownerClass.typenames.Count; i++)
                {
                    if (refType.data == actualMethod.ownerClass.typenames[i].name)
                    {
                        refType.data = callerClassTypenameParameters[i].name;
                        break;
                    }
                }
            }
            methodInstancesWithUndefinedCallsite.Remove(this);
            ResolveChainedInstanceType(this);
            // If this methodInstance has an indexRetrieval instance, then resolve its type for other MethodInstances that were called by that indexRetrievalInstance which will require this indexRetrievalInstnace defined
            if(this.indexRetrievedInstance != null)
            {
                ResolveIndexRetrievalInstanceType(this);
            }
            // If this methodInstance is owned by another MethodInstance, we must chain this method to the caller class, because the caller class of kind "IsFromLinkedMethodInstance" will never contain chainedInstances, and when the owner of this MethodInstance needs to get the type of the chain, he will be able to do it(because the owner methodInstance contains the caller class of this MethodInstance as the chainedInstance)
            if (callerClass.kind == KindOfInstance.IsFromLinkedMethodInstance)
                callerClass.chainedInstance = this;
        }
        /// <summary>
        /// This method resolves the type of a given component of this MethodInstance
        /// It also takes an extra parameter if we want to look for the type of this Instance
        /// with the inheritance classes AND the owner class name
        /// </summary>
        /// <param name="component">Target Instance to look for its type</param>
        /// <param name="ownerClass">Extra parameter that add another class to look for 
        /// alognside the inherited classes</param>
        private void ResolveComponentType(AbstractInstance component, string ownerClass = "")
        {
            // If the current component of the MethodInstance already has its type defined or is another MethodInstance, then we ignore this component
            if (!String.IsNullOrEmpty(component.type) || component is MethodInstance)
            {
                return;
            }
            // If this component is a property from a parent then we must use the inherited classes from this component, and check all the classes for a property that matches the name of this component
            if (component.inheritedClasses is not null && component.kind == KindOfInstance.IsPropertyFromInheritanceOrInThisClass)
            {
                // Traversing each inherited class
                var componentIheritance = component.inheritedClasses.ToList();
                componentIheritance.Add(ownerClass);
                for (int i = 0; i < componentIheritance.Count; i++)
                {
                    if (component.type is null && component.inheritedClasses is not null
                    && ClassEntityManager.instance.classEntities.TryGetValue(componentIheritance[i], out ClassEntity parentClass))
                    {
                        // Traversing each property of the inherited class
                        for (int i2 = 0; i2 < parentClass.properties.Count; i2++)
                        {
                            if (parentClass.properties[i2].name == component.name)
                            {
                                component.refType.data = parentClass.properties[i2].type;
                                break;
                            }
                        }
                    }
                    if (component.type is not null)
                        break;
                }
            }
        }
        /// <summary>
        /// Gets the type of all the chained instances from a given Instance
        /// </summary>
        /// <param name="ownerInstance"></param>
        private void ResolveChainedInstanceType(AbstractInstance ownerInstance)
        {
            // If this is not a parameter and the MehtodInstance does not have a chainedInstance then do nothing, or if this is a parameter and the parameter does not have a chainedInstance then do nothing, or if the ownerInstnace is another MehtodInstance and it is a parameter then do nothing
            if (ownerInstance.chainedInstance is null)
            {
                return;
            }
            AbstractInstance nextInstance = ownerInstance;
            AbstractInstance previousInstance = ownerInstance;
            nextInstance.inheritedClasses = this.inheritedClasses;
            string currentOwnerClass = ownerInstance.type;
            while (nextInstance is not null && previousInstance.type is not null)
            {
                if (InheritanceDictionaryManager.instance.inheritanceDictionary.TryGetValue(previousInstance.type, out List<string> inheritanceOfComponent))
                {
                    currentOwnerClass = previousInstance.type;
                    nextInstance.inheritedClasses = inheritanceOfComponent;
                }

                // Then resolve the type of this nextInstance
                ResolveComponentType(nextInstance, currentOwnerClass);
                // If the "nextInstance" does not have a type and is of kind "IsFromLinkedMethodInstance", then set the type right away
                if (String.IsNullOrEmpty(nextInstance.refType.data) && nextInstance.kind == KindOfInstance.IsFromLinkedMethodInstance)
                    nextInstance.refType.data = currentOwnerClass;
                // If the "nextInstance" has an "indexRetrievedInstance", then that means this "nextInstance" is an indexed collection the brackets are being used to get an element from it, and we must make a new chainedInstance representing the type of the element to be retrieved
                if (nextInstance.indexRetrievedInstance != null)
                {
                    ResolveIndexRetrievalInstanceType(nextInstance);

                    // After creating the indexRetrievalInstance as another chainedInstance, we purposefully make the nextInstance variable jump over the just added chainedProperty
                    nextInstance = nextInstance.chainedInstance;
                }
                previousInstance = nextInstance;
                nextInstance = nextInstance.chainedInstance;
            }
        }
        private void ResolveIndexRetrievalInstanceType(AbstractInstance instanceWithIndexRetrieval)
        {
            // Now with the indexRetrieval type solved, we move that from being the property of the chainedInstance.indexRetrievalInstance to chainedInstance.chainedInstance
            var nextInstancePlaceholder = instanceWithIndexRetrieval.chainedInstance;
            // Moving all the other chainedProperties after the indexRetrieval
            while (nextInstancePlaceholder != null)
            {
                instanceWithIndexRetrieval.chainedInstance = instanceWithIndexRetrieval.indexRetrievedInstance;
                nextInstancePlaceholder = nextInstancePlaceholder.chainedInstance;
                instanceWithIndexRetrieval.indexRetrievedInstance.chainedInstance = nextInstancePlaceholder;
            }
            instanceWithIndexRetrieval.chainedInstance = instanceWithIndexRetrieval.indexRetrievedInstance;
            instanceWithIndexRetrieval.indexRetrievedInstance = null;

            // After setting the chainedInstance as the indexRetrieval, we get the type of the nextInstance, and get the string between diamonds, which represents the type of the elements from the collection, the "Last()" method is used because the last typename from the collection is most likely the type of the elements
            instanceWithIndexRetrieval.chainedInstance.refType.data = Typename.GetTypenameList(instanceWithIndexRetrieval.type).Last().name;

            // Then we get the first methodInstance chained to this indexRetrieval, and we set the type of the callerClass of that chained MethodInstaance to the type of the indexRetrieval, because the indexRetrieval is the actual caller class, and also remove the chainedInstance from the indexRetrieval to avoid infinite references to each other
            var firstMethodInstanceChained = AbstractInstance.GetLastChainedInstance(instanceWithIndexRetrieval, true);
            if (firstMethodInstanceChained != null)
            {
                ((MethodInstance)firstMethodInstanceChained).callerClass = instanceWithIndexRetrieval.chainedInstance;
            }
        }
        /// <summary>
        /// This method checks the types of the components it has(className, parameters, and 
        /// with their respective property chains, including this MethodInstance)
        /// and if we found all the types, then this MethodInstance is ready to look for the actual Method, and then 
        /// remove itself from the methodInstancesWithUndefinedCallsite, and start defining the remaning classes with
        /// this info
        /// This method is responsible for finding all the types of the components ONLY IF they are normal Instances,
        /// if they are other MethodInstances then it won't find their type because all MethodInstances have the 
        /// responsibility to find their own types
        /// <param name="useLooseMatchingRules">If true, then this method will start looking for the actual 
        /// Method in the MethodDictionaryManager even though its components may not be defined with their types</param>
        /// </summary>
        public void SolveTypesOfComponents(bool useLooseMatchingRules)
        {
            //===========================  Finding the CHAINED INSTANCES types of the Caller Class
            // Get the inheritance of the callerClass if its type is known to process the cases where this MethodInstance is a normal kind or a kind that involves inheritance
            if (callerClass is not null && !String.IsNullOrEmpty(callerClass.type))
            {
                if(InheritanceDictionaryManager.instance.inheritanceDictionary.TryGetValue(callerClass.type, out List<string> inheritance))
                {
                    // The methodInstance must share the inherited classes with the caller class
                    this.inheritedClasses = inheritance;

                    // If the inheritance of this class has been resolved and the callerClass is NOT a MethodInstance, then get the inheritance for the next property in the property chain if this MethodInstance has any
                    if(callerClass is not MethodInstance)
                        ResolveChainedInstanceType(callerClass);
                }
            }

            //===========================  Finding the PARAMETERS and CALLER CLASS types
            // Then we resolve the parameters and class name if they are of a special kind of instance, and then we go through all of these components, and check their types, if the types are missing then we get the types
            // Putting all the components of this MethodInstance into a List to traverse them and resolve their types
            var methodInstanceComponents = new List<AbstractInstance>(calledParameters);
            if (callerClass is not null)
                methodInstanceComponents.Add(callerClass);
            for (int i = 0; i < methodInstanceComponents.Count; i++)
            {
                var currentMethodComponent = methodInstanceComponents[i];
                ResolveComponentType(currentMethodComponent);
            }

            //===========================  Finding the CHAINED INSTANCES types of the Parameters and checking if all the components of this MethodInstance are defined to look for the actual Method
            // Check if this MethodInstance knows the class parameters types, if so then proceed to find the actual Method, if not then do nothing
            bool componentsTypeKnown = true;
            for (int i = 0; i < calledParameters.Count; i++)
            {
                // If this parameter has a chained Instance, then we must find the types of all the chained instances, unless it is another MethodInstance
                if (calledParameters[i].chainedInstance is not null)
                {
                    // If this parameter is another MehtodInstance, then do nothing
                    if (calledParameters[i] is not MethodInstance)
                        ResolveChainedInstanceType(calledParameters[i]);
                    // Check if the last element has its type defined, if not then the parameters type are not known
                    if (String.IsNullOrEmpty(GetLastChainedInstance(calledParameters[i]).type))
                        componentsTypeKnown = false;
                }

                else if (String.IsNullOrEmpty(calledParameters[i].type))
                {
                    componentsTypeKnown = false;
                }
            }

            //===========================  Finding the ACTUAL METHOD
            // If the parameters type and the types of the components of the caller are known, then look into the methodDictionary the actual Method and get the return type, and also define the linkedCallsite with the actual Method
            if (componentsTypeKnown || useLooseMatchingRules)
            {
                if (MethodDictionaryManager.instance.methodDictionary.TryGetValue(this.GetMethodIdentifier(), out Method actualMethod1))
                    HandleActualMethod(actualMethod1);
                else if (kind == KindOfInstance.HasClassNameStatic)
                {
                    // If is static method, we then check the Dictionary using the className as if it was its own type
                    var backupCallerClassName = callerClass.refType.data;
                    callerClass.refType.data = callerClass.name;
                    if (MethodDictionaryManager.instance.methodDictionary.TryGetValue(this.GetMethodIdentifier(), out Method actualMethod2))
                    {
                        HandleActualMethod(actualMethod2);
                    }
                    else
                    {
                        callerClass.refType.data = backupCallerClassName;
                    }
                }
                else if (kind == KindOfInstance.IsConstructor)
                {
                    // We convert the alias class name the same as the methodName
                    this.callerClass.refType.data = methodName;

                    // And we check if there is a constructor Method tht matches this MethodINstance in the methodDictionary
                    if (MethodDictionaryManager.instance.methodDictionary.TryGetValue(this.GetMethodIdentifier(), out Method actualMethod))
                        HandleActualMethod(actualMethod);
                    // If the constructor method wasn't found, and if the parameter list has 0 elements, then this methodInstance is the default constructor and we must set the refType
                    else if(this.calledParameters.Count == 0)
                    {
                        this.refType.data = this.callerClass.type;
                        methodInstancesWithUndefinedCallsite.Remove(this);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the identifier of this MethodInstance based on the components of this method
        /// for better recognition
        /// If the components have their types defined(the className and the parameters), then 
        /// they are going to be used instead of their aliases(or "name"), else then their names 
        /// are going to be shown
        /// </summary>
        /// <returns>Identification of the MethodInstance's method</returns>
        private string GetIdentifier()
        {
            string result = "";
            // Getting the className identification of this MethodInstance
            if(callerClass is not null)
            {
                if (!String.IsNullOrEmpty(callerClass.type))
                {
                    result += callerClass.type;
                }
                else
                {
                    result += callerClass.name;
                }
                result += ".";
            }
            result += methodName;

            result += "(";
            // Getting the parameters identification of this MethodInstance
            if(calledParameters.Count > 0)
            {
                foreach(var param in calledParameters)
                {
                    if (!String.IsNullOrEmpty(param.type))
                    {
                        result += param.type;
                    }
                    else
                    {
                        result += param.name;
                    }
                    result += ",";
                }
                result = result.Substring(0, result.Length - 1);
            }
            result += ")";

            return result;
        }

        /// <summary>
        /// This method gets the MethodIdentifier of this MethodInstance which is used ONLY 
        /// when all the components of this class have been found out their types and we must get 
        /// the actual Method from the methodDictionary
        /// </summary>
        /// <returns></returns>
        private MethodIdentifier GetMethodIdentifier()
        {
            // Setting the parameters of the MethodIdentifier to request the actual Method to the methodDictionary
            var methodIdentifier = new MethodIdentifier();
            var parameters = new List<string>();
            foreach(var param in calledParameters)
            {
                // If this param has a chainedInstance, then replace the type of the param with the type of the Last chainedInstance
                parameters.Add((param.chainedInstance is not null) ? (GetLastChainedInstance(param).type) : (param.type));
            }
            methodIdentifier.methodParameters = parameters;
            if(this.inheritedClasses is not null)
            {
                methodIdentifier.ownerClassNameAndInheritedClasses = this.inheritedClasses.ToList();
            }
            else
            {
                methodIdentifier.ownerClassNameAndInheritedClasses = new();
            }
            // If there is a propertyChain and the caller class does not have as a chain the same methodInstance, then use the type of the last property from the propertyChain, else then use the type of the class name
            if(callerClass.chainedInstance is not null && callerClass.chainedInstance != this)
            {
                methodIdentifier.ownerClassNameAndInheritedClasses.Add(GetLastChainedInstance(callerClass.chainedInstance).type);
            }
            else
            {
                // If the type of the caller class type is unknown, then use the caller class as the type
                if(String.IsNullOrEmpty(callerClass.type))
                {
                    methodIdentifier.ownerClassNameAndInheritedClasses.Add(callerClass.name);
                }
                else
                {
                    // If we have the type of the caller class then check if it has template typename, if so, then remove the part of the typename in order to be able to match this MethodInstance with the actual Method at the MethodIdentifier equals from the Dictionary lookup
                    int typenameCharactersIndex = callerClass.type.IndexOf("<");
                    methodIdentifier.ownerClassNameAndInheritedClasses.Add(
                        (typenameCharactersIndex > 0) ? (callerClass.type.Substring(0, typenameCharactersIndex)) : (callerClass.type)
                        );
                }
            }
            methodIdentifier.methodName = methodName;
            methodIdentifier.methodInstanceCandidateNamespaces = this.candidateNamespaces;

            return methodIdentifier;
        }

    }
}