grammar CSharpGrammar;
WS  :   [ \t\n\r]+ -> skip ;

//===========================//===========================  Lexer
//===========================  Generic grammar
SEMICOLON: ';';

// Covers optional MODIFIERS for some properties, variables, classes and such that they could have like readonly or the attributes, like [Theory] for xUnit
MODIFIERS
    : ('static' | 'virtual' | 'override' | 'abstract' | 'sealed' | 'readonly' | 'async')+
    ;

// TODO: Implement a way to catch the parameters, TODO2: Also implement a way to catch a call inside this method call(maybe using the priority of the tokenization where we find all the Method Calls first and later we can check for them normally, AND also adding this as a new Lexer Rule: 'METHOD_CALL')
METHOD_PARENTHESIS
    : '(' .*? ')' 
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
        classDeclarations*
    '}'
    ;

classHeritage
    : ':' (PRIMITIVE_TYPE_NAME | userTypeName) (',' (PRIMITIVE_TYPE_NAME | userTypeName))*
    ;

//===========================  Class properties grammar
classDeclarations
    // Syntax for property declaration
    : attributes? ACCESS_MODIFIER? MODIFIERS? (PRIMITIVE_TYPE_NAME | userTypeName) IDENTIFIER SEMICOLON
    // Syntax for method declaration
    | attributes? ACCESS_MODIFIER? MODIFIERS* (PRIMITIVE_TYPE_NAME | userTypeName) IDENTIFIER METHOD_PARENTHESIS
    methodBodyContent
    ;

attributeIdentifier
    : '[' IDENTIFIER ('(' .*? ')')? ']'
    ;

attributes
    : attributeIdentifier (',' attributeIdentifier)*
    ;

//===========================  Method grammar
// TODO: REMOVE
methodDeclaration
    : ACCESS_MODIFIER? MODIFIERS* (PRIMITIVE_TYPE_NAME | userTypeName) IDENTIFIER METHOD_PARENTHESIS
    ;

methodBodyContent
    :
    '{'
    // TODO: Implement recognition for "Local Variables"
    .*?
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
