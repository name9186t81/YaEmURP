using System;

using UnityEngine;
using UnityEngine.EventSystems;

namespace YaEm.GUI
{
	public sealed class HSVSquare : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
	{
		[SerializeField] private RectTransform _cursorDisplay;
		private RectTransform _selfRect;
		private bool _canMove;
		private Color _color;
		public event Action<Color> ColorChanged;

		private void Start()
		{
			_selfRect = GetComponent<RectTransform>();
			_cursorDisplay.anchorMax = Vector2.one * 0.5f;
			_cursorDisplay.anchorMin = Vector2.one * 0.5f;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!_canMove) return;
			_cursorDisplay.anchoredPosition += eventData.delta;
			UpdateColor();
		}

		public void UpdateCursor(in Color color)
		{
			Color.RGBToHSV(color, out float h, out _, out float v);
			_cursorDisplay.localPosition = (new Vector2(h, v) - Vector2.one * 0.5f) * _selfRect.sizeDelta;
			_color = color;
			ColorChanged?.Invoke(_color);
		}

		private void UpdateColor()
		{
			Vector2 delta = _cursorDisplay.localPosition / _selfRect.sizeDelta + Vector2.one * 0.5f;
			_color = Color.HSVToRGB(Mathf.Clamp(delta.x, 0.01f, 0.99f), 1, Mathf.Clamp(delta.y, 0.01f, 0.99f));
			ColorChanged?.Invoke(_color);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_canMove = true;
			_cursorDisplay.position = eventData.pointerCurrentRaycast.worldPosition;
			UpdateColor();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_canMove = false;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_canMove = false;
		}

		public Color Color => _color;
	}
}