using System;
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
        /// Alias of the name the class is known by its alias
        /// </summary>
        public AbstractInstance? aliasClassName;
        /// <summary>
        /// Name of the method which does not have alias
        /// </summary>
        public string methodName;
        /// <summary>
        /// The parameters of the method used in the call known by their aliases
        /// </summary>
        public List<AbstractInstance> aliasParameters { get; private set; } = new List<AbstractInstance>();
        /// <summary>
        /// If this MethodCall got a caller class with properties(like "class.property.GetOwner().ToString()" if 
        /// this MethodInstance has as methodName "Tostring()"), 
        /// then we must store those other properties in this List where we are going to be able to get their types
        /// and get the true caller if this methodInstance
        /// The parts that are MethodInstances will be provided by the Mediator
        /// </summary>
        public List<AbstractInstance> propertyChain { get; private set; } = new List<AbstractInstance>();
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
        /// <param name="aliasClassName"></param>
        /// <param name="methodName"></param>
        /// <param name="aliasParams"></param>
        /// <param name="linkedCallsite"></param>
        /// <param name="kind"></param>
        public MethodInstance(AbstractInstance? aliasClassName, List<AbstractInstance> propertyChain, string methodName, List<AbstractInstance> aliasParams, Callsite linkedCallsite, KindOfInstance kind, List<string> usedNamespaces)
        {
            this.refType = new StringWrapper();
            this.aliasClassName = aliasClassName;
            this.propertyChain = propertyChain;

            this.methodName = methodName;
            this.aliasParameters = aliasParams;
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
            if (aliasClassName is not null && aliasClassName.inheritedClasses is null)
            {
                aliasClassName.inheritedClasses = inheritedClasses;
            }
            for (int i = 0; i < aliasParameters.Count; i++)
            {
                if (aliasParameters[i].inheritedClasses is null)
                {
                    aliasParameters[i].inheritedClasses = inheritedClasses;
                }
            }
        }
        public static void RegisterToTheMethodInstancesList(MethodInstance methodInstance)
        {
            MethodInstance.methodInstancesWithUndefinedCallsite.Add(methodInstance);
        }
        public void HandleActualMethod(Method actualMethod)
        {
            this.refType.data = actualMethod.returnType;
            this.linkedCallsite.calledMethod = actualMethod;
            methodInstancesWithUndefinedCallsite.Remove(this);
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
            // If the current component of the MethodInstance already has its type defined or is another MethodInstance, then we skip this component and keep going with the others
            if (!String.IsNullOrEmpty(component.type) || component is MethodInstance)
            {
                return;
            }

            if (component is not null && component.type is null)
            {
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
            // Get the inheritance of the aliasClassName if its type is known to process the cases where this MethodInstance is a normal kind or a kind that involves inheritance
            if (aliasClassName is not null && !String.IsNullOrEmpty(aliasClassName.type))
            {
                if(InheritanceDictionaryManager.instance.inheritanceDictionary.TryGetValue(aliasClassName.type, out List<string> inheritance))
                {
                    this.inheritedClasses = inheritance;
                    // If the inheritance of this class has been resolved, then get the inheritance for the next property in the property chain if this MethodInstance has any
                    for (int i = 0; i < propertyChain.Count; i++)
                    {
                        var component = propertyChain[i];
                        string currentOwnerClass = "";
                        // Get the inheritance of this component
                        if (i == 0)
                        {
                            currentOwnerClass = aliasClassName.type;
                            component.inheritedClasses = this.inheritedClasses;
                        }
                        else
                        {
                            if (InheritanceDictionaryManager.instance.inheritanceDictionary.TryGetValue(propertyChain[i-1].type, out List<string> inheritanceOfComponent))
                            {
                                currentOwnerClass = propertyChain[i - 1].type;
                                component.inheritedClasses = inheritanceOfComponent;
                            }
                        }

                        // Then resolve the type of this component
                        ResolveComponentType(component, currentOwnerClass);
                    }
                }
            }

            // Then we resolve the parameters and class name(and also propertyChain, like the "property.function()" from "class.property.function()" if any) if they are of a special kind of instance, and then we go through all of these components, and check their types, if they are missing get their types
            // Putting all the components of this MethodInstance into a List to traverse them and resolve their types
            var methodInstanceComponents = new List<AbstractInstance>(aliasParameters);
            if (aliasClassName is not null)
                methodInstanceComponents.Add(aliasClassName);
            for (int i = 0; i < methodInstanceComponents.Count; i++)
            {
                var currentMethodComponent = methodInstanceComponents[i];
                ResolveComponentType(currentMethodComponent);
            }

            // Check if this MethodInstance knows the class parameters types, if so then proceed to find the actual Method, if not then do nothing
            bool areParametersTypeKnown = true;
            for (int i = 0; i < aliasParameters.Count; i++)
            {
                if (String.IsNullOrEmpty(aliasParameters[i].type))
                {
                    areParametersTypeKnown = false;
                }
            }

            bool areCallerComponentsTypeKnown = propertyChain.Count >= 0;
            for (int i = 0; i < propertyChain.Count; i++)
            {
                if (String.IsNullOrEmpty(propertyChain[i].type))
                {
                    areCallerComponentsTypeKnown = false;
                }
                // If there are chained properties, then replace the aliasclassname type with the last component from the list
                if(i == propertyChain.Count - 1)
                {
                    aliasClassName.refType.data = propertyChain[i].refType.data;
                }
            }

            // If the parameters type and the types of the components of the caller are known, then look into the methodDictionary the actual Method and get the return type, and also define the linkedCallsite with the actual Method
            if (areParametersTypeKnown && areCallerComponentsTypeKnown)
            {
                if (MethodDictionaryManager.instance.methodDictionary.TryGetValue(this.GetMethodIdentifier(), out Method actualMethod1))
                    HandleActualMethod(actualMethod1);
                // If the normal method failed or the alias class name is from a parent of this class and we know their types then consult the methodDictionary, which will resolve the inheritance for us
                // TODO: REMOVE this else if and check if the code keeps working
                else if (kind == KindOfInstance.Normal || kind == KindOfInstance.IsInheritedOrInThisClass)
                {
                    // Consulting the Dictionary as many parents this MethodInstance knows this method has AND also itself, since the method could also be from the same class, using the GetMethodIdentifier()
                    if (MethodDictionaryManager.instance.methodDictionary.TryGetValue(this.GetMethodIdentifier(), out Method actualMethod))
                    {
                        HandleActualMethod(actualMethod);
                    }
                }
                else if (kind == KindOfInstance.HasClassNameStatic)
                {
                    // If is static method, we then check the Dictionary using the className as if it was its own type
                    aliasClassName.refType.data = aliasClassName.name;
                    if (MethodDictionaryManager.instance.methodDictionary.TryGetValue(this.GetMethodIdentifier(), out Method actualMethod2))
                    {
                        HandleActualMethod(actualMethod2);
                    }
                    else
                    {
                        aliasClassName.refType.data = null;
                    }
                }
                else if (kind == KindOfInstance.IsConstructor)
                {
                    // We convert the alias class name the same as the methodName
                    this.aliasClassName.refType.data = methodName;

                    // And we check if there is a constructor Method tht matches this MethodINstance in the methodDictionary
                    if (MethodDictionaryManager.instance.methodDictionary.TryGetValue(this.GetMethodIdentifier(), out Method actualMethod))
                        HandleActualMethod(actualMethod);
                    // If the constructor method wasn't found, and if the parameter list has 0 elements, then this methodInstance is the default constructor and we must set the refType
                    else if(this.aliasParameters.Count == 0)
                    {
                        this.refType.data = this.aliasClassName.type;
                    }
                }

            }
        }

        /// <summary>
        /// Gets the identifier of this MethodInstance based on the components of this method
        /// If the components have their types defined(the className and the parameters), then 
        /// they are going to be instead of their aliases(or "name"), else then they're names 
        /// are going to be shown
        /// </summary>
        /// <returns>Identification of the MethodInstance's method</returns>
        public string GetIdentifier()
        {
            string result = "";
            // Getting the className identification of this MethodInstance
            if(aliasClassName is not null)
            {
                if (!String.IsNullOrEmpty(aliasClassName.type))
                {
                    result += aliasClassName.type;
                }
                else
                {
                    result += aliasClassName.name;
                }
                result += ".";
            }
            result += methodName;

            result += "(";
            // Getting the parameters identification of this MethodInstance
            if(aliasParameters.Count > 0)
            {
                foreach(var param in aliasParameters)
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
            foreach(var param in aliasParameters)
            {
                parameters.Add(param.type);
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
            methodIdentifier.ownerClassNameAndInheritedClasses.Add((String.IsNullOrEmpty(aliasClassName.type)) ? (aliasClassName.name) : (aliasClassName.type));
            methodIdentifier.methodName = methodName;
            methodIdentifier.methodInstanceCandidateNamespaces = this.candidateNamespaces;

            return methodIdentifier;
        }
    }
}

