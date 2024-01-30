using Domain.CodeInfo.InstanceDefinitions;

namespace Domain.CodeInfo
{
    public class ClassEntity
    {

        // TODO: =================  MOVE this ClassEntity management into an interface  to be able to have multiple implementations more easily
        /// <summary>
        /// Creates a new ClassEntity with the given parameters only if there 
        /// wasn't a ClassEntity with that same identfiier
        /// Mainly used by the instancesManager or Instances to check get the ClassEntity 
        /// the know exists but don't know if has been already defined 
        /// </summary>
        /// <param name="belongingNamepsace"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static ClassEntity GetAlreadyRegisteredClassIfAny(string belongingNamepsace, string identifier)
        {

        }
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
        /// <summary>
        /// Contatins all the class entities that 
        /// </summary>
        public static List<ClassEntity> registeredClassEntities { get; private set; } = new List<ClassEntity>();

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