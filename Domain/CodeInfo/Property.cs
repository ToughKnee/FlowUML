using System.Reflection.Metadata;

namespace Domain.CodeInfo
{
    /// <summary>
    /// Represents a way to identify a function more easily
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Type of the property
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// Name of the property
        /// </summary>
        public string name{ get; set; }
        public Property(string type, string name)
        {
            this.type = type;
            this.name = name;
        }

        public override string ToString()
        {
            return type + " " + name;
        }
    }
}
