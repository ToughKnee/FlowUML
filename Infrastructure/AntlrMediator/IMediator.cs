using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Domain.CodeInfo.MethodSystem;
using Infrastructure.Builders;
using System.Collections.ObjectModel;

namespace Infrastructure.Mediators
{
    /// <summary>
    /// Interface to connect the ANTlR Service to other domain classes that need info from 
    /// the code being analyzed and use it
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Returns the dictionary that the mediator is using to store known instances, so that other 
        /// classes can access it and read it
        /// </summary>
        /// <returns></returns>
        public ReadOnlyDictionary<string, AbstractInstance> GetKnownInstancesDeclaredInCurrentMethodAnalysis();
        /// <summary>
        /// Returns the current analyzed class in the mediator
        /// </summary>
        /// <returns></returns>
        public string GetCurrentAnalyzedClassName();
        /// <summary>
        /// Returns the namespaces in use in the current code file being analyzed, used by classes
        /// that need to know the candidate namespaces when creating the MethodInstance to be able
        /// to dissambiguate and match the MethodInstances with the actual Methods
        /// </summary>
        /// <returns></returns>
        public List<string> GetUsedNamespaces();
        /// <summary>
        /// This will start defining the undefined MethodInstances that require knowing the 
        /// types of its components(the class name and the parameters)
        /// </summary>
        public void DefineUndefinedMethodInstances();
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
        /// Receive the class name and inheritance to manage it
        /// </summary>
        /// <param name="classAndInheritanceNames">Class' name including the inheritance of this class 
        /// from the code files</param>
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
        /// Also, this stores the true Instance of variables definitionss when there are many variable aliases AND
        /// a MethodInstance has a component of a simple variable and needs the true Instance of the variable when
        /// built by the MethodInstanceBuilder
        /// </summary>
        /// <param name="assigner">The "right part" of an assignment</param>
        /// <param name="assignee">The "left part" of an assignment</param>
        /// <param name="methodCallAssigner">The linked methodInstanceBuilder if the assigner is a methodCall</param>
        public void ReceiveLocalVariableDeclaration(string assignee, string? assigner, AbstractBuilder<AbstractInstance>? instanceAssignerBuilder);
        /// <summary>
        /// After the ANTLR visitor finishes a method analysis, then the Mediator
        /// should start processing the parameters and local variables received
        /// </summary>
        public void ReceiveMethodAnalysisEnd();
        /// <summary>
        /// This will receive the info for the callsites a method made, and be able to create 
        /// the callsite alongside the MethodInstance, 
        /// which must be connected, and send each class to their correspondent classes
        /// </summary>
        /// <param name="methodCallData">Contains all the data of the MethodCall, the linked method builder that 
        /// is in this parameter which has the info for the method that made this callsite, 
        /// to be able to set the callsite generated and let the builder be able to add this 
        /// callsite to the Method class to be built</param>
        public void ReceiveMethodCall(AbstractBuilder<AbstractInstance> methodCallBuilder);
        /// <summary>
        /// Receive the usedNamespaces to be able to disambiguate between classes with the same name
        /// Intended to be used after the ReceiveMethodCall was made
        /// </summary>
        /// <param name="usedNamespaces"></param>
        public void ReceiveUsedNamespaces(List<string>? usedNamespaces);
    }
}
