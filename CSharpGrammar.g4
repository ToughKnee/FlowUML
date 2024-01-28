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

//===========================  File grammar
cSharpFile
    : usingDirectives?
    classNamespace*
    ;

usingDirectives
    : usingDirective+
    ;

usingDirective
    : using identifier ( '.' identifier )* ';'
    ;

classNamespace
    : namespace identifier ( '.' identifier )* 
    '{'
        classDeclarations?
    '}'
    | namespace identifier ( '.' identifier )* ';'
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
    : ':' (primitiveTypeName | advancedTypeName) (',' (primitiveTypeName | advancedTypeName))*
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
        : attributes? accessModifier? modifiers? (primitiveTypeName | advancedTypeName) identifier SEMICOLON
        ;

    method
        :    
        // Syntax for method declaration
        attributes? accessModifier? modifiers* (primitiveTypeName | advancedTypeName) identifier 
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
    returnType?
    '}'
    ;

parameterList
    : attributes? parameter (',' attributes? parameter)*
    ;

parameter
    : (primitiveTypeName | advancedTypeName) identifier // identifier is unnecesary since we only need to know the (primitiveTypeName | advancedTypeName)s
    ;

//===========================  Local variables grammar
methodContent
    : constructorAssignment
    | functionCall ';'
    | localVariableDeclaration
    ;

constructorAssignment
    : (primitiveTypeName | advancedTypeName) identifier '=' NEW expression ';'
    ;

localVariableDeclaration
    : (primitiveTypeName | advancedTypeName) identifier '=' expression ';'
    ;
functionCall: expression;

returnType
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
