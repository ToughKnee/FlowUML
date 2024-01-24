using Antlr4.Runtime;
using Infrastructure.ANTLR.CSharp;

namespace Infrastructure.Antlr
{
    public class CSharpVisitor : CSharpGrammarBaseVisitor<object>
    {
        public void start()
        {
            var fileContents = File.ReadAllText("C:\\Users\\Usuario\\source\\repos\\FlowUML\\Infrastructure\\ANTLR\\CSharp\\testFile.txt");
            var inputStream = new AntlrInputStream(fileContents);
            var csharpLexer = new CSharpGrammarLexer(inputStream);
            var commonTkenStream = new CommonTokenStream(csharpLexer);
            var csharpParser = new CSharpGrammarParser(commonTkenStream);
            var csharpContext = csharpParser.classDeclaration();
            var visitor = new ;
        }

    }
}
