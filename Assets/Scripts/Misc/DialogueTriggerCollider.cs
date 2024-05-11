using UnityEngine;

using YaEm.Core;

namespace YaEm.Dialogues
{
	[DisallowMultipleComponent(), RequireComponent(typeof(Collider2D))]
	public sealed class DialogueTriggerCollider : DialogueTrigger
	{
		private void OnValidate()
		{
			if (TryGetComponent<Collider2D>(out var collider))
			{
				collider.isTrigger = true;
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if(collision.TryGetComponent<IActor>(out var actor) && actor.Controller != null && actor.Controller.Type == ControllerType.Player)
			{
				Trigger();
			}
		}
	}
}