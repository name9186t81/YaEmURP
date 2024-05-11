using Global;

using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace YaEm.Effects
{
	[RequireComponent(typeof(TextMeshProUGUI))]	
	public sealed class WobbleTextEffect : TextEffect
	{
		[SerializeField] private string _linkName = "wobble";
		[SerializeField] private float _amplitude = 1f;
		[SerializeField] private float _frequancy = 1f;
		[SerializeField] private float _length = 0f;
		private Vector2Int[] _indexPairs;

		public override void Init(TextMeshProUGUI text)
		{
			List<Vector2Int> pairs = new List<Vector2Int>();

			foreach (var link in text.textInfo.linkInfo)
			{
				if (link.GetLinkID() != _linkName) continue;

				pairs.Add(new Vector2Int(link.linkTextfirstCharacterIndex, link.linkTextfirstCharacterIndex + link.linkTextLength));
			}

			if(pairs.Count == 0)
			{
				Debug.LogWarning("No wobble links found in " + text + " component");
			}

			_indexPairs = pairs.ToArray();
		}

		public override void PlayEffect()
		{
			var textInfo = Text.textInfo;

			for (int i = 0, length = _indexPairs.Length; i < length; ++i)
			{
				Vector2Int vector = _indexPairs[i];

				for (int j = vector[0]; j < vector[1]; ++j)
				{
					var charInfo = textInfo.characterInfo[j];

					if (!charInfo.isVisible) continue;

					var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

					for (int k = 0; k < 4; ++k)
					{
						int ind = charInfo.vertexIndex + k;
						var orig = verts[ind];

						verts[ind] = orig + Vector3.up * (Mathf.Sin(Time.time * _frequancy + orig.x * 0.01f * _length) * _amplitude);
					}

				}
			}
		}

		public override void Reset()
		{
			Init(Text);
		}
	}
}