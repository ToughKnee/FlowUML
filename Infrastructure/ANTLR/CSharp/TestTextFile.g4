grammar TestTextFile;

program: line* EOF;

line: statement;

statement: (assignment | functionCall) ';';

assignment: IDENTIFIER '=' expression;

IDENTIFIER: [a-zA-Z][a-zA-Z0-9_]*;3x

functionCall: IDENTIFIER '(' (expression (',' expression)*)? ')';

expression
  : constant
  | IDENTIFIER
  | functionCall
  | '(' expression ')'
  | '!' expression
  | expression multOp expression 
  | expression addOp expression
  | expression compareOp expression 
  | expression boolOp expression
  ;

multOp: '*' | '/' | '%';
addOp: '+' | '-';
compareOp: '==' | '!=' | '>' | '<' | '>=' | '<=';
boolOp: BOOL_OPERATOR;
BOOL_OPERATOR: '&&' | '||'; 

block: '{' line* '}';
WS: [ \t\r\n]+ -> skip;

constant: INTEGER | FLOAT | STRING | BOOL | NULL;
INTEGER: [0-9]+;
FLOAT: [0-9]+ '.' [0-9]+;
STRING: ('"' ~'"'* '"') | ('\'' ~'\''* '\'');
BOOL: 'true' | 'false';
NULL: 'null';








