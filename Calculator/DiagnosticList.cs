using System;
using System.Collections;
using System.Collections.Generic;
using GParse;

namespace Calculator
{
    /// <summary>
    /// A class that acts as a <see cref="Diagnostic"/><see cref="List{T}"/> and also as a <see
    /// cref="IProgress{T}"/>. Used as the diagnosticReporter in the parser and lexer by default.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage ( "Naming", "CA1710:Identifiers should have correct suffix", Justification = "It has the correct suffix." )]
    public class DiagnosticList : IReadOnlyList<Diagnostic>, IProgress<Diagnostic>
    {
        private List<Diagnostic> Diagnostics { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Int32 Count => ( ( IReadOnlyList<Diagnostic> ) this.Diagnostics ).Count;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Diagnostic this[Int32 index] => ( ( IReadOnlyList<Diagnostic> ) this.Diagnostics )[index];

        /// <summary>
        /// Initializes this <see cref="DiagnosticList"/>
        /// </summary>
        public DiagnosticList ( )
        {
            this.Diagnostics = new List<Diagnostic> ( );
        }

        /// <summary>
        /// Reports a diagnostic
        /// </summary>
        /// <param name="item"></param>
        public void Report ( Diagnostic item ) =>
            this.Diagnostics.Add ( item );

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Diagnostic> GetEnumerator ( ) => ( ( IReadOnlyList<Diagnostic> ) this.Diagnostics ).GetEnumerator ( );

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator ( ) => ( ( IReadOnlyList<Diagnostic> ) this.Diagnostics ).GetEnumerator ( );
    }
}