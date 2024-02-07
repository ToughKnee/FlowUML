using System;

namespace Domain.CodeInfo.InstanceDefinitions
{
    public class InstancesDictionaryManager
    {
        /// <summary>
        /// This will make the link between variables with a name used inside methods,
        /// to a defined type of a method that may belong to a class and will allow
        /// to make the link between ClassEntitites calling other ClassEntities
        /// If the value of a key is "null", then this means that the key has the 
        /// type defined and the search to know the type of an instance is over
        /// </summary>
        public Dictionary<AbstractInstance, AbstractInstance?> instancesDictionary { get; private set; } = new Dictionary<AbstractInstance, AbstractInstance>();

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

        public void CleanInstancesDictionary()
        {
            _instance.instancesDictionary.Clear();
        }

        /// <summary>
        /// Add an assignment of a variable or parameter to the Dictionary 
        /// </summary>
        /// <param name="instanceAssigneeName"></param>
        /// <param name="instanceAssignerName"></param>
        public void AddAssignment(string instanceAssigneeName, string instanceAssignerName, bool hasType)
        {
            Instance instanceAssignee;
            if (hasType)
            {
                instanceAssignee = new Instance(instanceAssigneeName, instanceAssignerName);
                instancesDictionary.Add(instanceAssignee, null);
            }
            else
            {
                instanceAssignee = new Instance(instanceAssigneeName);
                var instanceAssigner = new Instance(instanceAssignerName);
                instancesDictionary.Add(instanceAssignee, instanceAssigner);
            }
        }
        /// <summary>
        /// Adds the definition of a method which is called when a ClassEntity is being defined too,
        /// if it comes from a ClassEntity
        /// This adds the method itself to the instancesDictionary as a key, and as a value will 
        /// be the return type of this method
        /// </summary>
        /// <param name="method">Method from a ClassEntity definition or just a lone function definition</param>
        public void AddMethodAssignment(string instanceAssigneeName, string methodCallAssigner)
        {
        }
    }
}