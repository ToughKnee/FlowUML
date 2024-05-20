using Antlr4.Runtime.Tree;
using Domain.CodeInfo.InstanceDefinitions;
using Domain.CodeInfo.MethodSystem;
using Infrastructure.Mediators;

namespace Infrastructure.Builders
{
    public class MethodInstanceBuilder : InstanceBuilder
    {
        /// <summary>
        /// This is set to true if the method "SetMethodCallChainedInstance" is called and there is a 
        /// methodInstanceBuilder in the parameters of this
        /// Used to know if we must add the callsite right now or after the parameters have been built
        /// </summary>
        private bool _hasMethodCallParameters = false;
        private string _methodName;
        // private AbstractInstance? _callerClass = null;
        /// <summary>
        /// List of objects that represents the parameters, which can only be of kind "Instance"
        /// or "MethodInstanceBuilder", this is like this to be able to place the parameters
        /// in the correct order in the Callsites of the Methods when we are defining all the callsites, 
        /// following the order where parameters are placed first, then the method that owns the params,
        /// and finally the chained methodCall if present
        /// </summary>
        private List<object> _calledParametersObjects = new();
        // private AbstractInstance? _callerClassChainedInstance = null;
        private AbstractInstance? _ownedChainedInstance = null;
        private MethodInstanceBuilder? _ownedChainedMethodInstanceBuilder = null;
        // private AbstractInstance? _indexRetrievalInstance = null;
        public MethodBuilder linkedMethodBuilder;
        private Callsite _linkedCallsite;

        public MethodInstanceBuilder(IMediator mediator)
        {
            _mediator = mediator;
            _knownInstancesDeclaredInCurrentMethodAnalysis = mediator.GetKnownInstancesDeclaredInCurrentMethodAnalysis();
        }

        public AbstractInstance Build()
        {
            // Seting the chained instance of the caller class
            if(_callerClass != null && _callerClassChainedInstance != null)
                _callerClass.chainedInstance = _callerClassChainedInstance;

            // Converting the object parameters into Instances, and building the methodCalls to follow the correct order of callsites
            List<AbstractInstance> parametersInstances = new();
            foreach(var objectParam in _calledParametersObjects)
            {
                if (objectParam is Instance)
                    parametersInstances.Add((Instance)objectParam);
                else if(objectParam is MethodInstanceBuilder)
                    parametersInstances.Add(((MethodInstanceBuilder)objectParam).Build());
            }
            
            var methodInstance = new MethodInstance(_callerClass
                , (_ownedChainedMethodInstanceBuilder != null) ? ((MethodInstance)_ownedChainedMethodInstanceBuilder.Build()).callerClass : ((_ownedChainedInstance != null) ? (_ownedChainedInstance) : (null))
                , _methodName
                , parametersInstances
                , _linkedCallsite
                , _methodInstanceKind
                , _mediator.GetUsedNamespaces());
            methodInstance.indexRetrievedInstance = this._indexRetrievalInstance;
            methodInstance.refType.data = _type;
            return methodInstance;
        }
        public MethodInstanceBuilder SetMethodName(string methodName)
        {
            this._methodName = methodName;
            return this;
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
                        parameterInstance.chainedInstance = AbstractInstance.GeneratePropertyChain(currentStringInstance, currentStringInstancePropertyChain);

                    // Adding the parameter to the method instance parameters list
                    _calledParametersObjects.Add(parameterInstance);
                }
                // If the parameter to manage is another MethodCall, then just add the bulder to the parameters list to be later built
                else if (calledParameters[i] is MethodInstanceBuilder)
                {
                   _calledParametersObjects.Add(calledParameters[i]);
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
            _hasMethodCallParameters = true;
            this._ownedChainedMethodInstanceBuilder = chainedMethodCall;
            chainedMethodCall.SetCallerClassName(null);
            chainedMethodCall._callerClass.kind = KindOfInstance.IsFromLinkedMethodInstance;
            // If the caller class of the owned chain methodCall has defined a type then remove it
            if (String.IsNullOrEmpty(chainedMethodCall._callerClass.name) && !String.IsNullOrEmpty(chainedMethodCall._callerClass.refType.data))
            {
                chainedMethodCall._callerClass.refType.data = "";
            }
            // If the chained caller class has a name(which means that the caller class is actually a property from this MethodCall), then remove the "IsFromLinkedMethodInstance" kind and replace it with "IsPropertyFromInheritanceOrInThisClass", to let the true type of the property be discovered by the MethodInstance
            else if(!String.IsNullOrEmpty(chainedMethodCall._callerClass.name))
                chainedMethodCall._callerClass.kind = KindOfInstance.IsPropertyFromInheritanceOrInThisClass;
            return this;
        }
        public MethodInstanceBuilder SetNormalInstanceChainedInstance(Instance chainedInstance)
        {
            this._ownedChainedInstance = chainedInstance;
            return this;
        }
        public MethodInstanceBuilder SetIndexRetrievalInstance(Instance indexRetrievalInstance)
        {
            this._indexRetrievalInstance = indexRetrievalInstance;
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

            // If there are no parameters that go before this method call, then Make the callsite for the method
            if (_hasMethodCallParameters != true)
            {
                _linkedCallsite = new Callsite(null);
                linkedMethodBuilder.AddCallsite(_linkedCallsite);
            }

            return this;
        }
        public MethodInstanceBuilder SetMethodKind(KindOfInstance kind)
        {
            this._methodInstanceKind = kind;
            return this;
        }
    }
}