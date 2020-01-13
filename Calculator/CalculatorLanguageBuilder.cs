using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Calculator.Definitions;

namespace Calculator
{
    /// <summary>
    /// A <see cref="CalculatorLanguage"/> builder
    /// </summary>
    public class CalculatorLanguageBuilder
    {

        private readonly ImmutableDictionary<String, Constant>.Builder constantsBuilder;
        private readonly ImmutableDictionary<(UnaryOperatorFix, String), UnaryOperator>.Builder unaryOperatorsBuilder;
        private readonly ImmutableDictionary<String, BinaryOperator>.Builder binaryOperatorsBuilder;
        private readonly ImmutableDictionary<String, Function>.Builder functionsBuilder;
        private readonly ImmutableDictionary<SpecialBinaryOperatorType, SpecialBinaryOperator>.Builder specialBinaryOperatorsBuilder;

        /// <summary>
        /// Initializes a language with a specified comparer
        /// </summary>
        /// <param name="identifierComparer">The comparer used for constants, operators and function names</param>
        public CalculatorLanguageBuilder ( StringComparer identifierComparer )
        {
            this.constantsBuilder = ImmutableDictionary.CreateBuilder<String, Constant> ( identifierComparer );
            this.unaryOperatorsBuilder = ImmutableDictionary.CreateBuilder<(UnaryOperatorFix, String), UnaryOperator> ( new UnaryOperatorKeyPairEqualityComparer ( identifierComparer ) );
            this.binaryOperatorsBuilder = ImmutableDictionary.CreateBuilder<String, BinaryOperator> ( identifierComparer );
            this.functionsBuilder = ImmutableDictionary.CreateBuilder<String, Function> ( identifierComparer );
            this.specialBinaryOperatorsBuilder = ImmutableDictionary.CreateBuilder<SpecialBinaryOperatorType, SpecialBinaryOperator> ( );
        }

        /// <summary>
        /// Initializes a case-insensitve language by using the <see
        /// cref="CalculatorLanguageBuilder.CalculatorLanguageBuilder(StringComparer)"/> constructor
        /// with the <see cref="StringComparer.InvariantCultureIgnoreCase"/>
        /// </summary>
        public CalculatorLanguageBuilder ( ) : this ( StringComparer.InvariantCultureIgnoreCase )
        {
        }

        #region Constant Management

        /// <summary>
        /// Adds a constant
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder AddConstant ( String identifier, Double value )
        {
            if ( String.IsNullOrWhiteSpace ( identifier ) )
                throw new ArgumentException ( "The identifier must not be null, empty or contain whitespaces", nameof ( identifier ) );

            this.constantsBuilder.Add ( identifier, new Constant ( identifier, value ) );
            return this;
        }

        /// <summary>
        /// Adds a constant with many different aliases
        /// </summary>
        /// <param name="identifiers"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder AddConstants ( IEnumerable<String> identifiers, Double value )
        {
            if ( identifiers == null )
                throw new ArgumentNullException ( nameof ( identifiers ) );
            foreach ( var alias in identifiers )
                this.AddConstant ( alias, value );
            return this;
        }

        /// <summary>
        /// Removes the constant that matches the provided name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveConstant ( String name )
        {
            this.constantsBuilder.Remove ( name );
            return this;
        }

        /// <summary>
        /// Removes the constants that match the provided names
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveConstants ( params String[] names )
        {
            this.constantsBuilder.RemoveRange ( names );
            return this;
        }

        #endregion Constant Management

        #region Unary Operator Management

        /// <summary>
        /// Adds an unary operator
        /// </summary>
        /// <param name="fix"></param>
        /// <param name="operator"></param>
        /// <param name="precedence"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder AddUnaryOperator ( UnaryOperatorFix fix, String @operator, Int32 precedence, Func<Double, Double> body )
        {
            if ( fix < UnaryOperatorFix.Prefix || fix > UnaryOperatorFix.Postfix )
                throw new ArgumentOutOfRangeException ( nameof ( fix ) );
            if ( String.IsNullOrWhiteSpace ( @operator ) )
                throw new ArgumentException ( "message", nameof ( @operator ) );
            if ( precedence < 1 )
                throw new ArgumentOutOfRangeException ( nameof ( precedence ), "The precedence must have a value greater than 0." );
            if ( body == null )
                throw new ArgumentNullException ( nameof ( body ) );

            this.unaryOperatorsBuilder.Add ( (fix, @operator), new UnaryOperator ( fix, @operator, precedence, body ) );
            return this;
        }

        /// <summary>
        /// Adds an unary operator with many different aliases
        /// </summary>
        /// <param name="fix"></param>
        /// <param name="operators"></param>
        /// <param name="precedence"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder AddUnaryOperators ( UnaryOperatorFix fix, IEnumerable<String> operators, Int32 precedence, Func<Double, Double> body )
        {
            if ( operators == null )
                throw new ArgumentNullException ( nameof ( operators ) );
            foreach ( var alias in operators )
                this.AddUnaryOperator ( fix, alias, precedence, body );
            return this;
        }

        /// <summary>
        /// Removes the unary operator that matches the provided value
        /// </summary>
        /// <param name="fix"></param>
        /// <param name="operator"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveUnaryOperator ( UnaryOperatorFix fix, String @operator )
        {
            this.unaryOperatorsBuilder.Remove ( (fix, @operator) );
            return this;
        }

        /// <summary>
        /// Removes the unary operators that match the provided values
        /// </summary>
        /// <param name="fix"></param>
        /// <param name="operators"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveUnaryOperators ( UnaryOperatorFix fix, params String[] operators )
        {
            this.unaryOperatorsBuilder.RemoveRange ( operators.Select ( alias => (fix, alias) ) );
            return this;
        }

