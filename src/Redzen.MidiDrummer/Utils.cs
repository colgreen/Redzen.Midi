using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redzen.Midi.NoteSets;

namespace RedzenMidiDrummer
{
    public static class Utils
    {
        public static int[] ParseSequenceDescriptorAsArray(string desc, int seqCount)
        {
            // Parse the descriptor.
            HashSet<int> idxSet = ParseSequenceDescriptor(desc, seqCount);

            // Convert the set based result to an array of sequence indexes.
            int[] arr = idxSet.ToArray();
            Array.Sort(arr);
            return arr;
        }

        public static HashSet<int> ParseSequenceDescriptor(string desc, int seqCount)
        {
            HashSet<int> idxSet = new HashSet<int>();

            string[] fields = desc.ToLowerInvariant().Split(',');
            foreach(string field in fields)
            {
                string str = field.Trim();

                int idx;
                if(int.TryParse(str, out idx))
                {
                    idxSet.Add(idx);
                    continue;
                }

                string[] x = str.Split('-');
                if(2 != x.Length)
                {   // TODO: warn.
                    continue;
                }

                int idx2;
                if(!int.TryParse(x[0], out idx) || !int.TryParse(x[1], out idx2) || idx > idx2)
                {   // TODO: warn.
                    continue;
                }                

                for(int i=idx; i<=idx2; i++) {
                    idxSet.Add(i);
                }
            }

            // Remove any indexes outside valid range.
            List<int> removeList = new List<int>();
            foreach(int idx in idxSet)
            {
                if(idx <0 || idx>=seqCount)
                {   // TODO: warn.
                    removeList.Add(idx);
                }
            }

            foreach(int idx in removeList) {
                idxSet.Remove(idx);
            }

            return idxSet;
        }

        public static HashSet<int> ParsePatternNotesDescriptor(string patternNotesDesc, Dictionary<string,NoteSet> noteSetDict)
        {
            HashSet<int> noteSet = new HashSet<int>();

            string[] fields = patternNotesDesc.ToLowerInvariant().Split(',');
            foreach(string field in fields)
            {
                string str = field.Trim();

                int noteId;
                if(int.TryParse(str, out noteId))
                {
                    // TODO: use gen purpose validation code.
                    if(noteId >= 0 && noteId <= 127) {
                        noteSet.Add(noteId);
                    }
                    // TODO: report problem.
                    continue;
                }
                
                NoteSet ns;
                if(noteSetDict.TryGetValue(str, out ns)) {
                    noteSet.UnionWith(ns.Notes);
                }
                // else 
                // TODO: report problem.
            }
            return noteSet;
        }

        public static string ToString(HashSet<int> noteIdSet)
        {
            List<int> noteIdList = new List<int>(noteIdSet);
            noteIdList.Sort();
            StringBuilder sb = new StringBuilder();

            if(0 != noteIdList.Count) {
                sb.Append(noteIdList[0]);
            }

            for(int i=1; i < noteIdList.Count; i++) {
                sb.Append(" " + noteIdList[i]);
            }
            return sb.ToString();
        }


        

    }
}
