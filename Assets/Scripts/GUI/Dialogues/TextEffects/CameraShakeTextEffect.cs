using TMPro;

using UnityEngine;

using YaEm.GUI;

namespace YaEm.Dialogues.Effects
{
	[CreateAssetMenu(fileName = "Camera shake Effect", menuName = "YaEm/Camera shake effect")]
	public sealed class CameraShakeTextEffect : TextEffectBase
	{
		[Header("Shake settings")]
		[SerializeField] private float _strength;
		[SerializeField] private CameraShake.ShakeType _shakeType;

		public override void Use(TMP_Text text, int start, int end)
		{
			if(FadeEffect != null && FadeEffect.Index == start)
			{
				CameraShake.Shake(_strength, _shakeType);
			}
		}
	}
}