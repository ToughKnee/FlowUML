using Domain.CodeInfo;
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
        /// <param name="builder">Builder containing all the necessary info toa
        /// build all the methods from a ClassEntity</param>
        public void ReceiveMethodBuilder(List<AbstractBuilder<Method>> builders);
        /// <summary>
        /// Receive and manager the builder that contains the info to create the class
        /// if there was a class in the file analysis
        /// </summary>
        /// <param name="builders">Builder containing all the info to create a 
        /// ClassEntity if there was a class in the code analyzed</param>
        public void ReceiveClassEntityBuilder(List<AbstractBuilder<ClassEntity>> builders);

        //===========================  Managing the KNWON methods, like the method Declarations
        public void ReceiveMethodDeclaration(string belongingNamespace, string ownerClass, string name
            , string parametersType, string returnType);

        //===========================  Managing the unknown methods, like callsites to other methods
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
        public void ReceiveClassName(string className);
        /// <summary>
        /// Receives the parameters that the ANTLR visitor found and manage them
        /// </summary>
        /// <param name="type">The type of the identifier</param>
        /// <param name="identifier">The name of the idenitifier</param>
        public void ReceiveParameters(string type, string identifier);
        /// <summary>
        /// Receives the local variable declaration found by an ANTLR visitor to manage them 
        /// after the ANTLR visitor finishes analyzing the method
        /// </summary>
        /// <param name="assigner">The "right part" of an assignation</param>
        /// <param name="assignee">The "left part" of an assignation</param>
        public void ReceiveLocalVariableDeclaration(string assignee, string assigner);
        /// <summary>
        /// After the ANTLR visitor finishes a method analysis, then the Mediator
        /// should start processing the parameters and local variables received
        /// </summary>
        public void ReceiveMethodAnalysisEnd();
    }
}
