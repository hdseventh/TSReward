using System;
using System.Threading.Tasks;
using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Wolfje.Plugins.SEconomy;
using Wolfje.Plugins.SEconomy.Journal;

namespace TSReward
{
	[ApiVersion(2, 1)]
	public class TSReward : TerrariaPlugin
	{
		public override string Author => "Ancientgods & Enerdy";

		public Config Config { get; private set; }

		public override string Description => "Lets you claim ingame rewards for voting on Terraria-Servers.com";

		public override string Name => "TSReward";

		public RestHelper Rests { get; private set; }

		public Timer Timer { get; private set; }

		public override Version Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

		public TSReward(Main game) : base(game)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.GameInitialize.Deregister(this, onInitialize);

				if (Timer.Enabled)
				{
					Timer.Stop();
				}
			}
		}

		public override void Initialize()
		{
			ServerApi.Hooks.GameInitialize.Register(this, onInitialize);
		}

		void onInitialize(EventArgs e)
		{
			Config = Config.Read();

			Commands.ChatCommands.Add(new Command("tsreward.reload", doReload, "tsreload")
			{
				HelpText = "Reload TSReward's configuration file."
			});

			Commands.ChatCommands.Add(new Command(doReward, "reward")
			{
				AllowServer = false,
				HelpText = "Claim rewards for voting for this server at Terraria-Servers.com."
			});

			Rests = new RestHelper();

			Timer = new Timer(Config.IntervalInSeconds * 1000);
			Timer.Elapsed += (object sender, ElapsedEventArgs args) =>
			{
				Config.IntervalMessage.Send(TSPlayer.All);
			};

			if (Config.ShowIntervalMessage)
				Timer.Start();
		}

		void doReload(CommandArgs args)
		{
			Config = Config.Read();

			Timer.Interval = Config.IntervalInSeconds * 1000;
			Timer.Enabled = Config.ShowIntervalMessage;

			args.Player.SendSuccessMessage("TSReward config reloaded sucessfully.");
		}

		async void doReward(CommandArgs args)
		{
			if (!args.Player.IsLoggedIn)
			{
				args.Player.SendErrorMessage("You need to be logged in to use this command!");
				return;
			}

			if (args.Parameters.Count < 1)
			{
				foreach (var port in Config.ServerKey.Keys)
				{
					await handleRewards(args.Player, port);
				}
			}
			else
			{
				int port;
				if (!Int32.TryParse(args.Parameters[0], out port))
				{
					args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}reward [port number]",
						Commands.Specifier);
					return;
				}

				if (!Config.ServerKey.ContainsKey(port))
				{
					args.Player.SendErrorMessage($"Port {args.Parameters[0]} is not on the list!");
					return;
				}

				await handleRewards(args.Player, port);
			}
		}

		async Task handleRewards(TSPlayer player, int port)
		{
			switch (await Rests.CheckVoteAsync(Config.ServerKey[port], player.User.Name))
			{
				case Response.NotFound:
					Config.VoteNotFoundMessage.Send(player);
					return;

				case Response.VotedNotClaimed:
					Config.OnRewardClaimMessage.Send(player);

					if (await Rests.SetAsClaimedAsync(Config.ServerKey[port], player.User.Name))
					{
						if (SEconomyPlugin.Instance != null)
						{
							var playerBankAccount = SEconomyPlugin.Instance.GetBankAccount(player);
							await SEconomyPlugin.Instance.WorldAccount.TransferToAsync(
								playerBankAccount,
								Config.SEconomyReward,
								Config.AnnounceOnReceive
									? BankAccountTransferOptions.AnnounceToReceiver
									: BankAccountTransferOptions.SuppressDefaultAnnounceMessages,
								"voting on terraria-servers.com",
								"Voted on terraria-servers.com");
						}

						for (int i = 0; i < Config.Commands.Length; i++)
						{
							Commands.HandleCommand(TSPlayer.Server, Config.Commands[i].Replace("%playername%", player.Name));
						}
					}
					return;

				case Response.VotedAndClaimed:
					player.SendErrorMessage("You have already claimed your reward today!");
					return;

				case Response.InvalidServerKey:
					player.SendErrorMessage("The server key is incorrect! Please contact an administrator.");
					return;

				case Response.Error:
					player.SendErrorMessage(
						"There was an error reading your vote on terraria-servers.com. Try again later.");
					return;
			}
		}
	}
}
