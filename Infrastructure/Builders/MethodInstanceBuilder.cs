using Antlr4.Runtime.Tree;
using Domain.CodeInfo.InstanceDefinitions;
using Domain.CodeInfo.MethodSystem;
using Infrastructure.Mediators;

namespace Infrastructure.Builders
{
    public class MethodInstanceBuilder : AbstractBuilder<MethodInstance>
    {
        private string _methodName;
        private AbstractInstance? _callerClass = null;
        private List<AbstractInstance> _calledParametersInstances = new();
        private AbstractInstance? _callerClassChainedInstance = null;
        private AbstractInstance? _ownedChainedInstance = null;
        private MethodInstanceBuilder? _ownedChainedMethodInstanceBuilder = null;
        public MethodBuilder linkedMethodBuilder;
        private KindOfInstance _methodInstanceKind = KindOfInstance.Normal;
        public bool isConstructor = false;
        private readonly string _paramIdentifier = "<p>";
        /// <summary>
        /// Used to get important data from the analysis of the code files which is 
        /// needed to build the methodInstance
        /// </summary>
        private IMediator _mediator;

        /// <summary>
        /// Reado only dictionary from the mediator, used to know when there are instances with their defined type used 
        /// in a method and we must identify it
        /// </summary>
        private IReadOnlyDictionary<string, AbstractInstance> _knownInstancesDeclaredInCurrentMethodAnalysis = new Dictionary<string, AbstractInstance>();

        public MethodInstanceBuilder(IMediator mediator)
        {
            _mediator = mediator;
            _knownInstancesDeclaredInCurrentMethodAnalysis = mediator.GetKnownInstancesDeclaredInCurrentMethodAnalysis();
        }

        public MethodInstance Build()
        {
            if (isConstructor)
                _methodInstanceKind = KindOfInstance.IsConstructor;

            // Make the callsite for the method
            var callsite = new Callsite(null);
            linkedMethodBuilder.AddCallsite(callsite);

            // Seting the chained instance of the caller class
            if(_callerClass != null)
                _callerClass.chainedInstance = _callerClassChainedInstance;

            var methodInstance = new MethodInstance(_callerClass
                // TODO: CHECK if the "_ownedChainedMethodInstanceBuilder.Build().callerClass" works when the 1st methodCall found its actual function and now has to set the type of the caller class of the chained MethodCall
                , (_ownedChainedMethodInstanceBuilder != null) ? (_ownedChainedMethodInstanceBuilder.Build().callerClass) : ((_ownedChainedInstance != null) ? (_ownedChainedInstance) : (null))
                , _methodName
                , _calledParametersInstances
                , callsite
                , _methodInstanceKind
                , _mediator.GetUsedNamespaces());
            return methodInstance;
        }

        /// <summary>
        /// Generate the whole propertyChain from the provided strings, and it avoids to put inside 
        /// the propertyChain the class that owns the propertyChain
        /// </summary>
        /// <param name="ownerName">The name of the Instance that will be the
        /// head of this propertyChain</param>
        /// <param name="propertyChainString">The text that represents the whole propertyChain as 
        /// a raw string, which will be converted into multiple chained AbstractInstances</param>
        /// <returns>The first Instance that has all the other properties chained</returns>
        public AbstractInstance GeneratePropertyChain(string ownerName, string propertyChainString)
        {
            var methodInstancePropertyChainString = propertyChainString.Split(".");
            AbstractInstance previousInstance = null;
            AbstractInstance firstChainedInstance = null;
            foreach (string componentString in methodInstancePropertyChainString)
            {
                // If the current component isn't the caller class, then it is part of the propertyChain
                if (componentString != ownerName)
                {
                    var component = new Instance(componentString);
                    component.kind = KindOfInstance.IsPropertyFromInheritanceOrInThisClass;
                    // If the previous component is null, then this is the first element of the propertyChain which will be received by the MethodInstance constructor
                    if (previousInstance is null)
                        firstChainedInstance = component;
                    // If not then we need to chain all the properties properly
                    else
                        previousInstance.chainedInstance = component;
                    previousInstance = component;
                }
            }
            return firstChainedInstance;
        }

