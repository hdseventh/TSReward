using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using System.IO;
using Newtonsoft.Json;

namespace TSReward
{
	public class Config
	{
		public Dictionary<int, string> ServerKey { get; set; } = new Dictionary<int, string>
		{
			[TShock.Config.ServerPort] = "key1"
		};

		public int SEconomyReward { get; set; } = 1000;

		public bool AnnounceOnReceive { get; set; } = true;

		public string[] Commands { get; set; } = new[]
		{
			"/heal %playername%",
			"/firework %playername%"
		};

		public Message VoteNotFoundMessage { get; set; } = new Message(
			"Vote not found!",
			"If you haven't voted yet, please go to terraria-servers.com",
			"and vote for the server to receive ingame rewards!"
			);

		public Message OnRewardClaimMessage { get; set; } = new Message(
			"Thank you for voting on terraria-servers.com",
			"We really appreciate it!"
			);

		public bool ShowIntervalMessage { get; set; } = true;

		public int IntervalInSeconds { get; set; } = 300;

		public Message IntervalMessage { get; set; } = new Message(
			"Vote on terraria-servers.com and receive 1000 coins!",
			"After voting you can use the command /reward!"
			);

		public static Config Read()
		{
			string filepath = Path.Combine(TShock.SavePath, "TSReward.json");
			try
			{
				Config config = new Config();

				if (File.Exists(filepath))
				{
					config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(filepath));
				}

				File.WriteAllText(filepath, JsonConvert.SerializeObject(config, Formatting.Indented));
				return config;
			}
			catch (Exception ex)
			{
				TShock.Log.ConsoleError(ex.ToString());
				return new Config();
			}
		}
	}
}
