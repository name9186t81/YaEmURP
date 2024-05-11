using System;

using UnityEngine;
using UnityEngine.UI;

namespace YaEm.GUI
{
	public sealed class TeamColorSelection : MonoBehaviour
	{
		private sealed class Data
		{
			public readonly Button Button;

			private int _index;
			private Color _color;
			public event Action<int> Selected;

			public Data(Button button, Color color, int index)
			{
				Button = button;
				_color = color;
				_index = index;

				button.onClick.AddListener(() => Selected?.Invoke(_index));
			}

			public int Index => _index;
			public Color Color { get => _color; set => _color = value; }
		}

		[SerializeField] private HSVSquare _square;
		[SerializeField] private Button[] _buttons;
		[SerializeField] private Button _resetButton;
		private Data _selected;
		private Data[] _datas;

		private void Start()
		{
			_datas = new Data[_buttons.Length];

			for(int i = 0; i < _datas.Length; i++)
			{
				_datas[i] = new Data(_buttons[i], ServiceLocator.Get<ColorTable>().GetColor(i + 1), i);
				_datas[i].Selected += Selected;
				_buttons[i].GetComponent<Image>().color = _datas[i].Color = ColorTable.GetDefaultColor(i + 1);
			}
			_selected = _datas[0];

			_square.ColorChanged += UpdateColor;
			_resetButton.onClick.AddListener(() => {
				UpdateColor(ColorTable.GetDefaultColor(_selected.Index + 1));
				_square.UpdateCursor(_selected.Color);
			});
		}

		private void UpdateColor(Color obj)
		{
			_selected.Color = obj;
			_selected.Button.GetComponent<Image>().color = obj;
			ServiceLocator.Get<ColorTable>().SetColor(_selected.Index + 1, obj);
		}

		private void Selected(int obj)
		{
			_selected = _datas[obj];
			_square.UpdateCursor(_selected.Color);
		}
	}
}