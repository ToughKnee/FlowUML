
using System.Xml.Linq;
using System;

namespace Domain.CodeInfo.InstanceDefinitions
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
        /// List containging other classes, interfaces, etc this instance's class inherits, so that we can
        /// identify instances like properties or methods from parent instances that were used in this instance
        /// This list MUST be a reference of another list containing the classes which are inherited by this class,
        /// The List instance this will reference is in the inheritanceDictionary, where all the inheritance will
        /// be managed in there
        /// </summary>
        public IReadOnlyList<string>? inheritedClasses;

        /// <summary>
        /// If null, this Instance is just an alias of another instance, if not null, then this Instance 
        /// has been assigned by a method or constructor and is not an alias of another Instance
        /// and we know its return type
        /// </summary>
        public string? type { get; set; } = null;

        public AbstractInstance(string name, string? type)
        {
            this.name = name;
            this.type = type;
        }
        public AbstractInstance(string name)
        {
            this.name = name;
            this.type = null;
        }
        public AbstractInstance()
        {
        }


        public static bool operator ==(AbstractInstance obj1, AbstractInstance obj2)
        {
            if (obj1.type == obj2.type || obj1.name == obj2.name)
                return true;

            return false;
        }

        public static bool operator !=(AbstractInstance obj1, AbstractInstance obj2)
        {
            if (obj1.type == obj2.type || obj1.name == obj2.name)
                return false;

            return true;
        }

        //public override bool Equals(object obj)
        //{
        //    if (obj == null || GetType() != obj.GetType())
        //        return false;

        //    AbstractInstance absInstance = (AbstractInstance)obj;
        //    return this.name == absInstance.name;
        //}

        //public override int GetHashCode()
        //{
        //    return this.name.GetHashCode();
        //}
    }
}