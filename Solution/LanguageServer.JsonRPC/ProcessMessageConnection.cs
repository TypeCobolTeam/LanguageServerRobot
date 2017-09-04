using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.JsonRPC
{
    /// <summary>
    /// A MessageConnection instance using a Process
    /// </summary>
    public class ProcessMessageConnection : MessageConnection
    {
        /// <summary>
        /// Constructor wiht Process full path and and Process Exit Evnet Handler.
        /// </summary>
        /// <param name="fullPath">The Process full path</param>
        /// <param name="exithandler">Process Exit Evant Handle</param>
        public ProcessMessageConnection(String fullPath, EventHandler exithandler = null) : this(fullPath, null, exithandler)
        {
        }

        /// <summary>
        /// Cosntructor with a process full path, argument string and Process Exit Evnet Handler.
        /// </summary>
        /// <param name="fullPath">The Process full path</param>
        /// <param name="arguments">Process argument string can be null</param>
        /// <param name="exithandler">Process Exit Evant Handle</param>
        public ProcessMessageConnection(String fullPath, String arguments, EventHandler exithandler = null)
        {
            base.State = ConnectionState.Disposed;
            System.Diagnostics.Contracts.Contract.Assert(fullPath != null);
            this.Process = new System.Diagnostics.Process();
            this.Process.StartInfo.FileName = fullPath;
            if (arguments != null)
                this.Process.StartInfo.Arguments = arguments;
            this.Process.StartInfo.UseShellExecute = false;
            this.Process.StartInfo.RedirectStandardOutput = true;
            this.Process.StartInfo.RedirectStandardInput = true;
            //Start the process
            try
            {
                if (exithandler != null)
                    ProcessExited += exithandler;

                if (!this.Process.Start())
                {   //We didn't succed to run the Process
                    base.State = ConnectionState.Disposed;
                }
                else
                {
                    base.Producer = new StreamMessageProducer(this.Process.StandardOutput.BaseStream);
                    base.Consumer = new TextWriterMessageConsumer(this.Process.StandardInput);
                    base.State = ConnectionState.New;
                }
            }
            catch (Exception e)
            {
                base.State = ConnectionState.Disposed;
                throw e;
            }
            WaitForProcessExit();
        }

        public System.Diagnostics.Process Process
        {
            get;
            private set;
        }

        /// <summary>
        /// Use asynchronous task to wait for process exit
        /// </summary>
        /// <returns></returns>
        private async Task<int> WaitForProcessExit()
        {
            int exit_code = 0;
            if (this.Process != null)
            {
                Func<int> action = () => { this.Process.WaitForExit(); return this.Process.ExitCode; } ;
                exit_code = await new Task<int>(action);
            }
            base.State = ConnectionState.Disposed;
            if (ProcessExited != null)
            {
                ProcessExited(this, null);
            }            
            return exit_code;
        }
        /// <summary>
        /// Process Exited event handler
        /// </summary>
        public event EventHandler ProcessExited;
    }
}
