using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using StreamingClient.Base.Util;
using StreamingClient.Base.Web;

using Twitch.Base.Models.NewAPI.Streams;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.Services.NewAPI
{
	/// <summary>
	/// The APIs for Streams-based services.
	/// </summary>
	public class StreamsService : NewTwitchAPIServiceBase
	{
		/// <summary>
		/// Creates an instance of the StreamsService.
		/// </summary>
		/// <param name="connection">The Twitch connection to use</param>
		public StreamsService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the list of top streams
        /// </summary>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>A list of streams</returns>
        public async Task<IEnumerable<StreamModel>> GetTopStreamsAsync(int maxResults = 20) => await GetStreamsAsync(maxResults: maxResults);

        /// <summary>
        /// Gets the list of streams for the specified user IDs.
        /// </summary>
        /// <param name="userIDs">The user IDs to get streams for</param>
        /// <returns>A list of streams</returns>
        public async Task<IEnumerable<StreamModel>> GetStreamsByUserIDsAsync(IEnumerable<string> userIDs)
		{
			Validator.ValidateList(userIDs, "userIDs");
			return await GetPagedDataResultAsync<StreamModel>("streams?user_id=" + string.Join("&user_id=", userIDs), userIDs.Count());
		}

		/// <summary>
		/// Gets the list of streams for the specified user logins.
		/// </summary>
		/// <param name="logins">The user logins to get streams for</param>
		/// <returns>A list of streams</returns>
		public async Task<IEnumerable<StreamModel>> GetStreamsByLoginsAsync(IEnumerable<string> logins)
		{
			Validator.ValidateList(logins, "logins");
			return await GetPagedDataResultAsync<StreamModel>("streams?user_login=" + string.Join("&user_login=", logins), logins.Count());
		}

		/// <summary>
		/// Gets the list of streams
		/// </summary>
		/// <param name="userIDs">An optional list of user IDs</param>
		/// <param name="userLogins">An optional list of user logins</param>
		/// <param name="gameIDs">An optional list of game IDs</param>
		/// <param name="languages">An optional list of languages</param>
		/// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
		/// <returns>A list of streams</returns>
		public async Task<IEnumerable<StreamModel>> GetStreamsAsync(IEnumerable<string> userIDs = null, IEnumerable<string> userLogins = null, IEnumerable<string> gameIDs = null, IEnumerable<string> languages = null, int maxResults = 20)
		{
			Dictionary<string, string> parameters = new();
			if (userIDs != null)
			{
				parameters.Add("user_id", string.Join("&user_id=", userIDs));
			}
			if (userLogins != null)
			{
				parameters.Add("user_login", string.Join("&user_login=", userLogins));
			}
			if (gameIDs != null)
			{
				parameters.Add("game_id", string.Join("&game_id=", gameIDs));
			}
			if (languages != null)
			{
				parameters.Add("language", string.Join("&language=", languages));
			}

			string parameterString = (parameters.Count > 0) ? "?" + string.Join("&", parameters.Select(kvp => kvp.Key + "=" + kvp.Value)) : string.Empty;
			return await GetPagedDataResultAsync<StreamModel>("streams" + parameterString, maxResults);
		}

		/// <summary>
		/// Gets the list of active streams from followed channels for the specified broadcaster
		/// </summary>
		/// <param name="broadcaster">The broadcaster to get followed streams for</param>
		/// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
		/// <returns>A list of streams</returns>
		public async Task<IEnumerable<StreamModel>> GetFollowedStreamsAsync(UserModel broadcaster, int maxResults = 20)
		{
			Validator.ValidateVariable(broadcaster, "broadcaster");
			return await GetPagedDataResultAsync<StreamModel>("streams/followed?user_id=" + broadcaster.id, maxResults);
		}

		/// <summary>
		/// Creates a stream marker for the specified broadcaster.
		/// </summary>
		/// <param name="broadcaster">The broadcaster to create the stream marker on</param>
		/// <param name="description">The description of the stream marker</param>
		/// <returns>The created stream marker</returns>
		public async Task<CreatedStreamMarkerModel> CreateStreamMarkerAsync(UserModel broadcaster, string description)
		{
			Validator.ValidateVariable(broadcaster, "broadcaster");
			return await PostAsync<CreatedStreamMarkerModel>("streams/markers", AdvancedHttpClient.CreateContentFromObject(new { user_id = broadcaster.id, description }));
		}
	}
}
