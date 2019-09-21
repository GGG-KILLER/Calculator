using System;
using System.Linq;
using GParse;

namespace Calculator
{
    /// <summary>
    /// The class that stores the methods for all generated diagnostics
    /// </summary>
    public static class CalculatorDiagnostics
    {
        private static String PunctuateIfNecessary ( String str )
        {
            if ( str is null )
                return null;
            else if ( str.Length == 0 )
                return null;

            var last = str[str.Length - 1];
            switch ( last )
            {
                case '.':
                case '?':
                case '!':
                case ';':
                    return str;

                default:
                    return str + '.';
            }
        }

        /// <summary>
        /// The class that stores the methods for all generated syntax error diagnostics
        /// </summary>
        public static class SyntaxError
        {
            /// <summary>
            /// Creates a <see cref="Diagnostic"/> saying that something was expected.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="expected"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpected ( SourceRange range, Object expected ) =>
                new Diagnostic ( "CALC0001", range, DiagnosticSeverity.Error, $"Syntax error, {expected} expected." );

            /// <summary>
            /// Creates a <see cref="Diagnostic"/> saying something was expected.
            /// </summary>
            /// <param name="location"></param>
            /// <param name="expected"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpected ( SourceLocation location, Object expected ) =>
                ThingExpected ( location.To ( location ), expected );

            /// <summary>
            /// Creates a <see cref="Diagnostic"/> saying something was expected for something else.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="expected"></param>
            /// <param name="for"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpectedFor ( SourceRange range, Object expected, String @for ) =>
                new Diagnostic ( "CALC0001", range, DiagnosticSeverity.Error, $"Syntax error, {expected} expected for {PunctuateIfNecessary ( @for )}" );

            /// <summary>
            /// Creates a <see cref="Diagnostic"/> saying something was expected for something else.
            /// </summary>
            /// <param name="location"></param>
            /// <param name="expected"></param>
            /// <param name="for"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpectedFor ( SourceLocation location, Object expected, String @for ) =>
                ThingExpectedFor ( location.To ( location ), expected, @for );

            /// <summary>
            /// Creates a <see cref="Diagnostic"/> saying something was expected after something else.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="expected"></param>
            /// <param name="after"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpectedAfter ( SourceRange range, Object expected, String after ) =>
                new Diagnostic ( "CALC0001", range, DiagnosticSeverity.Error, $"Syntax error, {expected} expected after {PunctuateIfNecessary ( after )}" );

            /// <summary>
            /// Creates a <see cref="Diagnostic"/> saying something was expected after something else.
            /// </summary>
            /// <param name="location"></param>
            /// <param name="expected"></param>
            /// <param name="after"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpectedAfter ( SourceLocation location, Object expected, String after ) =>
                ThingExpectedAfter ( location.To ( location ), expected, after );

            /// <summary>
            /// Produces a <see cref="Diagnostic"/> reporting an invalid superscript.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="error"></param>
            /// <returns></returns>
            public static Diagnostic InvalidSuperscript ( SourceRange range, String error ) =>
                new Diagnostic ( "CALC0002", range, DiagnosticSeverity.Error, $"Invalid superscript: {PunctuateIfNecessary ( error )}" );

            /// <summary>
            /// Produces a <see cref="Diagnostic"/> reporting an invalid number of type <paramref name="numberType"/>.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="numberType"></param>
            /// <returns></returns>
            public static Diagnostic InvalidNumber ( SourceRange range, String numberType ) =>
                new Diagnostic ( "CALC0003", range, DiagnosticSeverity.Error, $"Invalid {numberType} number." );

            /// <summary>
            /// Produces a <see cref="Diagnostic"/> reporting an invalid number of type <paramref name="numberType"/>.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="numberType"></param>
            /// <param name="reason"></param>
            /// <returns></returns>
            public static Diagnostic InvalidNumber ( SourceRange range, String numberType, String reason ) =>
                new Diagnostic ( "CALC0003", range, DiagnosticSeverity.Error, $"Invalid {numberType} number: {PunctuateIfNecessary ( reason )}" );
        }

        /// <summary>
        /// Highlights a range retrieving the line(s) referred to by the range and inserting ^'s
        /// under the code section that the range refers to.
        /// </summary>
        /// <param name="expression">The expression to highlight</param>
        /// <param name="range">The range to highlight</param>
        /// <returns></returns>
        public static String HighlightRange ( String expression, SourceRange range )
        {
            SourceLocation start = range.Start;
            SourceLocation end   = range.End;
            var lines            = expression.Split ( new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries );

            if ( start.Line != end.Line )
            {
                var builder = new System.Text.StringBuilder ( );
                var startLine = start.Line;
                var endLine = end.Line - 1;

                for ( var i = startLine; i <= endLine; i++ )
                {
                    var line = lines[i];
                    var lineLength = line.Length;

                    builder.AppendLine ( line )
                           .AppendLine ( i switch
                           {
                               _ when i == startLine => new String ( ' ', Math.Max ( start.Column - 1, 0 ) )
                                                        + new String ( '^', Math.Max ( lineLength - start.Column, 0 ) ),
                               _ when i == endLine => new String ( '^', end.Column ),
                               _ => new String ( '^', lineLength )
                           } );
                }

                return builder.ToString ( );
            }

            var len              = Math.Max ( end.Byte - start.Byte, 1 );
            return String.Join ( Environment.NewLine,
                                lines[start.Line - 1],
                                new String ( ' ', start.Column - 1 ) + new String ( '^', len ) );
        }
    }
}