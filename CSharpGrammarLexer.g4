// TODO: Cover the Constructors methods AND the 'new' keyword when called inside a methodBody
lexer grammar CSharpGrammarLexer;
WSa  :   [ \t\n\r]+ -> skip ;

//===========================//===========================  Lexer
//===========================  Generic grammar
SEMICOLON: ';';
COLON: ':';

NAMESPACE
    : 'namespace'
    ;

USING
    : 'using'
    ;

AWAIT: 'await';

NEW: 'new';

RETURN: 'return';

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

CLASS: 'class';

LDIAMOND: '<';
RDIAMOND: '>';
LPAREN: '(';
RPAREN: ')';
LBRACKET: '[';
RBRACKET: ']';
LCURLY: '{';
RCURLY: '}';
DOT: '.';
COMMA: ',';
EQUALS: '=';

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


// Words that may be anything like a property declaration, varaible declaration, etc, which ends with ' ; '
// TODO: Check if removing the '('?'*)' would still take what we need from a thing with a type like 'List<Team?>', and check if it gets 'List<Team>' wothout the '?' sign
IDENTIFIER
    : [a-zA-Z_] [a-zA-Z0-9_]*('?'?)
    ;

mode InsideClassMode;

MethodBody
    : LCURLY -> pushMode(InsideMethodBodyMode)
    ;

mode InsideMethodBodyMode;

ExpressionMethodCall
    : NEW? (MethodCall | [a-zA-Z_] [a-zA-Z0-9_]*('?'?)) ('.' MethodCall | '.' [a-zA-Z_] [a-zA-Z0-9_]*('?'?))*
    ;

MethodCall
    : [a-zA-Z_] [a-zA-Z0-9_]*('?'?) ('.' [a-zA-Z_] [a-zA-Z0-9_]*('?'?))* '(' ArgumentList? ')'
    ;

ArgumentList
    : (ExpressionMethodCall | [a-zA-Z_] [a-zA-Z0-9_]*('?'?)) ( ',' (ExpressionMethodCall | [a-zA-Z_] [a-zA-Z0-9_]*('?'?)) )*
    ;

InsideClass
    : CLASS -> pushMode(InsideClassMode)
    ;
