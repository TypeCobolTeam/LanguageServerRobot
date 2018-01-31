using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LanguageServer.JsonRPC;
using LanguageServer.Protocol;
using LanguageServer.Robot.Common.Model;
using Newtonsoft.Json.Linq;

namespace LanguageServer.Robot.Common.Controller
{
    /// <summary>
    /// A Controller for recording a specific scenario.
    /// </summary>
    public class ScenarioRobotConnectionController : ClientRobotConnectionController
    {
        /// <summary>
        /// Message Connection constructor.
        /// </summary>
        /// <param name="messageConnection"></param>
        public ScenarioRobotConnectionController(IMessageConnection messageConnection) : base(messageConnection)
        {
            base.ProducedMessageFilter = ClientProducedMessageFilter;
        }

        /// <summary>
        /// Did Open parameters as received from the didOpen notification.
        /// </summary>
        public DidOpenTextDocumentParams DidOpenParameters
        {
            get;
            internal set;
        }

        /// <summary>
        /// The Did Close Text Document notification if any have been seen
        /// </summary>
        public string DidCloseTextDocument
        {
            get;
            internal set;
        }

        /// <summary>
        /// Any Shtudown requet seen
        /// </summary>
        public string ShutdownRequest
        {
            get; internal set;
        }

        /// <summary>
        /// Any Exit Notification.
        /// </summary>
        public string ExitNotification
        {
            get; internal set;
        }

        /// <summary>
        /// The Session
        /// </summary>
        public Model.Session Session
        {
            get; internal set;
        }

        public Model.Script Script
        {
            get; internal set;
        }

        /// <summary>
        /// Initialize a Scenario. This means that we must send the initialize request, any didchangeConfiguration.
        /// Replay and didSave and textChange notification to this point.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="script"></param>
        public bool InitializeScenario(Model.Session session, Model.Script script)
        {
            this.Session = session;
            this.Script = script;
            if (session.initialize != null)
            {
                this.Consume(session.initialize);
                if (session.did_change_configuation != null)
                {
                    this.Consume(session.did_change_configuation);
                }
                bool bDidOpenSeen = false;//Have we seen a did open document ?
                //Apply didOpen all text changes from the beginning.
                foreach (var msg in script.messages)
                {
                    if (msg.category == Script.MessageCategory.Client)
                    {
                        JObject jsonJObject = null;
                        if (Utilities.Protocol.IsDidOpenTextDocumentNotification(msg.message, out jsonJObject))
                        {
                            bDidOpenSeen = true;
                            NotificationType notificationType = DidOpenTextDocumentNotification.Type;
                            object objParams = null;
                            JToken parameters = jsonJObject["params"];
                            if (parameters != null)
                            {
                                objParams = parameters.ToObject(notificationType.ParamsType);
                            }
                            DidOpenParameters = (DidOpenTextDocumentParams) objParams;
                            this.Consume(msg.message);
                        }
                        else if (Utilities.Protocol.IsDidSaveTextDocumentNotification(jsonJObject))
                        {
                            this.Consume(msg.message);
                        }
                        else if (Utilities.Protocol.IsDidChangeTextDocumentNotification(jsonJObject))
                        {
                            this.Consume(msg.message);
                        }
                    }
                }
                return bDidOpenSeen;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Add a LSP message to the scenario
        /// </summary>
        /// <param name="script">The Script associated to the message</param>
        /// <param name="msg">The Lsp Message to be added</param>
        public void AddMessage(Model.Script script, Model.Message.LspMessage msg)
        {
            if (this.Script == script && msg.From == Message.LspMessage.MessageFrom.Client)
            {
                this.Consume(msg.Message);
            }
        }

        /// <summary>
        /// Add a message to the scenario
        /// </summary>
        /// <param name="script">The Script associated to the message</param>
        /// <param name="msg">The message to be added</param>
        public void AddMessage(Model.Script script, string msg)
        {
            if (this.Script == script)
                this.Consume(msg);
        }

        /// <summary>
        /// Stop the Scenaro by sending a didClose notification a shutdown request and an exit notification.
        /// </summary>
        /// <param name="session">The current session</param>
        /// <param name="script">The Original script.</param>
        /// <returns></returns>
        public bool StopScenario(Model.Session session, Model.Script script)
        {
            //Send the close Notification            
            if (DidCloseTextDocument == null)
            {
                JObject jsonObject = null;
                DidCloseTextDocumentParams parameters =
                    new DidCloseTextDocumentParams(new TextDocumentIdentifier(DidOpenParameters.textDocument.uri));
                string message = CreateNotification(DidSaveTextDocumentNotification.Type, parameters, out jsonObject);
                this.Consume(message);
            }
            else
            {
                this.Consume(DidCloseTextDocument);
            }

            //Send the shutdown request         
            if (this.ShutdownRequest != null)   
                base.Consume(Utilities.Protocol.DEFAULT_SHUTDOWN);
            //Send the Exit notification
            if (this.ExitNotification != null)
                base.Consume(Utilities.Protocol.DEFAULT_EXIT);
            return false;
        }

        /// <summary>
        /// Abort the secanario recording.
        /// </summary>
        /// <returns>return true if ok, false otherwise.</returns>
        public bool AbortScenario()
        {
            //Send the shutdown request         
            if (this.ShutdownRequest != null)
                base.Consume(Utilities.Protocol.DEFAULT_SHUTDOWN);
            //Send the Exit notification
            if (this.ExitNotification != null)
                base.Consume(Utilities.Protocol.DEFAULT_EXIT);
            return true;
        }

        /// <summary>
        /// Method to filter messaged produced for the client
        /// </summary>
        /// <param name="message">The mmessage to filter</param>
        /// <param name="connection">The source connection of the message</param>
        /// <returns>true if the message is filtered, false otherwise.</returns>
        private bool ClientProducedMessageFilter(string message, IMessageConnection connection)
        {
            JObject jsonJObject = null;
            if (Utilities.Protocol.IsDidCloseTextDocumentNotification(message, out jsonJObject))
            {
                this.DidCloseTextDocument = message;
            }
            else if (Utilities.Protocol.IsShutdownRequest(jsonJObject))
            {
                ShutdownRequest = message;
            }
            else if (Utilities.Protocol.IsExitNotification(jsonJObject))
            {
                ExitNotification = message;
            }
            this.RobotModeController.FromClient(message);
            connection.SendMessage(message);
            return true;
        }

        /// <summary>
        /// Save the Scenario in a file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool SaveScenario(Script script, string filepath)
        {
            return false;
        }
    }
}
