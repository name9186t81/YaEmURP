using TMPro;

using UnityEngine;

namespace Global
{
	[DisallowMultipleComponent(), RequireComponent(typeof(RectTransform))]
	public sealed class TextScaler : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _text;
		[SerializeField] private Vector2 _padding;
		private RectTransform _rect;

		private void Start()
		{
			_rect = GetComponent<RectTransform>();
			_text.ForceMeshUpdate();
			_rect.sizeDelta = _text.textBounds.size + (Vector3)_padding;
		}
	}
}