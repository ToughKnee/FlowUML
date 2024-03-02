// TODO: Cover the Constructors methods AND the 'new' keyword when called inside a methodBody
parser grammar CSharpGrammarParser;
options {tokenVocab=CSharpGrammarLexer;}

//===========================//===========================  Lexer
//===========================  Generic grammar
namespace
    : NAMESPACE
    ;

using
    : USING
    ;
new: NEW;
accessModifier: ACCESS_MODIFIER;
primitiveTypeName: PRIMITIVE_TYPE_NAME;

identifier
    : IDENTIFIER
    ;

modifiers: MODIFIERS;

advancedTypeName
    : identifier (LDIAMOND genericType (COMMA genericType)* RDIAMOND)?
    ;

genericType
    : advancedTypeName
    | primitiveTypeName
    | genericType LDIAMOND genericType (COMMA genericType)* RDIAMOND
    ;
    
advancedIdentifier
    : identifier (DOT identifier)*
    ;

type
    : (primitiveTypeName | advancedTypeName)
    ;

//===========================  File grammar
cSharpFile
    : usingDirectives?
    fileNamespaces?
    classDeclarations?
    ;

usingDirectives
    : usingDirective+
    ;

namespaceIdentifier
    : identifier ( DOT identifier )*
    ;

usingDirective
    : using namespaceIdentifier SEMICOLON
    ;
    
fileNamespaces
    : fileNamespace+
    ;

fileNamespace
    : namespace namespaceIdentifier 
    LCURLY
        classDeclarations?
    RCURLY
    | namespace namespaceIdentifier SEMICOLON
    classDeclarations?
    ;

//===========================  Class grammar
classDeclarations
    : classDeclaration+;

classDeclaration
    : attributes? accessModifier? CLASS identifier classHeritage?
        classBodyContent?
    ;

// This will get the properties and methods regardless of their positions inside the body of the class
classBodyContent
    :
    LCURLY
        classContent*
    RCURLY
    ;

classHeritage
    : COLON type (COMMA type)*
    ;

//===========================  Class properties grammar
classContent
    // Syntax for property declaration
    : property
    | method
    ;

    property
        : attributes? accessModifier? modifiers? type identifier SEMICOLON
        ;

    method
        :    
        // Normal method syntax
        attributes? accessModifier? modifiers* type identifier 
        LPAREN parameterList? RPAREN
        methodBodyContent
        |
        // Constructor syntax
        attributes? accessModifier? modifiers* identifier 
        LPAREN parameterList? RPAREN
        methodBodyContent

        ;

attributeIdentifier
    : LBRACKET identifier (LPAREN .*? RPAREN)? RBRACKET
    ;

attributes
    : attributeIdentifier+
    ;


//===========================  Method grammar

methodBodyContent
    :
    LCURLY
    methodContent*
    RCURLY
    ;

parameterList
    : attributes? parameter (COMMA attributes? parameter)*
    ;

parameter
    : type identifier
    ;

methodContent
    : valueAssignment SEMICOLON
    | functionCall SEMICOLON
    | localVariableDeclaration SEMICOLON
    | returnExpression SEMICOLON
    // | ifStatements -- Which are going to have what comes next
    // | functionCallLexem -- This is the same as the function call, BUT they are recognized instantly, at the start of the lexer, and that way we are going to be able to know that inside the method's body, there are FunctionCallLexems, AND all the other stuff inside method bodies(garbage useless for us like arithmetic operators, parenthesis, etc), and thus we can introduce at the end of this rule the following rule
    // | (.*?) -- This rule, which is going to represent all the other garbage symbols useless to us
    ;

//===========================  Method contents Grammar
localVariableDeclaration
    : type identifier EQUALS expression
    ;

valueAssignment
    : advancedIdentifier EQUALS expression
    ;

functionCall: expression;

returnExpression
    : RETURN (identifier | expression)
    ;

// Something that returns something
expression
    : ExpressionMethodCall
    ;
