using System;
using GParse;

namespace Calculator
{
    /// <summary>
    /// The class that stores the methods for all generated diagnostics
    /// </summary>
    public static class CalculatorDiagnostics
    {
        private static string PunctuateIfNecessary(string str)
        {
            if (str is null)
                return null;
            else if (str.Length == 0)
                return null;

            var last = str[str.Length - 1];
            return last switch
            {
                '.' or '?' or '!' or ';' => str,
                _ => str + '.',
            };
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
            public static Diagnostic ThingExpected(SourceRange range, object expected) =>
                new Diagnostic(DiagnosticSeverity.Error, "CALC0001", $"Syntax error, {expected} expected.", range);

            /// <summary>
            /// Creates a <see cref="Diagnostic"/> saying something was expected.
            /// </summary>
            /// <param name="location"></param>
            /// <param name="expected"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpected(SourceLocation location, object expected) =>
                ThingExpected(location.To(location), expected);

            /// <summary>
            /// Creates a <see cref="Diagnostic"/> saying something was expected for something else.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="expected"></param>
            /// <param name="for"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpectedFor(SourceRange range, object expected, string @for) =>
                new Diagnostic(DiagnosticSeverity.Error, "CALC0001", $"Syntax error, {expected} expected for {PunctuateIfNecessary(@for)}", range);

            /// <summary>
            /// Creates a <see cref="Diagnostic"/> saying something was expected for something else.
            /// </summary>
            /// <param name="location"></param>
            /// <param name="expected"></param>
            /// <param name="for"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpectedFor(SourceLocation location, object expected, string @for) =>
                ThingExpectedFor(location.To(location), expected, @for);

            /// <summary>
            /// Creates a <see cref="Diagnostic"/> saying something was expected after something else.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="expected"></param>
            /// <param name="after"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpectedAfter(SourceRange range, object expected, string after) =>
                new Diagnostic(DiagnosticSeverity.Error, "CALC0001", $"Syntax error, {expected} expected after {PunctuateIfNecessary(after)}", range);

            /// <summary>
            /// Creates a <see cref="Diagnostic"/> saying something was expected after something else.
            /// </summary>
            /// <param name="location"></param>
            /// <param name="expected"></param>
            /// <param name="after"></param>
            /// <returns></returns>
            public static Diagnostic ThingExpectedAfter(SourceLocation location, object expected, string after) =>
                ThingExpectedAfter(location.To(location), expected, after);

            /// <summary>
            /// Produces a <see cref="Diagnostic"/> reporting an invalid superscript.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="error"></param>
            /// <returns></returns>
            public static Diagnostic InvalidSuperscript(SourceRange range, string error) =>
                new Diagnostic(DiagnosticSeverity.Error, "CALC0002", $"Invalid superscript: {PunctuateIfNecessary(error)}", range);

            /// <summary>
            /// Produces a <see cref="Diagnostic"/> reporting an invalid number of type <paramref name="numberType"/>.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="numberType"></param>
            /// <returns></returns>
            public static Diagnostic InvalidNumber(SourceRange range, string numberType) =>
                new Diagnostic(DiagnosticSeverity.Error, "CALC0003", $"Invalid {numberType} number.", range);

            /// <summary>
            /// Produces a <see cref="Diagnostic"/> reporting an invalid number of type <paramref name="numberType"/>.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="numberType"></param>
            /// <param name="reason"></param>
            /// <returns></returns>
            public static Diagnostic InvalidNumber(SourceRange range, string numberType, string reason) =>
                new Diagnostic(DiagnosticSeverity.Error, "CALC0003", $"Invalid {numberType} number: {PunctuateIfNecessary(reason)}", range);

            /// <summary>
            /// Produces a <see cref="Diagnostic"/> reporting an unknown character.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="ch"></param>
            /// <returns></returns>
            public static Diagnostic UnknownCharacter(SourceRange range, char ch) =>
                new Diagnostic(DiagnosticSeverity.Error, "CALC0004", $"Unknown character '{ch}'.", range);
        }

        /// <summary>
        /// Highlights a range retrieving the line(s) referred to by the range and inserting ^'s
        /// under the code section that the range refers to.
        /// </summary>
        /// <param name="expression">The expression to highlight</param>
        /// <param name="range">The range to highlight</param>
        /// <returns></returns>
        public static string HighlightRange(string expression, SourceRange range)
        {
            if (string.IsNullOrEmpty(expression))
                throw new ArgumentException("The expression should not be null or empty.", nameof(expression));

            var start = range.Start;
            var end = range.End;
            var lines = expression.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (start.Line != end.Line)
            {
                var builder = new System.Text.StringBuilder();
                var startLine = start.Line;
                var endLine = end.Line - 1;

                for (var i = startLine; i <= endLine; i++)
                {
                    var line = lines[i];
                    var lineLength = line.Length;

                    builder.AppendLine(line)
                           .AppendLine(i switch
                           {
                               _ when i == startLine => new string(' ', Math.Max(start.Column - 1, 0))
                                                        + new string('^', Math.Max(lineLength - start.Column, 0)),
                               _ when i == endLine => new string('^', end.Column),
                               _ => new string('^', lineLength)
                           });
                }

                return builder.ToString();
            }

            var len = Math.Max(end.Byte - start.Byte, 1);
            return string.Join(Environment.NewLine,
                                lines[start.Line - 1],
                                new string(' ', start.Column - 1) + new string('^', len));
        }
    }
}