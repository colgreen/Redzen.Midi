
namespace Redzen.Midi.Sequencing
{
    public class LiveNote
    {
        int _noteId;
        Channel _chan;
        int _ticksRemaining;

        #region Constructor

        public LiveNote(int noteId, Channel chan, int ticksRemaining)
        {
            _noteId = noteId;
            _chan = chan;
            _ticksRemaining = ticksRemaining;
        }

        #endregion

        #region Properties

        public int NoteId
        {
            get { return _noteId; }
        }

        public Channel Channel
        {
            get { return _chan; }
        }

        public int TicksRemaining
        {
            get { return _ticksRemaining; }
            set { _ticksRemaining = value; }
        }

        #endregion
    }
}
