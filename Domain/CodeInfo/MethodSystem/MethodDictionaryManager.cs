
using System.Reflection;

namespace Domain.CodeInfo.MethodSystem
{
    /// <summary>
    /// This class will handle information regarding the method all found classes have
    /// To know the inherited classes of a class, we need to provide the class name we want
    /// and this will return a list containing all the inherited types from this class
    /// </summary>
    public class MethodDictionaryManager
    {
        private static MethodDictionaryManager _instance;
        private MethodDictionaryManager()
        {
        }

        public static MethodDictionaryManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MethodDictionaryManager();
                }
                return _instance;
            }
        }

        public void CleanMethodDictionary()
        {
            _instance._methodDictionary.Clear();
        }

        /// <summary>
        /// Dictionary that contains has as a key the identification(signature) 
        /// of a Method, and as its value the Method itself
        /// </summary>
        private Dictionary<MethodIdentifier, Method> _methodDictionary = new();
        public IReadOnlyDictionary<MethodIdentifier, Method> methodDictionary => _methodDictionary.AsReadOnly();

        public void AddMethod(Method createdMethod)
        {
            _methodDictionary.Add(createdMethod.GetMethodIdentifier(), createdMethod);
        }
        /// <summary>
        /// Method to clear the dictionary that stored the Methods with all their data, but 
        /// when we need to find and match Methods with MethodInstances with less data then
        /// we clear the dictionary, and after chaning the way elements are stored in the dictionary
        /// then we can store the elements again
        /// </summary>
        public void CleanAndFillDictionary()
        {
            var originalMethodDictiionary = new List<Method>(_methodDictionary.Values.ToList());
            CleanMethodDictionary();
            foreach(var method in originalMethodDictiionary)
            {
                AddMethod(method);
            }
        }
    }
}