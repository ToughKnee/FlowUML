
using System.Xml.Linq;
using System;
using Domain.CodeInfo.InstanceDefinitions.ObserverPattern;

namespace Domain.CodeInfo.InstanceDefinitions
{
    /// <summary>
    /// An instance is a variable, parameter or method return type that has a type which we need to know when a ClassEntity calls another ClassEntity
    /// </summary>
    public abstract class AbstractInstance : IInstanceObserver
    {
        /// <summary>
        /// The name which, basically is a variable/parameter/property/method that has a 
        /// given Type provided by another Instance
        /// And this lets us map any variable to a ClassEntity eventually
        /// </summary>
        public virtual string name { get; set; }

        /// <summary>
        /// List containging other classes, interfaces, etc this class inherits, so that we can
        /// identify instances like properties or methods from parent instances that were used in this instance
        /// If this list contains elements, then this class needs to subscribe to an event that has 
        /// where we receive the name of a class being declared AND has inhertiance too, if that class
        /// is contained in this list, it means that this current instance has more inheritance and 
        /// we must add the inhertied stuff from the received class too
        /// </summary>
        public List<string> inheritanceNames = new List<string>();

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

        public void ReceiveClassAndInheritanceNames(string[] classAndInheritanceNames)
        {
            // Pass through all the inheritances types and compare them if one of them is equal to the class with inheritance
            for (int i = 0; i < inheritanceNames.Count; i++)
            {
                // If indeed the class is in the inheritanceList, then we must add its parents too
                if (inheritanceNames[i].CompareTo(classAndInheritanceNames[0]) == 0)
                {
                    for (global::System.Int32 j = 1; j < classAndInheritanceNames.Count(); j++)
                    {
                        inheritanceNames.Add(classAndInheritanceNames[j]);
                    }
                    break;
                }
            }
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