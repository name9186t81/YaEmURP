using UnityEngine;

namespace YaEm.Dialogues
{
	[CreateAssetMenu(fileName = "Dialogue", menuName = "YaEm/Dialogue")]
	public sealed class Dialogue : ScriptableObject
	{
		[SerializeField] private string _mainText;
		[SerializeField] private DialogueOption[] _options;
		[SerializeField] private bool _autoSwitch;
		[SerializeField] private Dialogue _nextDialogue;
		[SerializeField] private float _stayTime;

		public string MainText => _mainText;
		public DialogueOption[] Options => _options;
		public bool AutoSwitch => _autoSwitch;
		public Dialogue NextDialogues => _nextDialogue;
		public float StayTime => _stayTime;
	}
}
