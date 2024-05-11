using TMPro;

using UnityEngine;

namespace YaEm.Dialogues.Effects
{
	[CreateAssetMenu(fileName = "Pause Effect", menuName = "YaEm/Pause effect")]
	public sealed class PauseEffect : TextEffectBase
	{
		[Header("Pause time")]
		[SerializeField] private float _time;

		public override void Use(TMP_Text text, int start, int end)
		{
			if (FadeEffect == null)
			{
				Debug.LogWarning($"text: {text.text} MUST have fadein component in order for this effect to work");
				return;
			}

			if(!FadeEffect.Paused && FadeEffect.Index == end + 1)
			{
				FadeEffect.Pause(_time);
			}
		}
	}
}