using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Twitch.Base.Models.NewAPI;

namespace Twitch.Base.Services.NewAPI
{
	/// <summary>
	/// REST services against the New Twitch API
	/// </summary>
	public class NewTwitchAPIServiceBase : TwitchServiceBase
	{
		/// <summary>
		/// The New Twitch API base address.
		/// </summary>
		public const string BASE_ADDRESS = "https://api.twitch.tv/helix/";

		/// <summary>
		/// Creates an instance of the NewTwitchAPIServiceBase.
		/// </summary>
		/// <param name="connection">The Twitch connection to use</param>
		public NewTwitchAPIServiceBase(TwitchConnection connection) : base(connection, BASE_ADDRESS) { }

		/// <summary>
		/// Performs a GET REST request using the provided request URI for New Twitch API-wrapped data.
		/// </summary>
		/// <param name="requestUri">The request URI to use</param>
		/// <returns>A type-casted object of the contents of the response</returns>
		public async Task<IEnumerable<T>> GetDataResultAsync<T>(string requestUri)
		{
			NewTwitchAPIDataRestResult<T> result = await GetAsync<NewTwitchAPIDataRestResult<T>>(requestUri);
            return result != null && result.data != null && result.data.Count > 0 ? result.data : (IEnumerable<T>)new List<T>();
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI for New Twitch API-wrapped data to get total count.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <returns>The total count of the response</returns>
        public async Task<long> GetPagedResultTotalCountAsync(string requestUri)
		{
			if (!requestUri.Contains('?'))
			{
				requestUri += "?";
			}
			else
			{
				requestUri += "&";
			}
			requestUri += "first=1";

			JObject data = await GetJObjectAsync(requestUri);
            return data != null && data.ContainsKey("total") ? (long)data["total"] : 0;
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI for New Twitch API-wrapped data.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<IEnumerable<T>> GetPagedDataResultAsync<T>(string requestUri, int maxResults = 1)
		{
			if (!requestUri.Contains('?'))
			{
				requestUri += "?";
			}
			else
			{
				requestUri += "&";
			}

			Dictionary<string, string> queryParameters = new()
            {
				{ "first", ((maxResults > 100) ? 100 : maxResults).ToString() }
			};

			List<T> results = new();
			string cursor = null;
			do
			{
				if (!string.IsNullOrEmpty(cursor))
				{
					queryParameters["after"] = cursor;
				}
				NewTwitchAPIDataRestResult<T> data = await GetAsync<NewTwitchAPIDataRestResult<T>>(requestUri + string.Join("&", queryParameters.Select(kvp => kvp.Key + "=" + kvp.Value)));

				cursor = null;
				if (data != null && data.data != null && data.data.Count > 0)
				{
					results.AddRange(data.data);
					cursor = data.Cursor;
				}
			}
			while (results.Count < maxResults && !string.IsNullOrEmpty(cursor));

			return results;
		}


		/// <summary>
		///  Performs a GET REST request using the provided request URI for New Twitch API-wrapped data that has a single data result.
		/// </summary>
		/// <param name="requestUri">The request URI to use</param>
		/// <param name="maxResults">Maximum number of items per page of results</param>
		/// <param name="cursor">Pagination cursor</param>
		/// <returns>A single data node result set object of the response</returns>
		public async Task<NewTwitchAPISingleDataRestResult<T>> GetPagedSingleDataResultAsync<T>(string requestUri, int maxResults, string cursor = null)
		{
			if (!requestUri.Contains('?'))
			{
				requestUri += "?";
			}
			else
			{
				requestUri += "&";
			}

			Dictionary<string, string> queryParameters = new()
            {
				{ "first", maxResults.ToString() }
			};
			if (!string.IsNullOrEmpty(cursor))
			{
				queryParameters["after"] = cursor;
			}
			return await GetAsync<NewTwitchAPISingleDataRestResult<T>>(requestUri + string.Join("&", queryParameters.Select(kvp => kvp.Key + "=" + kvp.Value)));
		}

		/// <summary>
		/// Performs a POST REST request using the provided request URI for New Twitch API-wrapped data.
		/// </summary>
		/// <param name="requestUri">The request URI to use</param>
		/// <returns>A type-casted object of the contents of the response</returns>
		public async Task<T[]> PostDataResultAsync<T>(string requestUri)
		{
			NewTwitchAPIDataRestResult<T> result = await PostAsync<NewTwitchAPIDataRestResult<T>>(requestUri);
            return result != null && result.data != null && result.data.Count > 0 ? result.data.ToArray() : null;
        }

        /// <summary>
        /// Performs a POST REST request using the provided request URI for New Twitch API-wrapped data.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The post body content</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T[]> PostDataResultAsync<T>(string requestUri, HttpContent content)
		{
			NewTwitchAPIDataRestResult<T> result = await PostAsync<NewTwitchAPIDataRestResult<T>>(requestUri, content);
            return result != null && result.data != null && result.data.Count > 0 ? result.data.ToArray() : null;
        }
    }
}
