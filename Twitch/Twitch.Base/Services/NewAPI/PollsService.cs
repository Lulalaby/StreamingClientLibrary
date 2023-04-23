using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using StreamingClient.Base.Util;
using StreamingClient.Base.Web;

using Twitch.Base.Models.NewAPI.Polls;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.Services.NewAPI
{
	/// <summary>
	/// The APIs for Polls-based services.
	/// </summary>
	public class PollsService : NewTwitchAPIServiceBase
	{
		/// <summary>
		/// Creates an instance of the PollsService
		/// </summary>
		/// <param name="connection">The Twitch connection to use</param>
		public PollsService(TwitchConnection connection) : base(connection) { }

		/// <summary>
		/// Creates a poll.
		/// </summary>
		/// <param name="poll">The poll to create</param>
		/// <returns>The created poll</returns>
		public async Task<PollModel> CreatePollAsync(CreatePollModel poll)
		{
			Validator.ValidateVariable(poll, "poll");
			return (await PostDataResultAsync<PollModel>("polls", AdvancedHttpClient.CreateContentFromObject(poll)))?.FirstOrDefault();
		}

		/// <summary>
		/// Gets the poll for the specified broadcaster and poll ID.
		/// </summary>
		/// <param name="broadcaster">The broadcaster to get the poll for</param>
		/// <param name="id">The ID of the poll to get</param>
		/// <returns>The poll</returns>
		public async Task<PollModel> GetPollAsync(UserModel broadcaster, string id)
		{
			Validator.ValidateVariable(broadcaster, "broadcaster");
			Validator.ValidateString(id, "id");
			return (await GetDataResultAsync<PollModel>("polls?broadcaster_id=" + broadcaster.id + "&id=" + id))?.FirstOrDefault();
		}

		/// <summary>
		/// Ends the poll for the specified broadcaster and poll ID.
		/// </summary>
		/// <param name="broadcaster">The broadcaster to get the poll for</param>
		/// <param name="id">The ID of the poll to get</param>
		/// <returns>The poll</returns>
		public async Task<PollModel> EndPollAsync(UserModel broadcaster, string id)
		{
			Validator.ValidateVariable(broadcaster, "broadcaster");
			Validator.ValidateString(id, "id");

			JObject jobj = new()
            {
				["broadcaster_id"] = broadcaster.id,
				["id"] = id,
				["status"] = "TERMINATED"
			};

			return (await PostDataResultAsync<PollModel>("polls", AdvancedHttpClient.CreateContentFromObject(jobj)))?.FirstOrDefault();
		}
	}
}
