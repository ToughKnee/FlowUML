namespace Domain.CodeInfo.MethodSystem
{
    /// <summary>
    /// This is used to store calls that a method does to another place in the code, 
    /// BUT we don't know the entire method identification(which needs the Class 
    /// it comes from, its name and its parameters), so this will be listening 
    /// to an event to receive the definition of the method
    /// </summary>
    public class Callsite
    {
        /// <summary>
        /// The method which may not be completely defined and will have to receive 
        /// the complete Method from a ClassEntity which is being defined and defining its Methods
        /// This method is considered as complete when we know its 'returnType'
        /// </summary>
        public Method? calledMethod;
        public Callsite(Method? called)
        {
            calledMethod = called;
        }
    }
}