using System;
using GParse;

namespace Calculator.UI
{
    public class DiagnosticViewModel
    {
        private readonly Diagnostic _diagnostic;

        /// <inheritdoc cref="Diagnostic.Id"/>
        public string Id => _diagnostic.Id;

        /// <inheritdoc cref="Diagnostic.Severity"/>
        public DiagnosticSeverity Severity => _diagnostic.Severity;

        /// <inheritdoc cref="Diagnostic.Description"/>
        public string Description => _diagnostic.Description;

        /// <summary>
        /// The line where the code this diagnostic refers to starts at.
        /// </summary>
        public int StartLine => _diagnostic.Range.Start.Line;

        /// <summary>
        /// The column where the code this diagnostic refers to starts at.
        /// </summary>
        public int StartColumn => _diagnostic.Range.Start.Column;

        /// <summary>
        /// The line where the code this diagnostic refers to ends at.
        /// </summary>
        public int EndLine => _diagnostic.Range.End.Line;

        /// <summary>
        /// The column where the code this diagnostic refers to ends at.
        /// </summary>
        public int EndColumn => _diagnostic.Range.End.Column;

        public DiagnosticViewModel(Diagnostic diagnostic) => _diagnostic = diagnostic ?? throw new ArgumentNullException(nameof(diagnostic));
    }
}
