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
        /// <param name="builder">Builder containing all the necessary info to
        /// build all the methods from a ClassEntity</param>
        public void ReceiveMethodBuilder(List<AbstractBuilder<Method>> builders);
        /// <summary>
        /// Receive and manager the builder that contains the info to create the class
        /// if there was a class in the file analysis
        /// </summary>
        /// <param name="builder">Builder containing all the info to create a 
        /// ClassEntity if there was a class in the code analyzed</param>
        public void ReceiveClassEntityBuilder(List<AbstractBuilder<ClassEntity>> builders);
    }
}
