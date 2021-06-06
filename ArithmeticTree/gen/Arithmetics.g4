grammar Arithmetics;

expr  : left=expr op=('*'|'/') right=expr #opExpr
      | left=expr op=('+'|'-') right=expr #opExpr
      | '(' expr ')'                      #parenExpr
      | atom=DIGIT                        #atomExpr
      ;
      
DIGIT: ('0' .. '9');


WS: [ \r\n\t] + -> channel (HIDDEN);