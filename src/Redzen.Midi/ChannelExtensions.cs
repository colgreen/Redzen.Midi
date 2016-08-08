using System;

namespace Redzen.Midi
{
    /// <summary>
    /// Extension methods for the Channel enum.
    /// </summary>
    public static class ChannelExtensions
    {
        #region Public Static Methods

        /// <summary>
        /// Returns true if the specified channel is valid.
        /// </summary>
        /// <param name="channel">The channel to test.</param>
        public static bool IsValid(this Channel channel)
        {
            return (int)channel >= 0 && (int)channel < 16;
        }

        /// <summary>
        /// Throws an exception if channel is not valid.
        /// </summary>
        /// <param name="channel">The channel to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">The channel is out-of-range.</exception>
        public static void Validate(this Channel channel)
        {
            if(!channel.IsValid()) {
                throw new ArgumentOutOfRangeException("Channel out of range");
            }
        }

        #endregion
    }
}