        public MethodInstanceBuilder SetCallerClassName(string callerClassName, bool methodInstanceIsPartOfAnotherChain = false)
        {
            if(callerClassName == null)
            {
                return this;
            }
            // If the callerClassName has "." {TODO:Remove this{or there is a linkedMethodCaller}}, then this caller class has a property chain and we must separate it from the starting class and all the other components in this chain, and for each component we create an Instance of kind PropertyFromInheritanceOrThisClass
            if ((callerClassName != null && callerClassName.Contains(".")) || methodInstanceIsPartOfAnotherChain)
            {
                // If there is a callerMethodInstance, then the calledClass is part of the propertyChain and it must be specified to the GeneratePropertyChain function, otherwise then it is not part of the chain
                string classOwner = callerClassName.Split(".")[0];
                if (methodInstanceIsPartOfAnotherChain)
                    classOwner = "";

                // Setting the chainedInstance of the CALLER CLASS instance
                _callerClassChainedInstance = GeneratePropertyChain(classOwner, callerClassName);
                callerClassName = classOwner;
            }

            this._callerClass = ProcessInstanceInformation(callerClassName, _mediator.GetCurrentAnalyzedClassName());
            return this;
        }
        public MethodInstanceBuilder SetMethodName(string methodName)
        {
            this._methodName = methodName;
            return this;
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
        private Instance ProcessInstanceInformation(string currentStringInstance, string currentAnalyzedClassName)
        {
            if(currentStringInstance.Contains("."))
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
                // TODO: Check if the class name used for the query works
                processedInstance.inheritedClasses = InheritanceDictionaryManager.instance.inheritanceDictionary[currentAnalyzedClassName];
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
        public MethodInstanceBuilder SetParameters(List<object> calledParameters)
        {
            AbstractInstance parameterInstance = null;

            // Process each parameter and class name according to what it is(based on its position in the List
            for (int i = 0; i < calledParameters.Count; i++)
            {
                if (calledParameters[i] is string)
                {
                    string currentStringInstance = (string)calledParameters[i];
                    string currentStringInstancePropertyChain = "";

                    // Check if there are parameters that are a propertyChain, and if so, mark its kind and set the information for the MethodInstance to solve their types
                    if (currentStringInstance.Contains("."))
                    {
                        currentStringInstancePropertyChain = currentStringInstance;
                        currentStringInstance = currentStringInstance.Split(".")[0];
                    }

                    // Process the raw string instance to get the processed Instance with all its values set according to the context it was found and its own information
                    parameterInstance = ProcessInstanceInformation(currentStringInstance, _mediator.GetCurrentAnalyzedClassName());

                    // After the current Instance has been properly set, then generate its propertyChain
                    if (!String.IsNullOrEmpty(currentStringInstancePropertyChain))
                        parameterInstance.chainedInstance = GeneratePropertyChain(currentStringInstance, currentStringInstancePropertyChain);

                    // Adding the parameter to the method instance parameters list
                    _calledParametersInstances.Add(parameterInstance);
                }
                // If the parameter to manage is another MethodCall, then generate the Data and link the generated MethodInstance to this MethodCall
                else if (calledParameters[i] is MethodInstanceBuilder)
                {
                    var parameterMethodCall = ((MethodInstanceBuilder)calledParameters[i]).Build();
                    _calledParametersInstances.Add(parameterMethodCall);
                }
                else
                {
                    throw new Exception("The instance received isn't either a string nor a MethodCallData");
                }
            }

            return this;
        }
        public MethodInstanceBuilder SetMethodCallChainedInstance(MethodInstanceBuilder chainedMethodCall)
        {
            this._ownedChainedMethodInstanceBuilder = chainedMethodCall;
            chainedMethodCall.SetCallerClassName(null);
            chainedMethodCall._callerClass.kind = KindOfInstance.IsFromLinkedMethodInstance;
            // If the caller class of the owned chain methodCall has defined a type then remove it
            if (String.IsNullOrEmpty(chainedMethodCall._callerClass.name) && !String.IsNullOrEmpty(chainedMethodCall._callerClass.refType.data))
            {
                chainedMethodCall._callerClass.refType.data = "";
            }
            // If the chained caller class has a name, then remove the "IsFromLinkedMethodInstance" kind and replace it with "IsPropertyFromInheritanceOrInThisClass", to let the true type of the property be discovered by the MethodInstance
            else
                chainedMethodCall._callerClass.kind = KindOfInstance.IsPropertyFromInheritanceOrInThisClass;
            return this;
        }
        public MethodInstanceBuilder SetNormalInstanceChainedInstance(string chainedInstance)
        {
            this._ownedChainedInstance = new Instance(chainedInstance);
            return this;
        }
        /// <summary>
        /// Sets the methodBuilder which makes able the link between each callsite(which are the MethodInstances)
        /// and the actual Method that was called, this is used when the MethodInstance is built
        /// </summary>
        /// <param name="methodBuilder"></param>
        /// <returns></returns>
        public MethodInstanceBuilder SetLinkedMethodBuilder(MethodBuilder methodBuilder)
        {
            this.linkedMethodBuilder = methodBuilder;
            return this;
        }
        public MethodInstanceBuilder SetConstructorMethodKind(bool isConstructor)
        {
            this.isConstructor = isConstructor;
            return this;
        }
    }
}