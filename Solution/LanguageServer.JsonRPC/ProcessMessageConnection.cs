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
        public ProcessMessageConnection(String fullPath, String arguments, EventHandler exithandler = null) : base(0)
        {
            this.FullPath = fullPath;
            this.Arguments = arguments;
            if (exithandler != null)
                ProcessExited += exithandler;
        }

        /// <summary>
        /// Process Full path
        /// </summary>
        public string FullPath
        {
            get;
            protected set;
        }

        /// <summary>
        /// Process arguments.
        /// </summary>
        public string Arguments
        {
            get;
            protected set;
        }

        int _ExitCode;
        /// <summary>
        /// The underlying process exit code
        /// </summary>
        public int ExitCode
        {
            get
            {
                return System.Threading.Interlocked.Exchange(ref _ExitCode, _ExitCode);
            }
            protected set
            {
                System.Threading.Interlocked.Exchange(ref _ExitCode, (int)value);
            }
        }

        /// <summary>
        /// Start the Process
        /// </summary>
        protected void Start()
        {
            System.Diagnostics.Contracts.Contract.Assert(FullPath != null);
            this.Process = new System.Diagnostics.Process();
            this.Process.StartInfo.FileName = FullPath;
            if (Arguments != null)
                this.Process.StartInfo.Arguments = Arguments;
            this.Process.StartInfo.UseShellExecute = false;
            this.Process.StartInfo.RedirectStandardOutput = true;
            this.Process.StartInfo.RedirectStandardInput = true;            
            //Start the process
            try
            {                
                if (!this.Process.Start())
                {   //We didn't succed to run the Process
                    base.State = ConnectionState.Disposed;
                    return;
                }
                else
                {
                    base.Producer = new StreamMessageProducer(this.Process.StandardOutput.BaseStream);
                    base.Consumer = new StreamMessageConsumer(this.Process.StandardInput.BaseStream);
                }
            }
            catch (Exception e)
            {
                base.State = ConnectionState.Disposed;
                this.WriteConnectionLog(e.Message);
                return;
            }
            Func<int> action = () => 
            {
                this.Process.WaitForExit();
                ExitCode = this.Process.ExitCode;
                if (ProcessExited != null)
                    ProcessExited(this, null);
                return ExitCode;
            };
            WaitProcessExitTask = new Task<int>(action);
            WaitProcessExitTask.Start();
        }

        /// <summary>
        /// The task that wait for the process to exit.
        /// </summary>
        public Task<int> WaitProcessExitTask
        {
            get;
            private set;
        }
        /// <summary>
        /// Start the connection, Listening of incoming message.
        /// </summary>
        /// <param name="messageConsumer">The message consumer of listened message</param>
        /// <returns>Return the Listening task.</returns>
        public override async Task<bool> Start(IMessageConsumer messageConsumer)
        {
            ThrowIfClosedOrDisposed();
            ThrowIfListening();
            Start();
            if (!IsDisposed)
            {
                return await base.Start(messageConsumer);
            }
            return false;
        }

        /// <summary>
        /// The Underlying System Process.
        /// </summary>
        public System.Diagnostics.Process Process
        {
            get;
            private set;
        }
        /// <summary>
        /// Process Exited event handler
        /// </summary>
        public event EventHandler ProcessExited;
    }
}
