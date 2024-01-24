using Infrastructure.ANTLR.CSharp;

namespace Infrastructure.Antlr
{
    public class ANTLRService : CSharpGrammarBaseVisitor<object>
    {
        var fileContents = File.ReadAllText("C:\\Users\\Usuario\\source\\repos\\FlowUML\\Infrastructure\\ANTLR\\CSharp\\testFile.txt");
    }
}
