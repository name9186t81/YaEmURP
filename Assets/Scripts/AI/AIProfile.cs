using System;
using UnityEngine;

using Random = UnityEngine.Random;

namespace YaEm.AI
{
	[Serializable]
	public struct AIProfile
	{
		[Range(0, 1f)]
		public float Aggresivness;
		[Range(0f, 1f)]
		public float TeamWork;
		[Range(0f, 1f)]
		public float Experience;

		public static AIProfile Mix(AIProfile first, AIProfile second)
		{
			return new AIProfile()
			{
				Aggresivness = UnityEngine.Random.Range(first.Aggresivness, second.Aggresivness),
				TeamWork = UnityEngine.Random.Range(first.TeamWork, second.TeamWork),
				Experience = UnityEngine.Random.Range(first.Experience, second.Experience)
			};
		}

		public static AIProfile Mix(AIProfile first, AIProfile second, int seed)
		{
			var rnd = new System.Random(seed);

			return new AIProfile()
			{
				Aggresivness = Mathf.Lerp(Mathf.Min(first.Aggresivness, second.Aggresivness),
				Mathf.Max(first.Aggresivness, second.Aggresivness), 
				Mathf.InverseLerp(0, 1, (float)rnd.NextDouble())),

				TeamWork = Mathf.Lerp(Mathf.Min(first.TeamWork, second.TeamWork),
				Mathf.Max(first.TeamWork, second.TeamWork),
				Mathf.InverseLerp(0, 1, (float)rnd.NextDouble())),

				Experience = Mathf.Lerp(Mathf.Min(first.Experience, second.Experience),
				Mathf.Max(first.Experience, second.Experience),
				Mathf.InverseLerp(0, 1, (float)rnd.NextDouble())),
			};
		}
	}
}
