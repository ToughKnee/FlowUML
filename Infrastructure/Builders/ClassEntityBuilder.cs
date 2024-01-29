using Domain.CodeInfo;

namespace Infrastructure.Builders
{
    /// <summary>
    /// Method builder used by the antlr visitor and be passed to anothre class which 
    /// uses it as an abstract builder and get the finished Method
    /// </summary>
    public class ClassEntityBuilder : AbstractBuilder<ClassEntity>
    {
        public string name { get; private set; }
        public string? belongingNamespace { get; private set; } = null;
        public List<Property> properties { get; private set; } = new List<Property>();
        public List<MethodBuilder> methodBuilders { get; private set; } = new List<MethodBuilder>();

        /// <summary>
        /// This Build must be ran instead of running the MethodBuiler Build method
        /// </summary>
        /// <returns></returns>
        public ClassEntity Build()
        {
            var methods = new List<Method>();
            foreach (var builder in methodBuilders)
            {
                methods.Add(builder.Build());
            }
            ClassEntity builtClass = new ClassEntity(name, belongingNamespace, properties, methods);
            return builtClass;
        }
        public ClassEntityBuilder SetName(string name)
        {
            this.name = name;
            return this;
        }
        public ClassEntityBuilder SetNamespace(string belongingNamespace)
        {
            this.belongingNamespace = belongingNamespace;
            return this;
        }
        public ClassEntityBuilder AddProperty(string type, string name)
        {
            this.properties.Add(new Property(type, name));
            return this;
        }
        public ClassEntityBuilder AddMethod(MethodBuilder methodBuilder)
        {
            this.methodBuilders.Add(methodBuilder);
            return this;
        }
    }
}
