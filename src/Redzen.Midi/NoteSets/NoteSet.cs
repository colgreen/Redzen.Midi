using System.Collections.Generic;

namespace Redzen.Midi.NoteSets
{
    /// <summary>
    /// Represents a named set/collection of note IDs.
    /// </summary>
    public class NoteSet
    {
        /// <summary>
        /// Name of the note set.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The notes in the note set.
        /// </summary>
        public List<int> Notes { get; private set; }

        /// <summary>
        /// Construct a new note set.
        /// </summary>
        /// <param name="name">Name of the note set.</param>
        /// <param name="notes">The notes in the note set.</param>
        public NoteSet(string name, List<int> notes)
        {
            this.Name = name;
            this.Notes = notes;
        }
    }
}
