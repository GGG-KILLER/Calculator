using System;
using System.Collections.Generic;
using System.Linq;
using Calculator.Definitions;
using Calculator.Lexing;
using Calculator.Lexing.Modules;
using GParse.Parsing.Abstractions.Lexing;
using GParse.Parsing.Lexing;
using GParse.Parsing.Lexing.Modules;

namespace Calculator
{
    public class CalculatorLanguage
    {
        #region Definitions Storage

        /// <summary>
        /// The list of the constants that this language has
        /// </summary>
        private readonly List<ConstantDef> ConstantDefs = new List<ConstantDef> ( );

        /// <summary>
        /// The list of unary operators that this language has
        /// </summary>
        private readonly List<UnaryOperatorDef> UnaryOperatorDefs = new List<UnaryOperatorDef> ( );

        /// <summary>
        /// The list of binary operators that this language has
        /// </summary>
        private readonly List<BinaryOperatorDef> BinaryOperatorDefs = new List<BinaryOperatorDef> ( );

        /// <summary>
        /// The list of functions that this language has
        /// </summary>
        private readonly Dictionary<String, Delegate> FunctionDefs = new Dictionary<String, Delegate> ( );

        #endregion Definitions Storage

        /// <summary>
        /// The lexer builder of the language
        /// </summary>
        private readonly LexerBuilder<CalculatorTokenType> lexerBuilder = new LexerBuilder<CalculatorTokenType> ( );

        #region Definitions Enumeration

        /// <summary>
        /// The list of the constants that this language has
        /// </summary>
        public IEnumerable<ConstantDef> Constants => this.ConstantDefs;

        /// <summary>
        /// The list of unary operators that this language has
        /// </summary>
        public IEnumerable<UnaryOperatorDef> UnaryOperators => this.UnaryOperatorDefs;

        /// <summary>
        /// The list of binary operators that this language has
        /// </summary>
        public IEnumerable<BinaryOperatorDef> BinaryOperators => this.BinaryOperatorDefs;

        /// <summary>
        /// The list of functions that this language has
        /// </summary>
        public IEnumerable<KeyValuePair<String, Delegate>> Functions => this.FunctionDefs;

        #endregion Definitions Enumeration

        public CalculatorLanguage ( )
        {
            #region Lexer Builder Init

            // Identifiers
            this.lexerBuilder.AddModule ( new IdentifierLexerModule ( ) );

            // Trivia
            this.lexerBuilder.AddRegex ( "whitespace", CalculatorTokenType.Whitespace, @"\s+", true );

            // Punctuations
            this.lexerBuilder.AddLiteral ( "(", CalculatorTokenType.LParen, "(" );
            this.lexerBuilder.AddLiteral ( ")", CalculatorTokenType.RParen, ")" );
            this.lexerBuilder.AddLiteral ( ",", CalculatorTokenType.Comma, "," );

            // Numbers
            this.lexerBuilder.AddRegex ( "bin-number", CalculatorTokenType.Number, "0b([01]+)", "0b", match =>
            {
                var conv = Convert.ToInt64 ( match.Groups[0].Value.Substring ( 2 ), 2 );
                if ( conv > Int32.MaxValue )
                    throw new Exception ( "Binary number is too large!" );
                return ( Double ) conv;
            } );

            this.lexerBuilder.AddRegex ( "oct-number", CalculatorTokenType.Number, "0o([0-7]+)", "0o", match =>
            {
                var conv = Convert.ToInt64 ( match.Groups[0].Value.Substring ( 2 ), 8 );
                if ( conv > Int32.MaxValue )
                    throw new Exception ( "Octal number is too large!" );
                return ( Double ) conv;
            } );

            this.lexerBuilder.AddRegex ( "dec-number", CalculatorTokenType.Number, @"(?:\d+(?:\.\d+)?|\.\d+)(?:e[+-]?\d+)?", match =>
            {
                return Convert.ToDouble ( match.Value );
            } );

            this.lexerBuilder.AddRegex ( "hex-number", CalculatorTokenType.Number, "0x([a-fA-F0-9]+)", "0x", match =>
            {
                var conv = Convert.ToInt64 ( match.Groups[0].Value.Substring ( 2 ), 16 );
                if ( conv > Int32.MaxValue )
                    throw new Exception ( "Hexadecimal number is too large!" );
                return ( Double ) conv;
            } );

            #endregion Lexer Builder Init
        }

