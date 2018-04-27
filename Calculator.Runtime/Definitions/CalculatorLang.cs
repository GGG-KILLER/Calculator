using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator.Runtime.Definitions
{
    public class CalculatorLang
    {
        /// <summary>
        /// Name of this language
        /// </summary>
        public readonly String Name;

        /// <summary>
        /// Version of this language
        /// </summary>
        public readonly Version Version;

        /// <summary>
        /// The list of binary operators that this language has
        /// </summary>
        private readonly List<BinaryOperatorDef> BinaryOperatorDefs = new List<BinaryOperatorDef> ( );

        /// <summary>
        /// The list of the constants that this language has
        /// </summary>
        private readonly List<ConstantDef> ConstantDefs = new List<ConstantDef> ( );

        /// <summary>
        /// The list of functions that this language has
        /// </summary>
        private readonly Dictionary<String, Delegate> FunctionDefs = new Dictionary<String, Delegate> ( );

        /// <summary>
        /// The list of unary operators that this language has
        /// </summary>
        private readonly List<UnaryOperatorDef> UnaryOperatorDefs = new List<UnaryOperatorDef> ( );

        public CalculatorLang ( String Name, Version Version )
        {
            this.Name = Name;
            this.Version = Version;
        }

        /// <summary>
        /// The list of binary operators that this language has
        /// </summary>
        public IEnumerable<BinaryOperatorDef> BinaryOperators => this.BinaryOperatorDefs.AsReadOnly ( );

        /// <summary>
        /// The list of the constants that this language has
        /// </summary>
        public IEnumerable<ConstantDef> Constants => this.ConstantDefs.AsReadOnly ( );

        /// <summary>
        /// The list of functions that this language has
        /// </summary>
        public IEnumerable<KeyValuePair<String, Delegate>> Functions => this.FunctionDefs;

        /// <summary>
        /// The list of unary operators that this language has
        /// </summary>
        public IEnumerable<UnaryOperatorDef> UnaryOperators => this.UnaryOperatorDefs.AsReadOnly ( );

        public void AddBinaryOperator ( OperatorAssociativity associativity, String @operator, Int32 precedence, Func<Double, Double, Double> action )
        {
            if ( this.BinaryOperatorDefs.Any ( op => op.Operator == @operator ) )
                throw new Exception ( "Duplicated operator." );
            this.BinaryOperatorDefs.Add ( new BinaryOperatorDef ( associativity, @operator, precedence, action ) );
        }

        public void AddConstant ( String Identifier, Double Value )
        {
            if ( this.ConstantDefs.Any ( cons => cons.Identifier == Identifier ) )
                throw new Exception ( "Duplicated constant." );
            this.ConstantDefs.Add ( new ConstantDef
            {
                Identifier = Identifier,
                Value = Value
            } );
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

        public void AddFunction ( String name, Func<Double[], Double> func )
        {
            if ( this.FunctionDefs.ContainsKey ( name ) )
                throw new Exception ( "Duplicate function." );
            this.FunctionDefs[name] = func;
        }

        public void AddUnaryOperator ( UnaryOperatorFix fix, String @operator, Int32 precedence, Func<Double, Double> action )
        {
            if ( this.UnaryOperatorDefs.Any ( op => op.Operator == @operator && op.Fix == fix ) )
                throw new Exception ( "Duplicated operator." );
            this.UnaryOperatorDefs.Add ( new UnaryOperatorDef ( fix, @operator, precedence, action ) );
        }

        public BinaryOperatorDef GetBinaryOperator ( String op ) =>
            this.BinaryOperatorDefs.FirstOrDefault ( opdef => opdef.Operator == op );

        public ConstantDef GetConstant ( String id ) =>
            this.ConstantDefs.FirstOrDefault ( constdef => constdef.Identifier == id );

        public Delegate GetFunction ( String id ) =>
            this.FunctionDefs.FirstOrDefault ( funcdef => funcdef.Key == id ).Value;

        public UnaryOperatorDef GetUnaryOperator ( String op ) =>
            this.UnaryOperatorDefs.FirstOrDefault ( opdef => opdef.Operator == op );

        public UnaryOperatorDef GetUnaryOperator ( String @operator, UnaryOperatorFix fix ) =>
            this.UnaryOperatorDefs.FirstOrDefault ( opdef => opdef.Operator == @operator && opdef.Fix == fix );

        public Boolean HasBinaryOperator ( String op ) =>
            this.BinaryOperatorDefs.Any ( opdef => opdef.Operator == op );

        public Boolean HasConstant ( String id ) =>
            this.ConstantDefs.Any ( constdef => constdef.Identifier == id );

        public Boolean HasFunction ( String id ) =>
            this.FunctionDefs.ContainsKey ( id );

        public Boolean HasUnaryOperator ( String op ) =>
            this.UnaryOperatorDefs.Any ( opdef => opdef.Operator == op );
    }
}
