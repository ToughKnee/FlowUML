using Moq;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;

namespace Domain.CodeInfo.MethodSystem
{
    /// <summary>
    /// Represents a way to identify a function more easily
    /// This class is only created when we find a method definition
    /// (usually in a class declaration which contains all the methods for this class)
    /// </summary>
    public class Method
    {
        /// <summary>
        /// Namespace or package this method or function belongs to
        /// The namespace from the ClassEntity is not used because the 
        /// method may be alone
        /// </summary>
        public string? belongingNamespace { get; private set; } = null;
        /// <summary>
        /// The class that has this method's definition, if null then this is just a function
        /// </summary>
        public ClassEntity? ownerClass { get; private set; }
        /// <summary>
        /// Name of the method
        /// </summary>
        public string name { get; private set; }
        /// <summary>
        /// This stores only the Types of the parameters in the order they appear, the names of the parameters are not stored
        /// </summary>
        public List<string> parameters { get; private set; } = new List<string>();
        /// <summary>
        /// Name of the return type
        /// </summary>
        public string? returnType { get; private set; } = null;
        //public bool hasLambda { get; set; }
        /// <summary
        /// This will represent other methods or functions this method calls in its Method's 
        /// This way we will create an entire flow from a class to all other places of the code, regardless of the depth of the calls
        /// </summary>
        public List<Callsite> callsites { get; private set; } = new List<Callsite>();
        /// <summary>
        /// This list represents the typenames this method has, where a method like "public SNodeMyMethod<T, R>()"
        /// would mean a List with T and R respectively, this property may be assigned or modified by the class owner 
        /// if the class owner also has typenames
        /// </summary>
        public List<Typename>? typenames { get; set; }

        public Method(string belongingNamespace, string name, List<string> parameters, string retType, List<Callsite> callsites)
        {
            this.belongingNamespace = belongingNamespace;
            this.name = name;
            this.parameters = parameters;
            returnType = retType;
            this.callsites = callsites;

        }
        public Method(string belongingNamespace, ClassEntity owner, string name, List<string> parameters, string retType)
        {
            this.belongingNamespace = belongingNamespace;
            ownerClass = owner;
            this.name = name;
            this.parameters = parameters;
            returnType = retType;
        }

        public Method SetOwnerClass(ClassEntity ownerClass)
        {
            this.ownerClass = ownerClass;
            return this;
        }
        public string GetParamsAsString()
        {
            string result = "";
            if (parameters.Count > 0)
            {
                foreach (var param in parameters)
                {
                    result += param;
                    result += ",";
                }
                result = result.Substring(0, result.Length - 1);
            }

            return result;
        }

        // TODO: In the future, since the methods vary a lot from langauge to language, think of implementing a software patterm like strategy that returns the identifier based on the language chosen to make the analysis
        public string GetIdentifier()
        {
            // We need to provide the namespace of the class this belongs as well as the class name,
            // if we are not able to dissambiguate between 2 classes with the same name
            return belongingNamespace == null ? "" : belongingNamespace + "." +
                (ownerClass == null ? "" : ownerClass.name + ".") +
                name +
                "(" + GetParamsAsString() + ")";
        }

        /// <summary>
        /// This method get the MethodIdentifier of this MethodInstance which is used ONLY 
        /// when all the aliases have been found out their types and we must get 
        /// the actual Method from the methodDictionary
        /// </summary>
        /// <returns></returns>
        public MethodIdentifier GetMethodIdentifier()
        {
            // Setting the parameters of the MethodIdentifier to store this Method into the MethodDictionary
            var methodIdentifier = new MethodIdentifier();
            methodIdentifier.methodParameters = parameters;
            methodIdentifier.ownerClassNameAndInheritedClasses = ownerClass.inheritedClasses.ToList();
            methodIdentifier.ownerClassNameAndInheritedClasses.Add(ownerClass.name);
            methodIdentifier.methodName = name;
            methodIdentifier.methodBelongingNamespace = belongingNamespace;

            return methodIdentifier;
        }

        public override string ToString()
        {
            return $"{name}({string.Join(", ", parameters.Select(p => $"{p}"))}): {returnType}";
        }
    }
}
