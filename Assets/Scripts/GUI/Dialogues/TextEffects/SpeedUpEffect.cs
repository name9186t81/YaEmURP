using TMPro;

using UnityEngine;

namespace YaEm.Dialogues.Effects
{
	[CreateAssetMenu(fileName = "Speed Effect", menuName = "YaEm/Speed effect")]
	public sealed class SpeedUpEffect : TextEffectBase
	{
		[Header("Speed factor")]
		[SerializeField] private float _speed;

		public override void Use(TMP_Text text, int start, int end)
		{
			if (FadeEffect == null)
			{
				Debug.LogWarning($"text: {text.text} MUST have fadein component in order for this effect to work");
				return;
			}

			if (_speed > 0)
			{
				if (FadeEffect.Index >= start && FadeEffect.Index <= end)
				{
					FadeEffect.SpeedModififer = _speed;
				}

				if (FadeEffect.Index == end + 1)
				{
					FadeEffect.SpeedModififer = 1f;
				}
			}
			else
			{
				if (FadeEffect.Index == end)
				{
					FadeEffect.SpeedModififer = _speed;
				}

				if (FadeEffect.Index == start)
				{
					FadeEffect.SpeedModififer = 1f;
				}
			}
		}
	}
}