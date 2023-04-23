using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using StreamingClient.Base.Util;

using Twitch.Base.Models.NewAPI.Clips;
using Twitch.Base.Models.NewAPI.Games;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.Services.NewAPI
{
	/// <summary>
	/// The APIs for Clips-based services.
	/// </summary>
	public class ClipsService : NewTwitchAPIServiceBase
	{
		/// <summary>
		/// Creates an instance of the ClipsService.
		/// </summary>
		/// <param name="connection">The Twitch connection to use</param>
		public ClipsService(TwitchConnection connection) : base(connection) { }

		/// <summary>
		/// Creates a clip of the specified broadcaster.
		/// </summary>
		/// <param name="broadcaster">The broadcaster to create the clip from</param>
		/// <param name="hasDelay">Whether the clip is captured from the live stream or with a delay</param>
		/// <returns>The created clip</returns>
		public async Task<ClipCreationModel> CreateClipAsync(UserModel broadcaster, bool hasDelay = false)
		{
			Validator.ValidateVariable(broadcaster, "broadcaster");
			return (await PostDataResultAsync<ClipCreationModel>("clips?broadcaster_id=" + broadcaster.id + "&has_delay=" + hasDelay))?.FirstOrDefault();
		}

		/// <summary>
		/// Gets the clip information for a clip creation.
		/// </summary>
		/// <param name="clipCreation">The clip creation information</param>
		/// <returns>The created clip</returns>
		public async Task<ClipModel> GetClipAsync(ClipCreationModel clipCreation)
		{
			Validator.ValidateVariable(clipCreation, "clipCreation");
			return await GetClipByIDAsync(clipCreation.id);
		}

		/// <summary>
		/// Gets the clip information for the specified ID.
		/// </summary>
		/// <param name="id">The ID of the clip</param>
		/// <returns>The created clip</returns>
		public async Task<ClipModel> GetClipByIDAsync(string id)
		{
			Validator.ValidateString(id, "id");
			IEnumerable<ClipModel> clips = await GetDataResultAsync<ClipModel>("clips?id=" + id);
			return clips?.FirstOrDefault();
		}

		/// <summary>
		/// Gets the clip information for a broadcaster.
		/// </summary>
		/// <param name="broadcaster">The broadcaster to get clips for</param>
		/// <param name="startedAt">An optional start time</param>
		/// <param name="endedAt">An optional end time</param>
		/// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
		/// <returns>The clips for the broadcaster</returns>
		public async Task<IEnumerable<ClipModel>> GetBroadcasterClipsAsync(UserModel broadcaster, DateTimeOffset? startedAt = null, DateTimeOffset? endedAt = null, int maxResults = 20)
		{
			Validator.ValidateVariable(broadcaster, "broadcaster");
			return await GetClipsAsync("broadcaster_id", broadcaster.id, startedAt, endedAt, maxResults);
		}

		/// <summary>
		/// Gets the clip information for a game.
		/// </summary>
		/// <param name="game">The game to get clips for</param>
		/// <param name="startedAt">An optional start time</param>
		/// <param name="endedAt">An optional end time</param>
		/// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
		/// <returns>The clips for the broadcaster</returns>
		public async Task<IEnumerable<ClipModel>> GetGameClipsAsync(GameModel game, DateTimeOffset? startedAt = null, DateTimeOffset? endedAt = null, int maxResults = 20)
		{
			Validator.ValidateVariable(game, "game");
			return await GetClipsAsync("game_id", game.id, startedAt, endedAt, maxResults);
		}

		private async Task<IEnumerable<ClipModel>> GetClipsAsync(string typeName, string typeID, DateTimeOffset? startedAt = null, DateTimeOffset? endedAt = null, int maxResults = 20)
		{
			Dictionary<string, string> parameters = new();
			if (startedAt != null)
			{
				parameters.Add("started_at", startedAt.GetValueOrDefault().ToRFC3339String());
			}
			if (endedAt != null)
			{
				parameters.Add("endedAt", endedAt.GetValueOrDefault().ToRFC3339String());
			}

			string parameterString = (parameters.Count > 0) ? "&" + string.Join("&", parameters.Select(kvp => kvp.Key + "=" + kvp.Value)) : string.Empty;
			return await GetPagedDataResultAsync<ClipModel>("clips?" + typeName + "=" + typeID + parameterString, maxResults);
		}
	}
}
