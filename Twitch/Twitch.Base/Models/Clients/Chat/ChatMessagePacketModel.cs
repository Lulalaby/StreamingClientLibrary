using System;
using System.Collections.Generic;

namespace Twitch.Base.Models.Clients.Chat
{
	/// <summary>
	/// Information about a Chat message packet.
	/// </summary>
	public class ChatMessagePacketModel : ChatPacketModelBase
	{
		/// <summary>
		/// The ID of the command for a chat message.
		/// </summary>
		public const string CommandID = "PRIVMSG";

		/// <summary>
		/// The ID of the message.
		/// </summary>
		public string ID { get; set; }

		/// <summary>
		/// The message text.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// The user's ID.
		/// </summary>
		public string UserID { get; set; }

		/// <summary>
		/// The user's login name.
		/// </summary>
		public string UserLogin { get; set; }

		/// <summary>
		/// The user's display name.
		/// </summary>
		public string UserDisplayName { get; set; }

		/// <summary>
		/// The user's badge information.
		/// </summary>
		public string UserBadgeInfo { get; set; }

		/// <summary>
		/// The user's badges.
		/// </summary>
		public string UserBadges { get; set; }

		/// <summary>
		/// Indicates whether the user is a moderator.
		/// </summary>
		public bool Moderator { get; set; }

		/// <summary>
		/// Hexadecimal RGB color code of the message, if any.
		/// </summary>
		public string Color { get; set; }

		/// <summary>
		/// Information to replace text in the message with emote images.
		/// </summary>
		public string Emotes { get; set; }

		/// <summary>
		/// The channel ID.
		/// </summary>
		public string RoomID { get; set; }

		/// <summary>
		/// The amount of bits associated with the message, if any.
		/// </summary>
		public string Bits { get; set; }

		/// <summary>
		/// Timestamp when the server received the message.
		/// </summary>
		public string Timestamp { get; set; }

		/// <summary>
		/// Creates a new instance of the ChatMessagePacketModel class.
		/// </summary>
		/// <param name="packet">The Chat packet</param>
		public ChatMessagePacketModel(ChatRawPacketModel packet)
			: base(packet)
		{
			ID = packet.GetTagString("id");
			Message = packet.Get1SkippedParameterText;

			UserID = packet.GetTagString("user-id");
			UserLogin = packet.GetUserLogin;
			UserDisplayName = packet.GetTagString("display-name");
			UserBadgeInfo = packet.GetTagString("badge-info");
			UserBadges = packet.GetTagString("badges");
			Moderator = packet.GetTagBool("mod");

			Color = packet.GetTagString("color");
			Emotes = packet.GetTagString("emotes");
			RoomID = packet.GetTagString("room-id");

			Bits = packet.GetTagString("bits");

			Timestamp = packet.GetTagString("tmi-sent-ts");
		}


        /// <summary>
        /// A dictionary containing the user's badges and associated versions.
        /// </summary>
        public Dictionary<string, int> BadgeDictionary => ParseBadgeDictionary(UserBadges);

        /// <summary>
        /// A dictionary containing the user's badges and associated versions.
        /// </summary>
        public Dictionary<string, int> BadgeInfoDictionary => ParseBadgeDictionary(UserBadgeInfo);

        /// <summary>
        /// A dictionary containing the emote IDs used by the user in the message and their location in the message
        /// </summary>
        public Dictionary<string, List<Tuple<int, int>>> EmotesDictionary
		{
			get
			{
				Dictionary<string, List<Tuple<int, int>>> results = new();
				if (!string.IsNullOrEmpty(Emotes))
				{
					string[] splits = Emotes.Split(new char[] { '/', ':' });
					if (splits != null && splits.Length > 0 && splits.Length % 2 == 0)
					{
						for (int i = 0; i < splits.Length; i += 2)
						{
							string setID = splits[i];
							results[setID] = new List<Tuple<int, int>>();

							string[] setSplits = splits[i + 1].Split(new char[] { '-', ',' });
							if (setSplits != null && setSplits.Length > 0 && setSplits.Length % 2 == 0)
							{
								for (int j = 0; j < setSplits.Length; j += 2)
								{
									if (int.TryParse(setSplits[j], out int start) && int.TryParse(setSplits[j + 1], out int end))
									{
										results[setID].Add(new Tuple<int, int>(start, end));
									}
								}
							}
						}
					}
				}
				return results;
			}
		}
	}
}
