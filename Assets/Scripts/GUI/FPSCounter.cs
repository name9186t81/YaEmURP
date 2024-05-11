using TMPro;

using UnityEngine;

namespace YaEm.GUI
{
	public sealed class FPSCounter : MonoBehaviour
	{
		[SerializeField] private int _times;
		[SerializeField] private TextMeshProUGUI _text;
		private float[] _fps;
		private int _index;

		private void Awake()
		{
			_fps = new float[_times];
			Application.targetFrameRate = 180;
			QualitySettings.vSyncCount = 0;
		}

		private void Update()
		{
			float fps = 1 / Time.deltaTime;
			_fps[_index++ % _times] = fps;

			float sum = 0f;
			for(int i = 0; i < _times; ++i)
			{
				sum += _fps[i];
			}
			sum /= _times;
			_text.text = ((int)sum).ToString();

			_text.color = Color.Lerp(Color.red, Color.Lerp(Color.yellow, Color.green, Mathf.InverseLerp(90, 150, sum)), Mathf.InverseLerp(30, 90, sum));
		}
	}
}