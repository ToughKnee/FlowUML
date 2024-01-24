// TODO: Cover the Constructors methods AND the 'new' keyword when called inside a methodBody
grammar CSharpGrammar;
WS  :   [ \t\n\r]+ -> skip ;

//===========================//===========================  Lexer
//===========================  Generic grammar
SEMICOLON: ';';

AWAIT: 'await';

// Covers optional MODIFIERS for some properties, variables, classes and such that they could have like readonly or the attributes, like [Theory] for xUnit
MODIFIERS
    : ('static' | 'virtual' | 'override' | 'abstract' | 'sealed' | 'readonly' | 'async')+
    ;

// TODO: Implement a way to catch the parameters, TODO2: Also implement a way to catch a call inside this method call(maybe using the priority of the tokenization where we find all the Method Calls first and later we can check for them normally, AND also adding this as a new Lexer Rule: 'METHOD_CALL')

ACCESS_MODIFIER
    : 'public'
    | 'protected'
    | 'private'
    | 'internal'
    ;

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

advancedTypeName
    : IDENTIFIER ('<' genericType (',' genericType)* '>')?
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

//===========================  Class grammar
classDeclaration
    : attributes? ACCESS_MODIFIER? 'class' IDENTIFIER classHeritage?
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
    : ':' (PRIMITIVE_TYPE_NAME | advancedTypeName) (',' (PRIMITIVE_TYPE_NAME | advancedTypeName))*
    ;

//===========================  Class properties grammar
classContent
    // Syntax for property declaration
    : property
    | method
    ;

    property
        : attributes? ACCESS_MODIFIER? MODIFIERS? (PRIMITIVE_TYPE_NAME | advancedTypeName) IDENTIFIER SEMICOLON
        ;

    method
        :    
        // Syntax for method declaration
        attributes? ACCESS_MODIFIER? MODIFIERS* (PRIMITIVE_TYPE_NAME | advancedTypeName) IDENTIFIER 
        '(' parameterList? ')'
        methodBodyContent
        ;

attributeIdentifier
    : '[' IDENTIFIER ('(' .*? ')')? ']'
    ;

attributes
    : attributeIdentifier (',' attributeIdentifier)*
    ;

//===========================  Method grammar

methodBodyContent
    :
    '{'
    methodContent*
    '}'
    ;

parameterList
    : parameter (',' parameter)*
    ;

parameter
    : (PRIMITIVE_TYPE_NAME | advancedTypeName) IDENTIFIER // IDENTIFIER is unnecesary since we only need to know the (PRIMITIVE_TYPE_NAME | advancedTypeName)s
    ;

//===========================  Local variables grammar
methodContent
    : localVariableDeclaration
    | functionCall ';'
    ;

localVariableDeclaration
    : (PRIMITIVE_TYPE_NAME | advancedTypeName) IDENTIFIER '=' expression ';'
    ;
functionCall: expression;

// Something that returns something
expression
    : AWAIT? methodCall
    | /* other expressions */
    ;

methodCall
    : methodIdentifier '(' argumentList ')'
    ;

methodIdentifier
    : IDENTIFIER ( '.' methodCall | '.' IDENTIFIER)*
    ;

argumentList
    : (expression | IDENTIFIER) ( ',' (expression | IDENTIFIER) )*
    ;
