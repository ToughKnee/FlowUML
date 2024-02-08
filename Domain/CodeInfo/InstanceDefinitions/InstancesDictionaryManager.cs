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
        private Dictionary<AbstractInstance, AbstractInstance?> _instancesDictionary = new Dictionary<AbstractInstance, AbstractInstance>();
        public IReadOnlyDictionary<AbstractInstance, AbstractInstance?> instancesDictionary => _instancesDictionary.AsReadOnly();

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
            _instance._instancesDictionary.Clear();
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
                _instancesDictionary.Add(instanceAssignee, null);
            }
            else
            {
                instanceAssignee = new Instance(instanceAssigneeName);
                var instanceAssigner = new Instance(instanceAssignerName);
                _instancesDictionary.Add(instanceAssignee, instanceAssigner);
            }
        }
        public void AddMethodAssignment(string instanceAssigneeName, MethodInstance methodCallAssigner)
        {
            var instanceAssignee = new Instance(instanceAssigneeName);
            _instancesDictionary.Add(instanceAssignee, methodCallAssigner);
        }
        /// <summary>
        /// Adds the definition of a method which is called when a ClassEntity is being defined too,
        /// if it comes from a ClassEntity
        /// This adds the method itself to the instancesDictionary as a key, and as a value will 
        /// be the return type of this method
        /// </summary>
        /// <param name="method">Method from a ClassEntity definition or just a lone function definition</param>
        /*TODO: Complete this method*/public void AddMethodDeclaration(string instanceAssigneeName, string methodCallAssigner)
        {
        }
        /// <summary>
        /// This method adds a single methodInstance which only needs to be cleaned from the aliases
        /// </summary>
        /// <param name="methodInstance"></param>
        public void AddMethodInstance(MethodInstance methodInstance)
        {
            _instancesDictionary.Add(methodInstance, null);
        }
    }
}