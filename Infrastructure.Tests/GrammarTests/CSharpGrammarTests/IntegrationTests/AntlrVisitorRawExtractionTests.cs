using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Domain.CodeInfo.MethodSystem;
using FluentAssertions;
using Infrastructure.Antlr;
using Infrastructure.Builders;
using Infrastructure.Mediators;
using Moq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit.Abstractions;

namespace Infrastructure.Tests.GrammarTests.CSharpGrammarTests.IntegrationTests
{
   public class AntlrVisitorRawExtractionTests
   {
       private static string currentDirectoryPath = "..\\..\\..\\";
       private readonly string pathToTestFiles = "GrammarTests\\CSharpGrammarTests\\IntegrationTests\\TestFiles\\";
       private ANTLRService _antlrService;
       public AntlrVisitorRawExtractionTests()
       {
           InheritanceDictionaryManager.instance.CleanInheritanceDictionary();
           MethodDictionaryManager.instance.CleanMethodDictionary();
           ClassEntityManager.instance.CleanClassEntitiesDictionary();
       }
       public static IEnumerable<object[]> TextFile2Expectations
       {
           get
           {
               yield return new object[] {
                   "CleanArchitectureWorkshop.Application.UseCases",
                   new List<string> { "AddPlayerToTeamAsync", "CreateTeamAsync", "GetAllTeamsAsync", "GetTeamByIdAsync", "RemovePlayerFromTeamAsync", "GetTeamsByNameAsync" },
                   new List<string> { "string,string", "string", "", "string", "string,string", "string" },
                   new List<string> { "Task<Team>", "Task<Team>", "Task<List<Team>>", "Task<Team?>", "Task<Team>", "Task<List<Team>>" }
               };
           }
       }

