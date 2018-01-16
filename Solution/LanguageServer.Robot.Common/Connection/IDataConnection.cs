using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer.Robot.Common.Connection
{
    /// <summary>
    /// Data Connection Interface
    /// </summary>
    public interface IDataConnection
    {
        /// <summary>
        /// Initiate a connection
        /// </summary>
        /// <param name="connectionData">TSome data to establish a connection</param>
        /// <returns>true if ok, false otherwise</returns>
        /// <exception cref="Exception">Any exception that might appear</exception>
        bool OpenConnection(object connectionData);
        /// <summary>
        /// Write Serializable data
        /// </summary>
        /// <param name="data"></param>
        /// <returns>true if ok, false otherwise</returns>
        /// <exception cref="Exception">Any exception that might appear</exception>
        bool WriteData(object data);
        /// <summary>
        /// Read Serializable data
        /// </summary>
        /// <returns>The data read if any, null otherwise</returns>
        /// <exception cref="Exception">Any exception that might appear</exception>
        Object ReadData();
        /// <summary>
        /// Close a Connection
        /// </summary>
        /// <returns>true if ok, false otherwise</returns>
        /// <exception cref="Exception">Any exception that might appear</exception>
        bool CloseConnection();
    }
}
