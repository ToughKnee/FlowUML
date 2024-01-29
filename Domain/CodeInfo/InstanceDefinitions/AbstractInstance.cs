
namespace Domain.CodeInfo
{
    /// <summary>
    /// An instance is a variable, parameter or method return type that has a type which we need to know when a ClassEntity calls another ClassEntity
    /// </summary>
    public abstract class AbstractInstance
    {
        /// <summary>
        /// The name which, basically is a variable/parameter/property/method that has a 
        /// given Type provided by another Instance
        /// And this lets us map any variable to a ClassEntity eventually
        /// </summary>
        public virtual string name { get; set; }

        /// <summary>
        /// If null, this Instance is just an alias of another instance, if not null, then this Instance 
        /// has been assigned by a method or constructor and is not an alias of another Instance
        /// and we know its return type
        /// </summary>
        public ClassEntity? implementation { get; set; }

        public AbstractInstance(string name, ClassEntity? implementation)
        {
            this.name = name;
            this.implementation = implementation;
        }
        public AbstractInstance(string name)
        {
            this.name = name;
            this.implementation = null;
        }
        public AbstractInstance()
        {
            this.implementation = null;
        }

        public static bool operator ==(AbstractInstance obj1, AbstractInstance obj2)
        {
            if (obj1.implementation == obj2.implementation || obj1.name == obj2.name)
                return true;

            return false;
        }

        public static bool operator !=(AbstractInstance obj1, AbstractInstance obj2)
        {
            if (obj1.implementation == obj2.implementation || obj1.name == obj2.name)
                return false;

            return true;
        }
    }
}