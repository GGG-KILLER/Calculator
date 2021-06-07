using System;
using Calculator.Parsing;
using Calculator.Parsing.AST;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Calculator.Definitions;
    using Calculator.Parsing.Visitors;
    using GParse;
    using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
    using static ASTHelper;

    [TestClass]
    public class ParserTests
    {
        private readonly CalculatorLanguage language;

        public ParserTests ( )
        {
            // No need to define constants or functions, since they'd only be used during evaluation,
            // but since the whole library assumes you'll be executing the expression tree at some
            // point, you have to provide operator implementations.
            this.language = new CalculatorLanguageBuilder ( )
                .AddBinaryOperator ( Associativity.Left, "+", 1, Math.Max )
                .AddBinaryOperator ( Associativity.Left, "/", 2, Math.Max )
                .AddImplicitMultiplication ( 3, ( x, y ) => x * y )
                .AddBinaryOperator ( Associativity.Right, "^", 5, Math.Pow )
                .AddSuperscriptExponentiation ( 5, Math.Pow )
                .AddUnaryOperator ( UnaryOperatorFix.Prefix, "+", 6, x => x )
                .AddUnaryOperator ( UnaryOperatorFix.Postfix, "!", 7, x => x )
                .ToCalculatorLanguage ( );
        }

        private CalculatorTreeNode ParseExpression ( String expression )
        {
            CalculatorTreeNode parsed = this.language.Parse ( expression, out IEnumerable<Diagnostic> diagnostics );
            foreach ( Diagnostic diagnostic in diagnostics )
            {
                Logger.LogMessage ( @"{0} {1}: {2}
{3}", diagnostic.Id, diagnostic.Severity, diagnostic.Description, CalculatorDiagnostics.HighlightRange ( expression, diagnostic.Range ) );
            }

            Assert.AreEqual ( diagnostics.Count ( ), 0, "Expression wasn't parsed without errors, warnings or suggestions." );
            return parsed;
        }

        private readonly SimpleTreeReconstructor reconstructor = new SimpleTreeReconstructor ( );

        private void TestList ( params (String expr, CalculatorTreeNode expected)[] tests )
        {
            foreach ( (var expr, CalculatorTreeNode expected) in tests )
            {
                CalculatorTreeNode gotten = this.ParseExpression ( expr );
                Assert.IsTrue ( expected.StructurallyEquals ( gotten ), $"{expected.Accept ( this.reconstructor )} ≡ {gotten.Accept ( this.reconstructor )}" );
            }
        }

        [DataTestMethod]
        [DataRow ( "0x20", 0x20 )]
        [DataRow ( "0o17", 15 )]
        [DataRow ( "0b10", 2 )]
        [DataRow ( "10", 10 )]
        [DataRow ( "10.20", 10.20 )]
        [DataRow ( "10e12", 10e12 )]
        [DataRow ( "10e-12", 10e-12 )]
        [DataRow ( "10e+12", 10e+12 )]
        [DataRow ( "10.20e12", 10.20e12 )]
        [DataRow ( "10.20e+12", 10.20e+12 )]
        [DataRow ( "10.20e-12", 10.20e-12 )]
        [DataRow ( "10E12", 10e12 )]
        [DataRow ( "10E-12", 10e-12 )]
        [DataRow ( "10E+12", 10e+12 )]
        [DataRow ( "10.20E12", 10.20e12 )]
        [DataRow ( "10.20E+12", 10.20e+12 )]
        [DataRow ( "10.20E-12", 10.20e-12 )]
        public void NumbersAreParsedProperly ( String num, Double val )
        {
            CalculatorTreeNode parsed = this.ParseExpression ( num );
            Assert.IsInstanceOfType ( parsed, typeof ( NumberExpression ) );
            Assert.AreEqual ( ( Double ) ( parsed as NumberExpression ).Value.Value, val );
        }

        [DataTestMethod]
        [DataRow ( "pi" )]
        [DataRow ( "Pi" )]
        [DataRow ( "π" )]
        [DataRow ( "inf" )]
        [DataRow ( "Inf" )]
        [DataRow ( "InF" )]
        [DataRow ( "INf" )]
        [DataRow ( "INF" )]
        public void IdentifiersAreParsedProperly ( String expr )
        {
            CalculatorTreeNode parsed = this.ParseExpression ( expr );
            Assert.IsInstanceOfType ( parsed, typeof ( IdentifierExpression ) );
            Assert.AreEqual ( ( parsed as IdentifierExpression ).Identifier.Value, expr );
        }

        [TestMethod]
        public void UnaryOperatorsAreParsedProperly ( ) =>
            this.TestList (
                ("+1", Prefix ( "+", 1 )),
                ("1!", Postfix ( 1, "!" )),
                ("+1!", Prefix ( "+", Postfix ( 1, "!" ) )),
                ("++1", Prefix ( "+", Prefix ( "+", 1 ) )),
                ("1!!", Postfix ( Postfix ( 1, "!" ), "!" )),
                ("++1!!", Prefix ( "+", Prefix ( "+", Postfix ( Postfix ( 1, "!" ), "!" ) ) )),
                ("+(+1)!!", Prefix ( "+", Postfix ( Postfix ( Grouped ( Prefix ( "+", 1 ) ), "!" ), "!" ) ))
            );

        [TestMethod]
        public void BinaryOperatorsAreParsedProperly ( ) =>
            this.TestList (
                ("2 + 2", Binary ( 2, "+", 2 )),
                ("2 + 2 + 2", Binary ( Binary ( 2, "+", 2 ), "+", 2 )),
                ("2 + 2 / 2", Binary ( 2, "+", Binary ( 2, "/", 2 ) )),
                ("2 / 2 + 2", Binary ( Binary ( 2, "/", 2 ), "+", 2 )),
                ("2 / 2 / 2", Binary ( Binary ( 2, "/", 2 ), "/", 2 )),
                ("2 / 2 + 2 / 2", Binary ( Binary ( 2, "/", 2 ), "+", Binary ( 2, "/", 2 ) )),
                ("2 + 2 / 2 + 2", Binary ( Binary ( 2, "+", Binary ( 2, "/", 2 ) ), "+", 2 )),
                /* 2 + 2 / 2 + 2 / 2 + 2 / 2
                 * ⤷ ((2 + (2 / 2)) + (2 / 2)) + (2 / 2)
                 *    ⤷ ((2 + bin(2, / 2)) + bin(2, /, 2)) + bin(2, /, 2)
                 *      ⤷ (bin(2, +, bin(2, /, 2)) + bin(2, /, 2)) + bin(2, /, 2)
                 *         ⤷ bin(bin(2, +, bin(2 /, 2)), +, bin(2, /, 2)) + bin(2, /, 2)
                 *            ⤷ bin(bin(bin(2, +, bin(2 /, 2)), +, bin(2, /, 2)), +, bin(2, /, 2))
                 */
                ("2 + 2 / 2 + 2 / 2 + 2 / 2", Binary ( Binary ( Binary ( 2, "+", Binary ( 2, "/", 2 ) ), "+", Binary ( 2, "/", 2 ) ), "+", Binary ( 2, "/", 2 ) ))
            );

        [TestMethod]
        public void ImplicitOperationsAreParsedProperly ( ) =>
            this.TestList (
                ("2 2", Implicit ( 2, 2 )),
                ("a(b)", Function ( "a", "b" )),
                ("a b", Implicit ( "a", "b" )),
                ("(a)(b)", Implicit ( Grouped ( "a" ), Grouped ( "b" ) ))
            );

        [TestMethod]
        public void FunctionCallsAreParsedProperly ( ) =>
            this.TestList (
                ("func(a)", Function ( "func", "a" )),
                ("func(a, a, 2 / 2 + 2 / 2,   2 + 2 / 2 + 2)", Function (
                    "func",

                    // args:
                    "a",
                    "a",
                    Binary ( Binary ( 2, "/", 2 ), "+", Binary ( 2, "/", 2 ) ),
                    Binary ( Binary ( 2, "+", Binary ( 2, "/", 2 ) ), "+", 2 )
                )),
                ("func(a a)", Function (
                    "func",
                    Implicit (
                        "a",
                        "a"
                    )
                )),
                ("func(a a) a", Implicit (
                    Function (
                        "func",
                        Implicit ( "a", "a" )
                    ),
                    "a"
                )),
                ("a b(c d)", Implicit (
                    "a",
                    Function (
                        "b",
                        Implicit (
                            "c",
                            "d"
                        )
                    )
                ))
            );

        [TestMethod]
        public void SuperscriptExponetiationEpxressionsAreParsedProperly ( )
        {
            this.TestList (
                ("1¹", Superscript ( 1, 1 )),
                ("a¹", Superscript ( "a", 1 )),
                ("f(x)¹", Superscript ( Function ( "f", "x" ), 1 )),
                ("1⁻¹", Superscript ( 1, -1 )),
                ("a⁻¹", Superscript ( "a", -1 )),
                ("f(x)⁻¹", Superscript ( Function ( "f", "x" ), -1 )),
                ("1¹²³", Superscript ( 1, 123 )),
                ("a¹²³", Superscript ( "a", 123 )),
                ("f(x)¹²³", Superscript ( Function ( "f", "x" ), 123 )),
                ("1⁻¹²³", Superscript ( 1, -123 )),
                ("a⁻¹²³", Superscript ( "a", -123 )),
                ("f(x)⁻¹²³", Superscript ( Function ( "f", "x" ), -123 ))
            );
        }

        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage ( "Style", "IDE0042:Deconstruct variable declaration", Justification = "Would make code (even more) confusing." )]
        public void ComplexExpressionsAreParsedProperly ( )
        {
            (String expr, CalculatorTreeNode tree) funcaabb = ("func(a, a, 2 / 2 + 2 / 2, 2 + 2 / 2 + 2)", Function (
                "func",

                // args:
                "a",
                "a",
                Binary ( Binary ( 2, "/", 2 ), "+", Binary ( 2, "/", 2 ) ),
                Binary ( Binary ( 2, "+", Binary ( 2, "/", 2 ) ), "+", 2 )
            ));
            (String expr, CalculatorTreeNode tree) funcaabfuncaabb = ($"func(a, a, 2 / 2 + 2 / 2, {funcaabb.expr}, a b c)", Function (
                "func",

                // args:
                "a",
                "a",
                Binary ( Binary ( 2, "/", 2 ), "+", Binary ( 2, "/", 2 ) ),
                funcaabb.tree,
                Implicit (
                    Implicit (
                        "a",
                        "b"
                    ),
                    "c"
                )
            ));

            this.TestList (
                (funcaabb.expr, funcaabb.tree),
                (funcaabfuncaabb.expr, funcaabfuncaabb.tree),
                ($"({funcaabfuncaabb.expr})({funcaabfuncaabb.expr})", Implicit (
                    Grouped ( funcaabfuncaabb.tree ),
                    Grouped ( funcaabfuncaabb.tree )
                )),
                ($"+({funcaabb.expr} + {funcaabfuncaabb.expr} / const!) + {funcaabfuncaabb.expr}", Binary (
                    Prefix (
                        "+",
                        Grouped (
                            Binary (
                                funcaabb.tree,
                                "+",
                                Binary (
                                    funcaabfuncaabb.tree,
                                    "/",
                                    Postfix (
                                        "const",
                                        "!"
                                    )
                                )
                            )
                        )
                    ),
                    "+",
                    funcaabfuncaabb.tree
                ))
            );
        }
    }
}