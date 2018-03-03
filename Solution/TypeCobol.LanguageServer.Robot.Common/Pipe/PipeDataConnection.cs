using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using TypeCobol.LanguageServer.Robot.Common.Connection;

namespace TypeCobol.LanguageServer.Robot.Common.Pipe
{
    /// <summary>
    /// The base class of a Pipe connection stream.
    /// </summary>
    public abstract class PipeDataConnection : IDataConnection
    {
        /// <summary>
        /// The Underlying Pipe Stream
        /// </summary>
        PipeStream m_pipeStream;
        /// <summary>
        /// The Consumer Pipe connection.
        /// </summary>
        /// <param name="connectionData"></param>
        /// <returns>true if ok, false otherwise</returns>
        public abstract bool OpenConnection(object connectionData);

        /// <summary>
        /// The underlying pipe server stream.
        /// </summary>!= null)
        public PipeStream PipeDataStream
        {
            get
            {
                return m_pipeStream;
            }
            protected set
            {
                m_pipeStream = value;
            }
        }

        /// <summary>
        /// Close the Connection
        /// </summary>
        /// <returns>true if OK, fase  otherwise</returns>
        public virtual bool CloseConnection()
        {
            if (PipeDataStream != null)
                PipeDataStream.Close();
            PipeDataStream = null;
            return true;
        }

        /// <summary>
        /// Write Data
        /// </summary>
        /// <param name="data">The data to write</param>
        /// <returns>true if ok, false otherwise</returns>
        public bool WriteData(object data)
        {
            if (PipeDataStream != null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(PipeDataStream, data);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Read data
        /// </summary>
        /// <returns>true if ok, false otherwise</returns>
        public object ReadData()
        {
            if (PipeDataStream != null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(PipeDataStream);
            }
            else
                return null;
        }
    }
}

