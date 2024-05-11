using TMPro;

using UnityEngine;

namespace YaEm.Dialogues.Effects
{
	[CreateAssetMenu(fileName = "Shake Effect", menuName = "YaEm/Shake effect")]
	public sealed class TextShakeEffect : TextEffectBase
	{
		[Header("Wave settings")]
		[SerializeField] private float _frequancy;
		[SerializeField] private float _length;
		[SerializeField] private float _angleOffset;

		public override void Use(TMP_Text text, int start, int end)
		{
			var textInfo = text.textInfo;

			for (int i = start; i <= end; ++i)
			{
				var charInfo = textInfo.characterInfo[i];
				if (!charInfo.isVisible) continue;

				var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
				for (int j = 0; j < 4; ++j)
				{
					int ind = charInfo.vertexIndex + j;
					var orig = verts[ind];

					verts[ind] = orig + (Vector3)((_angleOffset * i + Time.realtimeSinceStartup * _frequancy).VectorFromAngle() * _length);
				}
			}
		}
	}
}