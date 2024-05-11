using TMPro;

using UnityEngine;

namespace Global
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public abstract class TextEffect : MonoBehaviour
	{
		public enum EffectPriority
		{
			VeryLow,
			Low,
			Medium,
			High,
			VeryHigh
		}

		public enum EffectState
		{
			Running,
			Dead
		}

		[SerializeField] private EffectPriority _priority;
		private TextMeshProUGUI _text;
		public EffectState State { get; protected set; } = EffectState.Running;

		private void Awake()
		{
			_text = GetComponent<TextMeshProUGUI>();

			Init(_text);
		}

		public virtual void Init(TextMeshProUGUI text)
		{
		}

		public virtual void PlayEffect()
		{

		}

		public abstract void Reset();

		protected TextMeshProUGUI Text => _text;
		public EffectPriority Priority => _priority;
	}
}