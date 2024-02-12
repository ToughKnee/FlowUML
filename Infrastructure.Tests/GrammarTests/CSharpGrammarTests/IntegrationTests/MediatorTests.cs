using Domain.CodeInfo.InstanceDefinitions;
using Infrastructure.Antlr;
using Infrastructure.Mediators;

namespace Infrastructure.Tests.GrammarTests.CSharpGrammarTests.IntegrationTests
{
    public class MediatorTests
    {
        private static string currentDirectoryPath = "..\\..\\..\\";
        private readonly string pathToTestFiles = "GrammarTests\\CSharpGrammarTests\\IntegrationTests\\TestFiles\\";
        private ANTLRService _antlrService;

        public MediatorTests()
        {
            InstancesDictionaryManager.instance.CleanInstancesDictionary();
        }

        [Fact]
        public void MediatorReceivesInstancesInfo_MediatorHandlesInfo_MediatorCreatesCorrespondingInstancesAndStoredInInstancesDictionaryCorrectly()
        {
            // Arrange
            var mediator = new AntlrMediator();
            _antlrService = new ANTLRService(mediator);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile6.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

            // Assert
            int i = 0;

        }
    }
}