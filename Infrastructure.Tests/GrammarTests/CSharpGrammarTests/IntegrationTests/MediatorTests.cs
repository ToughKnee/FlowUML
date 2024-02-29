using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Domain.CodeInfo.MethodSystem;
using FluentAssertions;
using Infrastructure.Antlr;
using Infrastructure.Mediators;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Infrastructure.Tests.GrammarTests.CSharpGrammarTests.IntegrationTests
{
    public class MediatorTests
    {
        private static string currentDirectoryPath = "..\\..\\..\\";
        private readonly string pathToTestFiles = "GrammarTests\\CSharpGrammarTests\\IntegrationTests\\TestFiles\\";
        private ANTLRService _antlrService;

        public MediatorTests()
        {
            InheritanceDictionaryManager.instance.CleanInheritanceDictionary();
            MethodDictionaryManager.instance.CleanMethodDictionary();
        }

        public void AssertInstanceAssignment(KeyValuePair<AbstractInstance, AbstractInstance> instanceAssignment, string? keyType, string keyIdentifier
            , string? valueType, string? valueIdentifier, List<string> keyInheritanceNames, List<string>? valueInheritanceNames)
        {
            (instanceAssignment.Key).name = keyIdentifier;
            if(valueIdentifier is not null)
                (instanceAssignment.Value).name = valueIdentifier;
            if(keyType is not null)
                (instanceAssignment.Key).refType.data = keyType;
            if (valueType is not null)
                (instanceAssignment.Value).refType.data = valueType;
            else
                instanceAssignment.Value.Should().Be(null);
            (instanceAssignment.Key).inheritedClasses.OrderBy(x => x).SequenceEqual(keyInheritanceNames.OrderBy(x => x)).Should().Be(true);
            if(valueInheritanceNames is not null)
                (instanceAssignment.Value).inheritedClasses.OrderBy(x => x).SequenceEqual(valueInheritanceNames.OrderBy(x => x)).Should().Be(true);
        }

        public List<string> MakeStrList(params string[] values)
        {
            return values.ToList();
        }

        public void AssertInstanceType(KeyValuePair<string, MethodInstance> methodInstance, string? keyType, string keyIdentifier
            , string? valueType, string? valueIdentifier, List<string> keyInheritanceNames, List<string>? valueInheritanceNames)
        {
        }

        [Fact]
        public void MediatorCreatedAllInstances_MediatorStartsBuildingClassEntities_ResolutionOfMethodInstancesAndTheirLinkedCallsitesDoneSuccesfully()
        {
            // Arrange
            var mediator = new AntlrMediator();
            _antlrService = new ANTLRService(mediator);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile6.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

            // Assert
            var classStrings = new List<string>();
            foreach(var classEntity in ClassEntityManager.instance.classEntities.Values)
            {
                classStrings.Add(classEntity.ToString());
            }
            //===========================  Checking the information about the ClassEntities and their Methods
            classStrings[0].Should().Be(@"Name: Entity
Namespace: CleanArchitectureWorkshop.Application.UseCases
Properties:
  grandParentProperty: string
Methods:
  CleanArchitectureWorkshop.Application.UseCases.GetEntityName(): string
Inherited Classes:
");
            classStrings[1].Should().Be(@"Name: ITeamsUseCase
Namespace: CleanArchitectureWorkshop.Application.UseCases
Properties:
  parentProperty: MyProperty
Methods:
  CleanArchitectureWorkshop.Application.UseCases.ToString(): string
Inherited Classes:
  Entity
");
            classStrings[2].Should().Be(@"Name: TeamsUseCase
Namespace: CleanArchitectureWorkshop.Application.UseCases
Properties:
  _teamsRepository: ITeamsRepository
Methods:
  CleanArchitectureWorkshop.Application.UseCases.AddPlayerToTeam(string, string): Team
Inherited Classes:
  Entity
  ITeamsUseCase
");
            classStrings[3].Should().Be(@"Name: MyProperty
Namespace: CleanArchitectureWorkshop.Application.UseCases
Properties:
  _printer: Printer
Methods:
  CleanArchitectureWorkshop.Application.UseCases.GetPrinter(): Printer
Inherited Classes:
");
            classStrings[4].Should().Be(@"Name: Printer
Namespace: CleanArchitectureWorkshop.Application.UseCases
Properties:
Methods:
  CleanArchitectureWorkshop.Application.UseCases.PrintResults(): Printer
Inherited Classes:
");
            classStrings[5].Should().Be(@"Name: TeamName
Namespace: CleanArchitectureWorkshop.Domain.TeamAggregate
Properties:
  Value: string
Methods:
  CleanArchitectureWorkshop.Domain.TeamAggregate.TeamName(string): TeamName
  CleanArchitectureWorkshop.Domain.TeamAggregate.Create(string): TeamName
Inherited Classes:
");
            classStrings[6].Should().Be(@"Name: UserName
Namespace: CleanArchitectureWorkshop.Domain.TeamAggregate
Properties:
  Value: string
Methods:
  CleanArchitectureWorkshop.Domain.TeamAggregate.UserName(string): UserName
  CleanArchitectureWorkshop.Domain.TeamAggregate.Create(string): UserName
Inherited Classes:
");
            classStrings[7].Should().Be(@"Name: Team
Namespace: CleanArchitectureWorkshop.Domain.TeamAggregate
Properties:
  _players: List<Player>
Methods:
  CleanArchitectureWorkshop.Domain.TeamAggregate.Team(TeamName): Team
  CleanArchitectureWorkshop.Domain.TeamAggregate.AddPlayer(Player): void
Inherited Classes:
  AggregateRoot<TeamName>
");
            classStrings[8].Should().Be(@"Name: Player
Namespace: CleanArchitectureWorkshop.Domain.TeamAggregate
Properties:
  Team: Team
Methods:
  CleanArchitectureWorkshop.Domain.TeamAggregate.Player(UserName): Player
  CleanArchitectureWorkshop.Domain.TeamAggregate.Player(): Player
  CleanArchitectureWorkshop.Domain.TeamAggregate.AssignTeam(Team): void
Inherited Classes:
");
            classStrings[9].Should().Be(@"Name: TeamsRepository
Namespace: CleanArchitectureWorkshop.Infrastructure.Repositories
Properties:
  _dbContext: ApplicationDbContext
Methods:
  CleanArchitectureWorkshop.Infrastructure.Repositories.TeamsRepository(ApplicationDbContext): TeamsRepository
  CleanArchitectureWorkshop.Infrastructure.Repositories.GetById(TeamName): Team
  CleanArchitectureWorkshop.Infrastructure.Repositories.UpdateTeam(Team): void
Inherited Classes:
  ITeamsRepository
");

            // TODO: Complete the other Callsites from the other Methods, not just TeamsUseCase
            //===========================  Checking the Callsites of each Method from the ClassEntities poiting to the respective Methods of other ClassEntities
            ClassEntityManager.instance.classEntities.Count.Should().Be(10);
            
            // Checking the callsites of TeamsUseCase
            var classEntitiesList = ClassEntityManager.instance.classEntities.Values.ToList();
            var TeamsUseCase_AddPlayerToTeamAsync_Method = classEntitiesList[2].methods[0];
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites.Count.Should().Be(8);
            var TeamNameCreateMethod = classEntitiesList[5].methods[1];
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites[0].calledMethod.Should().Be(TeamNameCreateMethod);

            var ITeamsRepositoryGetByIdAsyncMethod = classEntitiesList[9].methods[1];
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites[1].calledMethod.Should().Be(ITeamsRepositoryGetByIdAsyncMethod);

            var UserNameCreateMethod = classEntitiesList[6].methods[1];
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites[2].calledMethod.Should().Be(UserNameCreateMethod);

            var PlayerConstructorMethod = classEntitiesList[8].methods[0];
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites[3].calledMethod.Should().Be(PlayerConstructorMethod);

            var TeamAddPlayerMethod = classEntitiesList[7].methods[1];
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites[4].calledMethod.Should().Be(TeamAddPlayerMethod);

            var ITeamsRepositoryUpdateTeamAsyncMethod = classEntitiesList[9].methods[2];
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites[5].calledMethod.Should().Be(ITeamsRepositoryUpdateTeamAsyncMethod);

            var MyPropertyGetPrinterMethod = classEntitiesList[3].methods[0];
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites[6].calledMethod.Should().Be(MyPropertyGetPrinterMethod);

            var PrinetPrintResultsMethod = classEntitiesList[4].methods[0];
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites[7].calledMethod.Should().Be(PrinetPrintResultsMethod);

            // Checking the callsites of Entity
            var ITeamsUseCase_ToString_Method = classEntitiesList[1].methods[0];
            var GetEntityNameMethod = classEntitiesList[0].methods[0];
            ITeamsUseCase_ToString_Method.callsites[0].calledMethod.Should().Be(GetEntityNameMethod);

            // Checking the callsites of TeamName
            var TemaName_Create_Method = classEntitiesList[5].methods[0];
            var TeamNameConstructor = classEntitiesList[5].methods[0];
            TeamNameCreateMethod.callsites[0].calledMethod.Should().Be(TeamNameConstructor);

            // Checking the callsites of UserName
            var UserName_Create_Method = classEntitiesList[6].methods[0];
            var UserNameConstructor = classEntitiesList[6].methods[0];
            UserNameCreateMethod.callsites[0].calledMethod.Should().Be(UserNameConstructor);

        }
    }
}