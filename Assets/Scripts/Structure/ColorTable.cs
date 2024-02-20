using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace YaEm
{
	public static class ColorTable
	{
		public static Color GetColor(int teamNumber)
		{
			switch (teamNumber)
			{
				case 1: return Color.green;
				case 2: return Color.red;
				case 3: return Color.blue;
				case 4: return Color.yellow;
				case 5: return Color.cyan;
				default: return Color.white;
			}
		}
	}
}
