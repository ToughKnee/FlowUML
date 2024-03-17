using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Domain.CodeInfo.MethodSystem;
using System.Text.RegularExpressions;

namespace Infrastructure.Builders
{
    /// <summary>
    /// Method builder used by the antlr visitor and be passed to anothre class which 
    /// uses it as an abstract builder and get the finished Method
    /// </summary>
    public class MethodBuilder : AbstractBuilder<Method>
    {
        public string name { get; private set; }
        public string? returnType { get; private set; } = null;
        public string? belongingNamespace { get; private set; } = null;
        public List<string> parameters { get; private set; } = new List<string>();
        public List<Callsite> callsites { get; private set; } = new List<Callsite>();
        public List<Typename>? typenames { get; private set; } = null;

        /// <summary>
        /// This Build method must NOT be used because this is called when the ClassEntityBuilder Build method is called
        /// This can only be called if the method is a lone method, does not have a class owner
        /// </summary>
        /// <returns></returns>
        public Method Build()
        {
            Method method = new Method(belongingNamespace, name, parameters, returnType, callsites);
            // Setting the typenames if any
            method.typenames = this.typenames;

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
        public MethodBuilder SetParameters(string parameters)
        {
            List<string> result = new List<string>();

            // Using regex in order to split the parameters in the string using ',', but only if the ',' are not inside diamonds, to also cover template typename types 
            Regex regex = new Regex(@"[^<>,]+(?:<[^<>]+>)?"); // Regular expression to match segments
            foreach (Match match in regex.Matches(parameters))
            {
                result.Add(match.Value);
            }
            this.parameters = result;
            return this;
        }
        public MethodBuilder AddCallsite(Callsite callsite)
        {
            this.callsites.Add(callsite);
            return this;
        }
        public MethodBuilder SetTypename(List<Typename> typenameList)
        {
            typenames = typenameList;
            return this;
        }
    }
}
