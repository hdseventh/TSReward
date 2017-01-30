using System;
using System.Threading.Tasks;
using System.Net;

namespace TSReward
{
	public enum Response
	{
		NotFound = 0,
		VotedNotClaimed = 1,
		VotedAndClaimed = 2,
		InvalidServerKey = 3,
		Error = 4
	}

	public class RestHelper
	{
		public async Task<Response> CheckVoteAsync(string key, string username)
		{
			try
			{
				string response;

				using (var wc = new WebClient())
				{
					response = await wc.DownloadStringTaskAsync($"https://terraria-servers.com/api/?object=votes&element=claim&key={key}&username={username}");
				}

				if (response.Contains("invalid server key"))
				{
					return Response.InvalidServerKey;
				}
				else
				{
					return (Response)Int32.Parse(response);
				}
			}
			catch
			{
				return Response.Error;
			}
		}

		public async Task<bool> SetAsClaimedAsync(string key, string username)
		{
			try
			{
				using (var wc = new WebClient())
				{
					return await wc.DownloadStringTaskAsync($"http://terraria-servers.com/api/?action=post&object=votes&element=claim&key={key}&username={username}") == "1";
				}
			}
			catch
			{
				return false;
			}
		}
	}
}
