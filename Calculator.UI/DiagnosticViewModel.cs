using System;
using GParse;

namespace Calculator.UI
{
    public class DiagnosticViewModel
    {
        private readonly Diagnostic _diagnostic;

        /// <inheritdoc cref="Diagnostic.Id"/>
        public String Id => this._diagnostic.Id;

        /// <inheritdoc cref="Diagnostic.Severity"/>
        public DiagnosticSeverity Severity => this._diagnostic.Severity;

        /// <inheritdoc cref="Diagnostic.Description"/>
        public String Description => this._diagnostic.Description;

        /// <summary>
        /// The line where the code this diagnostic refers to starts at.
        /// </summary>
        public Int32 StartLine => this._diagnostic.Range.Start.Line;

        /// <summary>
        /// The column where the code this diagnostic refers to starts at.
        /// </summary>
        public Int32 StartColumn => this._diagnostic.Range.Start.Column;

        /// <summary>
        /// The line where the code this diagnostic refers to ends at.
        /// </summary>
        public Int32 EndLine => this._diagnostic.Range.End.Line;

        /// <summary>
        /// The column where the code this diagnostic refers to ends at.
        /// </summary>
        public Int32 EndColumn => this._diagnostic.Range.End.Column;

        public DiagnosticViewModel ( Diagnostic diagnostic )
        {
            this._diagnostic = diagnostic ?? throw new ArgumentNullException ( nameof ( diagnostic ) );
        }
    }
}
