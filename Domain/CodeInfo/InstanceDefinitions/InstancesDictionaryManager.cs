namespace Domain.CodeInfo.InstanceDefinitions
{
    public class InstancesDictionaryManager
    {
        private static InstancesDictionaryManager _instance;
        private InstancesDictionaryManager()
        {

        }

        public static InstancesDictionaryManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new InstancesDictionaryManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// This will make the link between variables with a name used inside methods,
        /// to a defined implementation of a method that may belong to a class and will allow
        /// to make the link between ClassEntitites calling other ClassEntities
        /// </summary>
        public Dictionary<AbstractInstance, AbstractInstance> instancesDictionary { get; private set; } = new Dictionary<AbstractInstance, AbstractInstance>();


        /// <summary>
        /// Add an assignment to the Dictionary, this applies when an assignment happens in code
        /// </summary>
        /// <param name="instanceAssignee"></param>
        /// <param name="instanceAssigner"></param>
        public void AddAssignation(AbstractInstance instanceAssignee, AbstractInstance instanceAssigner)
        {
            instancesDictionary.Add(instanceAssignee, instanceAssigner);
        }
        /// <summary>
        /// Adds the definition of a method which is called when a ClassEntity is being defined also,
        /// if it comes from a ClassEntity
        /// This add the method itseld to the instancesDictionary as a key, and as a value will 
        /// be the return type of this method
        /// </summary>
        /// <param name="method">Method from a ClassEntity definition or just a lone function definition</param>
        public void ManageNewMethodDefinition(Method method)
        {
            var callsite = new Callsite(method);

        }
    }
}