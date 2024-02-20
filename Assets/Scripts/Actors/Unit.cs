using YaEm.Movement;
using YaEm.Weapons;
using YaEm.Health;
using YaEm.Core;
using System;
using UnityEngine;
using YaEm.Effects;

namespace YaEm
{
	[RequireComponent(typeof(Rigidbody2D)), DisallowMultipleComponent()]
	public sealed class Unit : MonoBehaviour, IActor, IProvider<IHealth>, IProvider<IWeapon>, IProvider<Motor>, ITeamProvider, IDamageReactable
	{
		[SerializeField] private ControllerType _allowedControllerType;
		[SerializeField] private int _maxHealth;
		[SerializeField] private string _name;

		[SerializeField] private float _speed;
		[SerializeField] private float _scale = 1f;
		[SerializeField] private float _rotationSpeed;
		[SerializeField] private int _teamNumber;
		[SerializeField] private bool _canChangeTeam;
		[SerializeField] private bool _destroyOnDeath = true;
		[SerializeField] private bool _debug;

		private Transform _cached;
		private Rigidbody2D _cachedRigidbody;
		private IHealth _health;
		private IWeapon _weapon;
		private Motor _motor;
		private IController _controller;

		private void Start()
		{
			_cached = transform;
			_cachedRigidbody = GetComponent<Rigidbody2D>();
			_motor = new Motor(_speed, _rotationSpeed, this, transform, new RigidbodyVelocityProvider(_cachedRigidbody));
			_health = new YaEm.Health.Health(_maxHealth, _maxHealth, this);

			if(_destroyOnDeath)
				_health.OnDeath += (_) => Destroy(gameObject);

			_health.OnDamage += (DamageArgs args) => OnDamage?.Invoke(args);
			_health.Actor = this;

			if (!transform.TryGetComponentInChildren<IWeapon>(out _weapon))
			{
				Debug.LogWarning($"Unit: {_name} does not have weapon");
			}

			var comps = transform.GetComponentsInChildren<IActorComponent>(true);
			for (int i = 0, length = comps.Length; i < length; i++)
			{
				comps[i].Init(this);
			}

			OnInit?.Invoke();
		}

		private void Update()
		{
			_motor.Update(Time.deltaTime);
			_weapon?.UpdateEffector(Time.deltaTime);
			_health?.UpdateEffector(Time.deltaTime);
		}

		public Vector2 Position => _cached.position;

		public float Rotation => _cached.eulerAngles.z * Mathf.Deg2Rad;

		public IController Controller => _controller;

		public string Name => _name;

		public ref IHealth Value => ref _health;

		ref IWeapon IProvider<IWeapon>.Value => ref _weapon;

		ref Motor IProvider<Motor>.Value => ref _motor;

		public float Scale => _scale;

		public int TeamNumber => _teamNumber;

		public event Action<IController, IController> OnControllerChange;
		public event Action<int, int> OnTeamNumberChange;
		public event Action<DamageArgs> OnDamage;
		public event Action OnInit;
		public event Action<ControllerAction> OnAction;

		public bool TryChangeController(in IController controller)
		{
			bool res = CanChangeController(in controller);
			if (res)
			{
				OnControllerChange?.Invoke(Controller, controller);
				_controller = controller;
			}
			return res;
		}

		private enum ControllerType
		{
			PlayerOnly,
			PlayerAndAI,
			AIOnly
		}

		private bool CanChangeController(in IController controller)
		{
			switch (controller.Type)
			{
				case YaEm.Core.ControllerType.AI:
					{
						return _allowedControllerType == ControllerType.AIOnly || _allowedControllerType == ControllerType.PlayerAndAI;
					}
				case YaEm.Core.ControllerType.Player:
					{
						return _allowedControllerType == ControllerType.PlayerAndAI || _allowedControllerType == ControllerType.PlayerOnly;
					}
			}
			return _allowedControllerType == ControllerType.PlayerAndAI;
		}

		public bool TryChangeTeamNumber(int newTeamNumber)
		{
			if (_canChangeTeam)
			{
				OnTeamNumberChange?.Invoke(_teamNumber, newTeamNumber);
				_teamNumber = newTeamNumber;
				return true;
			}
			return false;
		}

		public bool CanTakeDamage(DamageArgs args)
		{
			return _health.CanTakeDamage(args);
		}

		public void TakeDamage(DamageArgs args)
		{
			_health.TakeDamage(args);
		}

		private void OnDrawGizmos()
		{
			if(!_debug) return;

			Gizmos.DrawWireSphere(transform.position, Scale);
		}
	}
}
