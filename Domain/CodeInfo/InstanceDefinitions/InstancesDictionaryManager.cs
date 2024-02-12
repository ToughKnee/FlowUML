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
        private Dictionary<AbstractInstance, AbstractInstance?> _instancesDictionary = new Dictionary<AbstractInstance, AbstractInstance?>();
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
        /// <param name="assignee"></param>
        /// <param name="assigner"></param>
        public void AddSimpleAssignment(Instance assignee, AbstractInstance assigner)
        {
            _instancesDictionary.Add(assignee, assigner);
        }
        /// <summary>
        /// Adds an instance with proper definition to the instances dictionary, 
        /// receiving things like properties or parameters
        /// </summary>
        /// <param name="definedInstance">The defined instance</param>
        public void AddInstanceWithDefinedType(AbstractInstance definedInstance)
        {
            _instancesDictionary.Add(definedInstance, null);
        }
        //===========================  Handling methods
        // Methods are complex parts of code which need to be identified with 3 things: Class owner, Method name, and parameters
        // With those 3 components we are able to store methods to the instancesDictionary, we need to identify methods this way
        // since methods will be received in 3 ways primarily:
        // The declaration of a method, which contains all the components plus the return type of the method, which is very important
        // Variable assignment, where the method has a return type which a variable receives
        // Simple Method call, as in a procedure
        // Thus methods are going to be handled with those 3 components and not more

        /// <summary>
        /// Adds the definition of a method which is called when a ClassEntity is being defined too,
        /// if it comes from a ClassEntity
        /// This adds the method itself to the instancesDictionary as a key, and as a value will 
        /// be the return type of this method
        /// </summary>
        /// <param name="method">Method from a ClassEntity definition or just a lone function definition</param>
        /*TODO: Complete this method*/
        public void AddMethodDeclaration(string instanceAssigneeName, string methodCallAssigner)
        {
        }
        /// <summary>
        /// When there is an assignment inside a method, we registerit into the instancesDictionary
        /// The parameters should be received with their custom id's, the id of a methodCall should only contain 
        /// </summary>
        /// <param name="instanceAssignee"></param>
        /// <param name="methodCallAssigner"></param>
        public void AddMethodAssignment(Instance instanceAssignee, MethodInstance methodCallAssigner)
        {
            instanceAssignee.type = methodCallAssigner.returnType;
            _instancesDictionary.Add(instanceAssignee, methodCallAssigner);
        }
        /// <summary>
        /// This method adds a single methodInstance which only needs to be cleaned from the aliases
        /// </summary>
        /// <param name="methodInstance"></param>
        public void AddMethodCallInstance(MethodInstance methodInstance)
        {
            _instancesDictionary.Add(methodInstance, null);
        }
    }
}