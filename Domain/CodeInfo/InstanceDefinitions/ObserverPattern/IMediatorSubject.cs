namespace Domain.CodeInfo.InstanceDefinitions.ObserverPattern
{
    /// <summary>
    /// Interface of the observer pattern for the Mediator class
    /// This will notify to AbstractInstances new classes with their parents 
    /// whenever there is a class with inheritance
    /// </summary>
    public interface IMediatorSubject
    {
        public List<IInstanceObserver> Observers { get; }

        /// <summary>
        /// This method makes an instance receive classes that are the grandparents of this Instance
        /// Where if a parent of this instance has another parent later on in the analysis, we can 
        /// chain the parents inheritance to this instance get the grandparents
        /// </summary>
        /// <param name="observer"></param>
        public void SubscribeInstanceToChainedInheritance(IInstanceObserver observer);

        public void Detach(IInstanceObserver observer);

        public void NotifyClassWithInheritanceNames(string[] classAndInheritance);
    }
}