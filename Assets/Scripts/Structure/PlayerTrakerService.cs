using System;
using UnityEngine;
using YaEm.Core;

namespace YaEm.GUI
{
	public class PlayerTrakerService : MonoBehaviour, IService
	{
		[SerializeField] private Unit _player;
		[SerializeField] private DebugController _playerController; //todo: implement interface to work with differents controllers
		[SerializeField] private MobileController _mobileController; //todo: implement interface to work with differents controllers
		[SerializeField] private bool _forceMobileBuild = false;
		private float _elapsed = 0f;
		private bool _isMobileBuild = false;
		/// <summary>
		/// called before Player is changed making the paramether as an new player
		/// </summary>
		public Action<IActor> OnPlayerChange;

		private void Awake()
		{
			if (!ServiceLocator.TrySet(this))
			{
				ServiceLocator.Register(this);
			}
			if(_player != null)
				_player.OnInit += Init;
#if UNITY_ANDROID
			_isMobileBuild = true;
#if UNITY_EDITOR
			_isMobileBuild = _forceMobileBuild;
#endif
#endif

			if (!_isMobileBuild && _mobileController != null) _mobileController.Disable();
			if (_player != null)
				TryChangePlayer(_player);
		}

		private void Init()
		{
			OnPlayerChange?.Invoke(_player);
			_player.OnInit -= Init;
		}

		private void Update()
		{
			if (_player == null)
			{
				_elapsed += Time.deltaTime;

				if (_elapsed < 0.2f) return;
				_elapsed = 0f;

				IActor[] actors = FindObjectsOfType<Unit>();
				foreach (var actor in actors)
				{
					if (_player != null || (actor is ITeamProvider prov && prov.TeamNumber == 1 && TryChangePlayer(actor))) break;
				}
			}
		}

		public bool TryChangePlayer(IActor player)
		{
			IController controller = _isMobileBuild ? (IController)_mobileController : _playerController;
			if (player.TryChangeController(controller))
			{
				OnPlayerChange?.Invoke(player);
				_player = player as Unit;
				if(_mobileController != null) _mobileController.Target = player as Unit;
				_elapsed = 0f;
				return true;
			}
			return false;
		}

		private void OnDisable()
		{
			if (ServiceLocator.TryGet<GlobalTimeModifier>(out var timer)) timer.SetTimeModificator(1);
		}
		public IActor Player => _player;
	}
}