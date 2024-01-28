using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Infrastructure.ANTLR.CSharp;
using System.Reflection;

namespace Infrastructure.Antlr
{
    public class ANTLRService
    {
        private CSharpGrammarParser csharpParser;
        private CSharpVisitor _cSharpVisitor;
        public List<(string, string)> properties;
        public List<(string, string)> methods;

        public ANTLRService(CSharpVisitor visitor)
        {
            _cSharpVisitor = visitor;
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

        public void RunVisitor()
        {
            var csharpContext = csharpParser.classDeclaration();
            _cSharpVisitor.Visit(csharpContext);
        }

        public void RunVisitorWithSpecificStartingRule(string startingRule)
        {
            var parserType = typeof(CSharpGrammarParser);
            MethodInfo startingRuleMethod = parserType.GetMethod(startingRule);
            if (startingRule != null)
            {
                var csharpContext = (IParseTree) startingRuleMethod.Invoke(csharpParser, null);
                _cSharpVisitor.Visit(csharpContext);
            }
            else
            {
                throw new ArgumentException("This rule does not exist");
            }
        }
    }
}
