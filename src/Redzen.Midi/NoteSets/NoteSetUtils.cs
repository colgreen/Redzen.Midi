using System;
using System.Collections.Generic;
using Redzen.Midi.Notes;

namespace Redzen.Midi.NoteSets
{
    /// <summary>
    /// NoteSet static helper methods.
    /// </summary>
    public static class NoteSetUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Create a list of standard note sets.
        /// </summary>
        /// <returns>A list of note sets.</returns>
        public static List<NoteSet> CreateDefaultNoteSets()
        {
            List<NoteSet> noteSetList = new List<NoteSet>();
            noteSetList.Add(CreateNoteSet(typeof(AkaiXR20)));
            noteSetList.Add(CreateNoteSet(typeof(NordDrum2)));
            noteSetList.Add(CreateNoteSet(typeof(Volcabeats)));
            noteSetList.Add(CreateNoteSet(typeof(Circuit)));
            noteSetList.Add(CreateNoteSet(typeof(Percussion)));
            return noteSetList;
        }

        #endregion

        #region Private Static Methods

        private static NoteSet CreateNoteSet(Type enumType)
        {
            if(!enumType.IsSubclassOf(typeof(Enum))) {
                throw new Exception("Unexpected type; not a System.Enum.");
            }

            Array arr = Enum.GetValues(enumType);
            List<int> noteList = new List<int>(arr.Length);

            foreach(int entry in arr) {
                noteList.Add(entry);
            }

            return new NoteSet(enumType.Name, noteList);
        }

        #endregion
    }
}
