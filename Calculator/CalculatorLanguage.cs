using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Calculator.Definitions;
using Calculator.Lexing;
using Calculator.Parsing;
using Calculator.Parsing.AST;
using Calculator.Parsing.Visitors;
using GParse;
using GParse.IO;
using GParse.Lexing;

namespace Calculator
{
    /// <summary>
    /// Defines the language used for a Calculator
    /// </summary>
    public readonly struct CalculatorLanguage : IEquatable<CalculatorLanguage>
    {
        private CalculatorLexerBuilder LexerBuilder { get; }

        private CalculatorParserBuilder ParserBuilder { get; }

        /// <summary>
        /// The list of the constants that this language has
        /// </summary>
        public ImmutableDictionary<String, Constant> Constants { get; }

        /// <summary>
        /// The list of unary operators that this language has
        /// </summary>
        public ImmutableDictionary<(UnaryOperatorFix, String), UnaryOperator> UnaryOperators { get; }

        /// <summary>
        /// The list of binary operators that this language has
        /// </summary>
        public ImmutableDictionary<String, BinaryOperator> BinaryOperators { get; }

        /// <summary>
        /// The list of functions that this language has
        /// </summary>
        public ImmutableDictionary<String, Function> Functions { get; }

        /// <summary>
        /// The tree evaluator
        /// </summary>
        public StatelessTreeEvaluator TreeEvaluator { get; }

        /// <summary>
        /// Initializes a new <see cref="CalculatorLanguage" />
        /// </summary>
        /// <param name="constants"></param>
        /// <param name="unaryOperators"></param>
        /// <param name="binaryOperators"></param>
        /// <param name="functions"></param>
        internal CalculatorLanguage (
            ImmutableDictionary<String, Constant> constants,
            ImmutableDictionary<(UnaryOperatorFix, String), UnaryOperator> unaryOperators,
            ImmutableDictionary<String, BinaryOperator> binaryOperators,
            ImmutableDictionary<String, Function> functions )
        {
            this.Constants = constants;
            this.UnaryOperators = unaryOperators;
            this.BinaryOperators = binaryOperators;
            this.Functions = functions;

            this.LexerBuilder = null;
            this.ParserBuilder = null;
            this.TreeEvaluator = null;
            this.LexerBuilder = new CalculatorLexerBuilder ( this );
            this.ParserBuilder = new CalculatorParserBuilder ( this );
            this.TreeEvaluator = new StatelessTreeEvaluator ( this );
        }

        #region Constant management

        /// <summary>
        /// Creates a new language with the given <see cref="Constant" /> set
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="value"></param>
        public CalculatorLanguage SetConstant ( String ident, Double value ) =>
            new CalculatorLanguage (
                this.Constants.SetItem ( ident.ToLower ( ), new Constant ( ident, value ) ),
                this.UnaryOperators,
                this.BinaryOperators,
                this.Functions
            );

        /// <summary>
        /// Checks whether this language has a <see cref="Constant" /> that matches the provided
        /// <paramref name="id" />
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Boolean HasConstant ( String id ) => this.Constants.ContainsKey ( id.ToLower ( ) );

        /// <summary>
        /// Returns the <see cref="Constant" /> matches the provided <paramref name="id" />
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Constant GetConstant ( String id ) => this.Constants[id.ToLower ( )];

        #endregion Constant management

        #region Unary operator management

        /// <summary>
        /// Creates a new language with the provided <see cref="UnaryOperator" /> set
        /// </summary>
        /// <param name="fix"></param>
        /// <param name="operator"></param>
        /// <param name="precedence"></param>
        /// <param name="action"></param>
        public CalculatorLanguage AddUnaryOperator ( UnaryOperatorFix fix, String @operator, Int32 precedence, Func<Double, Double> action ) =>
            new CalculatorLanguage (
                this.Constants,
                this.UnaryOperators.SetItem (
                    (fix, @operator.ToLower ( )),
                    new UnaryOperator ( fix, @operator, precedence, action )
                ),
                this.BinaryOperators,
                this.Functions
            );

        /// <summary>
        /// Checks whether this language contains an <see cref="UnaryOperator" /> that matches the given
        /// <paramref name="operator" />
        /// </summary>
        /// <param name="operator"></param>
        /// <param name="fix"></param>
        /// <returns></returns>
        public Boolean HasUnaryOperator ( String @operator, UnaryOperatorFix fix ) =>
            this.UnaryOperators.ContainsKey ( (fix, @operator.ToLower ( )) );

        /// <summary>
        /// Returns the <see cref="UnaryOperator" />
        /// </summary>
        /// <param name="operator"></param>
        /// <param name="fix"></param>
        /// <returns></returns>
        public UnaryOperator GetUnaryOperator ( String @operator, UnaryOperatorFix fix ) =>
            this.UnaryOperators[(fix, @operator.ToLower ( ))];

        #endregion Unary operator management

        #region Binary operator management

        /// <summary>
        /// Returns a new language with the provided <see cref="BinaryOperator" /> set
        /// </summary>
        /// <param name="associativity"></param>
        /// <param name="operator"></param>
        /// <param name="precedence"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public CalculatorLanguage AddBinaryOperator ( Associativity associativity, String @operator, Int32 precedence, Func<Double, Double, Double> action ) =>
            new CalculatorLanguage (
                this.Constants,
                this.UnaryOperators,
                this.BinaryOperators.SetItem ( @operator.ToLower ( ), new BinaryOperator ( associativity, @operator, precedence, action ) ),
                this.Functions
            );

        /// <summary>
        /// Checks whether this language contains a <see cref="BinaryOperator" /> that matches the
        /// provided <paramref name="op" />
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public Boolean HasBinaryOperator ( String op ) => this.BinaryOperators.ContainsKey ( op.ToLower ( ) );

        /// <summary>
        /// Returns the <see cref="BinaryOperator" /> that matches the provided <paramref name="op" />
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public BinaryOperator GetBinaryOperator ( String op ) => this.BinaryOperators[op.ToLower ( )];

        #endregion Binary operator management

        #region Function management

        /// <summary>
        /// Creates a new language with the provided <see cref="Function" /> set
        /// </summary>
        /// <param name="name"></param>
        /// <param name="overloadConfigurator"></param>
        public CalculatorLanguage AddFunction ( in String name, Action<FunctionBuilder> overloadConfigurator )
        {
            if ( overloadConfigurator == null )
                throw new ArgumentNullException ( nameof ( overloadConfigurator ) );

            var funDefBuilder = new FunctionBuilder ( name );
            overloadConfigurator ( funDefBuilder );
            Function definition = funDefBuilder.GetFunctionDefinition ( );

            return new CalculatorLanguage (
                this.Constants,
                this.UnaryOperators,
                this.BinaryOperators,
                this.Functions.SetItem ( name.ToLower ( ), definition )
            );
        }

        /// <summary>
        /// Checks whetehr this language contains a <see cref="Function" /> that matches the provided
        /// <paramref name="id" />
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Boolean HasFunction ( String id ) => this.Functions.ContainsKey ( id.ToLower ( ) );

        /// <summary>
        /// Returns the <see cref="Function" /> that matches the provided <paramref name="id" />
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Function GetFunction ( String id ) => this.Functions[id.ToLower ( )];

        #endregion Function management

        #region Factory Methods

        /// <summary>
        /// Builds a lexer for the provided <paramref name="expression" /> and
        /// <paramref name="diagnosticReporter" />
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="diagnosticReporter"></param>
        /// <returns></returns>
        public ILexer<CalculatorTokenType> GetLexer ( String expression, IProgress<Diagnostic> diagnosticReporter ) =>
            this.LexerBuilder.BuildLexer ( expression, diagnosticReporter );

        /// <summary>
        /// Builds a lexer for the provided <paramref name="reader" /> and
        /// <paramref name="diagnosticReporter" />
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnosticReporter"></param>
        /// <returns></returns>
        public ILexer<CalculatorTokenType> GetLexer ( ICodeReader reader, IProgress<Diagnostic> diagnosticReporter ) =>
            this.LexerBuilder.BuildLexer ( reader, diagnosticReporter );

        /// <summary>
        /// Builds a parser for the provided <paramref name="expression" /> and
        /// <paramref name="diagnosticReporter" />
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="diagnosticReporter"></param>
        /// <returns></returns>
        public CalculatorParser GetParser ( String expression, IProgress<Diagnostic> diagnosticReporter ) =>
            this.ParserBuilder.CreateParser (
                new TokenReader<CalculatorTokenType> ( this.GetLexer ( expression, diagnosticReporter ) ),
                diagnosticReporter ) as CalculatorParser;

        /// <summary>
        /// Builds a parser for the provided <paramref name="lexer" /> and
        /// <paramref name="diagnosticReporter" />
        /// </summary>
        /// <param name="lexer"></param>
        /// <param name="diagnosticReporter"></param>
        /// <returns></returns>
        public CalculatorParser GetParser ( ILexer<CalculatorTokenType> lexer, IProgress<Diagnostic> diagnosticReporter ) =>
            this.ParserBuilder.CreateParser ( new TokenReader<CalculatorTokenType> ( lexer ), diagnosticReporter ) as CalculatorParser;

        /// <summary>
        /// Builds a parser for the provided <paramref name="tokenReader" /> and
        /// <paramref name="diagnosticReporter" />
        /// </summary>
        /// <param name="tokenReader"></param>
        /// <param name="diagnosticReporter"></param>
        /// <returns></returns>
        public CalculatorParser GetParser ( ITokenReader<CalculatorTokenType> tokenReader, IProgress<Diagnostic> diagnosticReporter ) =>
            this.ParserBuilder.CreateParser ( tokenReader, diagnosticReporter ) as CalculatorParser;

        #endregion Factory Methods

        #region Direct Expression Execution Methods

        /// <summary>
        /// Lexes a given string and returns the tokens along with any diagnostics emmited by the lexer
        /// modules
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="diagnostics"></param>
        /// <returns></returns>
        public IEnumerable<Token<CalculatorTokenType>> Lex ( String expression, out IEnumerable<Diagnostic> diagnostics )
        {
            var toks = new List<Token<CalculatorTokenType>> ( );
            var diags = new DiagnosticList ( );
            diagnostics = diags;
            ILexer<CalculatorTokenType> lexer = this.LexerBuilder.BuildLexer ( expression, diags );
            while ( !lexer.EOF )
                toks.Add ( lexer.Consume ( ) );
            return toks;
        }

        /// <summary>
        /// Parses a given string and returns the result node along with any diagnostics emmited by the
        /// lexer modules and/or parselets
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="diagnostics"></param>
        /// <returns></returns>
        public CalculatorTreeNode Parse ( String expression, out IEnumerable<Diagnostic> diagnostics )
        {
            var diags = new DiagnosticList ( );
            diagnostics = diags;
            return ( this.ParserBuilder.CreateParser (
                new TokenReader<CalculatorTokenType> ( this.LexerBuilder.BuildLexer ( expression, diags ) ),
                diags
            ) as CalculatorParser ).Parse ( );
        }

        /// <summary>
        /// Evaluates an expression and returns the evaluated value along with any diagnostics emmited by
        /// the lexer modules and/or parselets
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="diagnostics"></param>
        /// <returns></returns>
        public Double Evaluate ( String expr, out IEnumerable<Diagnostic> diagnostics )
        {
            CalculatorTreeNode tree = this.Parse ( expr, out diagnostics );
            return tree.Accept ( this.TreeEvaluator );
        }

        #endregion Direct Expression Execution Methods

        #region Generated Code

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals ( Object obj ) => obj is CalculatorLanguage && this.Equals ( ( CalculatorLanguage ) obj );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( CalculatorLanguage other ) => EqualityComparer<ImmutableDictionary<String, Constant>>.Default.Equals ( this.Constants, other.Constants ) && EqualityComparer<ImmutableDictionary<(UnaryOperatorFix, String), UnaryOperator>>.Default.Equals ( this.UnaryOperators, other.UnaryOperators ) && EqualityComparer<ImmutableDictionary<String, BinaryOperator>>.Default.Equals ( this.BinaryOperators, other.BinaryOperators ) && EqualityComparer<ImmutableDictionary<String, Function>>.Default.Equals ( this.Functions, other.Functions );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = -780326226;
            hashCode = hashCode * -1521134295 + EqualityComparer<ImmutableDictionary<String, Constant>>.Default.GetHashCode ( this.Constants );
            hashCode = hashCode * -1521134295 + EqualityComparer<ImmutableDictionary<(UnaryOperatorFix, String), UnaryOperator>>.Default.GetHashCode ( this.UnaryOperators );
            hashCode = hashCode * -1521134295 + EqualityComparer<ImmutableDictionary<String, BinaryOperator>>.Default.GetHashCode ( this.BinaryOperators );
            hashCode = hashCode * -1521134295 + EqualityComparer<ImmutableDictionary<String, Function>>.Default.GetHashCode ( this.Functions );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="language1"></param>
        /// <param name="language2"></param>
        /// <returns></returns>
        public static Boolean operator == ( CalculatorLanguage language1, CalculatorLanguage language2 ) => language1.Equals ( language2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="language1"></param>
        /// <param name="language2"></param>
        /// <returns></returns>
        public static Boolean operator != ( CalculatorLanguage language1, CalculatorLanguage language2 ) => !( language1 == language2 );

        #endregion Generated Code
    }
}
