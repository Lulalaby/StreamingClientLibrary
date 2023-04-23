using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using StreamingClient.Base.Util;
using StreamingClient.Base.Web;

using Twitch.Base.Models.NewAPI.Channels;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.Services.NewAPI
{
	/// <summary>
	/// The APIs for Channels-based services.
	/// </summary>
	public class ChannelsService : NewTwitchAPIServiceBase
	{
		/// <summary>
		/// Creates an instance of the ChannelsService.
		/// </summary>
		/// <param name="connection">The Twitch connection to use</param>
		public ChannelsService(TwitchConnection connection) : base(connection) { }

		/// <summary>
		/// Gets channel information for the specified user.
		/// </summary>
		/// <param name="user">The user to get channel information for</param>
		/// <returns>The channel information</returns>
		public async Task<ChannelInformationModel> GetChannelInformationAsync(UserModel user)
		{
			Validator.ValidateVariable(user, "user");
			IEnumerable<ChannelInformationModel> results = await GetDataResultAsync<ChannelInformationModel>("channels?broadcaster_id=" + user.id);
			return results.FirstOrDefault();
		}

		/// <summary>
		/// Updates channel information for the specified channel information.
		/// </summary>
		/// <param name="channelInformation">The channel to update channel information for</param>
		/// <param name="title">The optional title to update to</param>
		/// <param name="gameID">The optional game ID to update to</param>
		/// <param name="broadcasterLanguage">The optional broadcast language to update to</param>
		/// <returns>Whether the update was successful or not</returns>
		public async Task<bool> UpdateChannelInformationAsync(ChannelInformationModel channelInformation, string title = null, string gameID = null, string broadcasterLanguage = null)
		{
			Validator.ValidateVariable(channelInformation, "channelInformation");
			return await UpdateChannelInformationAsync(channelInformation.broadcaster_id, title, gameID, broadcasterLanguage);
		}

		/// <summary>
		/// Updates channel information for the specified user.
		/// </summary>
		/// <param name="channel">The channel to update information for</param>
		/// <param name="title">The optional title to update to</param>
		/// <param name="gameID">The optional game ID to update to</param>
		/// <param name="broadcasterLanguage">The optional broadcast language to update to</param>
		/// <returns>Whether the update was successful or not</returns>
		public async Task<bool> UpdateChannelInformationAsync(UserModel channel, string title = null, string gameID = null, string broadcasterLanguage = null)
		{
			Validator.ValidateVariable(channel, "channel");
			return await UpdateChannelInformationAsync(channel.id, title, gameID, broadcasterLanguage);
		}

		private async Task<bool> UpdateChannelInformationAsync(string broadcasterID, string title = null, string gameID = null, string broadcasterLanguage = null)
		{
			JObject jobj = new();
			if (!string.IsNullOrEmpty(title))
			{ jobj["title"] = title; }
			if (!string.IsNullOrEmpty(gameID))
			{ jobj["game_id"] = gameID; }
			if (!string.IsNullOrEmpty(broadcasterLanguage))
			{ jobj["broadcaster_language"] = broadcasterLanguage; }
			HttpResponseMessage response = await PatchAsync("channels?broadcaster_id=" + broadcasterID, AdvancedHttpClient.CreateContentFromObject(jobj));
			return response.IsSuccessStatusCode;
		}

		/// <summary>
		/// Gets the most recent banned events for the specified channel.
		/// </summary>
		/// <param name="channel">The channel to get banned events for</param>
		/// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
		/// <returns>The set of banned events</returns>
		public async Task<IEnumerable<ChannelBannedEventModel>> GetChannelBannedEventsAsync(UserModel channel, int maxResults = 1)
		{
			Validator.ValidateVariable(channel, "channel");
			return await GetPagedDataResultAsync<ChannelBannedEventModel>("moderation/banned/events?broadcaster_id=" + channel.id, maxResults);
		}

		/// <summary>
		/// Returns all banned and timed-out users in a channel.
		/// </summary>
		/// <param name="channel">The channel to get banned and timed-out users for</param>
		/// <param name="userIDs">If specified, filters banned and timed-out users to those userIDs specified.</param>
		/// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
		/// <returns>The set of banned or timed-out users</returns>
		public async Task<IEnumerable<ChannelBannedUserModel>> GetChannelBannedUsersAsync(UserModel channel, IEnumerable<string> userIDs = null, int maxResults = 1)
		{
			Validator.ValidateVariable(channel, "channel");
			List<string> parameters = new();
			if (userIDs != null)
			{
				foreach (string userID in userIDs)
				{
					parameters.Add("user_id=" + userID);
				}
			}
			parameters.Add("broadcaster_id=" + channel.id);
			return await GetPagedDataResultAsync<ChannelBannedUserModel>("moderation/banned?" + string.Join("&", parameters), maxResults);
		}

		/// <summary>
		/// Gets the most recent moderator events for the specified channel.
		/// </summary>
		/// <param name="channel">The channel to get moderator events for</param>
		/// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
		/// <returns>The set of moderator events</returns>
		public async Task<IEnumerable<ChannelModeratorEventModel>> GetChannelModeratorEventsAsync(UserModel channel, int maxResults = 1)
		{
			Validator.ValidateVariable(channel, "channel");
			return await GetPagedDataResultAsync<ChannelModeratorEventModel>("moderation/moderators/events?broadcaster_id=" + channel.id, maxResults);
		}

		/// <summary>
		/// Returns all moderator users in a channel.
		/// </summary>
		/// <param name="channel">The channel to get moderators for</param>
		/// <param name="userIDs">If specified, filters moderator users to those userIDs specified.</param>
		/// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
		/// <returns>The set of moderator users</returns>
		public async Task<IEnumerable<ChannelModeratorUserModel>> GetChannelModeratorUsersAsync(UserModel channel, IEnumerable<string> userIDs = null, int maxResults = 1)
		{
			Validator.ValidateVariable(channel, "channel");
			List<string> parameters = new();
			if (userIDs != null)
			{
				foreach (string userID in userIDs)
				{
					parameters.Add("user_id=" + userID);
				}
			}
			parameters.Add("broadcaster_id=" + channel.id);
			return await GetPagedDataResultAsync<ChannelModeratorUserModel>("moderation/moderators?" + string.Join("&", parameters), maxResults);
		}

		/// <summary>
		/// Gets the list of channel editors for the specified channel.
		/// </summary>
		/// <param name="channel">The channel to get channel editors for</param>
		/// <returns>The list of channel editors</returns>
		public async Task<IEnumerable<ChannelEditorUserModel>> GetChannelEditorUsersAsync(UserModel channel)
		{
			Validator.ValidateVariable(channel, "channel");
			return await GetDataResultAsync<ChannelEditorUserModel>("channels/editors?broadcaster_id=" + channel.id);
		}

		/// <summary>
		/// Gets the information of the most recent Hype Train of the given channel ID.
		/// </summary>
		/// <param name="channel">The channel to get Hype Train data for</param>
		/// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
		/// <returns>The most recent Hype Train</returns>
		public async Task<IEnumerable<ChannelHypeTrainModel>> GetHypeTrainEventsAsync(UserModel channel, int maxResults = 1)
		{
			Validator.ValidateVariable(channel, "channel");
			return await GetPagedDataResultAsync<ChannelHypeTrainModel>($"hypetrain/events?broadcaster_id={channel.id}", maxResults);
		}
	}
}
