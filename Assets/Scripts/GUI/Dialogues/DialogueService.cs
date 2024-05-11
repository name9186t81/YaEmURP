using System;

namespace YaEm.Dialogues
{
	public sealed class DialogueService : IService
	{
		public event Action<Dialogue> OnDialogue;
		public event Action<Dialogue> OnDialogueEnded;

		public void TriggerDialogue(Dialogue rootDialogue)
		{
			OnDialogue?.Invoke(rootDialogue);
		}

		public void EndDialogue(Dialogue rootDialogue)
		{
			OnDialogueEnded?.Invoke(rootDialogue);
		}
	}
}