       [Theory]
       [MemberData(nameof(TextFile2Expectations))]
       public void AnalyzeBasicClassDeclaration_MediatorReceivesMethodBuilder_MethodsCorrectlyBuilt(string expBelNamespace
           , List<string> expName, List<string> expParameters, List<string> expRetType)
       {
           // Arrange
           List<AbstractBuilder<Method>> abstractBuilders = new List<AbstractBuilder<Method>>();
           var mediatorMock = new Mock<IMediator>();
           // Capture the received parameter to check it
           mediatorMock.Setup(m => m.ReceiveMethodBuilders(It.IsAny<List<AbstractBuilder<Method>>>()))
               .Callback<List<AbstractBuilder<Method>>>(r => abstractBuilders = r);
           mediatorMock.Setup(x => x.GetKnownInstancesDeclaredInCurrentMethodAnalysis())
               .Returns(() =>
               {
                   return new Dictionary<string, AbstractInstance>().AsReadOnly();
               });
            _antlrService = new ANTLRService(mediatorMock.Object);
           _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile2.txt", true);

           // Act
           _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

           // Assert
           mediatorMock.Verify(m => m.ReceiveMethodBuilders(It.IsAny<List<AbstractBuilder<Method>>>()), Times.Once);

           // Verify each class has been correctly identified
           Assert.True(abstractBuilders.Count == 6);
           for (int i = 0; i < abstractBuilders.Count; i++)
           {
               List<string> parametersExpected = new List<string>();
               if (expParameters[i].Length > 0)
               {
                   parametersExpected = expParameters[i].Split(",").ToList();
               }
               Method expectedResult = new Method(expBelNamespace, null, expName[i], parametersExpected, expRetType[i]);
               Method receivedResult = abstractBuilders[i].Build();
               receivedResult.Should().BeEquivalentTo(expectedResult);
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
           mediatorMock.Setup(m => m.ReceiveClassEntityBuilders(It.IsAny<List<AbstractBuilder<ClassEntity>>>()))
               .Callback<List<AbstractBuilder<ClassEntity>>>(r => abstractBuilders = r);
            mediatorMock.Setup(x => x.GetKnownInstancesDeclaredInCurrentMethodAnalysis())
                .Returns(() =>
                {
                    return new Dictionary<string, AbstractInstance>().AsReadOnly();
                });
            _antlrService = new ANTLRService(mediatorMock.Object);
           _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile3.txt", true);

           // Act
           _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

           // Assert
           mediatorMock.Verify(m => m.ReceiveMethodBuilders(It.IsAny<List<AbstractBuilder<Method>>>()), Times.Once);

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
                   new List<string> { "TeamsUseCase,ITeamsUseCase", "Team,AggregateRoot<TeamName>"},
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
            mediatorMock.Setup(x => x.GetKnownInstancesDeclaredInCurrentMethodAnalysis())
                .Returns(() =>
                {
                    return new Dictionary<string, AbstractInstance>().AsReadOnly();
                });
            mediatorMock.Setup(x => x.ReceiveNamespace(It.IsAny<string>()))
               .Callback<string>((param1) => receivedNamespace = param1);
           mediatorMock.Setup(x => x.ReceiveClassNameAndInheritance(It.IsAny<string>()))
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
                   new List<string> { "TeamsUseCase,ITeamsUseCase", "Team,AggregateRoot<TeamName>", "TeamManager", "Director", "Player,AggregateRoot<TeamName>" }
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
            mediatorMock.Setup(x => x.GetKnownInstancesDeclaredInCurrentMethodAnalysis())
                .Returns(() =>
                {
                    return new Dictionary<string, AbstractInstance>().AsReadOnly();
                });
            mediatorMock.Setup(x => x.ReceiveNamespace(It.IsAny<string>()))
                .Callback<string>((param1) =>
                {
                    receivedNamespaces.Add(param1);
                });
            mediatorMock.Setup(x => x.ReceiveClassNameAndInheritance(It.IsAny<string>()))
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

        public List<object> GetParametersListAsString(List<AbstractInstance> parameters)
        {
            var result = new List<object>();
            foreach (var instance in parameters)
            {
                result.Add(instance.name);
            }
            return result;
        }
       public static IEnumerable<object[]> MediatorCallsiteIfnoExpectations
       {
           get
           {
               yield return new object[] {
                   new List<string> { "TeamName", "_teamsRepository", "UserName", "", "team", "_teamsRepository", "TeamName", "_teamsRepository", "UserName", "team", "_teamsRepository", "_teamsRepository" },
                   new List<string> { "Create", "GetByIdAsync", "Create", "Player", "AddPlayer", "UpdateTeamAsync", "Create", "GetByIdAsync", "Create", "RemovePlayer", "UpdateTeamAsync", "GetTeamsByNameAsync" },
                   new List<List<string>> { 
                       new List<string> { "teamName" },
                       new List<string> { "teamId" },  
                       new List<string> { "playerName" },  
                       new List<string> { "playerId" },  
                       new List<string> { "player" },  
                       new List<string> { "team" },  
                       new List<string> { "teamName" },  
                       new List<string> { "teamId" },  
                       new List<string> { "playerName" },  
                       new List<string> { "playerId" },  
                       new List<string> { "team" },  
                       new List<string> { "searchTerm" }
                   },
                   new List<string> { "AddPlayerToTeamAsync", "AddPlayerToTeamAsync", "AddPlayerToTeamAsync", "AddPlayerToTeamAsync", "AddPlayerToTeamAsync", "AddPlayerToTeamAsync", "RemovePlayerFromTeamAsync", "RemovePlayerFromTeamAsync", "RemovePlayerFromTeamAsync", "RemovePlayerFromTeamAsync", "RemovePlayerFromTeamAsync", "GetTeamsByNameAsync" }
               };
           }
       }
       [Theory]
       [MemberData(nameof(MediatorCallsiteIfnoExpectations))]
       public void AnalyzeBasicClassDeclaration_MediatorMethodReceivesAnalysisEnd_CallsiteInfoReceivedAtTheCorrectTime(List<string> expClassNames, List<string> expMethodNames, List<List<string>> expParameters, List<string> expBuilderMethodsNames)
       {
           // Arrange
           var mediatorMock = new Mock<IMediator>();

           // Create a collection to store the captured parameters
           List<string> receivedClassNames = new List<string>();
           List<string> receivedMethodNames = new List<string>();
           List<List<object>> receivedParameters = new List<List<object>>();
           List<MethodBuilder> receivedMethodBuilders = new List<MethodBuilder>();

            //===========================  Capturing the received info from the antlr visitor
            mediatorMock.Setup(x => x.GetKnownInstancesDeclaredInCurrentMethodAnalysis())
                .Returns(() =>
                {
                    return new Dictionary<string, AbstractInstance>().AsReadOnly();
                });
            mediatorMock.Setup(x => x.ReceiveMethodCall(It.IsAny<AbstractBuilder<MethodInstance>>()))
                .Callback<AbstractBuilder<MethodInstance>>((methodCallBuilder) =>
                {
                    var convertedMethodCallBuilder = (MethodInstanceBuilder) methodCallBuilder;
                    var methodInstance = methodCallBuilder.Build();
                    receivedClassNames.Add(methodInstance.callerClass.name);
                    receivedMethodNames.Add(methodInstance.methodName);
                    receivedParameters.Add(GetParametersListAsString(methodInstance.calledParameters));
                    receivedMethodBuilders.Add(convertedMethodCallBuilder.linkedMethodBuilder);
                });
            mediatorMock.Setup(x => x.ReceiveLocalVariableDeclaration(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<AbstractBuilder<MethodInstance>>()))
                .Callback<string, string, AbstractBuilder<MethodInstance>>((param1, param2, methodCallBuilder) =>
                {
                    var convertedMethodCallBuilder = (MethodInstanceBuilder) methodCallBuilder;
                    var methodInstance = methodCallBuilder.Build();
                    receivedClassNames.Add(methodInstance.callerClass.name);
                    receivedMethodNames.Add(methodInstance.methodName);
                    receivedParameters.Add(GetParametersListAsString(methodInstance.calledParameters));
                    receivedMethodBuilders.Add(convertedMethodCallBuilder.linkedMethodBuilder);
                });

            // Creating the antlr visitor
            _antlrService = new ANTLRService(mediatorMock.Object);
           _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile5.txt", true);

           // Act
           _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

           // Assert
           receivedMethodBuilders.Count.Should().Be(12);

           // Verify each class has been correctly identified
           receivedClassNames.Should().BeEquivalentTo(expClassNames);
           receivedMethodNames.Should().BeEquivalentTo(expMethodNames);
           receivedParameters.Should().BeEquivalentTo(expParameters);
           for (int i = 0; i < expBuilderMethodsNames.Count; i++)
           {
               receivedMethodBuilders[i].name.Should().BeEquivalentTo(expBuilderMethodsNames[i]);
           }
       }

   }

   // AnalyzeBasicClassDeclaration_InstancesDictionaryReceivesInstances_InstancesRecognizedCorrectly
}