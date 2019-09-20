using System;
using Calculator.Parsing;
using Calculator.Parsing.AST;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Calculator.Definitions;
    using Calculator.Lexing;
    using Calculator.Parsing.Visitors;
    using GParse;
    using GParse.Lexing;
    using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
    using static ASTHelper;

    [TestClass]
    public class ParserTests
    {
        private readonly CalculatorLanguage language;

        public ParserTests ( )
        {
            // No need to define constants or functions, since they'd only be used during evaluation, but
            // since the whole library assumes you'll be executing the expression tree at some point, you
            // have to provide operator implementations.
            this.language = new CalculatorLanguageBuilder ( )
                .AddBinaryOperator ( Associativity.Left, "+", 1, Math.Max )
                .AddBinaryOperator ( Associativity.Left, "/", 2, Math.Max )
                .AddImplicitMultiplication ( 3, ( x, y ) => x * y )
                .AddBinaryOperator ( Associativity.Right, "^", 5, Math.Pow )
                .AddUnaryOperator ( UnaryOperatorFix.Prefix, "+", 6, x => x )
                .AddUnaryOperator ( UnaryOperatorFix.Postfix, "!", 7, x => x )
                .GetCalculatorLanguage ( );
        }

        private CalculatorTreeNode ParseExpression ( String expression )
        {
            CalculatorTreeNode parsed = this.language.Parse ( expression, out IEnumerable<Diagnostic> diagnostics );
            foreach ( Diagnostic diagnostic in diagnostics )
            {
                Logger.LogMessage ( @"{0} {1}: {2} {3}
{4}", diagnostic.Id, diagnostic.Severity, diagnostic.Range, diagnostic.Description, CalculatorDiagnostics.FormatDiagnostic ( expression, diagnostic ) );
            }

            Assert.AreEqual ( diagnostics.Count ( ), 0, "Expression wasn't parsed without errors, warnings or suggestions." );
            return parsed;
        }

        private readonly StatelessTreeReconstructor reconstructor = new StatelessTreeReconstructor ( );

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
                ("+1", UnaryOperator ( "+", 1, UnaryOperatorFix.Prefix )),
                ("1!", UnaryOperator ( "!", 1, UnaryOperatorFix.Postfix )),
                ("+1!", UnaryOperator (
                    "+",
                    UnaryOperator (
                        "!",
                        1,
                        UnaryOperatorFix.Postfix ),
                    UnaryOperatorFix.Prefix )),
                ("++1", UnaryOperator (
                    "+",
                    UnaryOperator (
                        "+",
                        1,
                        UnaryOperatorFix.Prefix ),
                    UnaryOperatorFix.Prefix )),
                ("1!!", UnaryOperator (
                    "!",
                    UnaryOperator (
                        "!",
                        1,
                        UnaryOperatorFix.Postfix ),
                    UnaryOperatorFix.Postfix )),
                ("++1!!", UnaryOperator (
                    "+",
                    UnaryOperator (
                        "+",
                        UnaryOperator (
                            "!",
                            UnaryOperator (
                                "!",
                                1,
                                UnaryOperatorFix.Postfix ),
                            UnaryOperatorFix.Postfix ),
                        UnaryOperatorFix.Prefix ),
                    UnaryOperatorFix.Prefix )),
                ("+(+1)!!", UnaryOperator ( "+", UnaryOperator ( "!", UnaryOperator ( "!", Grouped ( UnaryOperator ( "+", 1, UnaryOperatorFix.Prefix ) ), UnaryOperatorFix.Postfix ), UnaryOperatorFix.Postfix ), UnaryOperatorFix.Prefix ))
            );

        [TestMethod]
        public void BinaryOperatorsAreParsedProperly ( ) =>
            this.TestList (
                ("2 + 2", BinaryOperator ( 2, "+", 2 )),
                ("2 + 2 + 2", BinaryOperator ( BinaryOperator ( 2, "+", 2 ), "+", 2 )),
                ("2 + 2 / 2", BinaryOperator ( 2, "+", BinaryOperator ( 2, "/", 2 ) )),
                ("2 / 2 + 2", BinaryOperator ( BinaryOperator ( 2, "/", 2 ), "+", 2 )),
                ("2 / 2 / 2", BinaryOperator ( BinaryOperator ( 2, "/", 2 ), "/", 2 )),
                ("2 / 2 + 2 / 2", BinaryOperator ( BinaryOperator ( 2, "/", 2 ), "+", BinaryOperator ( 2, "/", 2 ) )),
                ("2 + 2 / 2 + 2", BinaryOperator ( BinaryOperator ( 2, "+", BinaryOperator ( 2, "/", 2 ) ), "+", 2 )),
                /* 2 + 2 / 2 + 2 / 2 + 2 / 2
                 * ⤷ ((2 + (2 / 2)) + (2 / 2)) + (2 / 2)
                 *    ⤷ ((2 + bin(2, / 2)) + bin(2, /, 2)) + bin(2, /, 2)
                 *      ⤷ (bin(2, +, bin(2, /, 2)) + bin(2, /, 2)) + bin(2, /, 2)
                 *         ⤷ bin(bin(2, +, bin(2 /, 2)), +, bin(2, /, 2)) + bin(2, /, 2)
                 *            ⤷ bin(bin(bin(2, +, bin(2 /, 2)), +, bin(2, /, 2)), +, bin(2, /, 2))
                 */
                ("2 + 2 / 2 + 2 / 2 + 2 / 2", BinaryOperator ( BinaryOperator ( BinaryOperator ( 2, "+", BinaryOperator ( 2, "/", 2 ) ), "+", BinaryOperator ( 2, "/", 2 ) ), "+", BinaryOperator ( 2, "/", 2 ) ))
            );

        [TestMethod]
        public void ImplicitOperationsAreParsedProperly ( ) =>
            this.TestList (
                ("2 2", ImplicitMultiplication ( 2, 2 )),
                ("a(b)", FunctionCall ( "a", "b" )),
                ("a b", ImplicitMultiplication ( "a", "b" )),
                ("(a)(b)", ImplicitMultiplication ( Grouped ( "a" ), Grouped ( "b" ) ))
            );

        [TestMethod]
        public void FunctionCallsAreParsedProperly ( ) =>
            this.TestList (
                ("func(a)", FunctionCall ( "func", "a" )),
                ("func(a, a, 2 / 2 + 2 / 2,   2 + 2 / 2 + 2)", FunctionCall (
                    "func",

                    // args:
                    "a",
                    "a",
                    BinaryOperator ( BinaryOperator ( 2, "/", 2 ), "+", BinaryOperator ( 2, "/", 2 ) ),
                    BinaryOperator ( BinaryOperator ( 2, "+", BinaryOperator ( 2, "/", 2 ) ), "+", 2 )
                )),
                ("func(a a)", FunctionCall (
                    "func",
                    ImplicitMultiplication (
                        "a",
                        "a"
                    )
                )),
                ("func(a a) a", ImplicitMultiplication (
                    FunctionCall (
                        "func",
                        ImplicitMultiplication ( "a", "a" )
                    ),
                    "a"
                )),
                ("a b(c d)", ImplicitMultiplication (
                    "a",
                    FunctionCall (
                        "b",
                        ImplicitMultiplication (
                            "c",
                            "d"
                        )
                    )
                ))
            );

        [TestMethod]
        public void ComplexExpressionsAreParsedProperly ( )
        {
            (String expr, CalculatorTreeNode tree) funcaabb = ("func(a, a, 2 / 2 + 2 / 2, 2 + 2 / 2 + 2)", FunctionCall (
                "func",

                // args:
                "a",
                "a",
                BinaryOperator ( BinaryOperator ( 2, "/", 2 ), "+", BinaryOperator ( 2, "/", 2 ) ),
                BinaryOperator ( BinaryOperator ( 2, "+", BinaryOperator ( 2, "/", 2 ) ), "+", 2 )
            ));
            (String expr, CalculatorTreeNode tree) funcaabfuncaabb = ($"func(a, a, 2 / 2 + 2 / 2, {funcaabb.expr}, a b c)", FunctionCall (
                "func",

                // args:
                "a",
                "a",
                BinaryOperator ( BinaryOperator ( 2, "/", 2 ), "+", BinaryOperator ( 2, "/", 2 ) ),
                funcaabb.tree,
                ImplicitMultiplication (
                    ImplicitMultiplication (
                        "a",
                        "b"
                    ),
                    "c"
                )
            ));

            this.TestList (
                (funcaabb.expr, funcaabb.tree),
                (funcaabfuncaabb.expr, funcaabfuncaabb.tree),
                ($"({funcaabfuncaabb.expr})({funcaabfuncaabb.expr})", ImplicitMultiplication (
                    Grouped ( funcaabfuncaabb.tree ),
                    Grouped ( funcaabfuncaabb.tree )
                )),
                ($"+({funcaabb.expr} + {funcaabfuncaabb.expr} / const!) + {funcaabfuncaabb.expr}", BinaryOperator (
                    UnaryOperator (
                        "+",
                        Grouped (
                            BinaryOperator (
                                funcaabb.tree,
                                "+",
                                BinaryOperator (
                                    funcaabfuncaabb.tree,
                                    "/",
                                    UnaryOperator (
                                        "!",
                                        "const",
                                        UnaryOperatorFix.Postfix
                                    )
                                )
                            )
                        ),
                        UnaryOperatorFix.Prefix
                    ),
                    "+",
                    funcaabfuncaabb.tree
                ))
            );
        }
    }
}