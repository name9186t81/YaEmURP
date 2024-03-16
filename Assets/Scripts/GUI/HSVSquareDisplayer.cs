using UnityEngine;
using UnityEngine.UI;

namespace YaEm.GUI
{
	public sealed class HSVSquareDisplayer : MonoBehaviour
	{
		[SerializeField] private HSVSquare _square;
		[SerializeField] private Image _image;

		private void Start()
		{
			_square.ColorChanged += UpdateColor;
		}

		private void UpdateColor(Color obj)
		{
			_image.color = obj;
		}
	}
}