using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Domain.CodeInfo;
using Infrastructure.ANTLR.CSharp;
using Infrastructure.Builders;
using Infrastructure.Mediators;
using System.Collections.Generic;
using System.Reflection;

namespace Infrastructure.Antlr
{
    public class ANTLRService
    {
        private IMediator _mediator;
        private CSharpGrammarParser csharpParser;
        public List<(string, string)> properties;
        public List<(string, string)> methods;

        public ANTLRService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void InitializeAntlr(string text, bool isTextPath)
        {
            string textToAnalyze = "";
            if (isTextPath)
            {
                textToAnalyze = File.ReadAllText(text);
            }
            else
            {
                textToAnalyze = text;
            }
            var inputStream = new AntlrInputStream(textToAnalyze);
            var csharpLexer = new CSharpGrammarLexer(inputStream);
            var commonTkenStream = new CommonTokenStream(csharpLexer);
            csharpParser = new CSharpGrammarParser(commonTkenStream);
        }

        public void RunVisitorWithSpecificStartingRule(string startingRule)
        {
            var parserType = typeof(CSharpGrammarParser);
            MethodInfo startingRuleMethod = parserType.GetMethod(startingRule);
            if (startingRule != null)
            {
                var csharpContext = (IParseTree) startingRuleMethod.Invoke(csharpParser, null);
                var cSharpVisitor = new CSharpVisitor(_mediator);
                cSharpVisitor.Visit(csharpContext);

                // After the visitor finished analyzing the file, pass the builder to the mediator
                _mediator.ReceiveMethodBuilders(cSharpVisitor.GetMethodBuilders());
                _mediator.ReceiveClassEntityBuilders(cSharpVisitor.GetClassBuilders());
                _mediator.DefineUndefinedMethodInstances();
            }
            else
            {
                throw new ArgumentException("This rule does not exist");
            }
        }
    }
}
