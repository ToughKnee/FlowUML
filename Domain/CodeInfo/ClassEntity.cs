using Domain.CodeInfo.InstanceDefinitions;
using Domain.CodeInfo.MethodSystem;
using System.Text;

namespace Domain.CodeInfo
{
    public class ClassEntity
    {
        /// <summary>
        /// Name of the class
        /// </summary>
        public string name { get; private set; }
        /// <summary>
        /// Namespace or package in which the class was found
        /// </summary>
        public string? classNamespace { get; private set; }
        /// <summary>
        /// Properties of the class
        /// </summary>
        public List<Property> properties { get; private set; } = new List<Property>();
        /// <summary>
        /// Methods of the class
        /// </summary>
        public List<Method> methods { get; private set; } = new List<Method>();
        /// <summary>
        /// List containing the names of the inherited classes found in the code 
        /// of this class and also its own class name, provided by the inheritanceDictionary
        /// </summary>
        public IReadOnlyCollection<string> inheritedClasses { get; set; }

        public ClassEntity(string name, string belongingNamepsace, List<Property> properties, List<Method> methods)
        {
            this.name = name;
            this.classNamespace = belongingNamepsace;
            this.properties = properties;
            this.methods = methods;
        }
        public ClassEntity(string name, string belongingNamepsace, List<Property> properties)
        {
            this.name = name;
            this.classNamespace = belongingNamepsace;
            this.properties = properties;
        }
        public ClassEntity(string name)
        {
            this.name = name;
        }

        public void AddMethod(Method method)
        {
            this.methods.Add(method);
            method.SetOwnerClass(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Name: {name}");
            sb.AppendLine($"Namespace: {classNamespace}");
            
            sb.AppendLine("Properties:");
            foreach (var property in properties)
            {
                sb.AppendLine($"  {property.name}: {property.type}");
            }

            sb.AppendLine("Methods:");
            foreach (var method in methods)
            {
                sb.AppendLine($"  {method}");
            }

            if (inheritedClasses != null)
            {
                sb.AppendLine("Inherited Classes:");
                foreach (var inheritedClass in inheritedClasses)
                {
                    sb.AppendLine($"  {inheritedClass}");
                }
            }

            return sb.ToString();
        }

    }
}