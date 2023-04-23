using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using StreamingClient.Base.Util;
using StreamingClient.Base.Web;

using Twitch.Base.Models.NewAPI;
using Twitch.Base.Models.NewAPI.Chat;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.Services.NewAPI
{
	/// <summary>
	/// The APIs for Chat-based services.
	/// </summary>
	public class ChatService : NewTwitchAPIServiceBase
	{
		/// <summary>
		/// Creates an instance of the ChatService.
		/// </summary>
		/// <param name="connection">The Twitch connection to use</param>
		public ChatService(TwitchConnection connection) : base(connection) { }

		/// <summary>
		/// Gets the chat badges for the specified channel.
		/// </summary>
		/// <param name="channel">The channel to get chat badges for</param>
		/// <returns>The chat badges for the channel</returns>
		public async Task<IEnumerable<ChatBadgeSetModel>> GetChannelChatBadgesAsync(UserModel channel)
		{
			Validator.ValidateVariable(channel, "channel");
			return ProcessChatBadges(await GetJObjectAsync(string.Format("https://badges.twitch.tv/v1/badges/channels/{0}/display?language=en", channel.id)));
		}

        /// <summary>
        /// Gets the chat badges available globally.
        /// </summary>
        /// <returns>The global chat badges</returns>
        public async Task<IEnumerable<ChatBadgeSetModel>> GetGlobalChatBadgesAsync() => ProcessChatBadges(await GetJObjectAsync("https://badges.twitch.tv/v1/badges/global/display?language=en"));

        /// <summary>
        /// Gets all global emotes.
        /// </summary>
        /// <returns>The global emotes</returns>
        public async Task<IEnumerable<ChatEmoteModel>> GetGlobalEmotesAsync() => await GetDataResultAsync<ChatEmoteModel>("chat/emotes/global");

        /// <summary>
        /// Gets the emotes for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get emotes for</param>
        /// <returns>The emotes for the channel</returns>
        public async Task<IEnumerable<ChatEmoteModel>> GetChannelEmotesAsync(UserModel channel)
		{
			Validator.ValidateVariable(channel, "channel");
			return await GetChannelEmotesAsync(channel.id);
		}

		/// <summary>
		/// Gets the emotes for the specified channel ID.
		/// </summary>
		/// <param name="channelID">The channel ID to get emotes for</param>
		/// <returns>The emotes for the channel</returns>
		public async Task<IEnumerable<ChatEmoteModel>> GetChannelEmotesAsync(string channelID)
		{
			Validator.ValidateString(channelID, "channelID");
			return await GetDataResultAsync<ChatEmoteModel>("chat/emotes?broadcaster_id=" + channelID);
		}

		/// <summary>
		/// Gets all global emotes.
		/// </summary>
		/// <returns>The global emotes</returns>
		public async Task<IEnumerable<ChatEmoteModel>> GetEmoteSetsAsync(IEnumerable<string> emoteSetIDs)
		{
			Validator.ValidateList(emoteSetIDs, "emoteSetIDs");

			List<ChatEmoteModel> results = new();

			for (int i = 0; i < emoteSetIDs.Count(); i += 10)
			{
				results.AddRange(await GetDataResultAsync<ChatEmoteModel>("chat/emotes/set?" + string.Join("&", emoteSetIDs.Skip(i).Take(10).Select(id => "emote_set_id=" + id))));
			}

			return results;
		}

		/// <summary>
		/// Sends a whisper from the broadcaster to the specified user.
		/// <param name="channelID">The channel ID to send the whisper from</param>
		/// <param name="userID">The user ID to send the whisper to</param>
		/// <param name="message">The message to whisper</param>
		/// </summary>
		public async Task SendWhisperAsync(string channelID, string userID, string message)
		{
			Validator.ValidateString(channelID, "channelID");
			Validator.ValidateString(userID, "userID");
			Validator.ValidateString(message, "message");

			JObject jobj = new()
            {
				["message"] = message
			};

			await PostAsync("whispers?from_user_id=" + channelID + "&to_user_id=" + userID, AdvancedHttpClient.CreateContentFromObject(jobj));
		}

		/// <summary>
		/// Sends an announcement to the broadcaster’s chat room.
		/// <param name="channelID">The channel ID send the announcement to</param>
		/// <param name="announcement">The announcement data to send</param>
		/// </summary>
		public async Task SendChatAnnouncementAsync(string channelID, AnnouncementModel announcement)
		{
			Validator.ValidateString(channelID, "channelID");

			await PostAsync("chat/announcements?broadcaster_id=" + channelID + "&moderator_id=" + channelID, AdvancedHttpClient.CreateContentFromObject(announcement));
		}

		/// <summary>
		/// Sends an announcement to the broadcaster’s chat room as the moderator.
		/// <param name="channelID">The channel ID send the announcement to</param>
		/// <param name="moderatorID">The ID of the moderator sending the announcement</param>
		/// <param name="announcement">The announcement data to send</param>
		/// </summary>
		public async Task SendChatAnnouncementAsync(string channelID, string moderatorID, AnnouncementModel announcement)
		{
			Validator.ValidateString(channelID, "channelID");
			Validator.ValidateString(moderatorID, "moderatorID");

			await PostAsync("chat/announcements?broadcaster_id=" + channelID + "&moderator_id=" + moderatorID, AdvancedHttpClient.CreateContentFromObject(announcement));
		}

		/// <summary>
		/// Raids the specified target channel.
		/// </summary>
		/// <param name="channelID">The channel ID to raid from</param>
		/// <param name="targetChannelID">The channel ID to raid</param>
		public async Task RaidChannelAsync(string channelID, string targetChannelID)
		{
			Validator.ValidateString(channelID, "channelID");
			Validator.ValidateString(targetChannelID, "targetChannelID");

			await PostAsync("raids?from_broadcaster_id=" + channelID + "&to_broadcaster_id=" + targetChannelID, AdvancedHttpClient.CreateEmptyContent());
		}

		/// <summary>
		/// Deletes the specified message in the broadcaster's chat room.
		/// </summary>
		/// <param name="channelID">The channel ID to delete message in</param>
		/// <param name="messageID">The message ID to delete</param>
		public async Task DeleteChatMessageAsync(string channelID, string messageID)
		{
			Validator.ValidateString(channelID, "channelID");
			Validator.ValidateString(messageID, "messageID");

			await DeleteAsync("moderation/chat?broadcaster_id=" + channelID + "&moderator_id=" + channelID + "&message_id=" + messageID);
		}

		/// <summary>
		/// Deletes the specified message in the broadcaster's chat room.
		/// </summary>
		/// <param name="channelID">The channel ID to delete message in</param>
		/// <param name="moderatorID">The ID of the moderator deleting the message</param>
		/// <param name="messageID">The message ID to delete</param>
		public async Task DeleteChatMessageAsync(string channelID, string moderatorID, string messageID)
		{
			Validator.ValidateString(channelID, "channelID");
			Validator.ValidateString(moderatorID, "moderatorID");
			Validator.ValidateString(messageID, "messageID");

			await DeleteAsync("moderation/chat?broadcaster_id=" + channelID + "&moderator_id=" + moderatorID + "&message_id=" + messageID);
		}

		/// <summary>
		/// Clears chat for the broadcaster's chat room.
		/// </summary>
		/// <param name="channelID">The channel ID to clear chat for</param>
		/// <param name="moderatorID">The ID of the moderator clearing the chat</param>
		public async Task ClearChatAsync(string channelID, string moderatorID)
		{
			Validator.ValidateString(channelID, "channelID");
			Validator.ValidateString(moderatorID, "moderatorID");

			await DeleteAsync("moderation/chat?broadcaster_id=" + channelID + "&moderator_id=" + moderatorID);
		}

		/// <summary>
		/// Clears chat for the broadcaster's chat room.
		/// </summary>
		/// <param name="channelID">The channel ID to clear chat for</param>
		public async Task ClearChatAsync(string channelID)
		{
			Validator.ValidateString(channelID, "channelID");

			await DeleteAsync("moderation/chat?broadcaster_id=" + channelID + "&moderator_id=" + channelID);
		}

		/// <summary>
		/// Bans the specified user from the broadcaster's chat room.
		/// </summary>
		/// <param name="channelID">The channel ID to ban the user from</param>
		/// <param name="userID">The user ID to ban</param>
		/// <param name="duration">The duration to ban for</param>
		/// <param name="reason">The reason for the ban</param>
		public async Task TimeoutUserAsync(string channelID, string userID, int duration, string reason)
		{
			Validator.ValidateString(channelID, "channelID");
			Validator.ValidateString(userID, "userID");
			Validator.ValidateVariable(duration, "duration");
			Validator.ValidateString(reason, "reason");

			JObject jdata = new();
			JObject jobj = new()
            {
				["user_id"] = userID,
				["reason"] = reason ?? string.Empty
			};
			if (duration > 0)
			{
				jobj["duration"] = duration;
			}
			jdata["data"] = jobj;

			await PostAsync("moderation/bans?broadcaster_id=" + channelID + "&moderator_id=" + channelID, AdvancedHttpClient.CreateContentFromObject(jdata));

		}

		/// <summary>
		/// Untimeouts the specified user from the broadcaster's chat room.
		/// </summary>
		/// <param name="channelID">The channel ID to untimeout the user from</param>
		/// <param name="userID">The user ID to untimeout</param>
		public async Task UntimeoutUserAsync(string channelID, string userID)
		{
			Validator.ValidateString(channelID, "channelID");
			Validator.ValidateString(userID, "userID");

			await DeleteAsync("moderation/bans?broadcaster_id=" + channelID + "&moderator_id=" + channelID + "&user_id=" + userID);
		}

		/// <summary>
		/// Bans the specified user from the broadcaster's chat room.
		/// </summary>
		/// <param name="channelID">The channel ID to ban the user from</param>
		/// <param name="userID">The user ID to ban</param>
		/// <param name="reason">The reason for the ban</param>
		public async Task BanUserAsync(string channelID, string userID, string reason)
		{
			Validator.ValidateString(channelID, "channelID");
			Validator.ValidateString(userID, "userID");
			Validator.ValidateString(reason, "reason");

			await TimeoutUserAsync(channelID, userID, 0, reason);
		}

		/// <summary>
		/// Unbans the specified user from the broadcaster's chat room.
		/// </summary>
		/// <param name="channelID">The channel ID to unban the user from</param>
		/// <param name="userID">The user ID to unban</param>
		public async Task UnbanUserAsync(string channelID, string userID)
		{
			Validator.ValidateString(channelID, "channelID");
			Validator.ValidateString(userID, "userID");

			await UntimeoutUserAsync(channelID, userID);
		}

		/// <summary>
		/// Mods the specified user in the broadcaster's chat room.
		/// </summary>
		/// <param name="channelID">The channel ID to mod the user for</param>
		/// <param name="userID">The user ID to mod</param>
		public async Task ModUserAsync(string channelID, string userID)
		{
			Validator.ValidateString(channelID, "channelID");
			Validator.ValidateString(userID, "userID");

			await PostAsync("moderation/moderators?broadcaster_id=" + channelID + "&user_id=" + userID, AdvancedHttpClient.CreateEmptyContent());
		}

		/// <summary>
		/// Unbans the specified user from the broadcaster's chat room.
		/// </summary>
		/// <param name="channelID">The channel ID to unban the user from</param>
		/// <param name="userID">The user ID to unban</param>
		public async Task UnmodUserAsync(string channelID, string userID)
		{
			Validator.ValidateString(channelID, "channelID");
			Validator.ValidateString(userID, "userID");

			await DeleteAsync("moderation/moderators?broadcaster_id=" + channelID + "&user_id=" + userID);
		}

		/// <summary>
		/// VIPs the specified user in the broadcaster's chat room.
		/// </summary>
		/// <param name="channelID">The channel ID to VIP the user for</param>
		/// <param name="userID">The user ID to VIP</param>
		public async Task VIPUserAsync(string channelID, string userID)
		{
			Validator.ValidateString(channelID, "channelID");
			Validator.ValidateString(userID, "userID");

			await PostAsync("channels/vips?broadcaster_id=" + channelID + "&user_id=" + userID, AdvancedHttpClient.CreateEmptyContent());
		}

		/// <summary>
		/// Un-VIPs the specified user from the broadcaster's chat room.
		/// </summary>
		/// <param name="channelID">The channel ID to un-VIP the user from</param>
		/// <param name="userID">The user ID to un-VIP</param>
		public async Task UnVIPUserAsync(string channelID, string userID)
		{
			Validator.ValidateString(channelID, "channelID");
			Validator.ValidateString(userID, "userID");

			await DeleteAsync("channels/vips?broadcaster_id=" + channelID + "&user_id=" + userID);
		}

		/// <summary>
		/// Gets the chatters for the broadcster's channel.
		/// </summary>
		/// <param name="channelID">The channel ID to get chatters for</param>
		/// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
		/// <returns>The chatters</returns>
		public async Task<IEnumerable<ChatterModel>> GetChattersAsync(string channelID, int maxResults = 1)
		{
			Validator.ValidateString(channelID, "channelID");
			return await GetPagedDataResultAsync<ChatterModel>("chat/chatters?broadcaster_id=" + channelID + "&moderator_id=" + channelID, maxResults);
		}

		/// <summary>
		/// Gets the chat settings for the broadcster's channel.
		/// </summary>
		/// <param name="channelID">The channel ID to update chat settings for</param>
		/// <returns>The chat settings</returns>
		public async Task<ChatSettingsModel> GetChatSettingsAsync(string channelID)
		{
			Validator.ValidateString(channelID, "channelID");

			IEnumerable<ChatSettingsModel> settings = await GetDataResultAsync<ChatSettingsModel>("chat/settings?broadcaster_id=" + channelID + "&moderator_id=" + channelID);
            return settings?.FirstOrDefault();
        }

        /// <summary>
        /// Updates the chat settings for the broadcster's channel.
        /// </summary>
        /// <param name="channelID">The channel ID to update chat settings for</param>
        /// <param name="settings">The settings to update for the channel</param>
        /// <returns>The updated chat settings</returns>
        public async Task<ChatSettingsModel> UpdateChatSettingsAsync(string channelID, ChatSettingsModel settings)
		{
			Validator.ValidateString(channelID, "channelID");
			Validator.ValidateVariable(settings, "settings");

			NewTwitchAPIDataRestResult<ChatSettingsModel> updatedSettings =
				await PatchAsync<NewTwitchAPIDataRestResult<ChatSettingsModel>>("chat/settings?broadcaster_id=" + channelID + "&moderator_id=" + channelID,
				AdvancedHttpClient.CreateContentFromObject(settings));

            return updatedSettings != null && updatedSettings.data.Count > 0 ? updatedSettings.data.First() : null;
        }

        /// <summary>
        /// Sends a Shoutout to the specified broadcaster.
        /// </summary>
        /// <param name="sourceChannelID">The channel ID to send the shoutout for</param>
        /// <param name="targetChannelID">The channel ID to shoutout</param>
        /// <returns>Whether the shoutout was successful</returns>
        public async Task<bool> SendShoutoutAsync(string sourceChannelID, string targetChannelID)
		{
			Validator.ValidateString(sourceChannelID, "sourceChannelID");
			Validator.ValidateString(targetChannelID, "targetChannelID");

			HttpResponseMessage response = await PostAsync("chat/shoutouts?from_broadcaster_id=" + sourceChannelID + "&to_broadcaster_id=" + targetChannelID + "&moderator_id=" + sourceChannelID, new StringContent(string.Empty));
			return response.IsSuccessStatusCode;
		}

		private IEnumerable<ChatBadgeSetModel> ProcessChatBadges(JObject jobj)
		{
			List<ChatBadgeSetModel> results = new();
			if (jobj.ContainsKey("badge_sets"))
			{
				jobj = (JObject)jobj["badge_sets"];
				foreach (KeyValuePair<string, JToken> setKVP in jobj)
				{
					ChatBadgeSetModel set = new()
					{
						id = setKVP.Key
					};

					JObject setJObj = (JObject)setKVP.Value;
					if (setJObj.ContainsKey("versions"))
					{
						setJObj = (JObject)setJObj["versions"];
						foreach (KeyValuePair<string, JToken> versionKVP in setJObj)
						{
							ChatBadgeModel badge = versionKVP.Value.ToObject<ChatBadgeModel>();
							badge.versionID = versionKVP.Key;
							set.versions[badge.versionID] = badge;
						}
					}

					results.Add(set);
				}
			}
			return results;
		}
	}
}
