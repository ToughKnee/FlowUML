using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Infrastructure.ANTLR.CSharp;
using System.Reflection;

namespace Infrastructure.Antlr
{
    public class ANTLRService
    {
        private CSharpGrammarParser csharpParser;
        public List<(string, string)> properties;
        public List<(string, string)> methods;

        public ANTLRService(string text, bool isTextPath)
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

        public void RunVisitor()
        {
            var csharpContext = csharpParser.classDeclaration();
            var visitor = new CSharpVisitor();
            visitor.Visit(csharpContext);
        }

        public void RunVisitorWithSpecificStartingRule(string startingRule)
        {
            var parserType = typeof(CSharpGrammarParser);
            MethodInfo startingRuleMethod = parserType.GetMethod(startingRule);
            if (startingRule != null)
            {
                var csharpContext = (IParseTree) startingRuleMethod.Invoke(csharpParser, null);
                var visitor = new CSharpVisitor();
                visitor.Visit(csharpContext);
            }
            else
            {
                throw new ArgumentException("This rule does not exist");
            }
        }
    }
}
