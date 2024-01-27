using Domain.CodeInfo.InstanceDefinitions;
using Infrastructure.Antlr;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using System;
using System.ComponentModel;
using Xunit.Abstractions;

namespace Infrastructure.Tests.GrammarTests.CSharpGrammarTests.IntegrationTests
{
    public class InstanceDictionaryAntlrTests
    {
        private readonly string pathToTestFile1 = "C:\\Users\\Usuario\\source\\repos\\FlowUML\\Infrastructure.Tests\\GrammarTests\\CSharpGrammarTests\\IntegrationTests\\TestFiles\\TextFile1.txt";
        private ANTLRService _antlrService;
        public InstanceDictionaryAntlrTests()
        {
        }

        [Fact]
        public void Analyze_BasicmethodRule_instanceDictHasCorrectCount()
        {
            // Arrange
            _antlrService = new ANTLRService(pathToTestFile1, true);

            // Act
            _antlrService.RunVisitorWithSpecificStartingRule("method");

            // Assert
            Assert.True(InstancesDictionaryManager.instance.instancesDictionary.Count == 5);
            
        }
    }
}