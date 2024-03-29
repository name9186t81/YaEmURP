using UnityEngine;
using YaEm.Core;

namespace YaEm.GUI
{
	public abstract class GUIElement : MonoBehaviour
	{
		private PlayerTrakerService _service;

		private void Start()
		{
			if (ServiceLocator.TryGet<PlayerTrakerService>(out _service))
			{
				_service.OnPlayerChange += PlayerChanged;
			}
			else
			{
				Debug.LogError("Player traker not found... destroying component...");
				Destroy(this);
				return;
			}

			Init();
		}

		protected virtual void Init() { }
		protected virtual void PlayerChanged(IActor player) { }
		protected IActor Player => _service.Player;
	}
}