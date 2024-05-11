using TMPro;

using UnityEngine;

namespace YaEm.Dialogues
{
	[DisallowMultipleComponent()]
	public sealed class DialogueDisplayer : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _text;
		[SerializeField] private TextEffectsParser _effects;
		[SerializeField] private DialogueOptionDisplayer _prefab;
		[SerializeField] private RectTransform[] _positions;
		[SerializeField] private Dialogue _forceDialogue;
		private Dialogue _rootDialogue;
		private Dialogue _dialogue;
		private bool _dirty = true;
		private int _parsedTextLength;
		private DialogueOptionDisplayer[] _dialogues;
		private float _elapsed;

		private void Awake()
		{
			ServiceLocator.Get<DialogueService>().OnDialogue += SetDialogue;
		}

		private void Start()
		{
			if(_forceDialogue != null)
			{
				UpdateDialogue(_forceDialogue);
			}
		}

		public void SetDialogue(Dialogue dialogue)
		{
			_rootDialogue = dialogue;
			UpdateDialogue(dialogue);
		}

		private void Update()
		{
			if (_dialogue == null) return;

			_elapsed += Time.deltaTime;

			float delta = (_elapsed - _effects.AppearanceTime) / _dialogue.StayTime;

			if(_effects.Displayed == _parsedTextLength && _dirty)
			{
				_dirty = false;

				for (int i = 0, length = _dialogues.Length; i < length; i++)
				{
					_dialogues[i].Display(i);
				}
			}
			if(delta > 1f && _dialogue.AutoSwitch)
			{
				UpdateDialogue(_dialogue.NextDialogues);
			}
		}

		private void UpdateDialogue(Dialogue dialogue)
		{
			if(dialogue == null)
			{
				ServiceLocator.Get<DialogueService>().EndDialogue(_rootDialogue);
				_dialogue = null;
				return;
			}
			_text.text = dialogue.MainText;
			_text.ForceMeshUpdate();
			_parsedTextLength = _text.GetParsedText().Length;

			if (_dialogues != null)
			{
				for(int i = 0; i < _dialogues.Length; i++)
				{
					_dialogues[i].Selected -= OptionSelected;
					Destroy(_dialogues[i].gameObject); //todo: use a fucking pool perhaps? no?
				}
			}

			_dialogues = new DialogueOptionDisplayer[dialogue.Options == null ? 0 : dialogue.Options.Length];
			for(int i = 0; i < _dialogues.Length; ++i)
			{
				_dialogues[i] = Instantiate(_prefab, _positions[i].transform);
				_dialogues[i].SetOption(dialogue.Options[i]);

				_dialogues[i].Selected += OptionSelected;
				_dialogues[i].StopDisplaying();
				_dirty = true;
			}

			_elapsed = 0f;
			_dialogue = dialogue;
			_effects.ResetEffects();
		}

		private void OptionSelected(DialogueOption obj)
		{
			UpdateDialogue(obj.Dialogue);
		}

		public float TotalElapsed => _elapsed;
		public float Elapsed => _elapsed - _effects.AppearanceTime;
		public float TextDelta => (_elapsed - _effects.AppearanceTime) / _dialogue.StayTime;
	}
}