using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.Robot.Common.Controller
{
    /// <summary>
    /// Interface for a rboting mode controller
    /// </summary>
    public interface IRobotModeController
    {
        /// <summary>
        /// Is Mode Session Initialized.
        /// </summary>
        bool IsModeInitialized
        {get;}

        /// <summary>
        /// Is Mode Session Started
        /// </summary>
        bool IsModeStarted
        { get; }

        /// <summary>
        /// Is Mode Session Stopped
        /// </summary>
        bool IsModeStopped
        { get; }

        /// <summary>
        /// Handler for a message received from the client
        /// </summary>
        /// <param name="message"></param>
        void FromClient(string message);

        /// <summary>
        /// Handler for a message recceived from the server.
        /// </summary>
        /// <param name="message"></param>
        void FromServer(string message);
    }
}
