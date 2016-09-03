using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace TSReward
{
	public class Message
	{
		public ColorData Color { get; set; }

		public string[] Text { get; set; }

		public Message(params string[] text)
		{
			Text = text;
			Color = ColorData.Default;
		}

		public void Send(TSPlayer player)
		{
			if (Text == null)
				return;

			for (int i = 0; i < Text.Length; i++)
			{
				player.SendMessage(Text[i], Color);
			}
		}
	}
}
