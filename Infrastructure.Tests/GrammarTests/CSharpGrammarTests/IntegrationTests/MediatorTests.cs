using Domain.CodeInfo.InstanceDefinitions;
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
            InstancesDictionaryManager.instance.CleanInstancesDictionary();
        }

        public void AssertInstanceAssignment(KeyValuePair<AbstractInstance, AbstractInstance> instanceAssignment, string? keyType, string keyIdentifier
            , string? valueType, string? valueIdentifier, List<string> keyInheritanceNames, List<string>? valueInheritanceNames)
        {
            (instanceAssignment.Key).name = keyIdentifier;
            if(valueIdentifier is not null)
                (instanceAssignment.Value).name = valueIdentifier;
            if(keyType is not null)
                (instanceAssignment.Key).type = keyType;
            if (valueType is not null)
                (instanceAssignment.Value).type = valueType;
            else
                instanceAssignment.Value.Should().Be(null);
            (instanceAssignment.Key).inheritanceNames.OrderBy(x => x).SequenceEqual(keyInheritanceNames.OrderBy(x => x)).Should().Be(true);
            if(valueInheritanceNames is not null)
            (instanceAssignment.Value).inheritanceNames.OrderBy(x => x).SequenceEqual(valueInheritanceNames.OrderBy(x => x)).Should().Be(true);
        }

        public List<string> MakeStrList(params string[] values)
        {
            return values.ToList();
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
            InstancesDictionaryManager.instance.instancesDictionary.Count.Should().Be(7);

            // Creating an ordered dict out of the instacesDictionary to check all elements

            List<KeyValuePair<AbstractInstance, AbstractInstance>> instancesDicList = InstancesDictionaryManager.instance.instancesDictionary.ToList();
 
            int i = 0;
            int o = 0;
            // Verify each value inside the instancesDictionary has the correct info
            AssertInstanceAssignment(instancesDicList[0] 
                , keyType: "ITeamsRepository", keyIdentifier: "CleanArchitectureWorkshop.Application.UseCases.TeamsUseCase._teamsRepository"
                , keyInheritanceNames: new List<string> { "ITeamsUseCase", "Entity" }
                , valueType: null, valueIdentifier: null
                , valueInheritanceNames: null
                );
            AssertInstanceAssignment(instancesDicList[1]
                , keyType: null, keyIdentifier: "team.AddPlayer(player)"
                , keyInheritanceNames: new List<string> { "ITeamsUseCase", "Entity" }
                , valueType: null, valueIdentifier: null
                , valueInheritanceNames: null
                );
        }
    }
}