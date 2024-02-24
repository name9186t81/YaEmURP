using System.Collections.Generic;

using UnityEngine;

namespace YaEm
{
	public static class AnimationCurveExtensions
	{
		public static float Area(this AnimationCurve curve, float end = 1f, bool ignoreNegative = false)
		{
			if (curve == null) return 0f;

			const float step = 0.001f;

			float preValue = curve.Evaluate(step);
			float area = 0f;
			for (float st = step; st < end; st += step)
			{
				float current = curve.Evaluate(st);

				if (ignoreNegative && current < 0) continue;

				area += (current + preValue) * step * 0.5f;
				preValue = current;
			}

			//area += (curve.Evaluate(end) + preValue) * step * 0.5f;
			return area;
		}

		public static AnimationCurve Diff(this AnimationCurve curve, bool normalize = false, float end = 1f)
		{
			float step = 0.01f;

			List<Keyframe> keys = new List<Keyframe>();
			float prev = curve.Evaluate(0);
			float dt = step;

			float min = 999f, max = -999f;
			while (dt <= 1f)
			{
				float next = curve.Evaluate(dt);

				float dx = (next - prev) / step;
				keys.Add(new Keyframe(dt, dx));

				dt += step;
				prev = next;

				min = Mathf.Min(min, dx);
				max = Mathf.Max(max, dx);
			}

			if (normalize)
			{
				for (int i = 0; i < keys.Count; i++)
				{
					keys[i] = new Keyframe(keys[i].time, Mathf.Lerp(-1, 1, Mathf.InverseLerp(min, max, keys[i].value)));
				}
			}

			return new AnimationCurve(keys.ToArray());

		}
	}
}
