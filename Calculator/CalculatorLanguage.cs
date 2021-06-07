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

        private TreeEvaluator TreeEvaluator { get; }

        /// <summary>
        /// The list of the constants that this language has
        /// </summary>
        public IImmutableDictionary<string, Constant> Constants { get; }

        /// <summary>
        /// The list of unary operators that this language has
        /// </summary>
        public IImmutableDictionary<(UnaryOperatorFix, string), UnaryOperator> UnaryOperators { get; }

        /// <summary>
        /// The list of binary operators that this language has
        /// </summary>
        public IImmutableDictionary<string, BinaryOperator> BinaryOperators { get; }

        /// <summary>
        /// The list of functions that this language has
        /// </summary>
        public IImmutableDictionary<string, Function> Functions { get; }

        /// <summary>
        /// The list of special operators that this language contains
        /// </summary>
        public IImmutableDictionary<SpecialBinaryOperatorType, SpecialBinaryOperator> SpecialBinaryOperators { get; }

        /// <summary>
        /// Initializes a new <see cref="CalculatorLanguage"/>
        /// </summary>
        /// <param name="constants"></param>
        /// <param name="unaryOperators"></param>
        /// <param name="binaryOperators"></param>
        /// <param name="functions"></param>
        /// <param name="specialOperators"></param>
        public CalculatorLanguage(IImmutableDictionary<string, Constant> constants,
                                   IImmutableDictionary<(UnaryOperatorFix, string), UnaryOperator> unaryOperators,
                                   IImmutableDictionary<string, BinaryOperator> binaryOperators,
                                   IImmutableDictionary<string, Function> functions,
                                   IImmutableDictionary<SpecialBinaryOperatorType, SpecialBinaryOperator> specialOperators)
        {
            Constants = constants;
            UnaryOperators = unaryOperators;
            BinaryOperators = binaryOperators;
            Functions = functions;
            SpecialBinaryOperators = specialOperators;

            LexerBuilder = null;
            ParserBuilder = null;
            TreeEvaluator = null;
            LexerBuilder = new CalculatorLexerBuilder(this);
            ParserBuilder = new CalculatorParserBuilder(this);
            TreeEvaluator = new TreeEvaluator(this);
        }

        /// <summary>
        /// Initializes an empty <see cref="CalculatorLanguage"/> with the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="stringComparer">
        /// The comparer used for constants, operators and function names
        /// </param>
        public CalculatorLanguage(IEqualityComparer<string> stringComparer) : this(
            ImmutableDictionary.Create<string, Constant>(stringComparer),
            ImmutableDictionary.Create<(UnaryOperatorFix, string), UnaryOperator>(new UnaryOperatorKeyPairEqualityComparer(stringComparer)),
            ImmutableDictionary.Create<string, BinaryOperator>(stringComparer),
            ImmutableDictionary.Create<string, Function>(stringComparer),
            ImmutableDictionary.Create<SpecialBinaryOperatorType, SpecialBinaryOperator>())
        {
        }

        #region Constant management

        /// <summary>
        /// Creates a new language with the given <see cref="Constant"/> set
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="value"></param>
        public CalculatorLanguage SetConstant(string ident, double value) =>
            new CalculatorLanguage(
                Constants.SetItem(ident, new Constant(ident, value)),
                UnaryOperators,
                BinaryOperators,
                Functions,
                SpecialBinaryOperators
            );

        /// <summary>
        /// Checks whether this language has a <see cref="Constant"/> that matches the provided
        /// <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasConstant(string id) => Constants.ContainsKey(id);

        /// <summary>
        /// Returns the <see cref="Constant"/> matches the provided <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Constant GetConstant(string id) => Constants[id];

        #endregion Constant management

        #region Unary operator management

        /// <summary>
        /// Creates a new language with the provided <see cref="UnaryOperator"/> set
        /// </summary>
        /// <param name="fix"></param>
        /// <param name="operator"></param>
        /// <param name="precedence"></param>
        /// <param name="action"></param>
        public CalculatorLanguage AddUnaryOperator(UnaryOperatorFix fix, string @operator, int precedence, Func<double, double> action) =>
            new CalculatorLanguage(
                Constants,
                UnaryOperators.SetItem(
                    (fix, @operator),
                    new UnaryOperator(fix, @operator, precedence, action)
                ),
                BinaryOperators,
                Functions,
                SpecialBinaryOperators
            );

        /// <summary>
        /// Checks whether this language contains an <see cref="UnaryOperator"/> that matches the
        /// given <paramref name="operator"/>
        /// </summary>
        /// <param name="operator"></param>
        /// <param name="fix"></param>
        /// <returns></returns>
        public bool HasUnaryOperator(string @operator, UnaryOperatorFix fix) =>
            UnaryOperators.ContainsKey((fix, @operator));

        /// <summary>
        /// Returns the <see cref="UnaryOperator"/>
        /// </summary>
        /// <param name="operator"></param>
        /// <param name="fix"></param>
        /// <returns></returns>
        public UnaryOperator GetUnaryOperator(string @operator, UnaryOperatorFix fix) =>
            UnaryOperators[(fix, @operator)];

        #endregion Unary operator management

        #region Binary operator management

        /// <summary>
        /// Returns a new language with the provided <see cref="BinaryOperator"/> set
        /// </summary>
        /// <param name="associativity"></param>
        /// <param name="operator"></param>
        /// <param name="precedence"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public CalculatorLanguage AddBinaryOperator(Associativity associativity, string @operator, int precedence, Func<double, double, double> action) =>
            new CalculatorLanguage(
                Constants,
                UnaryOperators,
                BinaryOperators.SetItem(@operator, new BinaryOperator(associativity, @operator, precedence, action)),
                Functions,
                SpecialBinaryOperators
            );

        /// <summary>
        /// Checks whether this language contains a <see cref="BinaryOperator"/> that matches the
        /// provided <paramref name="op"/>
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public bool HasBinaryOperator(string op) => BinaryOperators.ContainsKey(op);

        /// <summary>
        /// Returns the <see cref="BinaryOperator"/> that matches the provided <paramref name="op"/>
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public BinaryOperator GetBinaryOperator(string op) => BinaryOperators[op];

        #endregion Binary operator management

        #region Function management

        /// <summary>
        /// Creates a new language with the provided <see cref="Function"/> set
        /// </summary>
        /// <param name="name"></param>
        /// <param name="overloadConfigurator"></param>
        public CalculatorLanguage AddFunction(in string name, Action<FunctionBuilder> overloadConfigurator)
        {
            if (overloadConfigurator == null)
                throw new ArgumentNullException(nameof(overloadConfigurator));

            var funDefBuilder = new FunctionBuilder(name);
            overloadConfigurator(funDefBuilder);
            var definition = funDefBuilder.GetFunctionDefinition();

            return new CalculatorLanguage(
                Constants,
                UnaryOperators,
                BinaryOperators,
                Functions.SetItem(name, definition),
                SpecialBinaryOperators
            );
        }

        /// <summary>
        /// Checks whetehr this language contains a <see cref="Function"/> that matches the provided
        /// <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasFunction(string id) => Functions.ContainsKey(id);

        /// <summary>
        /// Returns the <see cref="Function"/> that matches the provided <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Function GetFunction(string id) => Functions[id];

        #endregion Function management

        #region Special binary operator management

        #region Implicit multiplication

        /// <summary>
        /// Sets the implicit multiplication configurations
        /// </summary>
        /// <param name="precedence"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public CalculatorLanguage SetImplicitMultiplication(int precedence, Func<double, double, double> body) =>
            new CalculatorLanguage(
                Constants,
                UnaryOperators,
                BinaryOperators,
                Functions,
                SpecialBinaryOperators.SetItem(
                    SpecialBinaryOperatorType.ImplicitMultiplication,
                    new SpecialBinaryOperator(SpecialBinaryOperatorType.ImplicitMultiplication, Associativity.Left, precedence, body)));

        /// <summary>
        /// Whether this language has a definition for implicit multiplication
        /// </summary>
        /// <returns></returns>
        public bool HasImplicitMultiplication() =>
            SpecialBinaryOperators.ContainsKey(SpecialBinaryOperatorType.ImplicitMultiplication);

        /// <summary>
        /// Returns the definition for the implicit multiplication operator
        /// </summary>
        /// <returns></returns>
        public SpecialBinaryOperator GetImplicitMultiplication() =>
            SpecialBinaryOperators[SpecialBinaryOperatorType.ImplicitMultiplication];

        #endregion Implicit multiplication

        #region Superscript exponentiation

        /// <summary>
        /// Sets the definition for the superscript exponentiation
        /// </summary>
        /// <param name="precedence"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public CalculatorLanguage SetSuperscriptExponentiation(int precedence, Func<double, double, double> body) =>
            new CalculatorLanguage(
                Constants,
                UnaryOperators,
                BinaryOperators,
                Functions,
                SpecialBinaryOperators.SetItem(
                    SpecialBinaryOperatorType.SuperscriptExponentiation,
                    new SpecialBinaryOperator(SpecialBinaryOperatorType.SuperscriptExponentiation, Associativity.Right, precedence, body)));

        /// <summary>
        /// Returns whether this language has a definition for superscript exponentiation
        /// </summary>
        /// <returns></returns>
        public bool HasSuperscriptExponentiation() =>
            SpecialBinaryOperators.ContainsKey(SpecialBinaryOperatorType.SuperscriptExponentiation);

        /// <summary>
        /// Returns whether this language's definition of superscript exponentiation
        /// </summary>
        /// <returns></returns>
        public SpecialBinaryOperator GetSuperscriptExponentiation() =>
            SpecialBinaryOperators[SpecialBinaryOperatorType.SuperscriptExponentiation];

        #endregion Superscript exponentiation

        #endregion Special binary operator management

        #region Factory Methods

        /// <summary>
        /// Builds a lexer for the provided <paramref name="expression"/> and <paramref name="diagnosticReporter"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="diagnosticReporter"></param>
        /// <returns></returns>
        public ILexer<CalculatorTokenType> GetLexer(string expression, DiagnosticList diagnosticReporter) =>
            LexerBuilder.GetLexer(expression, diagnosticReporter);

        /// <summary>
        /// Builds a lexer for the provided <paramref name="reader"/> and <paramref name="diagnosticReporter"/>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnosticReporter"></param>
        /// <returns></returns>
        public ILexer<CalculatorTokenType> GetLexer(ICodeReader reader, DiagnosticList diagnosticReporter) =>
            LexerBuilder.GetLexer(reader, diagnosticReporter);

        /// <summary>
        /// Builds a parser for the provided <paramref name="expression"/> and <paramref name="diagnosticReporter"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="diagnosticReporter"></param>
        /// <returns></returns>
        public CalculatorParser GetParser(string expression, DiagnosticList diagnosticReporter)
        {
            var lexer = GetLexer(expression, diagnosticReporter);
            return ParserBuilder.CreateParser(
                lexer,
                new TokenReader<CalculatorTokenType>(lexer),
                diagnosticReporter) as CalculatorParser;
        }

        /// <summary>
        /// Builds a parser for the provided <paramref name="lexer"/> and <paramref name="diagnosticReporter"/>
        /// </summary>
        /// <param name="lexer"></param>
        /// <param name="diagnosticReporter"></param>
        /// <returns></returns>
        public CalculatorParser GetParser(ILexer<CalculatorTokenType> lexer, DiagnosticList diagnosticReporter) =>
            ParserBuilder.CreateParser(lexer, new TokenReader<CalculatorTokenType>(lexer), diagnosticReporter) as CalculatorParser;

        #endregion Factory Methods

        #region Direct Expression Execution Methods

        /// <summary>
        /// Lexes a given string and returns the tokens along with any diagnostics emmited by the
        /// lexer modules
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="diagnostics"></param>
        /// <returns></returns>
        public IEnumerable<Token<CalculatorTokenType>> Lex(string expression, out IEnumerable<Diagnostic> diagnostics)
        {
            var toks = new List<Token<CalculatorTokenType>>();
            var diags = new DiagnosticList();
            diagnostics = diags;
            ILexer<CalculatorTokenType> lexer = GetLexer(expression, diags);
            while (!lexer.EndOfFile)
                toks.Add(lexer.Consume());
            return toks;
        }

        /// <summary>
        /// Parses a given string and returns the result node along with any diagnostics emmited by
        /// the lexer modules and/or parselets
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="diagnostics"></param>
        /// <returns></returns>
        public CalculatorTreeNode Parse(string expression, out IEnumerable<Diagnostic> diagnostics)
        {
            var diags = new DiagnosticList();
            diagnostics = diags;
            return GetParser(expression, diags).Parse();
        }

        /// <summary>
        /// Evaluates an expression and returns the evaluated value along with any diagnostics
        /// emmited by the lexer modules and/or parselets
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="diagnostics"></param>
        /// <returns></returns>
        public double Evaluate(string expr, out IEnumerable<Diagnostic> diagnostics)
        {
            var tree = Parse(expr, out diagnostics);
            return tree.Accept(TreeEvaluator);
        }

        #endregion Direct Expression Execution Methods

        #region Generated Code

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is CalculatorLanguage language && Equals(language);

        /// <inheritdoc/>
        public bool Equals(CalculatorLanguage other) =>
            EqualityComparer<IImmutableDictionary<string, Constant>>.Default.Equals(Constants, other.Constants)
            && EqualityComparer<IImmutableDictionary<(UnaryOperatorFix, string), UnaryOperator>>.Default.Equals(UnaryOperators, other.UnaryOperators)
            && EqualityComparer<IImmutableDictionary<string, BinaryOperator>>.Default.Equals(BinaryOperators, other.BinaryOperators)
            && EqualityComparer<IImmutableDictionary<string, Function>>.Default.Equals(Functions, other.Functions)
            && EqualityComparer<IImmutableDictionary<SpecialBinaryOperatorType, SpecialBinaryOperator>>.Default.Equals(SpecialBinaryOperators, other.SpecialBinaryOperators);

        /// <inheritdoc/>
        public override int GetHashCode() =>
            HashCode.Combine(Constants, UnaryOperators, BinaryOperators, Functions, SpecialBinaryOperators);

        /// <summary>
        /// Checks whether two instances of <see cref="CalculatorLanguage"/> are equal
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(CalculatorLanguage left, CalculatorLanguage right) => left.Equals(right);

        /// <summary>
        /// Checks whether two instances of <see cref="CalculatorLanguage"/> are not equal
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(CalculatorLanguage left, CalculatorLanguage right) => !(left == right);

        #endregion Generated Code
    }
}