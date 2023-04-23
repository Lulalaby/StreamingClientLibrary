namespace Twitch.Base.Services.NewAPI
{
	/// <summary>
	/// API wrapper for all New Twitch API services
	/// </summary>
	public class NewTwitchAPIServices
	{
		/// <summary>
		/// APIs for Ads interaction.
		/// </summary>
		public AdsService Ads { get; private set; }

		/// <summary>
		/// APIs for Bits interaction.
		/// </summary>
		public BitsService Bits { get; private set; }

		/// <summary>
		/// APIs for Channel Points interaction
		/// </summary>
		public ChannelPointsService ChannelPoints { get; private set; }

		/// <summary>
		/// APIs for Channels interaction.
		/// </summary>
		public ChannelsService Channels { get; private set; }

		/// <summary>
		/// APIs for Charity interaction.
		/// </summary>
		public CharityService Charity { get; private set; }

		/// <summary>
		/// APIs for Chat interaction.
		/// </summary>
		public ChatService Chat { get; private set; }

		/// <summary>
		/// APIs for Clips interaction.
		/// </summary>
		public ClipsService Clips { get; private set; }

		/// <summary>
		/// APIs for EventSub
		/// </summary>
		public EventSubService EventSub { get; private set; }

		/// <summary>
		/// APIs for Games interaction.
		/// </summary>
		public GamesService Games { get; private set; }

		/// <summary>
		/// APIs for Polls interaction.
		/// </summary>
		public PollsService Polls { get; private set; }

		/// <summary>
		/// APIs for Predictions interaction.
		/// </summary>
		public PredictionsService Predictions { get; private set; }

		/// <summary>
		/// APIs for Schedule interation.
		/// </summary>
		public ScheduleService Schedule { get; private set; }

		/// <summary>
		/// APIs for Streams interaction.
		/// </summary>
		public StreamsService Streams { get; private set; }

		/// <summary>
		/// APIs for Subscriptions interaction.
		/// </summary>
		public SubscriptionsService Subscriptions { get; private set; }

		/// <summary>
		/// APIs for Tags interaction.
		/// </summary>
		public TagsService Tags { get; private set; }

		/// <summary>
		/// APIs for Teams interaction.
		/// </summary>
		public TeamsService Teams { get; private set; }

		/// <summary>
		/// APIs for User interaction.
		/// </summary>
		public UsersService Users { get; private set; }

		/// <summary>
		/// APIs for Webhooks
		/// </summary>
		public WebhooksService Webhooks { get; private set; }

		/// <summary>
		/// Creates a new instance of the NewTwitchAPIServices class with the specified connection.
		/// </summary>
		/// <param name="connection">The Twitch connection</param>
		public NewTwitchAPIServices(TwitchConnection connection)
		{
			Ads = new AdsService(connection);
			Bits = new BitsService(connection);
			ChannelPoints = new ChannelPointsService(connection);
			Channels = new ChannelsService(connection);
			Charity = new CharityService(connection);
			Chat = new ChatService(connection);
			Clips = new ClipsService(connection);
			EventSub = new EventSubService(connection);
			Games = new GamesService(connection);
			Polls = new PollsService(connection);
			Predictions = new PredictionsService(connection);
			Schedule = new ScheduleService(connection);
			Streams = new StreamsService(connection);
			Subscriptions = new SubscriptionsService(connection);
			Tags = new TagsService(connection);
			Teams = new TeamsService(connection);
			Users = new UsersService(connection);
			Webhooks = new WebhooksService(connection);
		}
	}
}
