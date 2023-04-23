using System.Threading.Tasks;

using StreamingClient.Base.Util;

namespace Twitch.Base.Clients
{
	/// <summary>
	/// IRC client for interacting with the mock FDGT Chat service.
	/// </summary>
	public class FDGTChatClient : ChatClient
	{
		/// <summary>
		/// The default Chat connection url.
		/// </summary>
		public const string FDGT_CHAT_CONNECTION_URL = "wss://irc.fdgt.dev";

		/// <summary>
		/// Creates a new instance of the FDGTChatClient class.
		/// </summary>
		/// <param name="connection">The current connection</param>
		public FDGTChatClient(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Connects to the default ChatClient connection.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public override async Task ConnectAsync() => await ConnectAsync(FDGT_CHAT_CONNECTION_URL);

        /// <summary>
        /// Sends a test message to trigger an event through FDGT.
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns>An awaitable Task</returns>
        public async Task SendTestMessageAsync(string message)
		{
			Validator.ValidateString(message, "message");
			await SendAsync(string.Format("PRIVMSG #channel :{0}", message));
		}
	}
}
