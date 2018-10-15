using System;
using Calculator.Parsing;
using Calculator.Parsing.AST;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests
{
    using System.Linq;
    using Calculator.Parsing.Visitors;
    using static ASTHelper;

    [TestClass]
    public class ParserTests
    {
        private readonly CalculatorLanguage Language;

        public ParserTests ( )
        {
            this.Language = new CalculatorLanguage ( );

            // No need to define constants

            this.Language.AddUnaryOperator ( Definitions.UnaryOperatorFix.Prefix, "-", 1, a => a );
            this.Language.AddUnaryOperator ( Definitions.UnaryOperatorFix.Postfix, "@", 1, a => a );

            this.Language.AddBinaryOperator ( Definitions.OperatorAssociativity.Left, "/", 1, ( a, b ) => a );
            this.Language.AddBinaryOperator ( Definitions.OperatorAssociativity.Right, "-", 2, ( a, b ) => a );

            // Or functions, since they'd only be used during evaluation
        }

        private CalculatorASTNode ParseExpression ( String expression ) =>
            new CalculatorParser ( this.Language.GetLexer ( expression ), this.Language ).Parse ( );

        private readonly TreeReconstructor reconstructor = new TreeReconstructor ( );

        private void TestList ( params (String expr, CalculatorASTNode expected)[] tests )
        {
            foreach ( (var expr, CalculatorASTNode expected) in tests )
            {
                CalculatorASTNode gotten = this.ParseExpression ( expr );
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
            CalculatorASTNode parsed = this.ParseExpression ( num );
            Assert.IsInstanceOfType ( parsed, typeof ( NumberExpression ) );
            Assert.AreEqual ( ( parsed as NumberExpression ).Value, val );
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
            CalculatorASTNode parsed = this.ParseExpression ( expr );
            Assert.IsInstanceOfType ( parsed, typeof ( IdentifierExpression ) );
            Assert.AreEqual ( ( parsed as IdentifierExpression ).Identifier.Value, expr );
        }

        [TestMethod]
        public void UnaryOperatorsAreParsedProperly ( ) =>
            this.TestList (
                ("-1", UnaryOperator ( "-", 1, Definitions.UnaryOperatorFix.Prefix )),
                ("2@", UnaryOperator ( "@", 2, Definitions.UnaryOperatorFix.Postfix )),
                ("-2@", UnaryOperator ( "@",
                    UnaryOperator ( "-", 2, Definitions.UnaryOperatorFix.Prefix ), Definitions.UnaryOperatorFix.Postfix )),
                ("----2", UnaryOperator (
                    "-",
                    UnaryOperator (
                        "-",
                        UnaryOperator (
                            "-",
                            UnaryOperator (
                                "-",
                                2,
                                Definitions.UnaryOperatorFix.Prefix
                            ),
                            Definitions.UnaryOperatorFix.Prefix
                        ),
                        Definitions.UnaryOperatorFix.Prefix
                    ),
                    Definitions.UnaryOperatorFix.Prefix
                )),
                ("2@@@@", UnaryOperator (
                    "@",
                    UnaryOperator (
                        "@",
                        UnaryOperator (
                            "@",
                            UnaryOperator (
                                "@",
                                2,
                                Definitions.UnaryOperatorFix.Postfix
                            ),
                            Definitions.UnaryOperatorFix.Postfix
                        ),
                        Definitions.UnaryOperatorFix.Postfix
                    ),
                    Definitions.UnaryOperatorFix.Postfix
                )),
                ("----2@@@@", UnaryOperator (
                    "@",
                    UnaryOperator (
                        "@",
                        UnaryOperator (
                            "@",
                            UnaryOperator (
                                "@",
                                UnaryOperator (
                                    "-",
                                    UnaryOperator (
                                        "-",
                                        UnaryOperator (
                                            "-",
                                            UnaryOperator (
                                                "-",
                                                2,
                                                Definitions.UnaryOperatorFix.Prefix
                                            ),
                                            Definitions.UnaryOperatorFix.Prefix
                                        ),
                                        Definitions.UnaryOperatorFix.Prefix
                                    ),
                                    Definitions.UnaryOperatorFix.Prefix
                                ),
                                Definitions.UnaryOperatorFix.Postfix
                            ),
                            Definitions.UnaryOperatorFix.Postfix
                        ),
                        Definitions.UnaryOperatorFix.Postfix
                    ),
                    Definitions.UnaryOperatorFix.Postfix
                )),
                ("-const", UnaryOperator ( "-", "const", Definitions.UnaryOperatorFix.Prefix )),
                ("const@", UnaryOperator ( "@", "const", Definitions.UnaryOperatorFix.Postfix )),
                ("-const@", UnaryOperator ( "@",
                    UnaryOperator ( "-", "const", Definitions.UnaryOperatorFix.Prefix ), Definitions.UnaryOperatorFix.Postfix )),
                ("----const", UnaryOperator (
                    "-",
                    UnaryOperator (
                        "-",
                        UnaryOperator (
                            "-",
                            UnaryOperator (
                                "-",
                                "const",
                                Definitions.UnaryOperatorFix.Prefix
                            ),
                            Definitions.UnaryOperatorFix.Prefix
                        ),
                        Definitions.UnaryOperatorFix.Prefix
                    ),
                    Definitions.UnaryOperatorFix.Prefix
                )),
                ("const@@@@", UnaryOperator (
                    "@",
                    UnaryOperator (
                        "@",
                        UnaryOperator (
                            "@",
                            UnaryOperator (
                                "@",
                                "const",
                                Definitions.UnaryOperatorFix.Postfix
                            ),
                            Definitions.UnaryOperatorFix.Postfix
                        ),
                        Definitions.UnaryOperatorFix.Postfix
                    ),
                    Definitions.UnaryOperatorFix.Postfix
                )),
                ("----const@@@@", UnaryOperator (
                    "@",
                    UnaryOperator (
                        "@",
                        UnaryOperator (
                            "@",
                            UnaryOperator (
                                "@",
                                UnaryOperator (
                                    "-",
                                    UnaryOperator (
                                        "-",
                                        UnaryOperator (
                                            "-",
                                            UnaryOperator (
                                                "-",
                                                "const",
                                                Definitions.UnaryOperatorFix.Prefix
                                            ),
                                            Definitions.UnaryOperatorFix.Prefix
                                        ),
                                        Definitions.UnaryOperatorFix.Prefix
                                    ),
                                    Definitions.UnaryOperatorFix.Prefix
                                ),
                                Definitions.UnaryOperatorFix.Postfix
                            ),
                            Definitions.UnaryOperatorFix.Postfix
                        ),
                        Definitions.UnaryOperatorFix.Postfix
                    ),
                    Definitions.UnaryOperatorFix.Postfix
                ))
            );

        [TestMethod]
        public void BinaryOperatorsAreParsedProperly ( ) =>
            this.TestList (
                ("2 / 2", BinaryOperator ( 2, "/", 2 )),
                ("2 / 2 / 2", BinaryOperator ( BinaryOperator ( 2, "/", 2 ), "/", 2 )),
                ("2 / 2 - 2", BinaryOperator ( 2, "/", BinaryOperator ( 2, "-", 2 ) )),
                ("2 - 2 / 2", BinaryOperator ( BinaryOperator ( 2, "-", 2 ), "/", 2 )),
                ("2 - 2 - 2", BinaryOperator ( 2, "-", BinaryOperator ( 2, "-", 2 ) )),
                ("2 - 2 / 2 - 2", BinaryOperator ( BinaryOperator ( 2, "-", 2 ), "/", BinaryOperator ( 2, "-", 2 ) )),
                ("2 / 2 - 2 / 2", BinaryOperator ( BinaryOperator ( 2, "/", BinaryOperator ( 2, "-", 2 ) ), "/", 2 )),
                /* 2 / 2 - 2 / 2 - 2 / 2 - 2
                 * ⤷ ((2 / (2 - 2)) / (2 - 2)) / (2 - 2)
                 *    ⤷ ((2 / bin(2, - 2)) / bin(2, -, 2)) / bin(2, -, 2)
                 *      ⤷ (bin(2, /, bin(2, -, 2)) / bin(2, -, 2)) / bin(2, -, 2)
                 *         ⤷ bin(bin(2, /, bin(2 -, 2)), /, bin(2, -, 2)) / bin(2, -, 2)
                 *            ⤷ bin(bin(bin(2, /, bin(2 -, 2)), /, bin(2, -, 2)), /, bin(2, -, 2))
                 */
                ("2 / 2 - 2 / 2 - 2 / 2 - 2", BinaryOperator ( BinaryOperator ( BinaryOperator ( 2, "/", BinaryOperator ( 2, "-", 2 ) ), "/", BinaryOperator ( 2, "-", 2 ) ), "/", BinaryOperator ( 2, "-", 2 ) ))
            );

        [TestMethod]
        public void FunctionCallsAreParsedProperly ( ) =>
            this.TestList (
                ("func(a)", FunctionCall ( "func", "a" )),
                ("func(a, a, 2 - 2 / 2 - 2,   2 / 2 - 2 / 2)", FunctionCall (
                    "func",
                    // args:
                    "a",
                    "a",
                    BinaryOperator ( BinaryOperator ( 2, "-", 2 ), "/", BinaryOperator ( 2, "-", 2 ) ),
                    BinaryOperator ( BinaryOperator ( 2, "/", BinaryOperator ( 2, "-", 2 ) ), "/", 2 )
                ))
            );

        [TestMethod]
        public void ComplexExpressionsAreParsedProperly ( )
        {
            (String expr, CalculatorASTNode tree) funcaabb = ("func(a, a, 2 - 2 / 2 - 2, 2 / 2 - 2 / 2)", FunctionCall (
                "func",
                // args:
                "a",
                "a",
                BinaryOperator ( BinaryOperator ( 2, "-", 2 ), "/", BinaryOperator ( 2, "-", 2 ) ),
                BinaryOperator ( BinaryOperator ( 2, "/", BinaryOperator ( 2, "-", 2 ) ), "/", 2 )
            ));
            (String expr, CalculatorASTNode tree) funcaabfuncaabb = ($"func(a, a, 2 - 2 / 2 - 2, {funcaabb.expr})", FunctionCall (
                "func",
                // args:
                "a",
                "a",
                BinaryOperator ( BinaryOperator ( 2, "-", 2 ), "/", BinaryOperator ( 2, "-", 2 ) ),
                funcaabb.tree
            ));

            this.TestList (
                ($"-({funcaabb.expr} / {funcaabfuncaabb.expr} - const@) / {funcaabfuncaabb.expr}",
                BinaryOperator (
                    UnaryOperator (
                        "-",
                        BinaryOperator (
                            funcaabb.tree,
                            "/",
                            BinaryOperator ( funcaabfuncaabb.tree, "-", UnaryOperator ( "@", "const", Definitions.UnaryOperatorFix.Postfix ) )
                        ),
                        Definitions.UnaryOperatorFix.Prefix
                    ),
                    "/",
                    funcaabfuncaabb.tree
                ))
            );
        }
    }
}
