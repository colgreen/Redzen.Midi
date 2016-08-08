using System;
using System.Collections.Generic;

namespace Redzen.Midi
{
    /// <summary>
    /// Extension methods for the Control enum.
    /// </summary>
    /// Be sure to "using midi" if you want to use these as extension methods.
    public static class ControlExtensions
    {
        #region Public Static Methods

        /// <summary>
        /// Returns true if the specified control is valid.
        /// </summary>
        /// <param name="control">The Control to test.</param>
        public static bool IsValid(this Control control)
        {
            return (int)control >= 0 && (int)control < 128;
        }

        /// <summary>
        /// Throws an exception if control is not valid.
        /// </summary>
        /// <param name="control">The control to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">The control is out-of-range.</exception>
        public static void Validate(this Control control)
        {
            if(!control.IsValid()) {
                throw new ArgumentOutOfRangeException("Control out of range");
            }
        }

        #endregion
    }
}
