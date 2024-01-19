# FlowUML

# How to use antlr extension from vscode to debug grammars
Follow this [tutorial to debug grammars with vscode](https://github.com/mike-lischke/vscode-antlr4/blob/master/doc/grammar-debugging.md)  
The `launch.json` would look like this
```
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Debug ANTLR4 grammar",
            "type": "antlr-debug",
            "request": "launch",
            "input": "C:\\Users\\Usuario\\source\\repos\\FlowUML\\Infrastructure\\ANTLR\\CSharp\\testFile.txt",
            "grammar": "C:\\Users\\Usuario\\source\\repos\\FlowUML\\CSharpGrammar.g4",
            "startRule": "classDeclaration",
            "printParseTree": true,
            "visualParseTree": true
        }
    ]
}
```