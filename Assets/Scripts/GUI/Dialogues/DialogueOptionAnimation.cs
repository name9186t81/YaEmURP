using UnityEngine;

namespace YaEm.Dialogues
{
	[RequireComponent(typeof(DialogueOptionDisplayer))]
	public sealed class DialogueOptionAnimation : MonoBehaviour
	{
		[SerializeField] private DialogueOptionDisplayer _option;
		[SerializeField] private float _appearanceTime;
		[SerializeField] private float _timeDiff;
		private RectTransform _rectTransform;
		private Vector2 _position;
		private Vector2 _startPosition;
		private bool _enabled;
		private float _elapsed;

		private void Awake()
		{
			_option.Displayed += Displayer;
			_option.Hidden += Hidden;
			_rectTransform = _option.GetComponent<RectTransform>();	
		}

		private void Update()
		{
			if(!_enabled) { return; }

			_elapsed += Time.deltaTime;
			float delta = _elapsed / (_appearanceTime + _timeDiff * _option.Index);

			float factor = 1 - delta;
			_rectTransform.anchoredPosition = Vector2.Lerp(_startPosition, _position, 1 - factor * factor * factor);
			if(delta > 1f)
			{
				_enabled = false;
			}
		}
		private void Hidden()
		{
			_enabled = false;
		}

		private void Displayer()
		{
			_elapsed = 0f;

			_position = _rectTransform.anchoredPosition;
			_startPosition = _rectTransform.anchoredPosition -= Vector2.up * 200f;
			_enabled = true;
		}
	}
}