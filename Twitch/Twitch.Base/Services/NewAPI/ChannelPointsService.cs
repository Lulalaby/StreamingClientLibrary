using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using StreamingClient.Base.Util;
using StreamingClient.Base.Web;

using Twitch.Base.Models.NewAPI;
using Twitch.Base.Models.NewAPI.ChannelPoints;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.Services.NewAPI
{
	/// <summary>
	/// The APIs for Channel Points-based services.
	/// </summary>
	public class ChannelPointsService : NewTwitchAPIServiceBase
	{
		private class CustomChannelPointRewardRedemptionUpdateModel
		{
			public string status { get; set; }
		}

		/// <summary>
		/// Creates an instance of the ChannelPointsService.
		/// </summary>
		/// <param name="connection">The Twitch connection to use</param>
		public ChannelPointsService(TwitchConnection connection) : base(connection) { }

		/// <summary>
		/// Creates a new reward associated with the broadcaster with the specified information.
		/// </summary>
		/// <param name="broadcaster">The broadcaster to create the reward for</param>
		/// <param name="reward">The reward information</param>
		/// <returns>The created reward</returns>
		public async Task<CustomChannelPointRewardModel> CreateCustomRewardAsync(UserModel broadcaster, UpdatableCustomChannelPointRewardModel reward)
		{
			Validator.ValidateVariable(broadcaster, "broadcaster");
			Validator.ValidateVariable(reward, "reward");

			NewTwitchAPIDataRestResult<CustomChannelPointRewardModel> result = await PostAsync<NewTwitchAPIDataRestResult<CustomChannelPointRewardModel>>("channel_points/custom_rewards?broadcaster_id=" + broadcaster.id, AdvancedHttpClient.CreateContentFromObject(reward));
            return result != null && result.data != null ? result.data.FirstOrDefault() : null;
        }

        /// <summary>
        /// Deletes the specified reward associated with the broadcaster.
        /// </summary>
        /// <param name="broadcaster">The broadcaster to get rewards for</param>
        /// <param name="rewardID">The specific reward to get information for</param>
        /// <returns>Whether the reward was deleted</returns>
        public async Task<bool> DeleteCustomRewardAsync(UserModel broadcaster, Guid rewardID)
		{
			Validator.ValidateVariable(broadcaster, "broadcaster");
			Validator.ValidateGuid(rewardID, "rewardID");
			return await DeleteAsync($"channel_points/custom_rewards?broadcaster_id={broadcaster.id}&id={rewardID}");
		}

		/// <summary>
		/// Gets all rewards associated with the broadcaster.
		/// </summary>
		/// <param name="broadcaster">The broadcaster to get rewards for</param>
		/// <param name="managableRewardsOnly">Whether to return only rewards manageable by the current client application</param>
		/// <returns>The reward information</returns>
		public async Task<IEnumerable<CustomChannelPointRewardModel>> GetCustomRewardsAsync(UserModel broadcaster, bool managableRewardsOnly = false)
		{
			Validator.ValidateVariable(broadcaster, "broadcaster");
			return await GetPagedDataResultAsync<CustomChannelPointRewardModel>("channel_points/custom_rewards?broadcaster_id=" + broadcaster.id + "&only_manageable_rewards=" + managableRewardsOnly, maxResults: int.MaxValue);
		}

		/// <summary>
		/// Gets the specified reward associated with the broadcaster.
		/// </summary>
		/// <param name="broadcaster">The broadcaster to get rewards for</param>
		/// <param name="rewardID">The specific reward to get information for</param>
		/// <returns>The reward information</returns>
		public async Task<CustomChannelPointRewardModel> GetCustomRewardAsync(UserModel broadcaster, Guid rewardID)
		{
			Validator.ValidateVariable(broadcaster, "broadcaster");
			Validator.ValidateGuid(rewardID, "rewardID");

			IEnumerable<CustomChannelPointRewardModel> result = await GetPagedDataResultAsync<CustomChannelPointRewardModel>($"channel_points/custom_rewards?broadcaster_id={broadcaster.id}&id={rewardID}", maxResults: int.MaxValue);
            return result != null && result != null ? result.FirstOrDefault() : null;
        }

        /// <summary>
        /// Updates an existing reward associated with the broadcaster with the specified information.
        /// </summary>
        /// <param name="broadcaster">The broadcaster to create the reward for</param>
        /// <param name="rewardID">The ID of the reward</param>
        /// <param name="reward">The reward information</param>
        /// <returns>The updated reward</returns>
        public async Task<CustomChannelPointRewardModel> UpdateCustomRewardAsync(UserModel broadcaster, Guid rewardID, UpdatableCustomChannelPointRewardModel reward)
		{
			Validator.ValidateVariable(broadcaster, "broadcaster");
			Validator.ValidateGuid(rewardID, "rewardID");
			Validator.ValidateVariable(reward, "reward");

			return await UpdateCustomRewardAsync(broadcaster, rewardID, JObject.FromObject(reward));
		}

		/// <summary>
		/// Updates an existing reward associated with the broadcaster with the specified information.
		/// </summary>
		/// <param name="broadcaster">The broadcaster to create the reward for</param>
		/// <param name="rewardID">The ID of the reward</param>
		/// <param name="updatableFields">The reward information to update</param>
		/// <returns>The updated reward</returns>
		public async Task<CustomChannelPointRewardModel> UpdateCustomRewardAsync(UserModel broadcaster, Guid rewardID, JObject updatableFields)
		{
			Validator.ValidateVariable(broadcaster, "broadcaster");
			Validator.ValidateGuid(rewardID, "rewardID");
			Validator.ValidateVariable(updatableFields, "updatableFields");

			NewTwitchAPIDataRestResult<CustomChannelPointRewardModel> result = await PatchAsync<NewTwitchAPIDataRestResult<CustomChannelPointRewardModel>>($"channel_points/custom_rewards?broadcaster_id={broadcaster.id}&id={rewardID}", AdvancedHttpClient.CreateContentFromObject(updatableFields));
            return result != null && result.data != null ? result.data.FirstOrDefault() : null;
        }

        /// <summary>
        /// Gets the redemptions for the specified reward associated with the broadcaster.
        /// </summary>
        /// <param name="broadcaster">The broadcaster to get rewards for</param>
        /// <param name="rewardID">The specific reward to get information for</param>
        /// <param name="status">Filters the paginated Custom Reward Redemption objects for redemptions with the matching status. Can be one of UNFULFILLED, FULFILLED or CANCELED</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The reward redemptions</returns>
        public async Task<IEnumerable<CustomChannelPointRewardRedemptionModel>> GetCustomRewardRedemptionsAsync(UserModel broadcaster, Guid rewardID, string status, int maxResults = 20)
		{
			Validator.ValidateVariable(broadcaster, "broadcaster");
			Validator.ValidateGuid(rewardID, "rewardID");
			Validator.ValidateString(status, "status");
			return await GetPagedDataResultAsync<CustomChannelPointRewardRedemptionModel>($"channel_points/custom_rewards/redemptions?broadcaster_id={broadcaster.id}&reward_id={rewardID}&status={status}&sort=NEWEST", maxResults: maxResults);
		}

		/// <summary>
		/// Updates the specified redemption for the specified reward associated with the broadcaster.
		/// </summary>
		/// <param name="broadcaster">The broadcaster to update rewards for</param>
		/// <param name="rewardID">ID of the Custom Reward the redemptions to be updated are for.</param>
		/// <param name="id">ID of the Custom Reward Redemption to update</param>
		/// <param name="status">The new status for the reward. Can be FULFILLED or CANCELED</param>
		/// <returns>The reward redemptions</returns>
		public async Task<CustomChannelPointRewardRedemptionModel> UpdateCustomRewardRedemptionsAsync(UserModel broadcaster, Guid rewardID, Guid id, string status)
		{
			Validator.ValidateVariable(broadcaster, "broadcaster");
			Validator.ValidateGuid(rewardID, "rewardID");
			Validator.ValidateGuid(id, "id");
			Validator.ValidateString(status, "status");

			CustomChannelPointRewardRedemptionUpdateModel redemption = new()
            {
				status = status
			};

			NewTwitchAPIDataRestResult<CustomChannelPointRewardRedemptionModel> result = await PatchAsync<NewTwitchAPIDataRestResult<CustomChannelPointRewardRedemptionModel>>($"channel_points/custom_rewards/redemptions?broadcaster_id={broadcaster.id}&reward_id={rewardID}&id={id}", AdvancedHttpClient.CreateContentFromObject(redemption));
            return result != null && result.data != null ? result.data.FirstOrDefault() : null;
        }
    }
}