        #endregion Unary Operator Management

        #region Binary Operator Management

        /// <summary>
        /// Adds a binary operator
        /// </summary>
        /// <param name="assoc"></param>
        /// <param name="operator"></param>
        /// <param name="precedence"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder AddBinaryOperator ( Associativity assoc, String @operator, Int32 precedence, Func<Double, Double, Double> body )
        {
            if ( assoc < Associativity.None || Associativity.Right < assoc )
                throw new ArgumentOutOfRangeException ( nameof ( assoc ) );
            if ( String.IsNullOrWhiteSpace ( @operator ) )
                throw new ArgumentException ( "message", nameof ( @operator ) );
            if ( body == null )
                throw new ArgumentNullException ( nameof ( body ) );

            this.binaryOperatorsBuilder.Add ( @operator, new BinaryOperator ( assoc, @operator, precedence, body ) );
            return this;
        }

        /// <summary>
        /// Adds a binary operator with many different aliases
        /// </summary>
        /// <param name="assoc"></param>
        /// <param name="operators"></param>
        /// <param name="precedence"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder AddBinaryOperators ( Associativity assoc, IEnumerable<String> operators, Int32 precedence, Func<Double, Double, Double> body )
        {
            if ( operators == null )
                throw new ArgumentNullException ( nameof ( operators ) );
            foreach ( var alias in operators )
                this.AddBinaryOperator ( assoc, alias, precedence, body );
            return this;
        }

        /// <summary>
        /// Removes the binary operator that matches the provided value
        /// </summary>
        /// <param name="operator"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveBinaryOperator ( String @operator )
        {
            this.binaryOperatorsBuilder.Remove ( @operator );
            return this;
        }

        /// <summary>
        /// Removes the binary operators that match the provided values
        /// </summary>
        /// <param name="operators"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveBinaryOperators ( params String[] operators )
        {
            this.binaryOperatorsBuilder.RemoveRange ( operators );
            return this;
        }

        #endregion Binary Operator Management

        #region Function Management

        /// <summary>
        /// Adds a function
        /// </summary>
        /// <param name="name"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder AddFunction ( String name, Action<FunctionBuilder> builder )
        {
            if ( String.IsNullOrWhiteSpace ( name ) )
                throw new ArgumentException ( "message", nameof ( name ) );
            if ( builder == null )
                throw new ArgumentNullException ( nameof ( builder ) );

            var b = new FunctionBuilder ( name );
            builder ( b );
            this.functionsBuilder.Add ( name, b.GetFunctionDefinition ( ) );
            return this;
        }

        /// <summary>
        /// Adds a function with many different aliases
        /// </summary>
        /// <param name="names"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder AddFunctions ( IEnumerable<String> names, Action<FunctionBuilder> builder )
        {
            if ( names == null )
                throw new ArgumentNullException ( nameof ( names ) );
            foreach ( var name in names )
                this.AddFunction ( name, builder );
            return this;
        }

        /// <summary>
        /// Removes the function that matches the provided name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveFunction ( String name )
        {
            this.functionsBuilder.Remove ( name );
            return this;
        }

        /// <summary>
        /// Removes the functions that match the provided names
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveFunctions ( params String[] names )
        {
            this.functionsBuilder.RemoveRange ( names );
            return this;
        }

        #endregion Function Management

        #region Special Binary Operator Management

        #region Implicit Multiplication

        /// <summary>
        /// Adds implicit multiplication to the language. It's recommended that the precedence be
        /// higher than division.
        /// </summary>
        /// <param name="precedence"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder AddImplicitMultiplication ( Int32 precedence, Func<Double, Double, Double> body )
        {
            this.specialBinaryOperatorsBuilder.Add (
                SpecialBinaryOperatorType.ImplicitMultiplication,
                new SpecialBinaryOperator ( SpecialBinaryOperatorType.ImplicitMultiplication, Associativity.Left, precedence, body ) );
            return this;
        }

        /// <summary>
        /// Removes implicit multiplication from the language.
        /// </summary>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveImplicitMultiplication ( )
        {
            this.specialBinaryOperatorsBuilder.Remove ( SpecialBinaryOperatorType.ImplicitMultiplication );
            return this;
        }

        #endregion Implicit Multiplication

        #region Superscript Exponentiation

        /// <summary>
        /// Adds the definition for superscript exponentiation
        /// </summary>
        /// <param name="precedence"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder AddSuperscriptExponentiation ( Int32 precedence, Func<Double, Double, Double> body )
        {
            this.specialBinaryOperatorsBuilder.Add (
                SpecialBinaryOperatorType.SuperscriptExponentiation,
                new SpecialBinaryOperator ( SpecialBinaryOperatorType.SuperscriptExponentiation, Associativity.Right, precedence, body ) );
            return this;
        }

        /// <summary>
        /// Removes the definition for superscript exponentiation
        /// </summary>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveSuperscriptExponentiation ( )
        {
            this.specialBinaryOperatorsBuilder.Remove ( SpecialBinaryOperatorType.SuperscriptExponentiation );
            return this;
        }

        #endregion Superscript Exponentiation

        #endregion Special Binary Operator Management

        /// <summary>
        /// Instantiates the immutable calculator language
        /// </summary>
        /// <returns></returns>
        public CalculatorLanguage ToCalculatorLanguage ( ) =>
            new CalculatorLanguage (
                this.constantsBuilder.ToImmutable ( ),
                this.unaryOperatorsBuilder.ToImmutable ( ),
                this.binaryOperatorsBuilder.ToImmutable ( ),
                this.functionsBuilder.ToImmutable ( ),
                this.specialBinaryOperatorsBuilder.ToImmutable ( ) );
    }
}