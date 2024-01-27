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
        /// Properties of the class
        /// </summary>
        public List<Property> properties { get; private set; } = new List<Property>();
        /// <summary>
        /// Methods of the class
        /// </summary>
        public List<Method> methods { get; private set; } = new List<Method>();
        
        public ClassEntity(string name)
        {
            this.name = name;
        }

        public void AddMethod(Method method)
        {
            //InstancesDictionaryManager.instance.AddAssignation()
        }
    }
}