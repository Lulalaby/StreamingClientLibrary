using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;

namespace Twitch.Base.Services
{
	/// <summary>
	/// The APIs for OAuth-based services.
	/// </summary>
	public class OAuthService : TwitchServiceBase
	{
		private const string OAuthBaseAddress = "https://id.twitch.tv/";

		/// <summary>
		/// Creates an instance of the OAuthService.
		/// </summary>
		/// <param name="connection">The Twitch connection to use</param>
		public OAuthService(TwitchConnection connection) : base(connection, OAuthBaseAddress) { }

		internal OAuthService() : base(OAuthBaseAddress) { }

        /// <summary>
        /// Creates an OAuth token for authenticating with the Twitch services.
        /// </summary>
        /// <param name="clientID">The id of the client application</param>
        /// <param name="authorizationCode">The authorization code</param>
        /// <param name="redirectUrl">The URL to redirect to after authorization is complete</param>
        /// <returns>The OAuth token</returns>
        public async Task<OAuthTokenModel> GetOAuthTokenModelAsync(string clientID, string authorizationCode, string redirectUrl = null) => await GetOAuthTokenModelAsync(clientID, null, authorizationCode, redirectUrl);

        /// <summary>
        /// Creates an OAuth token for authenticating with the Twitch services.
        /// </summary>
        /// <param name="clientID">The id of the client application</param>
        /// <param name="clientSecret">The secret key of the client application</param>
        /// <param name="authorizationCode">The authorization code</param>
        /// <param name="redirectUrl">The URL to redirect to after authorization is complete</param>
        /// <returns>The OAuth token</returns>
        public async Task<OAuthTokenModel> GetOAuthTokenModelAsync(string clientID, string clientSecret, string authorizationCode, string redirectUrl = null)
		{
			Validator.ValidateString(clientID, "clientID");
			Validator.ValidateString(authorizationCode, "authorizationCode");

			Dictionary<string, string> parameters = new()
			{
				{ "client_id", clientID },
				{ "client_secret", clientSecret },
				{ "code", authorizationCode },
				{ "grant_type", "authorization_code" },
				{ "redirect_uri", redirectUrl },
			};
			FormUrlEncodedContent content = new(parameters.AsEnumerable());

			OAuthTokenModel token = await PostAsync<OAuthTokenModel>("oauth2/token?" + await content.ReadAsStringAsync(), AdvancedHttpClient.CreateContentFromObject(string.Empty), autoRefreshToken: false);
			token.clientID = clientID;
			token.clientSecret = clientSecret;
			token.authorizationCode = authorizationCode;
			return token;
		}

		/// <summary>
		/// Refreshes the specified OAuth token.
		/// </summary>
		/// <param name="token">The token to refresh</param>
		/// <returns>The refreshed token</returns>
		public async Task<OAuthTokenModel> RefreshTokenAsync(OAuthTokenModel token)
		{
			Validator.ValidateVariable(token, "token");

			Dictionary<string, string> parameters = new()
			{
				{ "client_id", token.clientID },
				{ "client_secret", token.clientSecret },
				{ "refresh_token", token.refreshToken },
				{ "grant_type", "refresh_token" },
			};
			FormUrlEncodedContent content = new(parameters.AsEnumerable());

			OAuthTokenModel newToken = await PostAsync<OAuthTokenModel>("oauth2/token?" + await content.ReadAsStringAsync(), AdvancedHttpClient.CreateContentFromObject(string.Empty), autoRefreshToken: false);
			newToken.clientID = token.clientID;
			newToken.clientSecret = token.clientSecret;
			newToken.authorizationCode = token.authorizationCode;
			return newToken;
		}
	}
}
