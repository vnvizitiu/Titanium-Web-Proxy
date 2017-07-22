﻿using StreamExtended.Network;
using System;
using System.Net;
using System.Net.Sockets;
using Titanium.Web.Proxy.Extensions;
using Titanium.Web.Proxy.Models;

namespace Titanium.Web.Proxy.Network.Tcp
{
    /// <summary>
    /// An object that holds TcpConnection to a particular server and port
    /// </summary>
    internal class TcpConnection : IDisposable
    {
        internal ExternalProxy UpStreamHttpProxy { get; set; }

        internal ExternalProxy UpStreamHttpsProxy { get; set; }

        internal ExternalProxy UpStreamProxy => UseUpstreamProxy ? IsHttps ? UpStreamHttpsProxy : UpStreamHttpProxy : null;

        internal string HostName { get; set; }

        internal int Port { get; set; }

        internal bool IsHttps { get; set; }

        internal bool UseUpstreamProxy { get; set; }

        /// <summary>
        /// Local NIC via connection is made
        /// </summary>
        internal IPEndPoint UpStreamEndPoint { get; set; }

        /// <summary>
        /// Http version
        /// </summary>
        internal Version Version { get; set; }

        internal TcpClient TcpClient { private get; set; }

        /// <summary>
        /// used to read lines from server
        /// </summary>
        internal CustomBinaryReader StreamReader { get; set; }

        /// <summary>
        /// Server stream
        /// </summary>
        internal CustomBufferedStream Stream { get; set; }

        /// <summary>
        /// Last time this connection was used
        /// </summary>
        internal DateTime LastAccess { get; set; }

        internal TcpConnection()
        {
            LastAccess = DateTime.Now;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            Stream?.Dispose();

            StreamReader?.Dispose();

            try
            {
                if (TcpClient != null)
                {
                    //This line is important!
                    //contributors please don't remove it without discussion
                    //It helps to avoid eventual deterioration of performance due to TCP port exhaustion
                    //due to default TCP CLOSE_WAIT timeout for 4 minutes
                    TcpClient.LingerState = new LingerOption(true, 0);
                    TcpClient.Dispose();
                }
            }
            catch
            {
            }
        }
    }
}
