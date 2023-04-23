using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StreamingClient.Base.Util;

namespace Twitch.Base.Models.Clients.PubSub
{
	/// <summary>
	/// A PubSub web socket message packet.
	/// </summary>
	public class PubSubMessagePacketModel : PubSubPacketModel
	{
        /// <summary>
        /// The topic of the message.
        /// </summary>
        [JsonIgnore]
        public string topic => (data != null && data.ContainsKey(nameof(topic))) ? data[nameof(topic)].ToString() : null;
        /// <summary>
        /// The topic type of the message.
        /// </summary>
        [JsonIgnore]
		public PubSubTopicsEnum topicType
		{
			get
			{
				if (!string.IsNullOrEmpty(topic))
				{
					string[] splits = topic.Split(new char[] { '.' });
					if (splits.Length > 0)
					{
						return EnumHelper.GetEnumValueFromString<PubSubTopicsEnum>(splits[0]);
					}
				}
				return default;
			}
		}
		/// <summary>
		/// The topic ID of the message.
		/// </summary>
		public string topicID
		{
			get
			{
				if (!string.IsNullOrEmpty(topic))
				{
					string[] splits = topic.Split(new char[] { '.' });
					if (splits.Length > 1)
					{
						return splits[1];
					}
				}
				return null;
			}
		}

        /// <summary>
        /// The message contents as a string.
        /// </summary>
        [JsonIgnore]
        public string message => (data != null && data.ContainsKey(nameof(message))) ? data[nameof(message)].ToString() : null;
        /// <summary>
        /// The message contents as a data model.
        /// </summary>
        [JsonIgnore]
        public PubSubMessagePacketDataModel messageData => (!string.IsNullOrEmpty(message)) ? JSONSerializerHelper.DeserializeFromString<PubSubMessagePacketDataModel>(message) : null;
    }

	/// <summary>
	/// The message data for a packet.
	/// </summary>
	public class PubSubMessagePacketDataModel
	{
		/// <summary>
		/// The type of data.
		/// </summary>
		public string type { get; set; }
		/// <summary>
		/// The JToken data of the message.
		/// </summary>
		public object data { get; set; }
        /// <summary>
        /// The JToken data of the message.
        /// </summary>
        [JsonIgnore]
        public JToken data_object => (data is string v) ? JToken.Parse(v) : (JToken)data;
    }
}
