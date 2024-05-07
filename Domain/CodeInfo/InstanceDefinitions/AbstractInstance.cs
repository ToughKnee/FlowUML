
using System.Xml.Linq;
using System;

namespace Domain.CodeInfo.InstanceDefinitions
{
    /// <summary>
    /// The enum will mark how should we treat this Instance when trying to get the actual 
    /// type, depending on the context this Instance was called was made
    /// </summary>
    public enum KindOfInstance
    {
        // normal means this Instance has its components (className and parameters) linked to other Instances which will have their type knwon when the ClassEntities and Method Builders start building
        Normal,
        // This case is easy since getting the actual Method comes down to adding the used namespace(including the belonging namespace) and consult the methodDictionary
        IsConstructor,
        // This too
        IsInheritedOrInThisClass,
        // This too
        HasClassNameStatic,
        // This case is special because the className is a property from an inherited class and we must use check the properties from the ClassEntities to get the type of this property
        IsPropertyFromInheritanceOrInThisClass,
        // If this element comes from an indexed collection(array, list or dictionary) then the type of this instance is actually the typename that the actual collection holds
        IsElementFromCollection,
        // This applies to normal caller class Instaance types, if there is a methodCall that is part of a chain from another methodCall, then the caller class automatically is assigned this type, only for info purposes
        IsFromLinkedMethodInstance,
        // This kind states that the current instance is a collection and its type is found in the the template typename of the collection this instance represents
        IsIndexRetrievalInstance
    }

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
        public IReadOnlyList<string> inheritedClasses;

        /// <summary>
        /// If null, this Instance is just an alias of another instance, if not null, then this Instance 
        /// has been assigned by a method or constructor and is not an alias of another Instance
        /// and we know its return type
        /// For the MethodInstance, this will be used as a
        /// property that is solely used to let other instances that were
        /// assigned by this MethodInstance know eventually their type
        /// </summary>
        public string? type => refType.data;
        /// <summary>
        /// This property is used to modify the tyoe if this Instance and be able to update other
        /// Instances that were pointing to the same type if one was assigning the type of the other
        /// Instance(like when a method call assigns a variable)
        /// </summary>
        public StringWrapper refType { get; set; }
        /// <summary>
        /// This bool represents wether this MethodCall does not have an callerClass and must come from 
        /// </summary>
        public KindOfInstance kind;
        /// <summary>
        /// This property defines the instance that is chained to this instance, like "myProperty"
        /// would be the chainedInstance in "myInstance.myProperty" 
        /// </summary>
        public AbstractInstance? chainedInstance { get; set; } = null;
        /// <summary>
        /// This represents the case when the instance is an indexed collection and it is retrieving an element from it,
        /// like "GetMyList()[0]" would result in a MethodInstance class with this property present and not null
        /// </summary>
        public AbstractInstance? indexRetrievedInstance { get; set; } = null;

        public AbstractInstance(string name, StringWrapper type)
        {   
            this.name = name;
            this.refType = type;
        }
        public AbstractInstance(string name)
        {
            this.name = name;
            this.refType = new StringWrapper();
        }
        public AbstractInstance()
        {
        }
        public static AbstractInstance? GetLastChainedInstance(AbstractInstance instance, bool returnFirstMethodInstance = false)
        {
            AbstractInstance nextInstance = instance;
            AbstractInstance previousInstance = nextInstance;
            while (nextInstance is not null)
            {
                previousInstance = nextInstance;
                nextInstance = nextInstance.chainedInstance;
                if (returnFirstMethodInstance && nextInstance is MethodInstance) return nextInstance;
            }
            if (returnFirstMethodInstance) return null;
            return previousInstance;
        }
        /// <summary>
        /// Generate the whole propertyChain from the provided strings, while avoiding to put inside 
        /// the propertyChain the class that owns the propertyChain
        /// </summary>
        /// <param name="ownerName">The name of the Instance that will be the
        /// head of this propertyChain</param>
        /// <param name="propertyChainString">The text that represents the whole propertyChain as 
        /// a raw string, which will be converted into multiple chained AbstractInstances</param>
        /// <returns>The first Instance that has all the other properties chained</returns>
        public static AbstractInstance GeneratePropertyChain(string ownerName, string propertyChainString)
        {
            var methodInstancePropertyChainString = propertyChainString.Split(".");
            AbstractInstance previousInstance = null;
            AbstractInstance firstChainedInstance = null;
            foreach (string componentString in methodInstancePropertyChainString)
            {
                // If the current component isn't the caller class, then it is part of the propertyChain
                if (componentString != ownerName)
                {
                    var component = new Instance(componentString);
                    component.kind = KindOfInstance.IsPropertyFromInheritanceOrInThisClass;
                    // If the previous component is null, then this is the first element of the propertyChain which will be received by the MethodInstance constructor
                    if (previousInstance is null)
                        firstChainedInstance = component;
                    // If not then we need to chain all the properties properly
                    else
                        previousInstance.chainedInstance = component;
                    previousInstance = component;
                }
            }
            return firstChainedInstance;
        }
    }
}