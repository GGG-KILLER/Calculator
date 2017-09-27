# Calculator
My first attempt at making a recursive-descent parser.

It is modular, meaning that you can modify all operators (apart from unary `+` and unary `-` as those are hard-coded in the lexer), modify all constants and functions are a WIP.

The calculator acts upon the following EBNF grammar:
```ebnf
expr	::= literal, binaryop, expr | literal;

literal	::= [unaryop], ident | [unaryop], number | [unaryop], '(', expr, ')';

number	::= { digit }, [ ".",  { digit } ] | ".", { digit };

unaryop  ::= "+" | "-" ;
binaryop ::= "+" | "-" | "*" | "/" | "^" | "%" ;
ident	 ::= letter, { letter | digit } ;
digit	 ::= "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" ;
letter   ::= "a" | "b" | "c" | "d" | "e" | "f" | "g" | "h" | "i" | "j"
           | "k" | "l" | "m" | "n" | "o" | "p" | "q" | "r" | "s" | "t"
           | "u" | "v" | "w" | "x" | "y" | "z" | "A" | "B" | "C" | "D"
           | "E" | "F" | "G" | "H" | "I" | "J" | "K" | "L" | "M" | "N"
           | "O" | "P" | "Q" | "R" | "S" | "T" | "U" | "V" | "W" | "X"
           | "Y" | "Z" ;
```

# Dependencies
- [GUtils.Text](https://github.com/GGG-KILLER/GUtils.NET/tree/master/GUtils.Text)
