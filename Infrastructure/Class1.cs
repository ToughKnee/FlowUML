using Infrastructure.Antlr;

var i = 0;
var antlrService = new ANTLRService("C:\\Users\\Usuario\\source\\repos\\FlowUML\\Infrastructure\\ANTLR\\CSharp\\testFile.txt", true);
antlrService.RunVisitor();
i = 1;