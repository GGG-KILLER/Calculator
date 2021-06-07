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

        private readonly ImmutableDictionary<string, Constant>.Builder constantsBuilder;
        private readonly ImmutableDictionary<(UnaryOperatorFix, string), UnaryOperator>.Builder unaryOperatorsBuilder;
        private readonly ImmutableDictionary<string, BinaryOperator>.Builder binaryOperatorsBuilder;
        private readonly ImmutableDictionary<string, Function>.Builder functionsBuilder;
        private readonly ImmutableDictionary<SpecialBinaryOperatorType, SpecialBinaryOperator>.Builder specialBinaryOperatorsBuilder;

        /// <summary>
        /// Initializes a language with a specified comparer
        /// </summary>
        /// <param name="identifierComparer">The comparer used for constants, operators and function names</param>
        public CalculatorLanguageBuilder(StringComparer identifierComparer)
        {
            constantsBuilder = ImmutableDictionary.CreateBuilder<string, Constant>(identifierComparer);
            unaryOperatorsBuilder = ImmutableDictionary.CreateBuilder<(UnaryOperatorFix, string), UnaryOperator>(new UnaryOperatorKeyPairEqualityComparer(identifierComparer));
            binaryOperatorsBuilder = ImmutableDictionary.CreateBuilder<string, BinaryOperator>(identifierComparer);
            functionsBuilder = ImmutableDictionary.CreateBuilder<string, Function>(identifierComparer);
            specialBinaryOperatorsBuilder = ImmutableDictionary.CreateBuilder<SpecialBinaryOperatorType, SpecialBinaryOperator>();
        }

        /// <summary>
        /// Initializes a case-insensitve language by using the <see
        /// cref="CalculatorLanguageBuilder.CalculatorLanguageBuilder(StringComparer)"/> constructor
        /// with the <see cref="StringComparer.InvariantCultureIgnoreCase"/>
        /// </summary>
        public CalculatorLanguageBuilder() : this(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        #region Constant Management

        /// <summary>
        /// Adds a constant
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder AddConstant(string identifier, double value)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("The identifier must not be null, empty or contain whitespaces", nameof(identifier));

            constantsBuilder.Add(identifier, new Constant(identifier, value));
            return this;
        }

        /// <summary>
        /// Adds a constant with many different aliases
        /// </summary>
        /// <param name="identifiers"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder AddConstants(IEnumerable<string> identifiers, double value)
        {
            if (identifiers == null)
                throw new ArgumentNullException(nameof(identifiers));
            foreach (var alias in identifiers)
                AddConstant(alias, value);
            return this;
        }

        /// <summary>
        /// Removes the constant that matches the provided name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveConstant(string name)
        {
            constantsBuilder.Remove(name);
            return this;
        }

        /// <summary>
        /// Removes the constants that match the provided names
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveConstants(params string[] names)
        {
            constantsBuilder.RemoveRange(names);
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
        public CalculatorLanguageBuilder AddUnaryOperator(UnaryOperatorFix fix, string @operator, int precedence, Func<double, double> body)
        {
            if (fix < UnaryOperatorFix.Prefix || fix > UnaryOperatorFix.Postfix)
                throw new ArgumentOutOfRangeException(nameof(fix));
            if (string.IsNullOrWhiteSpace(@operator))
                throw new ArgumentException("message", nameof(@operator));
            if (precedence < 1)
                throw new ArgumentOutOfRangeException(nameof(precedence), "The precedence must have a value greater than 0.");
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            unaryOperatorsBuilder.Add((fix, @operator), new UnaryOperator(fix, @operator, precedence, body));
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
        public CalculatorLanguageBuilder AddUnaryOperators(UnaryOperatorFix fix, IEnumerable<string> operators, int precedence, Func<double, double> body)
        {
            if (operators == null)
                throw new ArgumentNullException(nameof(operators));
            foreach (var alias in operators)
                AddUnaryOperator(fix, alias, precedence, body);
            return this;
        }

        /// <summary>
        /// Removes the unary operator that matches the provided value
        /// </summary>
        /// <param name="fix"></param>
        /// <param name="operator"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveUnaryOperator(UnaryOperatorFix fix, string @operator)
        {
            unaryOperatorsBuilder.Remove((fix, @operator));
            return this;
        }

        /// <summary>
        /// Removes the unary operators that match the provided values
        /// </summary>
        /// <param name="fix"></param>
        /// <param name="operators"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveUnaryOperators(UnaryOperatorFix fix, params string[] operators)
        {
            unaryOperatorsBuilder.RemoveRange(operators.Select(alias => (fix, alias)));
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
        public CalculatorLanguageBuilder AddBinaryOperator(Associativity assoc, string @operator, int precedence, Func<double, double, double> body)
        {
            if (assoc < Associativity.None || Associativity.Right < assoc)
                throw new ArgumentOutOfRangeException(nameof(assoc));
            if (string.IsNullOrWhiteSpace(@operator))
                throw new ArgumentException("message", nameof(@operator));
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            binaryOperatorsBuilder.Add(@operator, new BinaryOperator(assoc, @operator, precedence, body));
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
        public CalculatorLanguageBuilder AddBinaryOperators(Associativity assoc, IEnumerable<string> operators, int precedence, Func<double, double, double> body)
        {
            if (operators == null)
                throw new ArgumentNullException(nameof(operators));
            foreach (var alias in operators)
                AddBinaryOperator(assoc, alias, precedence, body);
            return this;
        }

        /// <summary>
        /// Removes the binary operator that matches the provided value
        /// </summary>
        /// <param name="operator"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveBinaryOperator(string @operator)
        {
            binaryOperatorsBuilder.Remove(@operator);
            return this;
        }

        /// <summary>
        /// Removes the binary operators that match the provided values
        /// </summary>
        /// <param name="operators"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveBinaryOperators(params string[] operators)
        {
            binaryOperatorsBuilder.RemoveRange(operators);
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
        public CalculatorLanguageBuilder AddFunction(string name, Action<FunctionBuilder> builder)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("message", nameof(name));
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var b = new FunctionBuilder(name);
            builder(b);
            functionsBuilder.Add(name, b.GetFunctionDefinition());
            return this;
        }

        /// <summary>
        /// Adds a function with many different aliases
        /// </summary>
        /// <param name="names"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder AddFunctions(IEnumerable<string> names, Action<FunctionBuilder> builder)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));
            foreach (var name in names)
                AddFunction(name, builder);
            return this;
        }

        /// <summary>
        /// Removes the function that matches the provided name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveFunction(string name)
        {
            functionsBuilder.Remove(name);
            return this;
        }

        /// <summary>
        /// Removes the functions that match the provided names
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveFunctions(params string[] names)
        {
            functionsBuilder.RemoveRange(names);
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
        public CalculatorLanguageBuilder AddImplicitMultiplication(int precedence, Func<double, double, double> body)
        {
            specialBinaryOperatorsBuilder.Add(
                SpecialBinaryOperatorType.ImplicitMultiplication,
                new SpecialBinaryOperator(SpecialBinaryOperatorType.ImplicitMultiplication, Associativity.Left, precedence, body));
            return this;
        }

        /// <summary>
        /// Removes implicit multiplication from the language.
        /// </summary>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveImplicitMultiplication()
        {
            specialBinaryOperatorsBuilder.Remove(SpecialBinaryOperatorType.ImplicitMultiplication);
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
        public CalculatorLanguageBuilder AddSuperscriptExponentiation(int precedence, Func<double, double, double> body)
        {
            specialBinaryOperatorsBuilder.Add(
                SpecialBinaryOperatorType.SuperscriptExponentiation,
                new SpecialBinaryOperator(SpecialBinaryOperatorType.SuperscriptExponentiation, Associativity.Right, precedence, body));
            return this;
        }

        /// <summary>
        /// Removes the definition for superscript exponentiation
        /// </summary>
        /// <returns></returns>
        public CalculatorLanguageBuilder RemoveSuperscriptExponentiation()
        {
            specialBinaryOperatorsBuilder.Remove(SpecialBinaryOperatorType.SuperscriptExponentiation);
            return this;
        }

        #endregion Superscript Exponentiation

        #endregion Special Binary Operator Management

        /// <summary>
        /// Instantiates the immutable calculator language
        /// </summary>
        /// <returns></returns>
        public CalculatorLanguage ToCalculatorLanguage() =>
            new CalculatorLanguage(
                constantsBuilder.ToImmutable(),
                unaryOperatorsBuilder.ToImmutable(),
                binaryOperatorsBuilder.ToImmutable(),
                functionsBuilder.ToImmutable(),
                specialBinaryOperatorsBuilder.ToImmutable());
    }
}