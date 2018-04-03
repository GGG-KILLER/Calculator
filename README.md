# Calculator
My second attempt at making a recursive-descent parser.

It is customizable, meaning that you can modify all operators, constants, unary operators and binary operators.
It also supports postfix operators.

# Features
- Constants
- Prefix operators
- Postfix operators
- Binary operators
- Functions
- Binary numbers (with 0b prefix, e.g.: 0b1010)
- Octal numbers (with 0o prefix, e.g.: 0o776)
- Hexadecimal numbers (with 0x prefix, e.g.: 0xFF)
- All constants are customizable (by default the calculator starts without any to be as unopinionated as possible)
- All prefix, postfix and binary operators are customizable (by default the calculator starts without any to be as unopinionated as possible)
- All functions are customizable (by default the calculator starts without any to be as unopinionated as possible)

## How to use it:
### Lexing:
```csharp
using GParse.Lexing;
using GParse.Lexing.Errors;
using Calculator.Parsing;

public class Program
{
	public static void Main ( )
	{
		try
		{
			var lang = CreateLang ( );
			var expr = Console.ReadLine ( );// obtain math expression string somehow
			// The expression must be passed to the constructor
			// (meaning you can't reutilize the lexer)
			var lexer = new CalculatorLexer ( lang, expr );
			foreach ( Token tok in lexer.Lex ( ) )
			{
				// Process each token here
			}
		}
		catch ( LexException ex )
		{
			// Deal with the lexing exception here
		}
	}
	
	// Simple language example, for a more in-depth example
	// see `Program.cs` in Calculator.CLI
	//
	// A full calculator language (with the runtime definitions)
	// is required during lexing because it's what will be passed
	// to all classes of the calculator execution process (Lexer,
	// Parser, and finally, the VM).
	private static CalculatorLang CreateLang ( )
	{
		var lang = new CalculatorLang ( "Simple Math", new Version ( 1, 0, 0 ) );
		
		// A constant
		lang.AddConstant ( "one", 1D );
		
		// Unary operators
		lang.AddOperator ( UnaryOperatorFix.Prefix, "-", 1, n => -n );
		lang.AddOperator ( UnaryOperatorFix.Postfix, "?", 1, n => ~n );
		
		// Binary operators
		lang.AddOperator ( OperatorAssociativity.Left, "+", 1, ( lhs, rhs ) => lhs + rhs );
		lang.AddOperator ( OperatorAssociativity.Right, "^", 2, ( lhs, rhs ) => Math.Pow ( lhs, rhs ) );
		
		// Functions
		lang.AddFunction ( "sin", Math.Sin );
		
		// Return the calculator language
		return lang;
	}
}
```
### Parsing:
```csharp
using GParse.Lexing;
using GParse.Lexing.Errors;
using GParse.Parsing.Errors;
using Calculator.Parsing;

public class Program
{
	public static void Main ( )
	{
		try
		{
			var lang = CreateLang ( );
			var expr = Console.ReadLine ( );// obtain math expression string somehow
			// The expression must be passed to the constructor
			// (meaning you can't reutilize the lexer)
			var lexer = new CalculatorLexer ( lang, expr );
			var parser = new CalculatorParser ( lang, lexer );
			var AST = parser.Parse ( );
			// Do something with the AST
		}
		catch ( LexException ex )
		{
			// Deal with the lexing exception here
		}
		catch ( ParseException ex )
		{
			// Deal with the parsing exception here
		}
	}
	
	// Simple language example, for a more in-depth example
	// see `Program.cs` in Calculator.CLI
	//
	// A full calculator language (with the runtime definitions)
	// is required during parsing because it's what will be passed
	// to all classes of the calculator execution process (Lexer,
	// Parser, and finally, the VM).
	private static CalculatorLang CreateLang ( )
	{
		var lang = new CalculatorLang ( "Simple Math", new Version ( 1, 0, 0 ) );
		
		// A constant
		lang.AddConstant ( "one", 1D );
		
		// Unary operators
		lang.AddOperator ( UnaryOperatorFix.Prefix, "-", 1, n => -n );
		lang.AddOperator ( UnaryOperatorFix.Postfix, "?", 1, n => ~n );
		
		// Binary operators
		lang.AddOperator ( OperatorAssociativity.Left, "+", 1, ( lhs, rhs ) => lhs + rhs );
		lang.AddOperator ( OperatorAssociativity.Right, "^", 2, ( lhs, rhs ) => Math.Pow ( lhs, rhs ) );
		
		// Functions
		lang.AddFunction ( "sin", Math.Sin );
		
		// Return the calculator language
		return lang;
	}
}
```
### Code execution:
```csharp
using GParse.Lexing;
using GParse.Lexing.Errors;
using GParse.Parsing.Errors;
using Calculator.Parsing;

public class Program
{
	public static void Main ( )
	{
		var lang = CreateLang ( );
		var vm = new CalculatorVM ( lang );
		try
		{
			var expr = Console.ReadLine ( );// obtain math expression string somehow
			var res = vm.Execute ( expr ); // Do something with the returned Double
		}
		catch ( LexException ex )
		{
			// Deal with the lexing exception here
		}
		catch ( ParseException ex )
		{
			// Deal with the parsing exception here
		}
	}
	
	// Simple language example, for a more in-depth example
	// see `Program.cs` in Calculator.CLI
	private static CalculatorLang CreateLang ( )
	{
		var lang = new CalculatorLang ( "Simple Math", new Version ( 1, 0, 0 ) );
		
		// A constant
		lang.AddConstant ( "one", 1D );
		
		// Unary operators
		lang.AddOperator ( UnaryOperatorFix.Prefix, "-", 1, n => -n );
		lang.AddOperator ( UnaryOperatorFix.Postfix, "?", 1, n => ~n );
		
		// Binary operators
		lang.AddOperator ( OperatorAssociativity.Left, "+", 1, ( lhs, rhs ) => lhs + rhs );
		lang.AddOperator ( OperatorAssociativity.Right, "^", 2, ( lhs, rhs ) => Math.Pow ( lhs, rhs ) );
		
		// Functions
		lang.AddFunction ( "sin", Math.Sin );
		
		// Return the calculator language
		return lang;
	}
}
```

## Grammar
The calculator doesn't has a fixed grammar mainly because of things being customizable, but this is the general EBNF grammar:
```ebnf
# Base definitions
constant         = ? Constants defined by the user in the CalculatorLang ?;
unaryop          = ? Unary operators defined by the user in the CalculatorLang ?;
binaryop         = ? Binary operators defined by the user in the CalculatorLang ?;
funcname         = ? Function names defined by the user in the CalculatorLang ?;
number           = ? Floating point decimal number or binary/octal/hexadecimal integer ?;

# Expressions
parenthesisexpr  = '(', expr, ')';
unaryexpr        = unaryop, expr | expr, unaryop;
binaryexpr       = expr, binaryop, expr;
functioncall     = funcname, '(', [ { expr, ',' },  expr ], ')';
expr             = number | constant | parenthesisexpr | unaryexpr | binaryexpr | functioncall;
```

# Dependencies
- [GParse](https://github.com/GGG-KILLER/GParse)

# License
GPL-3
