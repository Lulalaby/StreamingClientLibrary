using System;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace StreamingClient.Base.Web
{
    /// <summary>
    /// Handles web socket communication for client connections.
    /// </summary>
    public abstract class ClientWebSocketBase : WebSocketBase
    {
        /// <summary>
        /// The web socket connection.
        /// </summary>
        protected new ClientWebSocket webSocket;

        /// <summary>
        /// Connects the web socket to the server.
        /// </summary>
        /// <param name="endpoint">The endpoint to connect to</param>
        /// <returns>Whether the connection was successful</returns>
        public virtual async Task<bool> ConnectAsync(string endpoint)
        {
            try
            {
                webSocket = CreateWebSocket();
                SetWebSocket(webSocket);

                await webSocket.ConnectAsync(new Uri(endpoint), CancellationToken.None);

                await Task.Delay(1000);

                ReceiveAsync().Wait(1);

                return IsOpen();
            }
            catch (Exception ex)
            {
                await Disconnect();
                if (ex is WebSocketException && ex.InnerException is WebException exception)
                {
                    WebException webException = exception;
                    if (webException.Response is not null and HttpWebResponse)
                    {
                        HttpWebResponse response = (HttpWebResponse)webException.Response;
                        StreamReader reader = new(response.GetResponseStream());
                        string responseString = reader.ReadToEnd();
                        throw new WebSocketException(string.Format("{0} - {1} - {2}", response.StatusCode, response.StatusDescription, responseString), ex);
                    }
                }
                throw;
            }
        }

        /// <summary>
        /// Sends the packet of data.
        /// </summary>
        /// <param name="buffer">The buffer to send</param>
        /// <returns>An awaitable task</returns>
        protected override async Task SendInternalAsync(byte[] buffer) => await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

        /// <summary>
        /// Creates the web socket for connecting with.
        /// </summary>
        /// <returns>The web socket to use</returns>
        protected virtual ClientWebSocket CreateWebSocket() => new();
    }
}
