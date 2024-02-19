using Domain.CodeInfo;
using Domain.CodeInfo.MethodSystem;
using Infrastructure.Builders;

namespace Infrastructure.Mediators
{
    /// <summary>
    /// Interface to connect the ANTlR Service to other domain classes that need info from 
    /// the code being analyzed and use it
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Receive a builder which contains all the necessary info
        /// to create all the methods from a ClassEntity if there is one
        /// The builder contains all the info necessary to build
        /// all the methods found for a single class and just get the result
        /// </summary>
        /// <param name="builders">Builder containing all the necessary info to
        /// build all the methods from a ClassEntity</param>
        public void ReceiveMethodBuilders(List<AbstractBuilder<Method>> builders);
        /// <summary>
        /// Receive and manager the builder that contains the info to create the class
        /// if there was a class in the file analysis
        /// </summary>
        /// <param name="builders">Builder containing all the info to create a 
        /// ClassEntity if there was a class in the code analyzed</param>
        public void ReceiveClassEntityBuilders(List<AbstractBuilder<ClassEntity>> builders);

        //===========================  Managing unknown instances and method calls within methods declarations
        /// <summary>
        /// Receives the namespace from which the instances to be received are going to be 
        /// defined within, to help identify them between different files, classes and different methods
        /// </summary>
        /// <param name="belongingNamespace">Namespace which all the instances to be received 
        /// are going to be identified with</param>
        public void ReceiveNamespace(string? belongingNamespace);
        /// <summary>
        /// Receive the class name from which the instances to be recieved are going to be identified with
        /// using also the naemspace
        /// </summary>
        /// <param name="className">Class' name which all the instances to be received 
        /// are going to be identified with</param>
        public void ReceiveClassNameAndInheritance(string? classAndInheritanceNames);
        /// <summary>
        /// Receives a property inside a class to be managed
        /// </summary>
        /// <param name="type"></param>
        /// <param name="identifier"></param>
        public void ReceiveProperties(string type, string identifier);
        /// <summary>
        /// Receives the parameters that the ANTLR visitor found and manage them
        /// </summary>
        /// <param name="type">The type of the identifier</param>
        /// <param name="identifier">The name of the idenitifier</param>
        public void ReceiveParameters(string type, string identifier);
        /// <summary>
        /// Receives the local variable declaration found by an ANTLR visitor to manage them 
        /// after the ANTLR visitor finishes analyzing the method
        /// Also, this must not handle callsites since that is already covered by the "ReceiveMethodCall" method
        /// </summary>
        /// <param name="assigner">The "right part" of an assignment</param>
        /// <param name="assignee">The "left part" of an assignment</param>
        public void ReceiveLocalVariableDeclaration(string assignee, string assigner);
        /// <summary>
        /// After the ANTLR visitor finishes a method analysis, then the Mediator
        /// should start processing the parameters and local variables received
        /// </summary>
        public void ReceiveMethodAnalysisEnd();
        /// <summary>
        /// This will receive the info for the callsites a method made, and be able to create the callsite alongside the MethodInstance, 
        /// which must be connected, and send each class to their correspondent classes
        /// </summary>
        /// <param name="calledClassName">Name of the called class if any</param>
        /// <param name="calledMethodName">Name of the method called</param>
        /// <param name="calledParameters">List of parametrs from the calld method</param>
        /// <param name="linkedMethodBuilder">The linked method builder which has the info for the method that made this callsite, to be able to set the callsite generated and let the builder be able to add this callsite to the Method class to be built</param>
        public void ReceiveMethodCall(string calledClassName, string calledMethodName, List<string>? calledParameters, MethodBuilder linkedMethodBuilder, bool isConstructor);
        /// <summary>
        /// Receive the usedNamespaces to be able to disambiguate between classes with the same name
        /// Intended to be used after the ReceiveMethodCall was made
        /// </summary>
        /// <param name="usedNamespaces"></param>
        public void ReceiveUsedNamespaces(List<string>? usedNamespaces);
    }
}
