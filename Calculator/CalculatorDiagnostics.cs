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
            /// Creates a <see cref="Diagnostic" /> saying that something was expected.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="expected"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpected ( SourceRange range, Object expected ) =>
                new Diagnostic ( "CALC0001", range, DiagnosticSeverity.Error, $"Syntax error, {expected} expected." );

            /// <summary>
            /// Creates a <see cref="Diagnostic" /> saying something was expected.
            /// </summary>
            /// <param name="location"></param>
            /// <param name="expected"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpected ( SourceLocation location, Object expected ) =>
                ThingExpected ( location.To ( location ), expected );

            /// <summary>
            /// Creates a <see cref="Diagnostic" /> saying something was expected for something else.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="expected"></param>
            /// <param name="for"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpectedFor ( SourceRange range, Object expected, String @for ) =>
                new Diagnostic ( "CALC0001", range, DiagnosticSeverity.Error, $"Syntax error, {expected} expected for {PunctuateIfNecessary ( @for )}" );

            /// <summary>
            /// Creates a <see cref="Diagnostic" /> saying something was expected for something else.
            /// </summary>
            /// <param name="location"></param>
            /// <param name="expected"></param>
            /// <param name="for"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpectedFor ( SourceLocation location, Object expected, String @for ) =>
                ThingExpectedFor ( location.To ( location ), expected, @for );

            /// <summary>
            /// Creates a <see cref="Diagnostic" /> saying something was expected after something else.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="expected"></param>
            /// <param name="after"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpectedAfter ( SourceRange range, Object expected, String after ) =>
                new Diagnostic ( "CALC0001", range, DiagnosticSeverity.Error, $"Syntax error, {expected} expected after {PunctuateIfNecessary ( after )}" );

            /// <summary>
            /// Creates a <see cref="Diagnostic" /> saying something was expected after something else.
            /// </summary>
            /// <param name="location"></param>
            /// <param name="expected"></param>
            /// <param name="after"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpectedAfter ( SourceLocation location, Object expected, String after ) =>
                ThingExpectedAfter ( location.To ( location ), expected, after );

            /// <summary>
            /// Produces a <see cref="Diagnostic" /> reporting an invalid superscript.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="error"></param>
            /// <returns></returns>
            public static Diagnostic InvalidSuperscript ( SourceRange range, String error ) =>
                new Diagnostic ( "CALC0002", range, DiagnosticSeverity.Error, $"Invalid superscript: {PunctuateIfNecessary ( error )}" );

            /// <summary>
            /// Produces a <see cref="Diagnostic" /> reporting an invalid number of type
            /// <paramref name="numberType" />.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="numberType"></param>
            /// <returns></returns>
            public static Diagnostic InvalidNumber ( SourceRange range, String numberType ) =>
                new Diagnostic ( "CALC0003", range, DiagnosticSeverity.Error, $"Invalid {numberType} number." );

            /// <summary>
            /// Produces a <see cref="Diagnostic" /> reporting an invalid number of type
            /// <paramref name="numberType" />.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="numberType"></param>
            /// <param name="reason"></param>
            /// <returns></returns>
            public static Diagnostic InvalidNumber ( SourceRange range, String numberType, String reason ) =>
                new Diagnostic ( "CALC0003", range, DiagnosticSeverity.Error, $"Invalid {numberType} number: {PunctuateIfNecessary ( reason )}" );
        }

        /// <summary>
        /// Formats a diagnostic retrieving the line(s) referred to by the diagnostic and inserting ^'s
        /// under the code section that the diagnostic refers to.
        /// </summary>
        /// <param name="expression">
        /// The expression that was provided to the object that generated the
        /// <paramref name="diagnostic" />
        /// </param>
        /// <param name="diagnostic">
        /// The diagnostic generated by the object that had <paramref name="expression" /> provided to it
        /// </param>
        /// <returns></returns>
        public static String FormatDiagnostic ( String expression, Diagnostic diagnostic )
        {
            SourceLocation start = diagnostic.Range.Start;
            SourceLocation end   = diagnostic.Range.End;
            var len              = Math.Max ( end.Byte - start.Byte, 1 );
            var lines            = expression.Split ( new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries );

            if ( start.Line != end.Line )
            {
                lines = lines
                    .Skip ( start.Line - 1 )
                    .Take ( Math.Max ( end.Line - start.Line, 1 ) )
                    .ToArray ( );
                var newLines = new String[lines.Length * 2];
                for ( var i = 0; i < lines.Length; i++ )
                    newLines[i * 2] = lines[i];

                newLines[1] = new String ( ' ', start.Column - 1 ) + new String ( '^', newLines[0].Length - start.Column );
                for ( var i = 3; i < newLines.Length - 2; i += 2 )
                {
                    newLines[i] = new String ( '^', newLines[i - 1].Length );
                }
                newLines[newLines.Length - 1] = new String ( '^', end.Column );
                lines = newLines;
            }
            else
            {
                var newLines = new String[2];
                newLines[0] = lines[start.Line - 1];
                newLines[1] = new String ( ' ', start.Column - 1 ) + new String ( '^', len );
                lines = newLines;
            }

            return String.Join ( Environment.NewLine, lines );
        }
    }
}
