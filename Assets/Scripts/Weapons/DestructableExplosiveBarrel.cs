using YaEm.Health;

using System;
using UnityEngine;

using YaEm.Core;

namespace YaEm.Weapons
{
	[RequireComponent(typeof(Collider2D))]
	public class DestructableExplosiveBarrel : MonoBehaviour, IActor, IProvider<IHealth>, ITeamProvider, IDamageReactable
	{
		[SerializeField] private Transform _parent;
		[SerializeField] private int _maxHealth;
		[SerializeField] private HealthFlags _flags;
		[SerializeField] private float _timeBeforeDestruction;
		[SerializeField] private ParticleSystem _preDeathParticles; //todo move in another class
		[SerializeField] private Explosion _explosion;
		[SerializeField, Tooltip("The unit this barrel is attached to. Can be null.")] private Unit _unit;

		public Vector2 Position => _transform.position;

		public float Rotation => _transform.eulerAngles.z;

		public float Scale => 1f;

		public IController Controller => null;

		public string Name => "Barrel";

		public ref IHealth Value => ref _health;

		public int TeamNumber => _teamNumber;

		public event Action OnFinalDestroy;
		public event Action OnInit;
		public event Action<IController, IController> OnControllerChange;
		public event Action<int, int> OnTeamNumberChange;
		public event Action<DamageArgs> OnDamage;
		public event Action<ControllerAction> OnAction;

		private int _teamNumber;
		private Transform _transform;
		private IHealth _health;
		private float _elapsed;
		private bool _isDying;

		private float _offsetLength;
		private float _offsetAngle;
		private int _killerTeamNumber;
		private IActor _killer;

		private void Awake()
		{
			_transform = transform;
			_health = new Health.Health(_maxHealth, _maxHealth);
			_health.OnDeath += Death;
			if (_unit != null)
			{
				_teamNumber = _unit.TeamNumber;
				_unit.OnTeamNumberChange += UpdateTeam;

				if (_unit is IProvider<IHealth> prov2)
				{
					_unit.OnInit += Init;
				}
			}

			if (_parent != null)
			{
				_offsetLength = transform.localPosition.magnitude;
				_offsetAngle = ((Vector2)transform.localPosition).AngleRadFromVector();
				transform.SetParent(null);
			}
		}

		private void Init()
		{
			if (_unit is IProvider<IHealth> prov2)
			{
				prov2.Value.OnDeath += UnitDeath;
			}
			_unit.OnInit -= Init;
		}

		private void UnitDeath(DamageArgs obj)
		{
			Destroy(gameObject);
		}

		private void Death(DamageArgs obj)
		{
			_isDying = true;
			_health.OnDeath -= Death;
			_preDeathParticles?.Play();

			if (_unit is IProvider<IHealth> prov2)
			{
				prov2.Value.Flags |= HealthFlags.Invincible;
			}

			_killer = obj.Sender;
			if (_killer != null && _killer is ITeamProvider prov)
			{
				_killerTeamNumber = prov.TeamNumber;
			}
		}

		private void Update()
		{
			if (_parent != null)
			{
				_transform.position = (Vector2)_parent.position + (_offsetAngle + _parent.eulerAngles.z * Mathf.Deg2Rad).VectorFromAngle() * _offsetLength;
				_transform.rotation = Quaternion.Euler(0, 0, _parent.eulerAngles.z);
			}

			if (!_isDying) return;

			_elapsed += Time.deltaTime;
			if (_elapsed > _timeBeforeDestruction)
			{
				if (_unit is IProvider<IHealth> prov2)
				{
					prov2.Value.Flags &= ~HealthFlags.Invincible;
				}

				_explosion.Explode(Position, _killer, _killerTeamNumber);
				OnFinalDestroy?.Invoke();
				Destroy(gameObject);
			}
		}

		private void UpdateTeam(int arg1, int arg2)
		{
			_teamNumber = arg2;
			OnTeamNumberChange?.Invoke(arg1, arg2);
		}

		private void OnDestroy()
		{
			if (_unit != null)
			{
				_unit.OnTeamNumberChange -= UpdateTeam;
			}
		}

		public bool TryChangeController(in IController controller)
		{
			return false;
		}

		public bool TryChangeTeamNumber(int newTeamNumber)
		{
			return false;
		}

		public bool CanTakeDamage(DamageArgs args)
		{
			return _health.CanTakeDamage(args) && (args.DamageFlags & DamageFlags.Kinetic) != 0;
		}

		public void TakeDamage(DamageArgs args)
		{
			_health.TakeDamage(args);
		}
	}
}