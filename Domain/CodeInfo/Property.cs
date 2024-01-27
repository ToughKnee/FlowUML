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
        /// The class that has this method's definition, if null then this is just a function
        /// </summary>
        public ClassEntity? ownerClass { get; set; }
        public Property()
        {
        }
    }
}
