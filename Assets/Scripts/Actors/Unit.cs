using YaEm.Movement;
using YaEm.Weapons;
using YaEm.Health;
using YaEm.Core;
using System;
using UnityEngine;
using YaEm.Effects;
using YaEm.Ability;

namespace YaEm
{
	[RequireComponent(typeof(Rigidbody2D)), DisallowMultipleComponent()]
	public sealed class Unit : MonoBehaviour, 
		IActor, IProvider<IHealth>, IProvider<IWeapon>, 
		IProvider<Motor>, IProvider<IAbility>, 
		ITeamProvider, IDamageReactable
	{
		[SerializeField] private AbilityBuilder _builder;
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

		private bool _isVisible;
		private bool _isInited = false;
		private bool _isComponentsInited = false;
		private IAbility _ability;
		private Transform _cached;
		private Rigidbody2D _cachedRigidbody;
		private IHealth _health;
		private IWeapon _weapon;
		private Motor _motor;
		private IController _controller;

		private void Awake()
		{
			_cached = transform;
		}

		private void OnBecameInvisible()
		{
			_isVisible = false;
		}

		private void OnBecameVisible()
		{
			_isVisible = true;
		}

		private void Start()
		{
			Init();

			if (_builder != null) _ability = _builder.Build(this);
			InitComponents();

			OnInit?.Invoke();
		}

		public void InitComponents()
		{
			if (_isComponentsInited) return;
			_isComponentsInited = true;

			var comps = transform.GetComponentsInChildren<IActorComponent>(true);
			for (int i = 0, length = comps.Length; i < length; i++)
			{
				comps[i].Init(this);
			}

			if (TryGetComponent<IActorComponent>(out var selfComp))
			{
				selfComp.Init(this);
			}
		}

		public void Init()
		{
			if (_isInited) return;
			_isInited = true;
			_cachedRigidbody = GetComponent<Rigidbody2D>();
			_motor = new Motor(_speed, _rotationSpeed, this, transform, new RigidbodyVelocityProvider(_cachedRigidbody));
			_health = new YaEm.Health.Health(_maxHealth, _maxHealth, this);

			if (_destroyOnDeath)
				_health.OnDeath += (_) => Destroy(gameObject);

			_health.OnDamage += (DamageArgs args) => OnDamage?.Invoke(args);
			_health.OnPreDamage += (DamageArgs args) => OnPreDamage?.Invoke(args);
			_health.Actor = this;

			if (!transform.TryGetComponentInChildren<IWeapon>(out _weapon))
			{
				Debug.LogWarning($"Unit: {_name} does not have weapon");
			}

		}

		//todo make unit builder to not break SOLID
		public void SetAbility(IAbility ability)
		{
			_ability = ability;
		}

		public void SetScale(float scale)
		{
			_scale = scale;
		}

		private void Update()
		{
			float dt = Time.deltaTime;
			_motor.Update(dt);
			_weapon?.UpdateEffector(dt);
			_health?.UpdateEffector(dt);
			_ability?.Update(dt);
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

		ref Ability.IAbility IProvider<Ability.IAbility>.Value => ref _ability;

		public bool IsVisible => _isVisible;

		public event Action<IController, IController> OnControllerChange;
		public event Action<int, int> OnTeamNumberChange;
		public event Action<DamageArgs> OnDamage;
		public event Action OnInit;
		public event Action<ControllerAction> OnAction;
		public event Action<DamageArgs> OnPreDamage;

		public bool TryChangeController(in IController controller)
		{
			bool res = CanChangeController(in controller);
			if (res)
			{
				if(_controller != null)
				{
					_controller.ControllerAction -= ReadAction;
				}
				if(controller != null)
				{
					controller.ControllerAction += ReadAction;
				}
				OnControllerChange?.Invoke(Controller, controller);
				_controller = controller;
			}
			return res;
		}

		private void ReadAction(ControllerAction obj)
		{
			if (obj == ControllerAction.UseAbility && _ability != null && _ability is IDirectionalAbility dir)
			{
				dir.Direction = _motor.MoveDirection;
			}

			OnAction?.Invoke(obj);
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
