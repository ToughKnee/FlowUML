grammar CSharpGrammar;
WS  :   [ \t\n\r]+ -> skip ;

//===========================//===========================  Lexer
//===========================  Generic grammar
// For any kind of thing like Classes, Interfaces, Structs, Enums, Methods, Properties, Fields, Parameters, Variables
// TODO: Check if removing the '('?'*)' would still take what we need from a thing with a type like 'List<Team?>', and check if it gets 'List<Team>' wothout the '?' sign


// Covers optional MODIFIERS for some properties, variables, classes and such that they could have like readonly or the attributes, like [Theory] for xUnit
MODIFIERS
    : ('static' | 'virtual' | 'override' | 'abstract' | 'sealed' | 'readonly' | 'async')+
    ;
    
    
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

userTypeName
    : IDENTIFIER ('<' IDENTIFIER (',' IDENTIFIER)* '>')?
    ;

// Words that may be anything like a property declaration, varaible declaration, etc, which ends with ' ; '
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
        propertyDeclaration*
        | methodDeclaration*
    '}'
    ;

classHeritage
    : ':' (PRIMITIVE_TYPE_NAME | userTypeName) (',' (PRIMITIVE_TYPE_NAME | userTypeName))*
    ;

//===========================  Class properties grammar
propertyDeclaration
    : attributes? ACCESS_MODIFIER? MODIFIERS? (PRIMITIVE_TYPE_NAME | userTypeName) IDENTIFIER ';'
    ;
    //propertyDeclaration : (PRIMITIVE_TYPE_NAME | userTypeName) IDENTIFIER '(' ')' ';';


attributeIdentifier
    : '[' IDENTIFIER ('(' .*? ')')? ']'
    ;

attributes
    : attributeIdentifier (',' attributeIdentifier)*
    ;

//===========================  Method grammar
methodDeclaration
    : ACCESS_MODIFIER? MODIFIERS* (PRIMITIVE_TYPE_NAME | userTypeName) IDENTIFIER '(' parameterList? ')'
    ;

methodBodyContent
    :
    '{'
    '}'
    ;

parameterList
    : parameter (',' parameter)*
    ;

parameter
    : (PRIMITIVE_TYPE_NAME | userTypeName) IDENTIFIER // IDENTIFIER is unnecesary since we only need to know the (PRIMITIVE_TYPE_NAME | userTypeName)s
    ;

//===========================  Local variables grammar
// localVariable
//     : (PRIMITIVE_TYPE_NAME | userTypeName) IDENTIFIER '=' expression ';'
//     ;

// expression
//     : methodCall
//     | /* other expressions */
//     ;

// methodCall
//     : methodIdentifier '(' argumentList? ')'
//     ;

// methodIdentifier
//     : IDENTIFIER ( '.' IDENTIFIER | '.' methodCall )*
//     ;

// argumentList
//     : expression ( ',' expression )*
//     ;