        #region Constant management

        public void AddConstant ( String ident, Double val, Boolean isCaseSensitive )
        {
            if ( this.ConstantDefs.Any ( cons => cons.Identifier == ident ) )
                throw new Exception ( "Duplicated constant." );
            this.ConstantDefs.Add ( new ConstantDef ( ident, isCaseSensitive, val ) );
        }

        public Boolean HasConstant ( String id ) =>
            this.ConstantDefs.Any ( constdef => constdef.Identifier.Equals ( id, constdef.IsCaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase ) );

        public ConstantDef GetConstant ( String id ) =>
            this.ConstantDefs.FirstOrDefault ( constdef => constdef.Identifier.Equals ( id, constdef.IsCaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase ) );

        #endregion Constant management

        #region Unary operator management

        public void AddUnaryOperator ( UnaryOperatorFix fix, String @operator, Int32 precedence, Func<Double, Double> action )
        {
            if ( this.UnaryOperatorDefs.Any ( op => op.Operator == @operator && op.Fix == fix ) )
                throw new Exception ( "Duplicated operator." );

            this.AddOperator ( @operator );
            this.UnaryOperatorDefs.Add ( new UnaryOperatorDef ( fix, @operator, precedence, action ) );
        }

        public Boolean HasUnaryOperator ( String op, UnaryOperatorFix fix ) =>
            this.UnaryOperatorDefs.Any ( opdef => opdef.Operator == op && opdef.Fix == fix );

        public UnaryOperatorDef GetUnaryOperator ( String @operator, UnaryOperatorFix fix ) =>
            this.UnaryOperatorDefs.FirstOrDefault ( opdef => opdef.Operator == @operator && opdef.Fix == fix );

        #endregion Unary operator management

        #region Binary operator management

        public void AddBinaryOperator ( OperatorAssociativity associativity, String @operator, Int32 precedence, Func<Double, Double, Double> action )
        {
            if ( this.BinaryOperatorDefs.Any ( op => op.Operator == @operator ) )
                throw new Exception ( "Duplicated operator." );

            this.AddOperator ( @operator );
            this.BinaryOperatorDefs.Add ( new BinaryOperatorDef ( associativity, @operator, precedence, action ) );
        }

        public Boolean HasBinaryOperator ( String op ) =>
            this.BinaryOperatorDefs.Any ( opdef => opdef.Operator == op );

        public BinaryOperatorDef GetBinaryOperator ( String op ) =>
            this.BinaryOperatorDefs.FirstOrDefault ( opdef => opdef.Operator == op );

        #endregion Binary operator management

        #region Function management

        public void AddFunction ( String name, Func<Double> func )
        {
            if ( this.FunctionDefs.ContainsKey ( name ) )
                throw new Exception ( "Duplicate function." );
            this.FunctionDefs[name] = func;
        }

        public void AddFunction ( String name, Func<Double, Double> func )
        {
            if ( this.FunctionDefs.ContainsKey ( name ) )
                throw new Exception ( "Duplicate function." );
            this.FunctionDefs[name] = func;
        }

        public void AddFunction ( String name, Func<Double, Double, Double> func )
        {
            if ( this.FunctionDefs.ContainsKey ( name ) )
                throw new Exception ( "Duplicate function." );
            this.FunctionDefs[name] = func;
        }

        public void AddFunction ( String name, Func<Double, Double, Double, Double> func )
        {
            if ( this.FunctionDefs.ContainsKey ( name ) )
                throw new Exception ( "Duplicate function." );
            this.FunctionDefs[name] = func;
        }

        public Boolean HasFunction ( String id ) =>
            this.FunctionDefs.ContainsKey ( id );

        public Delegate GetFunction ( String id ) => this.FunctionDefs[id];

        #endregion Function management

        #region Lexer Stuff

        private readonly HashSet<String> operators = new HashSet<String> ( );

        private void AddOperator ( String raw )
        {
            if ( !this.operators.Contains ( raw ) )
            {
                this.operators.Add ( raw );
                this.lexerBuilder.AddLiteral ( raw, CalculatorTokenType.Operator, raw );
            }
        }

        public ILexer<CalculatorTokenType> GetLexer ( String expression ) =>
            this.lexerBuilder.BuildLexer ( expression );

        #endregion Lexer Stuff
    }
}
