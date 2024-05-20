using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Domain.CodeInfo.MethodSystem;
using FluentAssertions;
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
            InheritanceDictionaryManager.instance.CleanInheritanceDictionary();
            MethodDictionaryManager.instance.CleanMethodDictionary();
            ClassEntityManager.instance.CleanClassEntitiesDictionary();
        }

        public void AssertInstanceAssignment(KeyValuePair<AbstractInstance, AbstractInstance> instanceAssignment, string? keyType, string keyIdentifier
            , string? valueType, string? valueIdentifier, List<string> keyInheritanceNames, List<string>? valueInheritanceNames)
        {
            (instanceAssignment.Key).name = keyIdentifier;
            if (valueIdentifier is not null)
                (instanceAssignment.Value).name = valueIdentifier;
            if (keyType is not null)
                (instanceAssignment.Key).refType.data = keyType;
            if (valueType is not null)
                (instanceAssignment.Value).refType.data = valueType;
            else
                instanceAssignment.Value.Should().Be(null);
            (instanceAssignment.Key).inheritedClasses.OrderBy(x => x).SequenceEqual(keyInheritanceNames.OrderBy(x => x)).Should().Be(true);
            if (valueInheritanceNames is not null)
                (instanceAssignment.Value).inheritedClasses.OrderBy(x => x).SequenceEqual(valueInheritanceNames.OrderBy(x => x)).Should().Be(true);
        }

        public List<string> MakeStrList(params string[] values)
        {
            return values.ToList();
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
            foreach (var classEntity in ClassEntityManager.instance.classEntities.Values)
            {
                classStrings.Add(classEntity.ToString());
            }
            //===========================  Checking the information about the ClassEntities and their Methods
            classStrings[0].Should().Be(@"Name: Entity
Namespace: CleanArchitectureWorkshop.Application.UseCases
Properties:
  grandParentProperty: string
Methods:
  GetEntityName(): string
Inherited Classes:
");
            classStrings[1].Should().Be(@"Name: ITeamsUseCase
Namespace: CleanArchitectureWorkshop.Application.UseCases
Properties:
  parentProperty: MyProperty
Methods:
  ToString(): string
Inherited Classes:
  Entity
");
            classStrings[2].Should().Be(@"Name: TeamsUseCase
Namespace: CleanArchitectureWorkshop.Application.UseCases
Properties:
  _teamsRepository: ITeamsRepository
Methods:
  AddPlayerToTeam(string, string): Team
Inherited Classes:
  Entity
  ITeamsUseCase
");
            classStrings[3].Should().Be(@"Name: MyProperty
Namespace: CleanArchitectureWorkshop.Application.UseCases
Properties:
  _printer: Printer
Methods:
  GetPrinter(): Printer
Inherited Classes:
");
            classStrings[4].Should().Be(@"Name: Printer
Namespace: CleanArchitectureWorkshop.Application.UseCases
Properties:
Methods:
  PrintResults(): Printer
Inherited Classes:
");
            classStrings[5].Should().Be(@"Name: TeamName
Namespace: CleanArchitectureWorkshop.Domain.TeamAggregate
Properties:
  Value: string
Methods:
  TeamName(string): TeamName
  Create(string): TeamName
Inherited Classes:
");
            classStrings[6].Should().Be(@"Name: UserName
Namespace: CleanArchitectureWorkshop.Domain.TeamAggregate
Properties:
  Value: string
Methods:
  UserName(string): UserName
  Create(string): UserName
Inherited Classes:
");
            classStrings[7].Should().Be(@"Name: Team
Namespace: CleanArchitectureWorkshop.Domain.TeamAggregate
Properties:
  _players: List<Player>
Methods:
  Team(TeamName): Team
  AddPlayer(Player): void
Inherited Classes:
  AggregateRoot<TeamName>
");
            classStrings[8].Should().Be(@"Name: Player
Namespace: CleanArchitectureWorkshop.Domain.TeamAggregate
Properties:
  Team: Team
Methods:
  Player(UserName): Player
  Player(): Player
  AssignTeam(Team): void
Inherited Classes:
");
            classStrings[9].Should().Be(@"Name: TeamsRepository
Namespace: CleanArchitectureWorkshop.Infrastructure.Repositories
Properties:
  _dbContext: ApplicationDbContext
Methods:
  TeamsRepository(ApplicationDbContext): TeamsRepository
  GetById(TeamName): Team
  UpdateTeam(Team): void
Inherited Classes:
  ITeamsRepository
");

            //===========================  Checking the Callsites of each Method from the ClassEntities poiting to the respective Methods of other ClassEntities
            ClassEntityManager.instance.classEntities.Count.Should().Be(10);

            var classEntitiesList = ClassEntityManager.instance.classEntities.Values.ToList();
            var TeamsUseCase_AddPlayerToTeamAsync_Method = classEntitiesList[2].methods[0];
            var TeamNameCreateMethod = classEntitiesList[5].methods[1];
            var ITeamsRepositoryGetByIdAsyncMethod = classEntitiesList[9].methods[1];
            var UserNameCreateMethod = classEntitiesList[6].methods[1];
            var PlayerConstructorMethod = classEntitiesList[8].methods[0];
            var TeamAddPlayerMethod = classEntitiesList[7].methods[1];
            var ITeamsRepositoryUpdateTeamAsyncMethod = classEntitiesList[9].methods[2];
            var MyPropertyGetPrinterMethod = classEntitiesList[3].methods[0];
            var PrinetPrintResultsMethod = classEntitiesList[4].methods[0];
            var ITeamsUseCase_ToString_Method = classEntitiesList[1].methods[0];
            var GetEntityNameMethod = classEntitiesList[0].methods[0];
            var TemaName_Create_Method = classEntitiesList[5].methods[0];
            var TeamNameConstructor = classEntitiesList[5].methods[0];
            var UserName_Create_Method = classEntitiesList[6].methods[0];
            var UserNameConstructor = classEntitiesList[6].methods[0];

            // Checking the callsites of TeamsUseCase
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites.Count.Should().Be(8);
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites[0].calledMethod.Should().Be(TeamNameCreateMethod);
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites[1].calledMethod.Should().Be(ITeamsRepositoryGetByIdAsyncMethod);
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites[2].calledMethod.Should().Be(UserNameCreateMethod);
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites[3].calledMethod.Should().Be(PlayerConstructorMethod);
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites[4].calledMethod.Should().Be(TeamAddPlayerMethod);
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites[5].calledMethod.Should().Be(ITeamsRepositoryUpdateTeamAsyncMethod);
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites[6].calledMethod.Should().Be(MyPropertyGetPrinterMethod);
            TeamsUseCase_AddPlayerToTeamAsync_Method.callsites[7].calledMethod.Should().Be(PrinetPrintResultsMethod);

            // Checking the callsites of Entity
            ITeamsUseCase_ToString_Method.callsites[0].calledMethod.Should().Be(GetEntityNameMethod);

            // Checking the callsites of TeamName
            TeamNameCreateMethod.callsites[0].calledMethod.Should().Be(TeamNameConstructor);

            // Checking the callsites of UserName
            UserNameCreateMethod.callsites[0].calledMethod.Should().Be(UserNameConstructor);
        }

        [Fact]
        public void MediatorCreatedAllInstances_MediatorStartsBuildingClassEntities_ResolutionOfMethodInstancesAndTheirLinkedCallsitesDoneSuccesfully2()
        {
            // Arrange
            var mediator = new AntlrMediator();
            _antlrService = new ANTLRService(mediator);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile7.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

            // Assert
            var classStrings = new List<string>();
            foreach (var classEntity in ClassEntityManager.instance.classEntities.Values)
            {
                classStrings.Add(classEntity.ToString());
            }
            //===========================  Checking the information about the ClassEntities and their Methods
            classStrings[0].Should().Be(@"Name: Program
Namespace: MyNamespace
Properties:
Methods:
  Main(): void
Inherited Classes:
");
            classStrings[1].Should().Be(@"Name: ClassA
Namespace: MyNamespace
Properties:
Methods:
  MethodA(): void
  AnotherMethodA(): void
Inherited Classes:
");
            classStrings[2].Should().Be(@"Name: ClassB
Namespace: MyNamespace
Properties:
Methods:
  MethodB(): void
  AnotherMethodB(): void
Inherited Classes:
");

            //===========================  Checking the Callsites of each Method from the ClassEntities poiting to the respective Methods of other ClassEntities
            ClassEntityManager.instance.classEntities.Count.Should().Be(3);

            var classEntitiesList = ClassEntityManager.instance.classEntities.Values.ToList();
            var Program_Main = classEntitiesList[0].methods[0];
            var MethodA = classEntitiesList[1].methods[0];
            var MethodB = classEntitiesList[2].methods[0];
            var ClassA_MethodA = classEntitiesList[1].methods[0];
            var AnotherMethodA = classEntitiesList[1].methods[1];
            var AnotherMethodB = classEntitiesList[2].methods[1];
            var ClassB_MethodB = classEntitiesList[2].methods[0];

            // Checking the callsites of Main Method from the Program class
            Program_Main.callsites.Count.Should().Be(2);
            Program_Main.callsites[0].calledMethod.Should().Be(MethodA);
            Program_Main.callsites[1].calledMethod.Should().Be(MethodB);

            // Checking the callsites of the ClassA class
            ClassA_MethodA.callsites.Count.Should().Be(3);
            ClassA_MethodA.callsites[0].calledMethod.Should().Be(MethodB);
            ClassA_MethodA.callsites[1].calledMethod.Should().Be(AnotherMethodA);
            ClassA_MethodA.callsites[2].calledMethod.Should().Be(AnotherMethodB);

            // Checking the callsites of the ClassB class
            ClassB_MethodB.callsites.Count.Should().Be(2);
            ClassB_MethodB.callsites[0].calledMethod.Should().Be(MethodA);
            ClassB_MethodB.callsites[1].calledMethod.Should().Be(AnotherMethodB);
        }
        [Fact]
        public void MediatorCreatedAllInstances_MediatorStartsBuildingClassEntities_ResolutionOfMethodInstancesAndTheirLinkedCallsitesDoneSuccesfully3()
        {
            // Arrange
            var mediator = new AntlrMediator();
            _antlrService = new ANTLRService(mediator);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile8.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

            // Assert
            var classStrings = new List<string>();
            foreach (var classEntity in ClassEntityManager.instance.classEntities.Values)
            {
                classStrings.Add(classEntity.ToString());
            }
            //===========================  Checking the information about the ClassEntities and their Methods
            classStrings[0].Should().Be(@"Name: Program
Namespace: MyNamespace
Properties:
Methods:
  Main(): void
Inherited Classes:
");
            classStrings[1].Should().Be(@"Name: BaseClass
Namespace: MyNamespace
Properties:
Methods:
  CommonMethod(): void
Inherited Classes:
");
            classStrings[2].Should().Be(@"Name: DerivedClassA
Namespace: MyNamespace
Properties:
Methods:
  MethodA(): void
  AnotherMethodA(): void
Inherited Classes:
  BaseClass
");
            classStrings[3].Should().Be(@"Name: DerivedClassB
Namespace: MyNamespace
Properties:
Methods:
  MethodB(): void
  AnotherMethodB(): void
Inherited Classes:
  BaseClass
");

            //===========================  Checking the Callsites of each Method from the ClassEntities poiting to the respective Methods of other ClassEntities
            ClassEntityManager.instance.classEntities.Count.Should().Be(4);

            var classEntitiesList = ClassEntityManager.instance.classEntities.Values.ToList();
            var Program_Main = classEntitiesList[0].methods[0];
            var MethodA = classEntitiesList[2].methods[0];
            var MethodB = classEntitiesList[3].methods[0];
            var DerivedClassA_MethodA = classEntitiesList[2].methods[0];
            var CommonMethod = classEntitiesList[1].methods[0];
            var AnotherMethodA = classEntitiesList[2].methods[1];
            var ClassB_MethodB = classEntitiesList[3].methods[0];
            var AnotherMethodB = classEntitiesList[3].methods[1];

            // Checking the callsites of the Program class
            Program_Main.callsites.Count.Should().Be(4);
            Program_Main.callsites[2].calledMethod.Should().Be(MethodA);
            Program_Main.callsites[3].calledMethod.Should().Be(MethodB);

            // Checking the callsites of the DervivedClassA class
            DerivedClassA_MethodA.callsites.Count.Should().Be(2);
            DerivedClassA_MethodA.callsites[0].calledMethod.Should().Be(CommonMethod);
            DerivedClassA_MethodA.callsites[1].calledMethod.Should().Be(AnotherMethodA);

            // Checking the callsites of the DerivedClassB class
            ClassB_MethodB.callsites.Count.Should().Be(2);
            ClassB_MethodB.callsites[0].calledMethod.Should().Be(CommonMethod);
            ClassB_MethodB.callsites[1].calledMethod.Should().Be(AnotherMethodB);
        }
        [Fact]
        public void MediatorCreatedComplexInstancesWithNestedMethodCalls_MediatorStartsBuildingClassEntities_ResolutionOfMethodInstancesAndTheirLinkedCallsitesDoneSuccesfully()
        {
            // Arrange
            var mediator = new AntlrMediator();
            _antlrService = new ANTLRService(mediator);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "AdvancedLevel\\AdvTextFile1.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

            // Assert
            var classStrings = new List<string>();
            foreach (var classEntity in ClassEntityManager.instance.classEntities.Values)
            {
                classStrings.Add(classEntity.ToString());
            }
            //===========================  Checking the information about the ClassEntities and their Methods
            classStrings[0].Should().Be(@"Name: Program
Namespace: MyNamespace
Properties:
Methods:
  Main(): void
Inherited Classes:
");
            classStrings[1].Should().Be(@"Name: Class1
Namespace: MyNamespace
Properties:
  Prop1: Class2
Methods:
  Method1(): void
  ReturnName(): string
Inherited Classes:
");
            classStrings[2].Should().Be(@"Name: Class2
Namespace: MyNamespace
Properties:
  data: string
  class1Data: Class1
Methods:
  FunctionCall(): Class3
  FunctionCall2(Class3, string): Class1
Inherited Classes:
");
            classStrings[3].Should().Be(@"Name: Class3
Namespace: MyNamespace
Properties:
Methods:
  OtherFunctionCall(): void
Inherited Classes:
");

            //===========================  Checking the Callsites of each Method from the ClassEntities poiting to the respective Methods of other ClassEntities
            ClassEntityManager.instance.classEntities.Count.Should().Be(4);

            var classEntitiesList = ClassEntityManager.instance.classEntities.Values.ToList();

            var Program_Main = classEntitiesList[0].methods[0];
            var Method1 = classEntitiesList[1].methods[0];
            var ReturnName = classEntitiesList[1].methods[1];
            var FunctionCall = classEntitiesList[2].methods[0];
            var FunctionCall2 = classEntitiesList[2].methods[1];
            var OtherFunctionCall = classEntitiesList[3].methods[0];

            // Checking the callsites of the Program class
            Program_Main.callsites.Count.Should().Be(3);
            Program_Main.callsites[0].calledMethod.Should().Be(null);
            Program_Main.callsites[1].calledMethod.Should().Be(FunctionCall);
            Program_Main.callsites[2].calledMethod.Should().Be(OtherFunctionCall);

            // Checking the callsites of the Class1 class
            Method1.callsites.Count.Should().Be(2);
            Method1.callsites[0].calledMethod.Should().Be(ReturnName);

            // Checking the callsites of the Class3 class
            OtherFunctionCall.callsites.Count.Should().Be(4);
            OtherFunctionCall.callsites[1].calledMethod.Should().Be(FunctionCall);
            OtherFunctionCall.callsites[2].calledMethod.Should().Be(FunctionCall2);
            OtherFunctionCall.callsites[3].calledMethod.Should().Be(FunctionCall);
        }
        [Fact]
        public void MediatorCreatedComplexInstancesWithSeveralProperties_MediatorStartsBuildingClassEntities_ResolutionOfMethodInstancesAndTheirLinkedCallsitesDoneSuccesfully()
        {
            // Arrange
            var mediator = new AntlrMediator();
            _antlrService = new ANTLRService(mediator);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "AdvancedLevel\\AdvTextFile2.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

            // Assert
            var classStrings = new List<string>();
            foreach (var classEntity in ClassEntityManager.instance.classEntities.Values)
            {
                classStrings.Add(classEntity.ToString());
            }
            //===========================  Checking the information about the ClassEntities and their Methods
            classStrings[0].Should().Be(@"Name: Program
Namespace: MyNamespace
Properties:
Methods:
  Main(): void
Inherited Classes:
");
            classStrings[1].Should().Be(@"Name: Class1
Namespace: MyNamespace
Properties:
  class1Prop1: Class2
Methods:
  Method1(): void
Inherited Classes:
");
            classStrings[2].Should().Be(@"Name: Class2
Namespace: MyNamespace
Properties:
  class2Propr1: Class3
Methods:
Inherited Classes:
");
            classStrings[3].Should().Be(@"Name: Class3
Namespace: MyNamespace
Properties:
  class3Propr1: Class1
Methods:
  FunctionCall(): Class3
Inherited Classes:
");

            //===========================  Checking the Callsites of each Method from the ClassEntities poiting to the respective Methods of other ClassEntities
            ClassEntityManager.instance.classEntities.Count.Should().Be(4);

            var classEntitiesList = ClassEntityManager.instance.classEntities.Values.ToList();

            // Creating the references to the methods to be checked
            var Program_Main = classEntitiesList[0].methods[0];
            var Method1 = classEntitiesList[1].methods[0];
            var FunctionCall = classEntitiesList[3].methods[0];

            // Checking the callsites of the Program class
            Program_Main.callsites.Count.Should().Be(3);
            Program_Main.callsites[0].calledMethod.Should().Be(null);
            Program_Main.callsites[1].calledMethod.Should().Be(FunctionCall);
            Program_Main.callsites[2].calledMethod.Should().Be(Method1);
        }
        [Fact]
        public void MediatorCreatedInstancesSharingALocalVariableComponentAndRepeatedMethodFromDifferentClasses_MediatorStartsBuildingClassEntities_ResolutionOfMethodInstancesAndTheirLinkedCallsitesDoneSuccesfully()
        {
            // Arrange
            var mediator = new AntlrMediator();
            _antlrService = new ANTLRService(mediator);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "AdvancedLevel\\AdvTextFile3.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

            // Assert
            var classStrings = new List<string>();
            foreach (var classEntity in ClassEntityManager.instance.classEntities.Values)
            {
                classStrings.Add(classEntity.ToString());
            }
            //===========================  Checking the information about the ClassEntities and their Methods
            classStrings[0].Should().Be(@"Name: Program
Namespace: MyNamespace
Properties:
Methods:
  Main(): void
  Main2(): void
Inherited Classes:
");
            classStrings[1].Should().Be(@"Name: Class1
Namespace: MyNamespace
Properties:
Methods:
  SomeMethod(): void
  SomeOtherMethod(Class5, Class6): Class3
Inherited Classes:
");
            classStrings[2].Should().Be(@"Name: Class2
Namespace: MyNamespace
Properties:
Methods:
  AnotherMethod(Class1): void
Inherited Classes:
");
            classStrings[3].Should().Be(@"Name: Class3
Namespace: MyNamespace
Properties:
  data6: Class6
Methods:
  GetClass2(): Class2
  GetClass1(Class2): Class1
  GetClass4(): Class4
  GetClass5(): Class5
  GetClass6(): Class6
  AnotherMethod(Class3): void
  YetAnotherMethod(Class3): string
  AlmostFinalMethod(Class3): int
  LastMethod(Class3): float
Inherited Classes:
");

            //===========================  Checking the Callsites of each Method from the ClassEntities poiting to the respective Methods of other ClassEntities
            ClassEntityManager.instance.classEntities.Count.Should().Be(7);

            var classEntitiesList = ClassEntityManager.instance.classEntities.Values.ToList();

            // Creating the references to the methods to be checked
            var Program_Main = classEntitiesList[0].methods[0];
            var Program_Main2 = classEntitiesList[0].methods[1];
            var GetClass2 = classEntitiesList[3].methods[0];
            var GetClass1 = classEntitiesList[3].methods[1];
            var SomeMethod = classEntitiesList[1].methods[0];
            var Class2_AnotherMethod = classEntitiesList[2].methods[0];
            var GetClass4 = classEntitiesList[3].methods[2];
            var Class3_AnotherMethod = classEntitiesList[3].methods[5];
            var Class3_YetAnotherMethod = classEntitiesList[3].methods[6];
            var Class4_YetAnotherMethod = classEntitiesList[4].methods[0];
            var Class4_YetSomeAnotherMethod = classEntitiesList[4].methods[1];
            var GetClass5 = classEntitiesList[3].methods[3];
            var Class5_AlmostFinalMethod = classEntitiesList[5].methods[0];
            var GetClass6 = classEntitiesList[3].methods[4];
            var Class3_AlmostFinalMethod = classEntitiesList[3].methods[7];
            var Class3_LastMethod = classEntitiesList[3].methods[8];
            var Class6_LastMethod = classEntitiesList[6].methods[0];

            var SomeOtherMethod = classEntitiesList[1].methods[1];

            // Checking the callsites of the Program class
            Program_Main.callsites.Count.Should().Be(15);
            Program_Main.callsites[0].calledMethod.Should().Be(null);
            Program_Main.callsites[1].calledMethod.Should().Be(GetClass2);
            Program_Main.callsites[2].calledMethod.Should().Be(GetClass1);
            Program_Main.callsites[3].calledMethod.Should().Be(SomeMethod);
            Program_Main.callsites[4].calledMethod.Should().Be(Class2_AnotherMethod);
            Program_Main.callsites[5].calledMethod.Should().Be(GetClass4);
            Program_Main.callsites[6].calledMethod.Should().Be(Class3_AnotherMethod);
            Program_Main.callsites[7].calledMethod.Should().Be(Class3_YetAnotherMethod);
            Program_Main.callsites[8].calledMethod.Should().Be(Class4_YetAnotherMethod);
            Program_Main.callsites[9].calledMethod.Should().Be(GetClass5);
            Program_Main.callsites[10].calledMethod.Should().Be(Class5_AlmostFinalMethod);
            Program_Main.callsites[11].calledMethod.Should().Be(GetClass6);
            Program_Main.callsites[12].calledMethod.Should().Be(Class3_AlmostFinalMethod);
            Program_Main.callsites[13].calledMethod.Should().Be(Class3_LastMethod);
            Program_Main.callsites[14].calledMethod.Should().Be(Class6_LastMethod);

            Program_Main2.callsites.Count.Should().Be(6);
            Program_Main2.callsites[0].calledMethod.Should().Be(null);
            Program_Main2.callsites[1].calledMethod.Should().Be(GetClass2);
            Program_Main2.callsites[2].calledMethod.Should().Be(GetClass1);
            Program_Main2.callsites[3].calledMethod.Should().Be(GetClass6);
            Program_Main2.callsites[4].calledMethod.Should().Be(Class4_YetSomeAnotherMethod);
            Program_Main2.callsites[5].calledMethod.Should().Be(SomeOtherMethod);
        }
        [Fact]
        public void MediatorCreatedInstancesWithLists_MediatorStartsBuildingClassEntitiesAndRecognizesInstancesWithTypenames_ResolutionOfMethodInstancesAndTheirLinkedCallsitesDoneSuccesfully()
        {
            // Arrange
            var mediator = new AntlrMediator();
            _antlrService = new ANTLRService(mediator);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "AdvancedLevel\\AdvTextFile4.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

            // Assert
            var classStrings = new List<string>();
            foreach (var classEntity in ClassEntityManager.instance.classEntities.Values)
            {
                classStrings.Add(classEntity.ToString());
            }
            //===========================  Checking the information about the ClassEntities and their Methods
            classStrings[0].Should().Be(@"Name: SNode
Namespace: MyNamespace
Properties:
  Next: SNode<T,R>
  Value: T
  RValue: R
Methods:
  SNode(T): SNode
  SNode(): SNode
  GetValue(): T
  GetRValue(): R
Inherited Classes:
");

            //===========================  Checking the Callsites of each Method from the ClassEntities poiting to the respective Methods of other ClassEntities
            ClassEntityManager.instance.classEntities.Count.Should().Be(2);

            var classEntitiesList = ClassEntityManager.instance.classEntities.Values.ToList();

            // Creating the references to the methods to be checked
            var Program_Main = classEntitiesList[1].methods[0];
            var GetValue = classEntitiesList[0].methods[2];
            var GetRValue = classEntitiesList[0].methods[3];
            var SNodeIntOperation = classEntitiesList[1].methods[1];

            // Checking the callsites of the Program class
            Program_Main.callsites.Count.Should().Be(6);
            Program_Main.callsites[0].calledMethod.Should().Be(null);
            Program_Main.callsites[1].calledMethod.Should().Be(null);
            Program_Main.callsites[2].calledMethod.Should().Be(null);
            Program_Main.callsites[3].calledMethod.Should().Be(GetValue);
            Program_Main.callsites[4].calledMethod.Should().Be(GetRValue);
            Program_Main.callsites[5].calledMethod.Should().Be(SNodeIntOperation);
        }
        [Fact]
        public void MediatorCreatedInstancesWithIndexedCollections1_MediatorStartsBuildingClassEntitiesAndRecognizesCorrectlyInstancesFromLists_ResolutionOfMethodInstancesAndTheirLinkedCallsitesDoneSuccesfully()
        {
            // Arrange
            var mediator = new AntlrMediator();
            _antlrService = new ANTLRService(mediator);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile9.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

            // Assert
            var classStrings = new List<string>();
            foreach (var classEntity in ClassEntityManager.instance.classEntities.Values)
            {
                classStrings.Add(classEntity.ToString());
            }
            //===========================  Checking the information about the ClassEntities and their Methods
//            classStrings[0].Should().Be(@"Name: SNode
//Inherited Classes:
//");

            //===========================  Checking the Callsites of each Method from the ClassEntities poiting to the respective Methods of other ClassEntities
            ClassEntityManager.instance.classEntities.Count.Should().Be(3);

            var classEntitiesList = ClassEntityManager.instance.classEntities.Values.ToList();

            // Creating the references to the methods to be checked
            var Program_Main = classEntitiesList[0].methods[0];
            var AssignMethod = classEntitiesList[0].methods[1];
            var GetMyListMethod = classEntitiesList[0].methods[2];
            var myFuncCallMethod = classEntitiesList[1].methods[0];
            var otherThingMethod = classEntitiesList[1].methods[1];
            var differentOtherThingMethod = classEntitiesList[1].methods[2];
            var normalThingMethod = classEntitiesList[1].methods[3];
            var myOtherClassFunctionMethod = classEntitiesList[2].methods[0];

            // Checking the callsites of the Program class
            Program_Main.callsites.Count.Should().Be(17);
            Program_Main.callsites[0].calledMethod.Should().Be(null);
            Program_Main.callsites[1].calledMethod.Should().Be(otherThingMethod);
            Program_Main.callsites[2].calledMethod.Should().Be(differentOtherThingMethod);
            Program_Main.callsites[3].calledMethod.Should().Be(myOtherClassFunctionMethod);
            Program_Main.callsites[4].calledMethod.Should().Be(GetMyListMethod);
            Program_Main.callsites[5].calledMethod.Should().Be(otherThingMethod);
            Program_Main.callsites[6].calledMethod.Should().Be(GetMyListMethod);
            Program_Main.callsites[7].calledMethod.Should().Be(AssignMethod);

            Program_Main.callsites[8].calledMethod.Should().Be(null);
            Program_Main.callsites[9].calledMethod.Should().Be(otherThingMethod);
            Program_Main.callsites[10].calledMethod.Should().Be(myFuncCallMethod);
            Program_Main.callsites[11].calledMethod.Should().Be(normalThingMethod);
            Program_Main.callsites[12].calledMethod.Should().Be(otherThingMethod);
            Program_Main.callsites[13].calledMethod.Should().Be(myOtherClassFunctionMethod);
            Program_Main.callsites[14].calledMethod.Should().Be(myFuncCallMethod);

            Program_Main.callsites[15].calledMethod.Should().Be(null);
            Program_Main.callsites[16].calledMethod.Should().Be(myOtherClassFunctionMethod);
        }

        [Fact]
        public void MediatorCreatedInstancesWithVeryDeepMethodCallsAndTypeCasterAndSpecialNewConstructor_MethodInstancesTakeShortcutsToDiscoverInstancesTypes_ResolutionOfMethodInstancesAndTheirLinkedCallsitesDoneSuccesfully()
        {
            // Arrange
            var mediator = new AntlrMediator();
            _antlrService = new ANTLRService(mediator);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile10.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

            // Assert
            var classStrings = new List<string>();
            foreach (var classEntity in ClassEntityManager.instance.classEntities.Values)
            {
                classStrings.Add(classEntity.ToString());
            }
            //===========================  Checking the information about the ClassEntities and their Methods
            //            classStrings[0].Should().Be(@"Name: SNode
            //Inherited Classes:
            //");

            //===========================  Checking the Callsites of each Method from the ClassEntities poiting to the respective Methods of other ClassEntities
            ClassEntityManager.instance.classEntities.Count.Should().Be(3);

            var classEntitiesList = ClassEntityManager.instance.classEntities.Values.ToList();

            // Creating the references to the methods to be checked
            var Program_Main = classEntitiesList[0].methods[0];
            var CalculateNumbersMethod = classEntitiesList[0].methods[1];
            var PrintResultsMethod = classEntitiesList[1].methods[1];
            var ResultsConstructorMethod = classEntitiesList[1].methods[0];
            var PrintOtherResultsMethod = classEntitiesList[2].methods[0];

            // Checking the callsites of the Program class
            Program_Main.callsites.Count.Should().Be(7);
            Program_Main.callsites[0].calledMethod.Should().Be(CalculateNumbersMethod);
            Program_Main.callsites[1].calledMethod.Should().Be(PrintResultsMethod);
            Program_Main.callsites[2].calledMethod.Should().Be(PrintOtherResultsMethod);
            Program_Main.callsites[3].calledMethod.Should().Be(PrintOtherResultsMethod);
            Program_Main.callsites[4].calledMethod.Should().Be(ResultsConstructorMethod);
            Program_Main.callsites[5].calledMethod.Should().Be(PrintOtherResultsMethod);
            Program_Main.callsites[6].calledMethod.Should().Be(PrintOtherResultsMethod);
        }
        [Fact]
        public void MediatorCreatedInstancesWithVeryDeepMethodCalls_MethodInstancesTakeShortcutsToDiscoverInstancesTypes_ResolutionOfMethodInstancesAndTheirLinkedCallsitesDoneSuccesfully()
        {
            // Arrange
            var mediator = new AntlrMediator();
            _antlrService = new ANTLRService(mediator);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "BasicLevel\\TextFile11.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

            // Assert
            var classStrings = new List<string>();
            foreach (var classEntity in ClassEntityManager.instance.classEntities.Values)
            {
                classStrings.Add(classEntity.ToString());
            }
            //===========================  Checking the information about the ClassEntities and their Methods
            //            classStrings[0].Should().Be(@"Name: SNode
            //Inherited Classes:
            //");

            //===========================  Checking the Callsites of each Method from the ClassEntities poiting to the respective Methods of other ClassEntities
            ClassEntityManager.instance.classEntities.Count.Should().Be(4);

            var classEntitiesList = ClassEntityManager.instance.classEntities.Values.ToList();

            // Creating the references to the methods to be checked
            var Program_Main = classEntitiesList[0].methods[0];
            var CalculateNumbersMethod = classEntitiesList[0].methods[1];
            var GetConstantTimeMethod = classEntitiesList[0].methods[2];
            var operatorBBMethod = classEntitiesList[0].methods[3];
            var operatorCCMethod = classEntitiesList[0].methods[4];
            var conflicterFunctionMethodReturnFloat = classEntitiesList[0].methods[5];
            var GetRangeMethod = classEntitiesList[1].methods[0];
            var GetIDMethod = classEntitiesList[1].methods[1];
            var FoVMethod = classEntitiesList[2].methods[0];
            var GetCameraValueMethod = classEntitiesList[2].methods[1];
            var conflicterFunctionMethodReturnDouble = classEntitiesList[2].methods[2];
            var PrintResultsMethod = classEntitiesList[3].methods[0];

            // Checking the callsites of the Program class
            Program_Main.callsites.Count.Should().Be(13);
            Program_Main.callsites[0].calledMethod.Should().Be(GetRangeMethod);
            Program_Main.callsites[1].calledMethod.Should().Be(FoVMethod);
            Program_Main.callsites[2].calledMethod.Should().Be(GetConstantTimeMethod);
            Program_Main.callsites[3].calledMethod.Should().Be(operatorBBMethod);
            Program_Main.callsites[4].calledMethod.Should().Be(CalculateNumbersMethod);
            Program_Main.callsites[5].calledMethod.Should().Be(FoVMethod);
            Program_Main.callsites[6].calledMethod.Should().Be(GetCameraValueMethod);
            Program_Main.callsites[7].calledMethod.Should().Be(GetIDMethod);
            Program_Main.callsites[8].calledMethod.Should().Be(PrintResultsMethod);
            Program_Main.callsites[9].calledMethod.Should().Be(operatorBBMethod);
            Program_Main.callsites[10].calledMethod.Should().Be(conflicterFunctionMethodReturnFloat);
            Program_Main.callsites[11].calledMethod.Should().Be(operatorCCMethod);
            Program_Main.callsites[12].calledMethod.Should().Be(conflicterFunctionMethodReturnDouble);
        }
        [Fact]
        public void MediatorCreatedInstancesWithMethodCallsWithExtraParenthesesAndTypeCastersInParentheses_MethodInstancesResolveTheirTypes_ResolutionOfMethodInstancesAndTheirLinkedCallsitesDoneSuccesfully()
        {
            // Arrange
            var mediator = new AntlrMediator();
            _antlrService = new ANTLRService(mediator);
            _antlrService.InitializeAntlr(currentDirectoryPath + pathToTestFiles + "AdvancedLevel\\AdvTextFile5.txt", true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("cSharpFile");

            // Assert
            var classStrings = new List<string>();
            foreach (var classEntity in ClassEntityManager.instance.classEntities.Values)
            {
                classStrings.Add(classEntity.ToString());
            }
            //===========================  Checking the information about the ClassEntities and their Methods
            //            classStrings[0].Should().Be(@"Name: SNode
            //Inherited Classes:
            //");

            //===========================  Checking the Callsites of each Method from the ClassEntities poiting to the respective Methods of other ClassEntities
            //ClassEntityManager.instance.classEntities.Count.Should().Be(2);

            var classEntitiesList = ClassEntityManager.instance.classEntities.Values.ToList();

            // Creating the references to the methods to be checked
            var Program_Main = classEntitiesList[0].methods[0];
            var CreateClass2Method = classEntitiesList[0].methods[1];
            var Class1FunctionMethod = classEntitiesList[1].methods[0];
            var BuildMethod = classEntitiesList[2].methods[0];
            var Class3FunctionMethod = classEntitiesList[3].methods[0];

            // Checking the callsites of the Program class
            Program_Main.callsites.Count.Should().Be(4);
            Program_Main.callsites[0].calledMethod.Should().Be(CreateClass2Method);
            Program_Main.callsites[1].calledMethod.Should().Be(Class3FunctionMethod);
            Program_Main.callsites[2].calledMethod.Should().Be(BuildMethod);
            Program_Main.callsites[3].calledMethod.Should().Be(Class1FunctionMethod);
        }
    }
}