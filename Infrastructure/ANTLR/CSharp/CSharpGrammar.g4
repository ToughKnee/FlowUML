// TODO: Cover the Constructors methods AND the 'new' keyword when called inside a methodBody
grammar CSharpGrammar;
WS  :   [ \t\n\r]+ -> skip ;

//===========================//===========================  Lexer
//===========================  Generic grammar
SEMICOLON: ';';

NAMESPACE
    : 'namespace'
    ;

namespace
    : NAMESPACE
    ;

USING
    : 'using'
    ;

using
    : USING
    ;

AWAIT: 'await';

NEW: 'new';

RETURN: 'return';

// Covers optional MODIFIERS for some properties, variables, classes and such that they could have like readonly or the attributes, like [Theory] for xUnit
MODIFIERS
    : ('static' | 'virtual' | 'override' | 'abstract' | 'sealed' | 'readonly' | 'async')+
    ;

modifiers: MODIFIERS;

ACCESS_MODIFIER
    : 'public'
    | 'protected'
    | 'private'
    | 'internal'
    ;

accessModifier: ACCESS_MODIFIER;

// Covers types which may or may not have generic types with them
// TODO: Add the primitive types in here
PRIMITIVE_TYPE_NAME
    : 'string'
    | 'char'
    | 'bool'
    | 'int'
    | 'uint'
    | 'float'
    | 'long'
    | 'ulong'
    | 'short'
    | 'ushort'
    | 'byte'
    | 'decimal'
    | 'var'
    | 'object'
    | 'dynamic'
    ;

primitiveTypeName: PRIMITIVE_TYPE_NAME;

advancedTypeName
    : identifier ('<' genericType (',' genericType)* '>')?
    ;

genericType
    : advancedTypeName
    | genericType '<' genericType (',' genericType)* '>'
    ;
    
// Words that may be anything like a property declaration, varaible declaration, etc, which ends with ' ; '
// TODO: Check if removing the '('?'*)' would still take what we need from a thing with a type like 'List<Team?>', and check if it gets 'List<Team>' wothout the '?' sign
IDENTIFIER
    : [a-zA-Z_] [a-zA-Z0-9_]*('?'?)
    ;

identifier
    : IDENTIFIER
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
    : identifier ( '.' identifier )*
    ;

usingDirective
    : using namespaceIdentifier ';'
    ;
    
fileNamespaces
    : fileNamespace+
    ;

fileNamespace
    : namespace namespaceIdentifier 
    '{'
        classDeclarations?
    '}'
    | namespace namespaceIdentifier ';'
    classDeclarations?
    ;

//===========================  Class grammar
classDeclarations
    : classDeclaration+
    ;

classDeclaration
    : attributes? accessModifier? 'class' identifier classHeritage?
        classBodyContent?
    ;

// This will get the properties and methods regardless of their positions inside the body of the class
classBodyContent
    :
    '{'
        classContent*
    '}'
    ;

classHeritage
    : ':' type (',' type)*
    ;

//===========================  Class properties grammar
classContent
    // Syntax for property declaration
    : constructorDeclaration
    | property
    | method
    ;

    constructorDeclaration
        : 
        attributes? accessModifier? modifiers* identifier 
        '(' parameterList? ')'
        methodBodyContent
        ;

    property
        : attributes? accessModifier? modifiers? type identifier SEMICOLON
        ;

    method
        :    
        attributes? accessModifier? modifiers* type identifier 
        '(' parameterList? ')'
        methodBodyContent
        ;

attributeIdentifier
    : '[' identifier ('(' .*? ')')? ']'
    ;

attributes
    : attributeIdentifier+
    ;


//===========================  Method grammar

methodBodyContent
    :
    '{'
    methodContent*
    '}'
    ;

parameterList
    : attributes? parameter (',' attributes? parameter)*
    ;

parameter
    : type identifier
    ;

//===========================  Local variables grammar
methodContent
    : constructorAssignment
    | functionCall ';'
    | localVariableDeclaration
    | returnExpression
    ;

constructorAssignment
    : type identifier '=' NEW expression ';'
    ;

localVariableDeclaration
    : type identifier '=' expression ';'
    ;
functionCall: expression;

returnExpression
    : RETURN NEW? (identifier | expression) ';' 
    ;

// Something that returns something
expression
    : AWAIT? methodCall
    | /* other expressions */
    ;

methodCall
    : methodIdentifier '(' argumentList ')'
    ;

methodIdentifier
    : identifier ( '.' methodCall | '.' identifier)*
    ;

argumentList
    : (expression | identifier) ( ',' (expression | identifier) )*
    ;
