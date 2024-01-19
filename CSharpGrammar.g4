grammar CSharpGrammar;
WS  :   [ \t\n\r]+ -> skip ;

//===========================  Generic grammar
// For any kind of thing like Classes, Interfaces, Structs, Enums, Methods, Properties, Fields, Parameters, Variables
// TODO: Check if removing the '('?'*)' would still take what we need from a thing with a type like 'List<Team?>', and check if it gets 'List<Team>' wothout the '?' sign
IDENTIFIER 
    : [a-zA-Z_] [a-zA-Z0-9_]*('?'?)
    ;

// Words that may be anything like a property declaration, varaible declaration, etc, which ends with ' ; '

// Covers types which may or may not have generic types with them
// TODO: Add the primitive types in here
typeName
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
    | IDENTIFIER ('<' IDENTIFIER (',' IDENTIFIER)* '>')?
    ;

accessModifier
    : 'public'
    | 'protected'
    | 'private'
    | 'internal'
    ;

//===========================  Class grammar
classDeclaration
    : attributes? accessModifier? 'class' IDENTIFIER classHeritage?
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
    : ':' typeName (',' typeName)*
    ;

//===========================  Class properties grammar
propertyDeclaration
    : attributes? accessModifier? modifiers? typeName IDENTIFIER '('
    ;
    //propertyDeclaration : typeName IDENTIFIER '(' ')' ';';
// Covers optional modifiers for some properties, variables, classes and such that they could have like readonly or the attributes, like [Theory] for xUnit
modifiers
    : ('static' | 'virtual' | 'override' | 'abstract' | 'sealed' | 'readonly' | 'async')+
    ;

attributeIdentifier
    : '[' IDENTIFIER ('(' .*? ')')? ']'
    ;

attributes
    : attributeIdentifier (',' attributeIdentifier)*
    ;

//===========================  Method grammar
methodDeclaration
    : accessModifier? modifiers* typeName IDENTIFIER '(' parameterList? ')'
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
    : typeName IDENTIFIER // IDENTIFIER is unnecesary since we only need to know the typenames
    ;

//===========================  Local variables grammar
// localVariable
//     : typeName IDENTIFIER '=' expression ';'
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
