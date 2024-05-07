// TODO: Cover the Constructors methods AND the 'new' keyword when called inside a methodBody
grammar CSharpGrammar;
WS  :   [ \t\n\r]+ -> skip ;
COMMENT: '/*' .*? '*/' -> skip;
LINE_COMMENT: '//' ~[\r\n]* -> skip;

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

new: 'new';

RETURN: 'return';

operators
    : '=='
    | '||'
    | '!='
    | '&&'
    | '<'
    | '>'
    | '<='
    | '>='
    | 'is not'
    | 'is'
    ;

STRING 
    : '"' .*? '"' 
    | '\'' .*? '\'' 
    ;
string: STRING ;

NUMBER 
    : '-'? [0-9]+ ('.' [0-9]+)? 'f'?
    ;
number: NUMBER ;

// Covers optional MODIFIERS for some properties, variables, classes and such that they could have like readonly or the attributes, like [Theory] for xUnit
MODIFIERS
    : ('static' | 'virtual' | 'override' | 'abstract' | 'sealed' | 'readonly' | 'async')+
    ;

modifiers: new | MODIFIERS;

ACCESS_MODIFIER
    : 'public'
    | 'protected'
    | 'private'
    | 'internal'
    ;

accessModifier: ACCESS_MODIFIER;

advancedTypeName
    : (identifier | advancedIdentifier) ('<' genericType (',' genericType)* '>')?
    ;

genericType
    : advancedTypeName
    | genericType '<' genericType (',' genericType)* '>'
    ;
    
// Words that may be anything like a property declaration, variable declaration, etc
IDENTIFIER
// The unicode values correspond to '[' and ']'
    : [a-zA-Z_] [a-zA-Z0-9_]*('\u005B' '\u005D')? ('?')?
    ;

identifier
    : IDENTIFIER
    ;
    
advancedIdentifier
    : '!'? identifier ('.' identifier)*
    ;

type
    : advancedTypeName
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
    : attributes? accessModifier? typeOfClassDeclaration identifier templateTypeName? classHeritage?
        classBodyContent
    | attributes? accessModifier? 'enum' identifier
        '{' (advancedIdentifier ','?)* '}'
    ;

typeOfClassDeclaration
    : 'asbtract'? 'class'
    | 'interface'
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

templateTypeName
    : '<' identifier (',' identifier)* '>'
    ;

//===========================  Class properties grammar
classContent
: property
| method
;

property
    : (attributes | accessModifier | modifiers )* type identifier (methodBodyContent)? ('=' '>'? expression)? SEMICOLON?
    ;

method
    :    
    (attributes | accessModifier | modifiers)* type? identifier templateTypeName?
    '(' parameterList? ')' baseConstructor?
    (methodBodyContent | ';')
    ;

baseConstructor
    : ':' methodCall
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
    : attributes? parameter parameterInitializer? (',' attributes? parameter parameterInitializer?)*
    ;

parameterInitializer
    : '=' (advancedIdentifier | string | number)
    ;

parameter
    : type identifier
    ;

statement
    : whileLoopStatement 
    | ifStatement
    | foreachStatement
    | forStatement
    | anyStatement
    ;  // otherStatement represents all other kinds of statements in your language
whileLoopStatement
    : 'while' '(' (expression) ')'
    (methodBodyContent | expression ';')
    ;
ifStatement
    : 'else'? 'if' '(' (expression) ')'
    (methodBodyContent | expression ';')
    | 'else'
    (methodBodyContent | expression ';')
    ;
foreachStatement
    : 'foreach' '(' (type advancedIdentifier 'in' advancedIdentifier) ')'
    (methodBodyContent | expression ';')
    ;
forStatement
    : 'for' '(' localVariableDefinition ';' expression ';' expression ')'
    (methodBodyContent | expression ';')
    ;
anyStatement
    : advancedIdentifier gibberish*
    methodBodyContent
    ;

//===========================  Method content grammar
methodContent
    : expression ';'
    | statement
    | localVariableDefinition ';'
    | returnExpression ';'
    ;

localVariableDefinition
    : type? (identifier | advancedIdentifier) assigner expression
    ('{' gibberish* '}')? // This parentheses captures the info we don't need like data initializers of collections like "new List() {1,2,1}"
    (',' localVariableDefinition)?
    | type identifier
    ;

assigner
    : '='
    | '+='
    | '-='
    | '*='
    | '/='
    ;

