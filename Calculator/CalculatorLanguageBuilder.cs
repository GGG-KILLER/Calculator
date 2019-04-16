using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Calculator.Definitions;

namespace Calculator
{
    /// <summary>
    /// A <see cref="CalculatorLanguage" /> builder
    /// </summary>
    public class CalculatorLanguageBuilder
    {
        private readonly ImmutableDictionary<String, Constant>.Builder constantsBuilder = ImmutableDictionary.CreateBuilder<String, Constant> ( );
        private readonly ImmutableDictionary<(UnaryOperatorFix, String), UnaryOperator>.Builder unaryOperatorsBuilder = ImmutableDictionary.CreateBuilder<(UnaryOperatorFix, String), UnaryOperator> ( );
        private readonly ImmutableDictionary<String, BinaryOperator>.Builder binaryOperatorsBuilder = ImmutableDictionary.CreateBuilder<String, BinaryOperator> ( );
        private readonly ImmutableDictionary<String, Function>.Builder functionsBuilder = ImmutableDictionary.CreateBuilder<String, Function> ( );

        #region Constant Management

        /// <summary>
        /// Adds a constant
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder SetConstant ( String identifier, Double value )
        {
            if ( String.IsNullOrWhiteSpace ( identifier ) )
                throw new ArgumentException ( "message", nameof ( identifier ) );

            this.constantsBuilder[identifier.ToLower ( )] = new Constant ( identifier.ToLower ( ), value );
            return this;
        }

        /// <summary>
        /// Adds a constant with many different aliases
        /// </summary>
        /// <param name="identifiers"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder SetConstant ( IEnumerable<String> identifiers, Double value )
        {
            if ( identifiers == null )
                throw new ArgumentNullException ( nameof ( identifiers ) );
            foreach ( var alias in identifiers )
                this.SetConstant ( alias, value );
            return this;
        }

        /// <summary>
        /// Removes the constant that matches the provided name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveConstant ( String name )
        {
            this.constantsBuilder.Remove ( name.ToLower ( ) );
            return this;
        }

        /// <summary>
        /// Removes the constants that match the provided names
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveConstants ( params String[] names )
        {
            this.constantsBuilder.RemoveRange ( names.Select ( n => n.ToLower ( ) ) );
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
        public CalculatorLanguageBuilder SetUnaryOperator ( UnaryOperatorFix fix, String @operator, Int32 precedence, Func<Double, Double> body )
        {
            if ( fix < UnaryOperatorFix.Prefix || fix > UnaryOperatorFix.Postfix )
                throw new ArgumentOutOfRangeException ( nameof ( fix ) );
            if ( String.IsNullOrWhiteSpace ( @operator ) )
                throw new ArgumentException ( "message", nameof ( @operator ) );
            if ( precedence < 1 )
                throw new ArgumentOutOfRangeException ( nameof ( precedence ), "The precedence must have a value greater than 0." );
            if ( body == null )
                throw new ArgumentNullException ( nameof ( body ) );

            this.unaryOperatorsBuilder[(fix, @operator.ToLower ( ))] = new UnaryOperator ( fix, @operator.ToLower ( ), precedence, body );
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
        public CalculatorLanguageBuilder SetUnaryOperator ( UnaryOperatorFix fix, IEnumerable<String> operators, Int32 precedence, Func<Double, Double> body )
        {
            if ( operators == null )
                throw new ArgumentNullException ( nameof ( operators ) );
            foreach ( var alias in operators )
                this.SetUnaryOperator ( fix, alias, precedence, body );
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
            this.unaryOperatorsBuilder.Remove ( (fix, @operator.ToLower ( )) );
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
            this.unaryOperatorsBuilder.RemoveRange ( operators.Select ( alias => (fix, alias.ToLower ( )) ) );
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
        public CalculatorLanguageBuilder SetBinaryOperator ( OperatorAssociativity assoc, String @operator, Int32 precedence, Func<Double, Double, Double> body )
        {
            if ( assoc < OperatorAssociativity.None || OperatorAssociativity.Right < assoc )
                throw new ArgumentOutOfRangeException ( nameof ( assoc ) );
            if ( String.IsNullOrWhiteSpace ( @operator ) )
                throw new ArgumentException ( "message", nameof ( @operator ) );
            if ( body == null )
                throw new ArgumentNullException ( nameof ( body ) );

            this.binaryOperatorsBuilder[@operator.ToLower ( )] = new BinaryOperator ( assoc, @operator.ToLower ( ), precedence, body );
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
        public CalculatorLanguageBuilder SetBinaryOperator ( OperatorAssociativity assoc, IEnumerable<String> operators, Int32 precedence, Func<Double, Double, Double> body )
        {
            if ( operators == null )
                throw new ArgumentNullException ( nameof ( operators ) );
            foreach ( var alias in operators )
                this.SetBinaryOperator ( assoc, alias, precedence, body );
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
        public CalculatorLanguageBuilder RemoveBinaryOperator ( params String[] operators )
        {
            this.binaryOperatorsBuilder.RemoveRange ( operators.Select ( n => n.ToLower ( ) ) );
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
        public CalculatorLanguageBuilder SetFunction ( String name, Action<FunctionBuilder> builder )
        {
            if ( String.IsNullOrWhiteSpace ( name ) )
                throw new ArgumentException ( "message", nameof ( name ) );
            if ( builder == null )
                throw new ArgumentNullException ( nameof ( builder ) );

            var b = new FunctionBuilder ( name.ToLower ( ) );
            builder ( b );
            this.functionsBuilder[name.ToLower ( )] = b.GetFunctionDefinition ( );
            return this;
        }

        /// <summary>
        /// Adds a function with many different aliases
        /// </summary>
        /// <param name="names"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder SetFunction ( IEnumerable<String> names, Action<FunctionBuilder> builder )
        {
            if ( names == null )
                throw new ArgumentNullException ( nameof ( names ) );
            foreach ( var name in names )
                this.SetFunction ( name, builder );
            return this;
        }

        /// <summary>
        /// Removes the function that matches the provided name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveFunction ( String name )
        {
            this.functionsBuilder.Remove ( name.ToLower ( ) );
            return this;
        }

        /// <summary>
        /// Removes the functions that match the provided names
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveFunctions ( params String[] names )
        {
            this.functionsBuilder.RemoveRange ( names.Select ( n => n.ToLower ( ) ) ); 
            return this;
        }

        #endregion Function Management

        /// <summary>
        /// Instantiates the immutable calculator language
        /// </summary>
        /// <returns></returns>
        public CalculatorLanguage GetCalculatorLanguage ( ) =>
            new CalculatorLanguage (
                this.constantsBuilder.ToImmutable ( ),
                this.unaryOperatorsBuilder.ToImmutable ( ),
                this.binaryOperatorsBuilder.ToImmutable ( ),
                this.functionsBuilder.ToImmutable ( ) );
    }
}
