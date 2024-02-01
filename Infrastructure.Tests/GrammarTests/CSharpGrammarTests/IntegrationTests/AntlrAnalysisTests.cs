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
                    new List<(string, string)> { ("string", "teamName"), ("string", "playerName") },
                    new List<(string, string)> { ("teamId", "TeamName.Create(teamName)"), ("team", "_teamsRepository.GetByIdAsync(teamId)"), ("playerId", "UserName.Create(playerName)") }
                };
            }
        }

        [Theory]
        [MemberData(nameof(TextFile1Expectations))]
        public void AnalyzeBasicClassDeclaration_MediatorReceivesExpectedStrings(List<(string, string)> expectedParameters, List<(string, string)> expectedLocalVariables)
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();

            // Create a collection to store the captured parameters
            List<(string, string)> receivedParameters = new List<(string, string)>();
            List<(string, string)> receivedLocalVariables = new List<(string, string)>();

            mediatorMock.Setup(x => x.ReceiveParameters(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((param1, param2) =>
                {
                    receivedParameters.Add((param1, param2));
                });
            mediatorMock.Setup(x => x.ReceiveLocalVariableDeclaration(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((param1, param2) =>
                {
                    receivedLocalVariables.Add((param1, param2));
                });
            _antlrService = new ANTLRService(mediatorMock.Object);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile1.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("method");

            // Assert
            // Checking all the received strings are correct
            receivedParameters.Should().BeEquivalentTo(expectedParameters);
            receivedLocalVariables.Should().BeEquivalentTo(expectedLocalVariables);

            //foreach (KeyValuePair<AbstractInstance, AbstractInstance> assignment in InstancesDictionaryManager.instance.instancesDictionary)
            //{
            //    string identifier = assignment.Key.name;
            //    string value = assignment.Value.name;
            //    Assert.True(value == expectedValues[i]);
            //    Assert.True(identifier == expectedIdentifiers[i]);
            //    i++;
            //}

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
        public void AnalyzeBasicClassDeclaration_MediatorReceivesMethodBuilder_MethodsCorrectlyBuilt(string expBelNamespace, string expOwnerName
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
                    },
                    new List<int> { 6, 3 }
                };
            }
        }

        [Theory]
        [MemberData(nameof(TextFile3Expectations))]
        public void AnalyzeBasicClassDeclaration_MediatorReceivesClassBuilder_CorrectClassesBuilt(string expBelNamespace, List<string> expName
            , List<List<Property>> expProperties, List<int> expMethodCount)
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
                ClassEntity builtClass = abstractBuilders[i].Build();
                ClassEntity expectedResult = new ClassEntity(expName[i], expBelNamespace, expProperties[i]);
                builtClass.name.Should().BeEquivalentTo(expectedResult.name);
                builtClass.classNamespace.Should().BeEquivalentTo(expectedResult.classNamespace);
                builtClass.properties.Should().BeEquivalentTo(expectedResult.properties);
                builtClass.methods.Count.Should().Be(expMethodCount[i], because: "The constructor also count as a method so it should be recognized as one for the second class");
            }
        }

        public static IEnumerable<object[]> TextFile3ExpectationsForNamespaceInfo
        {
            get
            {
                yield return new object[] {
                    "CleanArchitectureWorkshop.Application.UseCases",
                    new List<string> { "TeamsUseCase", "Team"},
                };
            }
        }

        [Theory]
        [MemberData(nameof(TextFile3ExpectationsForNamespaceInfo))]
        public void AnalyzeBasicClassDeclaration_MediatorReceivesNamespaceAndClassNames_CorrectInfoReceivedAtTheCorrectTime(string expNamespace, List<string> expClassNames)
        {
            // The correct time refers to having received the namespaces and className before the method "ReceiveMethodAnalysisEnd"
            // from the IMediator is received, since the mediator should work with that info AFTER the visitor finishes creeating a method

            // Arrange
            var mediatorMock = new Mock<IMediator>();

            // Create a collection to store the captured parameters
            string receivedNamespace = "";
            List<string> receivedClassNames = new List<string>();
            List<(string, string)> receivedParameters = new List<(string, string)>();
            List<(string, string)> receivedLocalVariables = new List<(string, string)>();

            //===========================  Capturing the received info from the antlr visitor
            mediatorMock.Setup(x => x.ReceiveNamespace(It.IsAny<string>()))
                .Callback<string>((param1) => receivedNamespace = param1);
            mediatorMock.Setup(x => x.ReceiveClassName(It.IsAny<string>()))
                .Callback<string>((param1) =>
                {
                    receivedClassNames.Add(param1);
                });

            mediatorMock.Setup(x => x.ReceiveMethodAnalysisEnd())
                .Callback(() =>
                {
                    receivedClassNames.Count.Should().NotBe(0);
                    // Check the info from namespace and className is the expected info at this moment
                    if (receivedClassNames.Count == 1)
                    {
                        receivedNamespace.Should().Be(expNamespace);
                        receivedClassNames[0].Should().Be(expClassNames[0]);
                    }
                    else if (receivedClassNames.Count == 2)
                    {
                        mediatorMock.Verify(m => m.ReceiveMethodAnalysisEnd(), Times.AtLeast(6));
                        receivedNamespace.Should().Be(expNamespace);
                        receivedClassNames[1].Should().Be(expClassNames[1]);
                    }
                    else { Assert.True(false); }
                });

            // Creating the antlr visitor
            _antlrService = new ANTLRService(mediatorMock.Object);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile3.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

            // Assert
            mediatorMock.Verify(m => m.ReceiveMethodAnalysisEnd(), Times.Exactly(9));
        }

        
        public static IEnumerable<object[]> TextFile3ExpectationsForNamespaceInfo2
        {
            get
            {
                yield return new object[] {
                    new List<string> { "CleanArchitectureWorkshop.Application.UseCases", "com.packages.TeamUtils"},
                    new List<string> { "TeamsUseCase", "Team", "TeamManager", "Director", "Player"}
                };
            }
        }
        [Theory]
        [MemberData(nameof(TextFile3ExpectationsForNamespaceInfo2))]

        public void AnalyzeBasicClassDeclaration_MediatorReceivesNamespaceAndClassNames_CorrectInfoReceivedAtTheCorrectTime2(List<string> expNamespaces, List<string> expClassNames)
        {
            // The correct time refers to having received the namespaces and className before the method "ReceiveMethodAnalysisEnd"
            // from the IMediator is received, since the mediator should work with that info AFTER the visitor finishes creeating a method

            // Arrange
            var mediatorMock = new Mock<IMediator>();

            // Create a collection to store the captured parameters
            List<string> receivedNamespaces = new List<string>();
            List<string> receivedClassNames = new List<string>();
            List<(string, string)> receivedParameters = new List<(string, string)>();
            List<(string, string)> receivedLocalVariables = new List<(string, string)>();

            //===========================  Capturing the received info from the antlr visitor
            mediatorMock.Setup(x => x.ReceiveNamespace(It.IsAny<string>()))
                .Callback<string>((param1) =>
                {
                    receivedNamespaces.Add(param1);
                });
            mediatorMock.Setup(x => x.ReceiveClassName(It.IsAny<string>()))
                .Callback<string>((param1) =>
                {
                    receivedClassNames.Add(param1);
                });

            mediatorMock.Setup(x => x.ReceiveMethodAnalysisEnd())
                .Callback(() =>
                {
                    receivedClassNames.Count.Should().NotBe(0);
                    // Check the info from namespace and className is the expected info at this moment
                    if (receivedClassNames.Count == 1)
                    {
                        receivedNamespaces[0].Should().Be(expNamespaces[0]);
                        receivedClassNames[0].Should().Be(expClassNames[0]);
                    }
                    else if (receivedClassNames.Count == 2)
                    {
                        mediatorMock.Verify(m => m.ReceiveMethodAnalysisEnd(), Times.AtLeast(1));
                        receivedNamespaces[0].Should().Be(expNamespaces[0]);
                        receivedClassNames[1].Should().Be(expClassNames[1]);
                    }
                    else if (receivedClassNames.Count == 3)
                    {
                        mediatorMock.Verify(m => m.ReceiveMethodAnalysisEnd(), Times.AtLeast(4));
                        receivedNamespaces[1].Should().Be(expNamespaces[1]);
                        receivedClassNames[2].Should().Be(expClassNames[2]);
                    }
                    else if (receivedClassNames.Count == 4)
                    {
                        mediatorMock.Verify(m => m.ReceiveMethodAnalysisEnd(), Times.AtLeast(7));
                        receivedNamespaces[1].Should().Be(expNamespaces[1]);
                        receivedClassNames[3].Should().Be(expClassNames[3]);
                    }
                    else if (receivedClassNames.Count == 5)
                    {
                        mediatorMock.Verify(m => m.ReceiveMethodAnalysisEnd(), Times.AtLeast(8));
                        receivedNamespaces[1].Should().Be(expNamespaces[1]);
                        receivedClassNames[4].Should().Be(expClassNames[4]);
                    }
                    else { Assert.True(false); }
                });

            // Creating the antlr visitor
            _antlrService = new ANTLRService(mediatorMock.Object);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile4.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

            // Assert
            mediatorMock.Verify(m => m.ReceiveMethodAnalysisEnd(), Times.Exactly(9));
        }

        public static IEnumerable<object[]> MediatorMethodDeclaraationExpectations
        {
            get
            {
                yield return new object[] {
                    new List<string> { "CleanArchitectureWorkshop.Application.UseCases", "CleanArchitectureWorkshop.Application.UseCases", "CleanArchitectureWorkshop.Application.UseCases", "CleanArchitectureWorkshop.Application.UseCases", "CleanArchitectureWorkshop.Application.UseCases", "CleanArchitectureWorkshop.Application.UseCases"},
                    new List<string> { "TeamsUseCase", "TeamsUseCase", "TeamsUseCase", "TeamsUseCase", "TeamsUseCase", "TeamsUseCase"},
                    new List<string> { "AddPlayerToTeamAsync", "CreateTeamAsync", "GetAllTeamsAsync", "GetTeamByIdAsync", "RemovePlayerFromTeamAsync",   "GetTeamsByNameAsync"},
                    new List<string> { "string,string", "string", "", "string", "string,string", "string" },
                    new List<string> { "Task<Team>", "Task<Team>", "Task<List<Team>>", "Task<Team?>", "Task<Team>", "Task<List<Team>>"},
                };
            }
        }
        [Theory]
        [MemberData(nameof(MediatorMethodDeclaraationExpectations))]
        public void AnalyzeBasicClassDeclaration_MediatorReceivesMethodDeclaration_CorrectInfoReceived(List<string> expNamespace, List<string> expOwnerClass, List<string> expMethodName, List<string> expParameters, List<string> expReturnType)
        {
            // The correct time refers to having received the namespaces and className before the method "ReceiveMethodAnalysisEnd"
            // from the IMediator is received, since the mediator should work with that info AFTER the visitor finishes creeating a method

            // Arrange
            var mediatorMock = new Mock<IMediator>();

            // Create a collection to store the captured parameters
            List<string> receivedNamespace = new List<string>();
            List<string> receivedOwnerClass = new List<string>();
            List<string> receivedMethodName = new List<string>();
            List<string> receivedParameters = new List<string>();
            List<string> receivedReturnType = new List<string>();

            //===========================  Capturing the received info from the antlr visitor
            mediatorMock.Setup(x => x.ReceiveMethodDeclaration(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string, string, string, string>((param1, param2, param3, param4, param5) =>
                {
                    receivedNamespace.Add(param1);
                    receivedOwnerClass.Add(param2);
                    receivedMethodName.Add(param3);
                    receivedParameters.Add(param4);
                    receivedReturnType.Add(param5);
                });

            // Creating the antlr visitor
            _antlrService = new ANTLRService(mediatorMock.Object);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile2.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

            // Assert
            mediatorMock.Verify(m => m.ReceiveMethodDeclaration(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(6));

            // Verify each class has been correctly identified
            receivedNamespace.Should().BeEquivalentTo(expNamespace);
            receivedOwnerClass.Should().BeEquivalentTo(expOwnerClass);
            receivedMethodName.Should().BeEquivalentTo(expMethodName);
            receivedParameters.Should().BeEquivalentTo(expParameters);
            receivedReturnType.Should().BeEquivalentTo(expReturnType);
        }

    }

    // AnalyzeBasicClassDeclaration_InstancesDictionaryReceivesInstances_InstancesRecognizedCorrectly
}