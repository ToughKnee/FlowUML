using Antlr4.Runtime.Tree;
using Domain.CodeInfo.InstanceDefinitions;
using Domain.CodeInfo.MethodSystem;
using Infrastructure.Mediators;

namespace Infrastructure.Builders
{
    /// <summary>
    /// Instance Builder usd by the c sharp visitor just like the method instnace builder
    /// where these builders are also passed to the Mediator
    /// This Builder only focuses on one part of the method instance builder which is the 
    /// class caller, even though in this kind of objects there are no call, the class callers
    /// are the "myObject" from the "myObject.prop1.prop2[0]"
    /// This is inherited by the method instnace builder because that part of the class callers
    /// has the same processing for both builders
    /// </summary>
    public class InstanceBuilder : AbstractBuilder<AbstractInstance>
    {
        protected AbstractInstance? _callerClass = null;
        protected AbstractInstance? _callerClassChainedInstance = null;
        protected AbstractInstance? _indexRetrievalInstance = null;
        private string? _type = null;
        protected KindOfInstance _methodInstanceKind = KindOfInstance.Normal;
        protected readonly string _paramIdentifier = "<p>";
        /// <summary>
        /// Used to get important data from the analysis of the code files which is 
        /// needed to build the methodInstance
        /// </summary>
        protected IMediator _mediator;
        /// <summary>
        /// Reado only dictionary from the mediator, used to know when there are instances with their defined type used 
        /// in a method and we must identify it
        /// </summary>
        protected IReadOnlyDictionary<string, AbstractInstance> _knownInstancesDeclaredInCurrentMethodAnalysis = new Dictionary<string, AbstractInstance>();

        public InstanceBuilder(IMediator mediator)
        {
            _mediator = mediator;
            _knownInstancesDeclaredInCurrentMethodAnalysis = mediator.GetKnownInstancesDeclaredInCurrentMethodAnalysis();
        }
        public InstanceBuilder()
        {
        }

