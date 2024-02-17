using Domain.CodeInfo.InstanceDefinitions;
using Domain.CodeInfo.MethodSystem;

namespace Domain.CodeInfo
{
    public class ClassEntity
    {
        /// <summary>
        /// Name of the class
        /// </summary>
        public string name { get; private set; }
        /// <summary>
        /// Namespace or package in which the class was found
        /// </summary>
        public string? classNamespace { get; private set; }
        /// <summary>
        /// Properties of the class
        /// </summary>
        public List<Property> properties { get; private set; } = new List<Property>();
        /// <summary>
        /// Methods of the class
        /// </summary>
        public List<Method> methods { get; private set; } = new List<Method>();

        public ClassEntity(string name, string belongingNamepsace, List<Property> properties, List<Method> methods)
        {
            this.name = name;
            this.classNamespace = belongingNamepsace;
            this.properties = properties;
            this.methods = methods;
        }
        public ClassEntity(string name, string belongingNamepsace, List<Property> properties)
        {
            this.name = name;
            this.classNamespace = belongingNamepsace;
            this.properties = properties;
        }
        public ClassEntity(string name)
        {
            this.name = name;
        }

        public void AddMethod(Method method)
        {
            this.methods.Add(method);
            method.SetOwnerClass(this);
        }

    }
}