returnExpression
    : RETURN (identifier | expression)
    ;

typeCaster
    : '(' advancedTypeName ')'
    ;
// Something that returns something
expression
    :
    typeCaster?
    '('?'('?'('?
        typeCaster?
        (
        ternaryOperatorExpression indexRetrieval?
        | localVariableDefinition
        | comparisonExpression
        | methodCall
        | advancedIdentifier ('--')? ('++')?
        | advancedIdentifier indexRetrieval?
        | string
        | number
        | returnExpression)
        (arithmeticOperations)*
    ')'?')'?')'?
    // TODO: Do the following features AND make sure to put the optional parentheses around them
    // Do the rule that will capture comparisons which return booleans like "vector == Vector3.zero"
    ;

indexRetrieval
    : ('[' (string | number | advancedIdentifier) ']')+ expressionChain?
    ;

expressionMethodCall
    : AWAIT? methodCall
    ;

methodCall
    : 'throw'? new? '!'? (advancedIdentifier | type | new) templateTypeName? ('(' argumentList? ')') indexRetrieval? expressionChain?
    | 'throw'? new type templateTypeName? ('[' argumentList? ']')? indexRetrieval? expressionChain?
    ;

// This rule denotes the properties or methods from other complex expressions like methods or a value from a collection retrieved with indexers, simple properties chains like "myClass.myProp1.myProp2" are covered by the "advancedIdentifier" rule 
expressionChain
    : ('.' methodCall | '.' advancedIdentifier)+ indexRetrieval?
    ;

argumentList
    : (outParameter | expression) ( ',' (outParameter | expression) )*
    | gibberish
    ;

outParameter
    : 'out' localVariableDefinition /*type identifier*/
    ;
    
// Gibberish here refers to things that we are not interested in like expressions enclosed in braces
// The logic is to basically state that 'if the thing we are currently looking at(while parsing text) is not something important(like an expression), then it is rubbish and we don't care'
gibberish: ('<' 
    | '[' 
    | '!' 
    | '#' 
    | '$' 
    | '%' 
    | '&' 
    | '\'' 
    | '*' 
    | '+' 
    | '-' 
    | '/' 
    | '>' 
    | '?' 
    | '@' 
    | '^' 
    | '_' 
    | '`' 
    | '~'
    | NUMBER
    | STRING
    | operators
    )+;

arithmeticOperations
    : (gibberish (gibberish | expression))
    ;

// In order to implement a way to recognize nested rules, like the ternary operator, we must create 3 rules, the normal rule which captures the cases where the is NO  nesting(which usually does not have special characters like '()', which must be used when we want to nest values) -- After that we need a component rule which basically has everythig that will be in the rule normally, BUT it must not include the previous rule, which would be itself, but this alone would leave us without nesting, so we need the last rule -- Which represents the case where there IS nesting, it would look the same as the first rule structurally, but it will differ at being able to be composed of the second rule AND the first rule also, ONLY IF we start the rule with another rule that MUST be present, like 'nestedTernaryOperator' having a '(' without the question mark, while the 'ternaryOperatorExpression' starts with '('?, including the question mark, and if that quesiton mark wasn't there in the third rule then ANTLR marks it as left recursion which can't be handled
//===========================  Ternary operataor nesting rules
ternaryOperatorExpression
    : '('? ternaryOperatorComponent* ')'?
    '?' '('? ternaryOperatorComponent* ')'?
    ':' '('? ternaryOperatorComponent* ')'?
    ;

ternaryOperatorComponent
    : (nestedTernaryOperator
    | comparisonExpression
    | expressionMethodCall
    | advancedIdentifier
    | number
    | string) arithmeticOperations*
    // | gibberish
    ;

nestedTernaryOperator
    : '(' (expression | gibberish)* ')'
    '?' '('? (expression | gibberish)* ')'?
    ':' '('? (expression | gibberish)* ')'?
    ;
//======

//===========================  Comparison nesting rules
comparisonExpression
    : '('? comparisonExpressionComponent (operators comparisonExpressionComponent)+ ')'? 
    ;

comparisonExpressionComponent
    : (nestedTernaryOperator
    | nestedComparisonExpression
    | expressionMethodCall
    | advancedIdentifier
    | number
    | string) arithmeticOperations*
    ;

nestedComparisonExpression
    : '(' (expression | gibberish) (operators (expression | gibberish))* ')'
    ;

//======
