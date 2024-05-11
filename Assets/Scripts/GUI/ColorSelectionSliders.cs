using UnityEngine;
using UnityEngine.UI;

namespace YaEm.GUI
{
	public sealed class ColorSelectionSliders : MonoBehaviour
	{
		[SerializeField] private HSVSquare _hsvSquare;
		[SerializeField] private Material _sliderGradient;

		[SerializeField] private Slider _hueSlider;
		[SerializeField] private Slider _brightnessSlider;
		[SerializeField] private Image _hueBackground;
		[SerializeField] private Image _brightnessBackground;

		private void Start()
		{
			_brightnessBackground.material = _sliderGradient;
			_brightnessBackground.material.SetColor("_Color", Color.black);

			_hueSlider.minValue = 0.01f;
			_hueSlider.maxValue = 0.99f;

			_brightnessSlider.minValue = 0.01f;
			_brightnessSlider.maxValue = 0.99f;

			_hsvSquare.ColorChanged += ColorChanged;
			_hueSlider.onValueChanged.AddListener((float h) => _hsvSquare.UpdateCursor(Color.HSVToRGB(h, 1, _brightnessSlider.value)));
			_brightnessSlider.onValueChanged.AddListener((float v) => _hsvSquare.UpdateCursor(Color.HSVToRGB(_hueSlider.value, 1, v)));
		}

		private void ColorChanged(Color obj)
		{
			Color.RGBToHSV(obj, out float h, out float s, out float v);

			_hueSlider.value = h;
			_brightnessSlider.value = v;

			_hueSlider.handleRect.GetComponent<Image>().color = Color.HSVToRGB(h, 1, v);
			_brightnessSlider.handleRect.GetComponent<Image>().color = Color.HSVToRGB(1, 0, v);

			_brightnessBackground.material.SetColor("_Color2", Color.HSVToRGB(h, 1, 1));
			//_hueBackground.color = Color.Lerp(Color.black, Color.white, v);
		}
	}
}