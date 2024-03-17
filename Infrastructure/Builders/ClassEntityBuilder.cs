using Antlr4.Runtime.Tree;
using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Domain.CodeInfo.MethodSystem;

namespace Infrastructure.Builders
{
    /// <summary>
    /// Method builder used by the antlr visitor and be passed to another class which 
    /// uses it as an abstract builder and get the finished ClassEntity alongside the Methods
    /// </summary>
    public class ClassEntityBuilder : AbstractBuilder<ClassEntity>
    {
        public string name { get; private set; }
        public string? belongingNamespace { get; private set; } = null;
        public List<Property> properties { get; private set; } = new List<Property>();
        public List<MethodBuilder> methodBuilders { get; private set; } = new List<MethodBuilder>();
        public List<Typename>? typenames { get; private set; } = null;

        /// <summary>
        /// Creates the ClassEntity alognside the correspondent Methods from 
        /// the respective MethodBuilders by building the Methods also
        /// As well as passing the created Methods and ClassEntity to the Managers of those kind of objects
        /// </summary>
        /// <returns>The ClassEntity with the respective Methods</returns>
        public ClassEntity Build()
        {
            ClassEntity builtClass = new ClassEntity(name, belongingNamespace, properties);

            // Getting the inheritance of this class from the inheritanceDictionary
            if(InheritanceDictionaryManager.instance.inheritanceDictionary.TryGetValue(name, out List<string> inheritedClasses))
                builtClass.inheritedClasses = inheritedClasses.AsReadOnly();
            else
                builtClass.inheritedClasses = new List<string>();

            // Setting the typenames if any
            builtClass.typenames = this.typenames;

            // Building all the Methods from this ClassEntity
            foreach (var methodBuilder in methodBuilders)
            {
                var method = methodBuilder.Build();
                builtClass.AddMethod(method);
                MethodDictionaryManager.instance.AddMethod(method);
            }

            ClassEntityManager.instance.AddClassEntityInstance(builtClass);
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
        public ClassEntityBuilder SetTypename(List<Typename> typenameList)
        {
            typenames = typenameList;
            return this;
        }
    }
}
