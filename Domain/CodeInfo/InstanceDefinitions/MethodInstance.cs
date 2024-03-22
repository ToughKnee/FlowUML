using System;
using System.ComponentModel;
using Domain.CodeInfo.MethodSystem;

namespace Domain.CodeInfo.InstanceDefinitions
{
    /// <summary>
    /// Special class for the instances that are return types from methods
    /// At first it will store the parts of a method(owner class, name and parameters) and 
    /// mark the 'methodIsUnknown' bool as true, thus 
    /// When 'methodIsUnknown' is false, then it means that the owner class of this method defined it
    /// and the MOST important part is that we can know its return type everytime this method is called,
    /// meaning that all the normal instances that are assigned by this method will be known now
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
        /// If this is not false, then this MethodInstance has a Callsite which has the definition 
        /// of a Method that came from a ClassEntity being defined
        /// </summary>
        public bool methodIsUnknown { get; private set; } = true;
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
            methodIsUnknown = false;
            RegisterToTheMethodInstancesList(this);
        }
        /// <summary>
        /// Set the inheritance of this MethoInstance and also the inheritance of the components of this 
        /// MethodInstance if they are null
        /// </summary>
        /// <param name="inheritance"></param>
        public void SetInheritance(IReadOnlyList<string> inheritance)
        {
            this.inheritedClasses = inheritance;
            if (callerClass is not null && callerClass.inheritedClasses is null)
            {
                callerClass.inheritedClasses = inheritedClasses;
            }
            for (int i = 0; i < calledParameters.Count; i++)
            {
                if (calledParameters[i].inheritedClasses is null)
                {
                    calledParameters[i].inheritedClasses = inheritedClasses;
                }
            }
        }
        public static void RegisterToTheMethodInstancesList(MethodInstance methodInstance)
        {
            MethodInstance.methodInstancesWithUndefinedCallsite.Add(methodInstance);
        }
        public void HandleActualMethod(Method actualMethod)
        {
            // TODO: Make an if statement which checks if the "actualMethod.returnType == <TYPENAME>T", where the thing in diamonds will represent the identifier for return types that are TEMPLATES TYPES, while the single 'T' represents the real typename from the definition of the method or class
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
        }
        public void ResolveCollectionInstanceType(AbstractInstance instance)
        {
            var newRefType = new StringWrapper();
            newRefType.data = instance.type;
            int firstCharacterIndex = 0;
            int lastCharacterIndex = 0;

            // If the type is an array, then get rid of the brackets to leave the type of the elements
            if (instance.type.Contains("["))
            {
                firstCharacterIndex = instance.type.IndexOf('[') + 1;
                newRefType.data = newRefType.data.Substring(0, firstCharacterIndex - 1);
                instance.refType = newRefType;
            }
            // If the indexed collection has diamonds with the type of the elements inside them, then get the contents of the diamonds
            else if (instance.type.Contains("<"))
            {
                firstCharacterIndex = instance.type.IndexOf('<') + 1;
                lastCharacterIndex = instance.type.IndexOf('>');
                newRefType.data = newRefType.data.Substring(firstCharacterIndex, lastCharacterIndex - firstCharacterIndex);
                instance.refType = newRefType;
            }
            else
                throw new Exception("The indexed collection did not contain any bracket or diamond to be able to get the type of the contained elements");
        }
        /// <summary>
        /// This method resolves the type of a given component of this MethodInstance
        /// It also takes an extra parameter if we want to look for the type of this Instance
        /// with the inheritance classes AND the owner class name
        /// </summary>
        /// <param name="component">Target Instance to look for its type</param>
        /// <param name="ownerClass">Extra parameter that add another class to look for 
        /// alognside the inherited classes</param>
        public void ResolveComponentType(AbstractInstance component, string ownerClass = "")
        {
            // If this component is an element from an indexed collection and it knows the type of the collection, then we assign its type to the type contained in the indexed collection
            if (component.kind == KindOfInstance.IsElementFromCollection && !String.IsNullOrEmpty(component.type) && (component.type.Contains("<") || component.type.Contains("[")))
            {
                ResolveCollectionInstanceType(component);
                return;
            }
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
        /// Gets the type of all the chained instances from a given Instance, and this represents the propertyChain
        /// </summary>
        /// <param name="ownerInstance"></param>
        public void ResolveChainedInstanceType(AbstractInstance ownerInstance)
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
                previousInstance = nextInstance;
                nextInstance = nextInstance.chainedInstance;
            }

        }
        /// <summary>
        /// This method checks the types of the aliases it has(className and parameters)
        /// and if there are no unknown types, then this MethodInstance will remove itself from
        /// the methodInstancesWithUndefinedCallsite, and start defining the remaning classes with
        /// this info
        /// If there are still unknwon types, we need to look for it with the help of other classes
        /// And we have several places to look for
        /// </summary>
        public void SolveTypesOfAliases()
        {
            // If this method is actually an array of a class, then define its type as the array correctly
            if (this.methodName.Contains("["))
            {
                this.refType.data = this.methodName;
                this.refType.data = this.refType.data.Substring(0, refType.data.IndexOf("[") + 1) + "]";
                methodInstancesWithUndefinedCallsite.Remove(this);
                ResolveChainedInstanceType(this);
                return;
            }
            // Get the inheritance of the callerClass if its type is known to process the cases where this MethodInstance is a normal kind or a kind that involves inheritance
            if (callerClass is not null && !String.IsNullOrEmpty(callerClass.type))
            {
                if(InheritanceDictionaryManager.instance.inheritanceDictionary.TryGetValue(callerClass.type, out List<string> inheritance))
                {
                    this.inheritedClasses = inheritance;

                    // If the inheritance of this class has been resolved and the callerClass is NOT a MethodInstance, then get the inheritance for the next property in the property chain if this MethodInstance has any
                    if(callerClass is not MethodInstance)
                        ResolveChainedInstanceType(callerClass);
                }
            }

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

            // Check if this MethodInstance knows the class parameters types, if so then proceed to find the actual Method, if not then do nothing
            bool areParametersTypeKnown = true;
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
                        areParametersTypeKnown = false;
                }

                else if (String.IsNullOrEmpty(calledParameters[i].type))
                {
                    areParametersTypeKnown = false;
                }
            }

            // If the parameters type and the types of the components of the caller are known, then look into the methodDictionary the actual Method and get the return type, and also define the linkedCallsite with the actual Method
            if (areParametersTypeKnown)
            {
                if (MethodDictionaryManager.instance.methodDictionary.TryGetValue(this.GetMethodIdentifier(), out Method actualMethod1))
                    HandleActualMethod(actualMethod1);
                else if (kind == KindOfInstance.HasClassNameStatic)
                {
                    // If is static method, we then check the Dictionary using the className as if it was its own type
                    callerClass.refType.data = callerClass.name;
                    if (MethodDictionaryManager.instance.methodDictionary.TryGetValue(this.GetMethodIdentifier(), out Method actualMethod2))
                    {
                        HandleActualMethod(actualMethod2);
                    }
                    else
                    {
                        callerClass.refType.data = null;
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
            // If this method call returned an indexed collection, and it was called with "[]" after it, then this method call must return the type of the elements in the collection
            if (kind == KindOfInstance.IsElementFromCollection)
            {
                ResolveCollectionInstanceType(this);
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
        public string GetIdentifier()
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
        /// This method get the MethodIdentifier of this MethodInstance which is used ONLY 
        /// when all the aliases have been found out their types and we must get 
        /// the actual Method from the methodDictionary
        /// </summary>
        /// <returns></returns>
        public MethodIdentifier GetMethodIdentifier()
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
            // If there is a propertyChain, then use the type of the last property from the propertyChain, else then use the type of the class name
            if(callerClass.chainedInstance is not null)
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