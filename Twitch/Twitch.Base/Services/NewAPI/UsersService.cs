using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using StreamingClient.Base.Util;
using StreamingClient.Base.Web;

using Twitch.Base.Models.NewAPI;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.Services.NewAPI
{
	/// <summary>
	/// The APIs for User-based services.
	/// </summary>
	public class UsersService : NewTwitchAPIServiceBase
	{
		/// <summary>
		/// Creates an instance of the UsersService.
		/// </summary>
		/// <param name="connection">The Twitch connection to use</param>
		public UsersService(TwitchConnection connection) : base(connection) { }

		/// <summary>
		/// Gets the current user.
		/// </summary>
		/// <returns>The resulting user</returns>
		public async Task<UserModel> GetCurrentUserAsync()
		{
			IEnumerable<UserModel> users = await GetDataResultAsync<UserModel>("users");
			return users?.FirstOrDefault();
		}

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="user">The user to get</param>
        /// <returns>The resulting user</returns>
        public async Task<UserModel> GetUserAsync(UserModel user) => await GetUserByIDAsync(user.id);

        /// <summary>
        /// Gets a user by their user ID.
        /// </summary>
        /// <param name="userID">The ID of the user</param>
        /// <returns>The user associated with the ID</returns>
        public async Task<UserModel> GetUserByIDAsync(string userID)
		{
			IEnumerable<UserModel> users = await GetUsersByIDAsync(new List<string>() { userID });
			return users.FirstOrDefault();
		}

		/// <summary>
		/// Gets a user by their login.
		/// </summary>
		/// <param name="login">The login of the user</param>
		/// <returns>The user associated with the login</returns>
		public async Task<UserModel> GetUserByLoginAsync(string login)
		{
			IEnumerable<UserModel> users = await GetUsersByLoginAsync(new List<string>() { login });
			return users.FirstOrDefault();
		}

        /// <summary>
        /// Gets the users by their user IDs.
        /// </summary>
        /// <param name="userIDs">The IDs of the users</param>
        /// <returns>The users associated with the IDs</returns>
        public async Task<IEnumerable<UserModel>> GetUsersByIDAsync(IEnumerable<string> userIDs) => await GetUsersAsync(userIDs, new List<string>());

        /// <summary>
        /// Gets the users by their logins.
        /// </summary>
        /// <param name="logins">The logins of the users</param>
        /// <returns>The users associated with the logins</returns>
        public async Task<IEnumerable<UserModel>> GetUsersByLoginAsync(IEnumerable<string> logins) => await GetUsersAsync(new List<string>(), logins);

        /// <summary>
        /// Gets the users by their user IDs &amp; logins.
        /// </summary>
        /// <param name="userIDs">The IDs of the users</param>
        /// <param name="logins">The logins of the users</param>
        /// <returns>The users associated with the IDs &amp; logins</returns>
        public async Task<IEnumerable<UserModel>> GetUsersAsync(IEnumerable<string> userIDs, IEnumerable<string> logins)
		{
			Validator.ValidateVariable(userIDs, "userIDs");
			Validator.ValidateVariable(logins, "logins");
			Validator.Validate(userIDs.Any() || logins.Any(), "At least one userID or login must be specified");

			List<string> parameters = new();
			foreach (string userID in userIDs)
			{
				parameters.Add("id=" + userID);
			}
			foreach (string login in logins)
			{
				parameters.Add("login=" + login);
			}

			return await GetDataResultAsync<UserModel>("users?" + string.Join("&", parameters));
		}

		/// <summary>
		/// Gets number of followers for a user.
		/// </summary>
		/// <param name="user">The user to search for who they follow</param>
		/// <returns>The total number of followers</returns>
		public async Task<long> GetFollowerCountAsync(UserModel user)
		{
			Validator.ValidateVariable(user, "user");
			return await GetPagedResultTotalCountAsync("users/follows?to_id=" + user.id);
		}

        /// <summary>
        /// Gets follower information for and/or to a user.
        /// </summary>
        /// <param name="from">The user to search for who they follow</param>
        /// <param name="to">The user to search for who follows them</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The follow information</returns>
        public async Task<IEnumerable<UserFollowModel>> GetFollowsAsync(UserModel from = null, UserModel to = null, int maxResults = 1) => await GetFollowsAsync(from?.id, to?.id, maxResults);

        /// <summary>
        /// Gets follower information for and/or to a user.
        /// </summary>
        /// <param name="fromID">The user to search for who they follow</param>
        /// <param name="toID">The user to search for who follows them</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The follow information</returns>
        public async Task<IEnumerable<UserFollowModel>> GetFollowsAsync(string fromID = null, string toID = null, int maxResults = 1)
		{
			Validator.Validate(!string.IsNullOrEmpty(fromID) || !string.IsNullOrEmpty(toID), "At least either fromID or toID must be specified");

			Dictionary<string, string> queryParameters = new();
			if (!string.IsNullOrEmpty(fromID))
			{
				queryParameters.Add("from_id", fromID);
			}
			if (!string.IsNullOrEmpty(toID))
			{
				queryParameters.Add("to_id", toID);
			}

			return await GetPagedDataResultAsync<UserFollowModel>("users/follows?" + string.Join("&", queryParameters.Select(kvp => kvp.Key + "=" + kvp.Value)), maxResults);
		}

		/// <summary>
		/// Updates the description of the current user.
		/// </summary>
		/// <param name="description">The description to set</param>
		/// <returns>The updated current user</returns>
		public async Task<UserModel> UpdateCurrentUserDescriptionAsync(string description)
		{
			NewTwitchAPIDataRestResult<UserModel> result = await PutAsync<NewTwitchAPIDataRestResult<UserModel>>("users?description=" + AdvancedHttpClient.URLEncodeString(description));
            return result != null && result.data != null ? result.data.FirstOrDefault() : null;
        }
    }
}
