namespace Domain.CodeInfo.InstanceDefinitions.ObserverPattern
{
    /// <summary>
    /// Interface of the observer pattern for the AbstractInstance class
    /// Used to let the instances from a class with inheritance receive other classes which also have 
    /// inheritance and check it there is a grandparent for the current instances, where if we have that 
    /// class in the inheritance list of the instance, then we must add its parents too
    /// Observers can only be properties or methods since they are the ones that persist into their children
    /// </summary>
    public interface IInstanceObserver
    {
        void ReceiveClassAndInheritanceNames(string[] classAndInheritanceNames);
    }
}   