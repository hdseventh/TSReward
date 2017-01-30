using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TSReward
{
	public class ColorData
	{
		public int R { get; set; }

		public int G { get; set; }

		public int B { get; set; }

		public ColorData(int r, int g, int b)
		{
			R = r;
			G = g;
			B = b;
		}

		public static ColorData Default => new ColorData(40, 160, 240);

		public static implicit operator Color(ColorData data) => new Color(data.R, data.G, data.B);

		public static implicit operator ColorData(Color color) => new ColorData(color.R, color.G, color.B);
	}
}
