using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using FluentAssertions;
using Infrastructure.Antlr;
using Infrastructure.Builders;
using Infrastructure.Mediators;
using Moq;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace Infrastructure.Tests.GrammarTests.CSharpGrammarTests.IntegrationTests
{
    public class InstanceDictionaryAntlrTests
    {
        private static string currentDirectoryPath = "..\\..\\..\\";
        private readonly string pathToTestFiles = "GrammarTests\\CSharpGrammarTests\\IntegrationTests\\TestFiles\\";
        private ANTLRService _antlrService;
        public InstanceDictionaryAntlrTests()
        {
            InstancesDictionaryManager.instance.CleanInstancesDictionary();
        }

        [Fact]
        public void Analyze_BasicmethodRuleFromFile_InstancesDictHasCorrectCountOfElements()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            _antlrService = new ANTLRService(mediatorMock.Object);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "TextFile1.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("method");

            // Assert
            Assert.True(InstancesDictionaryManager.instance.instancesDictionary.Count == 5);
        }

        public static IEnumerable<object[]> TextFile2Expectations
        {
            get
            {
                yield return new object[] { 
                    new List<string> { "TeamsUseCase", "TeamsUseCase", "TeamsUseCase", "TeamsUseCase", "TeamsUseCase", "TeamsUseCase" },
                    new List<string> { "Task<Team>", "Task<Team>", "Task<List<Team>>", "Task<Team?>", "Task<Team>", "Task<List<Team>>" },
                    new List<string> { "AddPlayerToTeamAsync", "CreateTeamAsync", "GetAllTeamsAsync", "GetTeamByIdAsync", "RemovePlayerFromTeamAsync", "GetTeamsByNameAsync" },
                    new List<string> { "string,string", "string", "", "string", "string,string", "string" }
                };
            }
        }

        [Theory]
        [MemberData(nameof(TextFile2Expectations))]
        public void Analyze_BasicClassDeclaration_MediatorReceivesBuilder(List<string> expOwnerName, List<string> expRetType, List<string> expName, List<string> expParameters)
        {
            // Arrange
            List<AbstractBuilder<Method>> abstractBuilders = new List<AbstractBuilder<Method>>();
            var mediatorMock = new Mock<IMediator>();
            // Capture the received parameter to check it
            mediatorMock.Setup(m => m.ReceiveMethodBuilder(It.IsAny<List<AbstractBuilder<Method>>>()))
                .Callback< List < AbstractBuilder < Method >>> (r => abstractBuilders = r);
            _antlrService = new ANTLRService(mediatorMock.Object);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "TextFile2.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("classDeclaration");

            // Assert
            mediatorMock.Verify(m => m.ReceiveMethodBuilder(It.IsAny<List<AbstractBuilder<Method>>>()), Times.Once);

            // Verify each class has been correctly identified
            Assert.True(abstractBuilders.Count == 6);
            for (int i = 0; i < abstractBuilders.Count; i++)
            {
                List<string> parametersExpected = new List<string>();
                if (expParameters[i].Length > 0)
                {
                    parametersExpected = expParameters[i].Split(",").ToList();
                }
                Method expectedResult = new Method(expOwnerName[i], expRetType[i], expName[i], parametersExpected);
                abstractBuilders[i].Build().Should().BeEquivalentTo(expectedResult);
            }
        }
    }
}