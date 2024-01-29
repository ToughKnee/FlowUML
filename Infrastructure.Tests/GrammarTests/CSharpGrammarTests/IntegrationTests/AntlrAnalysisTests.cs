using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using FluentAssertions;
using Infrastructure.Antlr;
using Infrastructure.Builders;
using Infrastructure.Mediators;
using Moq;
using System.Collections;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace Infrastructure.Tests.GrammarTests.CSharpGrammarTests.IntegrationTests
{
    public class AntlrAnalysisTests
    {
        private static string currentDirectoryPath = "..\\..\\..\\";
        private readonly string pathToTestFiles = "GrammarTests\\CSharpGrammarTests\\IntegrationTests\\TestFiles\\";
        private ANTLRService _antlrService;
        public AntlrAnalysisTests()
        {
            InstancesDictionaryManager.instance.CleanInstancesDictionary();
        }

        public static IEnumerable<object[]> TextFile1Expectations
        {
            get
            {
                yield return new object[] {
                    new List<string> { "string", "string", "a", "TeamsUseCase", "TeamsUseCase", "TeamsUseCase" },
                    new List<string> { "TeamsUseCase", "TeamsUseCase", "TeamsUseCase", "TeamsUseCase", "TeamsUseCase", "TeamsUseCase" }
                };
            }
        }

        [Theory]
        [MemberData(nameof(TextFile1Expectations))]
        //===========================  TODO: COMPLETE Test
        //===========================  TODO: COMPLETE Test
        //===========================  TODO: COMPLETE Test
        public void Analyze_BasicmethodRuleFromFile_InstancesDictHasCorrectCountOfElements(List<string> expectedTypes, List<string> expectedValues)
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            _antlrService = new ANTLRService(mediatorMock.Object);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile1.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("method");

            // Assert
            Assert.True(InstancesDictionaryManager.instance.instancesDictionary.Count == 5);

            int i = 0;
            foreach (KeyValuePair<AbstractInstance, AbstractInstance> assignment in InstancesDictionaryManager.instance.instancesDictionary)
            {
                string type = assignment.Key.name;
                string value = assignment.Value.name;
                Assert.True(type == expectedTypes[i]);
                i++;
            }
            
        }

        public static IEnumerable<object[]> TextFile2Expectations
        {
            get
            {
                yield return new object[] {
                    "CleanArchitectureWorkshop.Application.UseCases",
                    "TeamsUseCase",
                    new List<string> { "AddPlayerToTeamAsync", "CreateTeamAsync", "GetAllTeamsAsync", "GetTeamByIdAsync", "RemovePlayerFromTeamAsync", "GetTeamsByNameAsync" },
                    new List<string> { "string,string", "string", "", "string", "string,string", "string" },
                    new List<string> { "Task<Team>", "Task<Team>", "Task<List<Team>>", "Task<Team?>", "Task<Team>", "Task<List<Team>>" }
                };
            }
        }

        [Theory]
        [MemberData(nameof(TextFile2Expectations))]
        public void Analyze_BasicClassDeclaration_MediatorReceivesMethodBuilderAndCorrectMethodsBuilt(string expBelNamespace, string expOwnerName
            , List<string> expName, List<string> expParameters, List<string> expRetType)
        {
            // Arrange
            List<AbstractBuilder<Method>> abstractBuilders = new List<AbstractBuilder<Method>>();
            var mediatorMock = new Mock<IMediator>();
            // Capture the received parameter to check it
            mediatorMock.Setup(m => m.ReceiveMethodBuilder(It.IsAny<List<AbstractBuilder<Method>>>()))
                .Callback<List<AbstractBuilder<Method>>>(r => abstractBuilders = r);
            _antlrService = new ANTLRService(mediatorMock.Object);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile2.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

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
                Method expectedResult = new Method(expBelNamespace, new ClassEntity(expOwnerName), expName[i], parametersExpected, expRetType[i]);
                abstractBuilders[i].Build().Should().BeEquivalentTo(expectedResult);
            }
        }

        public static IEnumerable<object[]> TextFile3Expectations
        {
            get
            {
                yield return new object[] {
                    "CleanArchitectureWorkshop.Application.UseCases",
                    new List<string> { "TeamsUseCase", "Team"},
                    new List<List<Property>> { 
                        new List<Property> { new Property("ITeamsRepository", "_teamsRepository") },
                        new List<Property> { new Property("List<Player>", "_players"), new Property("IReadOnlyCollection<Player>", "Players") }
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(TextFile3Expectations))]
        public void Analyze_BasicClassDeclaration_MediatorReceivesClassBuilderAndCorrectClassesBuilt(string expBelNamespace, List<string> expName
            , List<List<Property>> expProperties)
        {
            // Arrange
            List<AbstractBuilder<ClassEntity>> abstractBuilders = new List<AbstractBuilder<ClassEntity>>();
            var mediatorMock = new Mock<IMediator>();
            // Capture the received parameter to check it
            mediatorMock.Setup(m => m.ReceiveClassEntityBuilder(It.IsAny<List<AbstractBuilder<ClassEntity>>>()))
                .Callback<List<AbstractBuilder<ClassEntity>>>(r => abstractBuilders = r);
            _antlrService = new ANTLRService(mediatorMock.Object);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile3.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

            // Assert
            mediatorMock.Verify(m => m.ReceiveMethodBuilder(It.IsAny<List<AbstractBuilder<Method>>>()), Times.Once);

            // Verify each class has been correctly identified
            Assert.True(abstractBuilders.Count == 2);
            for (int i = 0; i < abstractBuilders.Count; i++)
            {
                ClassEntity expectedResult = new ClassEntity(expName[i], expBelNamespace, expProperties[i]);
                abstractBuilders[i].Build().name.Should().BeEquivalentTo(expectedResult.name);
                abstractBuilders[i].Build().classNamespace.Should().BeEquivalentTo(expectedResult.classNamespace);
                abstractBuilders[i].Build().properties.Should().BeEquivalentTo(expectedResult.properties);
            }
        }
    }
}