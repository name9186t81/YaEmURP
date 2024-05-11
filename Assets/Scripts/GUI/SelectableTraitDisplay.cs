using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace YaEm.GUI
{
	[DisallowMultipleComponent(), RequireComponent(typeof(Button))]
	public sealed class SelectableTraitDisplay : MonoBehaviour
	{
		[SerializeField] private Image _selectBox;
		[SerializeField] private Color _selectColor;
		[SerializeField] private Color _unselectColor;
		[SerializeField] private Image _image;
		[SerializeField] private TextMeshProUGUI _text;
		public event Action<string> OnClick;
		private GameObject _prefab;
		private string _value;

		private void Start()
		{
			if(TryGetComponent<Button>(out var button))
			{
				button.onClick.AddListener(() => OnClick?.Invoke(_value));
			}
			else
			{
				Debug.LogWarning("WHERE IS MY BUTTON?!");
			}
		}

		public void UpdateDisplay(Sprite sprite, string text, string value)
		{
			_image.sprite = sprite;
			_text.text = text;
			_value = value;
		}

		public void Select()
		{
			_selectBox.color = _selectColor;
		}

		public void UnSelect()
		{
			_selectBox.color = _unselectColor;
		}

		public Color Color { get { return _image.color; } set { _image.color = value; } }
		public string Value => _value;
	}
}