        public AbstractInstance Build()
        {
            _callerClass.chainedInstance = _callerClassChainedInstance;
            if(_type != null)
            {
                _callerClass.kind = KindOfInstance.Normal;
                _callerClass.refType.data = _type;
            }
            AbstractInstance.GetLastChainedInstance(_callerClass).indexRetrievedInstance = _indexRetrievalInstance;
            return _callerClass;
        }
        /// <summary>
        /// Method that processes the raw information regarding a string instance and return the processed
        /// Instance with all the values set accoridng to the context it was found and its own information
        /// </summary>
        /// <param name="currentStringInstance">The raw string instance to be processed, this must not have 
        /// any chained properties(like "myStringInstance.myProperty", which would not be allowed)</param>
        /// <param name="currentAnalyzedClassName">Data regarding the context where the instance was found,
        /// used to be able to detect the type when this instance belongs to an inherited class</param>
        /// <returns></returns>
        protected Instance ProcessInstanceInformation(string currentStringInstance, string currentAnalyzedClassName)
        {
            if (currentAnalyzedClassName == null) currentAnalyzedClassName = "";
            if (currentStringInstance.Contains("."))
            {
                throw new ArgumentException("String instance should not contain chained properties");
            }

            var processedInstance = new Instance(currentStringInstance);
            processedInstance.inheritedClasses = null;

            // If the current instance is an element from an indexed list, then remove the part with brackets and look for the collection type in the known instances
            if (currentStringInstance.Contains("["))
            {
                currentStringInstance = currentStringInstance.Substring(0, currentStringInstance.IndexOf("["));
                if (_knownInstancesDeclaredInCurrentMethodAnalysis.TryGetValue(currentStringInstance, out AbstractInstance knownClassInstance))
                {
                    processedInstance.refType = knownClassInstance.refType;
                    processedInstance.kind = KindOfInstance.IsElementFromCollection;
                }
            }
            // If we already registered an instance with the same name of the className or parameter, then we link that instance to this method
            else if (_knownInstancesDeclaredInCurrentMethodAnalysis.TryGetValue(currentStringInstance, out AbstractInstance knownClassInstance))
            {
                // If this is a simple variable, then copy all the values of the real Instance that defined this simple variable
                if(!currentStringInstance.Contains("."))
                {
                    processedInstance.name = (knownClassInstance).name;
                    processedInstance.chainedInstance = knownClassInstance.chainedInstance;
                    processedInstance.indexRetrievedInstance = knownClassInstance.indexRetrievedInstance;
                }
                processedInstance.refType = knownClassInstance.refType;
                processedInstance.kind = knownClassInstance.kind;
            }
            // Check again but adding the parameter identifier if this instance was a parameter
            else if (_knownInstancesDeclaredInCurrentMethodAnalysis.TryGetValue(_paramIdentifier + currentStringInstance, out AbstractInstance knownClassInstanceParam))
            {
                processedInstance.refType = knownClassInstanceParam.refType;
                processedInstance.kind = knownClassInstanceParam.kind;
            }
            // If that component wasn't in that dictionary, isn't empty and isn't the "this" nor "base" keyword, then this instance may come from a property of a parent class or is a static property and we must set that state using the HasClassNameStaticOrParentProperty kind
            else if (!String.IsNullOrEmpty(currentStringInstance) && currentStringInstance != "this" && currentStringInstance != "base")
            {
                processedInstance.kind = KindOfInstance.IsPropertyFromInheritanceOrInThisClass;
                // We set the inheritedClasses of the component to the inheritance of this current class we are analyzing since an inherited class may contain this component as its property
                if (InheritanceDictionaryManager.instance.inheritanceDictionary.TryGetValue(currentAnalyzedClassName, out List<string> inheritedClasses))
                    processedInstance.inheritedClasses = inheritedClasses.AsReadOnly();
                _methodInstanceKind = KindOfInstance.HasClassNameStatic;
            }
            // If this is not empty and is the "this" and isn't the "base" keyword, then this component is of type of the class we are analyzing
            else if (!String.IsNullOrEmpty(currentStringInstance) && currentStringInstance == "this")
            {
                processedInstance.refType.data = currentAnalyzedClassName;
            }
            // If this is the called class name component, is empty or is the "this" or "base" keyword, then this method uses inheritance to get its type and we set this inheritance information
            else if (String.IsNullOrEmpty(currentStringInstance) || currentStringInstance == "this" || currentStringInstance == "base")
            {
                processedInstance.kind = KindOfInstance.IsInheritedOrInThisClass;
                _methodInstanceKind = KindOfInstance.IsInheritedOrInThisClass;
                processedInstance.refType.data = currentAnalyzedClassName;
            }
            return processedInstance;
        }
        public InstanceBuilder SetCallerClassName(string callerClassName)
        {
            if(callerClassName == null)
            {
                return this;
            }
            // If the callerClassName has ".", then this caller class has a property chain and we must separate it from the starting class and all the other components in this chain, and for each component we create an Instance of kind PropertyFromInheritanceOrThisClass
            if ((callerClassName != null && callerClassName.Contains(".")))
            {
                // If there is a callerMethodInstance, then the calledClass is part of the propertyChain and it must be specified to the GeneratePropertyChain function, otherwise then it is not part of the chain
                string classOwner = callerClassName.Split(".")[0];

                // Setting the chainedInstance of the CALLER CLASS instance
                _callerClassChainedInstance = AbstractInstance.GeneratePropertyChain(classOwner, callerClassName);
                callerClassName = classOwner;
            }

            this._callerClass = ProcessInstanceInformation(callerClassName, _mediator.GetCurrentAnalyzedClassName());
            return this;
        }
        public InstanceBuilder SetIndexRetrievalInstance(AbstractInstance indexRetrievalInstance)
        {
            this._indexRetrievalInstance = indexRetrievalInstance;
            return this;
        }
        public InstanceBuilder SetType(string type)
        {
            this._type = type;
            return this;
        }
    }
}