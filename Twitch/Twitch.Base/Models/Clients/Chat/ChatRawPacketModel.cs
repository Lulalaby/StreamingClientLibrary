using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using StreamingClient.Base.Util;

namespace Twitch.Base.Models.Clients.Chat
{
	/// <summary>
	/// Information about a Chat client packet.
	/// </summary>
	public class ChatRawPacketModel
	{
		/// <summary>
		/// The raw text of the packet.
		/// </summary>
		public string RawText { get; set; }

		/// <summary>
		/// Tha tags of the packet.
		/// </summary>
		public Dictionary<string, string> Tags { get; set; }

		/// <summary>
		/// The prefix of the packet.
		/// </summary>
		public string Prefix { get; set; }

		/// <summary>
		/// The command for the packet.
		/// </summary>
		public string Command { get; set; }

		/// <summary>
		/// The parameters for the command.
		/// </summary>
		public List<string> Parameters { get; set; }

        /// <summary>
        /// Gets the user login associated with the packet.
        /// </summary>
        [JsonIgnore]
        public string GetUserLogin => Prefix[..Prefix.IndexOf("!")];

        /// <summary>
        /// Gets the entire set of parameters concatenated into one string, minus the first parameter.
        /// </summary>
        [JsonIgnore]
        public string Get1SkippedParameterText => string.Join(" ", Parameters.Skip(1));

        /// <summary>
        /// Gets the tag value associated with the specified key
        /// </summary>
        /// <param name="key">The key to look up</param>
        /// <returns>The value associated with the specified key</returns>
        public string GetTagString(string key) => Tags.GetOrDefault(key);

        /// <summary>
        /// Gets the tag value associated with the specified key
        /// </summary>
        /// <param name="key">The key to look up</param>
        /// <returns>The value associated with the specified key</returns>
        public int GetTagInt(string key)
		{
			string value = GetTagString(key);
			int.TryParse(value, out int result);
			return result;
		}

		/// <summary>
		/// Gets the tag value associated with the specified key
		/// </summary>
		/// <param name="key">The key to look up</param>
		/// <returns>The value associated with the specified key</returns>
		public long GetTagLong(string key)
		{
			string value = GetTagString(key);
			long.TryParse(value, out long result);
			return result;
		}

		/// <summary>
		/// Gets the tag value associated with the specified key
		/// </summary>
		/// <param name="key">The key to look up</param>
		/// <returns>The value associated with the specified key</returns>
		public bool GetTagBool(string key)
		{
			string value = GetTagString(key);
			int.TryParse(value, out int result);
			return result == 1;
		}
	}
}
