using Global;

using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace YaEm.Dialogues
{
	[RequireComponent(typeof(Button))]
	public sealed class DialogueOptionDisplayer : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _text;
		[SerializeField] private TextEffectsParser _effects;
		private int _index;
		private Button _button;
		private DialogueOption _option;

		public int Index => _index;
		public event Action<DialogueOption> Selected;
		public event Action Displayed;
		public event Action Hidden;

		private void Awake()
		{
			_button = GetComponent<Button>();
			_button.onClick.AddListener(() => Selected?.Invoke(_option));
		}

		public void SetOption(DialogueOption option)
		{
			_option = option;
			_text.text = option.Text;
			_text.ForceMeshUpdate();
			_effects.ResetEffects();
		}

		public void Display(int index)
		{
			_index = index;
			_effects.Enable();
			gameObject.SetActive(true);
			Displayed?.Invoke();
		}

		public void StopDisplaying()
		{
			_effects.Disable();
			gameObject.SetActive(false);
			Hidden?.Invoke();
		}
	}
}