
namespace Domain.CodeInfo.InstanceDefinitions
{
    /// <summary>
    /// This class will handle information regarding the inheritance all found classes have
    /// To know the inherited classes of a class, we need to provide the class name we want
    /// and this will return a list containing all the inherited types from this class
    /// </summary>
    public class InheritanceDictionaryManager
    {
        private static InheritanceDictionaryManager _instance;
        private InheritanceDictionaryManager()
        {
        }

        public static InheritanceDictionaryManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new InheritanceDictionaryManager();
                }
                return _instance;
            }
        }

        public void CleanInheritanceDictionary()
        {
            _instance._inheritanceDictionary.Clear();
        }

        /// <summary>
        /// Dictionary used to be able to provide a class name, and get its inherited classes
        /// </summary>
        private Dictionary<string, List<string>> _inheritanceDictionary = new();
        public IReadOnlyDictionary<string, List<string>> inheritanceDictionary => _inheritanceDictionary.AsReadOnly();

        /// <summary>
        /// Receives data with a class and its inheritance from the class that handles the raw data processing 
        /// and stores it
        /// </summary>
        /// <param name="classWithInheritance">String array that should contain the name of the classes 
        /// as the first value, and the other values are the inherited classes </param>
        public void ReceiveClassDeclaration(string[] classWithInheritance)
        {
            List<string> receivedClassInheritedClasses = new List<string>();

            var inheritanceDictList = _inheritanceDictionary.ToList();
            // We first pass through all the registered classes with inheritance to find if the received class is a parent of another class, and place the parents of this class as the parents of the other class too(since they are the grandparents), we need to get the grandparents of the received class too
            for (int j = 0; j < inheritanceDictList.Count; j++)
            {
                // If the received class is a parent of a registered class, then we've found the grandparents of this class and we must add its parents too
                if (inheritanceDictList[j].Value.Contains(classWithInheritance[0]))
                {
                    for (global::System.Int32 i = 1; i < classWithInheritance.Count(); i++)
                    {
                        inheritanceDictList[j].Value.Add(classWithInheritance[i]);
                    }
                }
                // Now we pass through all the inherited classes from the received class to do the same check
                for (global::System.Int32 i = 1; i < classWithInheritance.Count(); i++)
                {
                    // If one of the parents of the received class has the name of a registered class, then we add the parents of the registered class(the grandparents of the received class)
                    if (classWithInheritance[i].CompareTo(inheritanceDictList[j].Key) == 0)
                    {
                        foreach(var grandParent in inheritanceDictList[j].Value)
                        {
                            receivedClassInheritedClasses.Add(grandParent);
                        }
                    }
                }
            }

            // Now we add the new class to the inheritanceDictionary
            // We get all the inheritance classes and put them in the inheritance List this class has if any
            for (int i = 1; i < classWithInheritance.Count(); i++)
            {
                receivedClassInheritedClasses.Add(classWithInheritance[i]);
            }

            // Add the new entry to the inheritanceDictionary
            _inheritanceDictionary.Add(classWithInheritance[0], receivedClassInheritedClasses);
        }

    }
}