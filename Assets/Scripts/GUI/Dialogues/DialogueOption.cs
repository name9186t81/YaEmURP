using UnityEngine;

namespace YaEm.Dialogues
{
	[CreateAssetMenu(fileName = "Dialogue option", menuName = "YaEm/DialogueOption")]
	public sealed class DialogueOption : ScriptableObject
	{
		[SerializeField] private string _text;
		[SerializeField] private Dialogue _changedDialogue;

		public string Text => _text;
		public Dialogue Dialogue => _changedDialogue;
	}
}
