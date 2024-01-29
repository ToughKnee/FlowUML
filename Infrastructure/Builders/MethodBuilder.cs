using Domain.CodeInfo;

namespace Infrastructure.Builders
{
    /// <summary>
    /// Method builder used by the antlr visitor and be passed to anothre class which 
    /// uses it as an abstract builder and get the finished Method
    /// </summary>
    public class MethodBuilder : AbstractBuilder<Method>
    {
        public string name { get; private set; }
        public string ownerClass { get; private set; }
        public string? returnType { get; private set; } = null;
        public string? belongingNamespace { get; private set; } = null;
        public List<string> parameters { get; private set; } = new List<string>();
        public List<Callsite> callsites { get; private set; } = new List<Callsite>();

        /// <summary>
        /// This Build method must NOT be used because this is called when the ClassEntityBuilder Build method is called
        /// This can only be called if the method is a lone method, does not have a class owner
        /// </summary>
        /// <returns></returns>
        public Method Build()
        {
            ClassEntity owner = new ClassEntity(ownerClass);
            Method method = new Method(belongingNamespace, owner, name, parameters, returnType, callsites);
            return method;
        }
        public MethodBuilder SetName(string name)
        {
            this.name = name;
            return this;
        }
        public MethodBuilder SetReturnType(string retType)
        {
            this.returnType = retType;
            return this;
        }
        public MethodBuilder SetBelongingNamespace(string? belongingNamespace)
        {
            this.belongingNamespace = belongingNamespace;
            return this;
        }
        // TODO: =================  DELETEME 
        public MethodBuilder SetOwnerClass(string ownerClass)
        {
            this.ownerClass = ownerClass;
            return this;
        }
        public MethodBuilder SetParameters(string parameters)
        {
            this.parameters = parameters.Split(",").ToList();
            return this;
        }
        public MethodBuilder AddCallsite(Callsite callsite)
        {
            this.callsites.Add(callsite);
            return this;
        }
    }
}
