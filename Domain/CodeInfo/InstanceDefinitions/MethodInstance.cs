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

        // TODO: Make a List of strings which has the namespaces where this callsite was called, to help identify from which namespace came this method if there are multiple classes with the same name
        // TODO: Make this class subscribe to a method which receives a Method, and this will be called everytime a Method instance is built, to fill, BUT we also need to have the instancesDictionary clean this class to be able to compare the received Method first

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

        // TODO: Add the correct method identifier used(the method signature/)
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
        /// Property that is mainly declared to let other instances that were 
        /// assigned by this class know eventually their type
        /// </summary>
        public string returnType = "";
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
        /// <param name="unknownMethod"></param>
        public MethodInstance(AbstractInstance? aliasClassName, string methodName, List<AbstractInstance> aliasParams, Callsite linkedCallsite, bool unknownMethod, List<string> usedNamespaces)
        {
            if (unknownMethod)
            {
                this.aliasClassName = aliasClassName;
                if(aliasClassName is not null && aliasClassName.inheritedClasses is null)
                {
                    aliasClassName.inheritedClasses = inheritedClasses;
                }

                this.methodName = methodName;
                this.aliasParameters = aliasParams;
                for (global::System.Int32 i = 0; i < aliasParameters.Count; i++)
                {
                    if (aliasParams[i].inheritedClasses is null)
                    {
                        aliasParams[i].inheritedClasses = inheritedClasses;
                    }
                }
                this.linkedCallsite = linkedCallsite;
                this.methodIsUnknown = unknownMethod;
            }
            this.candidateNamespaces = usedNamespaces;

            RegisterToTheMethodInstancesList(this);
        }
        public MethodInstance(Method method)
        {
            this.linkedCallsite = new Callsite(method);
            methodIsUnknown = false;
            RegisterToTheMethodInstancesList(this);
        }
        public static void RegisterToTheMethodInstancesList(MethodInstance methodInstance)
        {
            MethodInstance.methodInstancesWithUndefinedCallsite.Add(methodInstance);
        }
        /// <summary>
        /// This method checks the types of the aliases it has(className and parameters)
        /// and if there are no unknown types, then this MethodInstance will remove itself from
        /// the methodInstancesWithUndefinedCallsite, and start defining the remaning classes with
        /// this info
        /// If there are still unknwon types, we need to look for it with the help of other classes
        /// And we have several places to look for
        /// </summary>
        public void CheckTypesOfAliases()
        {
            //===========================  Check if this MethodInstance knows the class name and parameters types, if so then proceed to find the actual Method, if not then do nothing
            bool isAliasClassTypeKnown = true;
            bool isAliasClassAParent = false;
            if (aliasClassName is not null)
            {
                if (String.IsNullOrEmpty(aliasClassName.type))
                {
                   isAliasClassTypeKnown = false;
                }
            }
            // If the alias class name is null, then the method called came from a parent of the class owning this method, and we must consult the methodDictionary as many times as parents this MethoInstance knows
            else
            {
                isAliasClassTypeKnown = false;
                isAliasClassAParent = true;
            }

            bool areParametersTypeKnown = true;
            for (int i = 0; i < aliasParameters.Count; i++)
            {
                if (String.IsNullOrEmpty(aliasParameters[i].type))
                {
                    areParametersTypeKnown = false;
                    // Make function to look for the unknown type
                }
            }
            //======
            // If both the parameters and className are known, then look into the methodDictionary the actual Method and get the return type, and also defining the linkedCallstie with that Method
            if(isAliasClassTypeKnown && areParametersTypeKnown)
            {
                if (MethodDictionaryManager.instance.methodDictionary.TryGetValue(this.GetMethodIdentifier(), out Method actualMethod))
                {
                    this.returnType = actualMethod.returnType;
                    this.linkedCallsite.calledMethod = actualMethod;
                }
                
            }
            // If the alias class name is from a parent and we know the types then consult the methodDictionary as many parents as there are
            else if (isAliasClassAParent && areParametersTypeKnown)
            {
                // Consulting the Dictionary as many parents this MethodInstance knows this method has
                for (int i = 0; i < this.inheritedClasses.Count; i++)
                {
                    this.aliasClassName = new Instance(inheritedClasses[i]);
                    // Consulting the methodDictionary with the known inherited type
                    if (MethodDictionaryManager.instance.methodDictionary.TryGetValue(this.GetMethodIdentifier(), out Method actualMethod))
                    {
                        this.returnType = actualMethod.returnType;
                        this.linkedCallsite.calledMethod = actualMethod;
                        // If we found the class, break the cycle
                        break;
                    }
                    this.aliasClassName = null;
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
            methodIdentifier.methodOwnerClass = (String.IsNullOrEmpty(aliasClassName.type)) ? (aliasClassName.name) : (aliasClassName.type);
            methodIdentifier.methodName = methodName;
            methodIdentifier.methodInstanceCandidateNamespaces = this.candidateNamespaces;

            return methodIdentifier;
        }
    }
}