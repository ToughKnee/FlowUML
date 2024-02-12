using Domain.CodeInfo.InstanceDefinitions;
using FluentAssertions;
using Infrastructure.Antlr;
using Infrastructure.Mediators;
using System.Collections;
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

        public void AssertInstanceAssignment(DictionaryEntry instanceAssignment, string keyIdentifier, string? valueIdentifier
            , string? keyType, string? valueType, List<string> keyInheritanceNames, List<string> valueInheritanceNames)
        {
            ((AbstractInstance)instanceAssignment.Key).name = keyIdentifier;
            ((AbstractInstance)instanceAssignment.Value).name = valueIdentifier;
            ((AbstractInstance)instanceAssignment.Key).type = keyType;
            ((AbstractInstance)instanceAssignment.Value).type = valueType;
            ((AbstractInstance)instanceAssignment.Key).inheritanceNames.OrderBy(x => x).SequenceEqual(keyInheritanceNames.OrderBy(x => x));
            ((AbstractInstance)instanceAssignment.Value).inheritanceNames.OrderBy(x => x).SequenceEqual(valueInheritanceNames.OrderBy(x => x));
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
            int i = 0;
            InstancesDictionaryManager.instance.instancesDictionary.Count.Should().Be(12);

            // Creating an ordered dict out of the instacesDictionary to check all elements
            OrderedDictionary orderedDict = new OrderedDictionary();
            foreach (KeyValuePair<AbstractInstance, AbstractInstance> kvp in InstancesDictionaryManager.instance.instancesDictionary)
            {
                orderedDict.Add(kvp.Key, kvp.Value);
            }

            // Verify each value inside the instancesDictionary has the correct info

        }
    }
}