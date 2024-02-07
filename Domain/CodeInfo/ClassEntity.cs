using Domain.CodeInfo.InstanceDefinitions;

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
            ClassEntityManager.instance.ReceiveNewClassEntityInstance(this);
        }
        public ClassEntity(string name, string belongingNamepsace, List<Property> properties)
        {
            this.name = name;
            this.classNamespace = belongingNamepsace;
            this.properties = properties;
            ClassEntityManager.instance.ReceiveNewClassEntityInstance(this);
        }
        public ClassEntity(string name)
        {
            this.name = name;
            ClassEntityManager.instance.ReceiveNewClassEntityInstance(this);
        }

        public void AddMethod(Method method)
        {
            this.methods.Add(method);
            method.SetOwnerClass(this);
        }

        public static bool operator ==(ClassEntity obj1, ClassEntity obj2)
        {
            if (obj1.name == obj2.name && obj1.classNamespace == obj2.classNamespace)
                return true;

            return false;
        }

        public static bool operator !=(ClassEntity obj1, ClassEntity obj2)
        {
            if (obj1.name == obj2.name && obj1.classNamespace == obj2.classNamespace)
                return false;

            return true;
        }
    }
}