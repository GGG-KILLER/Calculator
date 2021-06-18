using System;
using GParse;

namespace Calculator.UI
{
    public class DiagnosticViewModel
    {
        private readonly Diagnostic _diagnostic;
        private readonly SourceRange _range;

        /// <inheritdoc cref="Diagnostic.Id"/>
        public string Id => _diagnostic.Id;

        /// <inheritdoc cref="Diagnostic.Severity"/>
        public DiagnosticSeverity Severity => _diagnostic.Severity;

        /// <inheritdoc cref="Diagnostic.Description"/>
        public string Description => _diagnostic.Description;

        /// <summary>
        /// The line where the code this diagnostic refers to starts at.
        /// </summary>
        public int StartLine => _range.Start.Line;

        /// <summary>
        /// The column where the code this diagnostic refers to starts at.
        /// </summary>
        public int StartColumn => _range.Start.Column;

        /// <summary>
        /// The line where the code this diagnostic refers to ends at.
        /// </summary>
        public int EndLine => _range.End.Line;

        /// <summary>
        /// The column where the code this diagnostic refers to ends at.
        /// </summary>
        public int EndColumn => _range.End.Column;

        public DiagnosticViewModel(Diagnostic diagnostic, SourceRange range)
        {
            _diagnostic = diagnostic ?? throw new ArgumentNullException(nameof(diagnostic));
            _range = range ?? throw new ArgumentNullException(nameof(range));
        }
    }
}