using YaEm.Effects;
using YaEm.Health;
using YaEm.Core;
using YaEm;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace YaEm.Weapons
{
	[RequireComponent(typeof(PolarTransform)), DisallowMultipleComponent()]
	public sealed class RangedWeapon : MonoBehaviour, IWeapon
	{
		public enum SpreadType
		{
			Fixed,
			Angle
		}

		[SerializeField] private SpreadType _spreadType;
		[SerializeField] private int _spreadIterations;
		[SerializeField] private int _spreadOffset;
		[SerializeField] private float _spreadAngle;
		[SerializeField] private WeaponFlags _flags;
		[SerializeField] private DamageFlags _damageFlags;
		[SerializeField] private ProjectileBuilder _projectileBuilder;
		[SerializeField] private Vector2 _shootPoint;
		[SerializeField] private int _baseDamage;
		[SerializeField] private int _projectileCount = 1;
		[SerializeField] private float _preFireDelay;
		[SerializeField] private float _delayPerProjectile;
		[SerializeField] private float _postFireDelay;
		[SerializeField] private float _effectiveRange;
		[SerializeField] private float _useRange;
		[SerializeField] private float _speedModificator = 1f;
		[SerializeField] private bool _chargable;
		[SerializeField] private bool _keepAfterDeath;
		[SerializeField] private bool _debug;

		private bool _isCharging;
		private float _chargingTime;
		private GlobalTimeModifier _timeModificator;
		private ISpreadProvider _spreadProvider;
		private float _speedModifier = 1f;
		private float _timeMod = 1f;
		private float _reloadModifier = 1f;
		private float _damageModifier = 1f;
		private List<IEffect<IWeapon>> _effects;
		private readonly HashSet<EffectType> _types = new HashSet<EffectType>();
		private Transform _transform;
		private bool _canFire = true;
		private bool _isFiring = false;
		private bool _isInited = false;
		private Pool<IProjectile> _projectilePool;
		private IActor _actor;
		private PolarTransform _polarTransform;

		public event Action OnPreFireStart;
		public event Action OnPreFireEnd;
		public event Action OnFire;
		public event Action OnEndFire;
		public event Action OnAttack;
		public event Action OnInit;
		public event Action OnAttackEnded;

		public UnityEvent PreFireStart;
		public UnityEvent PreFireEnd;
		public UnityEvent Fire;
		public UnityEvent EndFire;

		private void Start()
		{
			_projectilePool = new Pool<IProjectile>(() => _projectileBuilder.BuildProjectile(_actor));

			OnPreFireStart += () => PreFireStart?.Invoke();
			OnFire += () => Fire?.Invoke();
			OnPreFireEnd += () => PreFireEnd?.Invoke();
			OnAttackEnded += () => EndFire?.Invoke();

			_effects = new List<IEffect<IWeapon>>();
			_spreadProvider = SpreadProviderFactory.GetSpread(_spreadType, _spreadIterations, _spreadAngle * Mathf.Deg2Rad, _spreadOffset);
		}

		private void OnDestroy()
		{
			_timeModificator.OnTimeModificated -= TimeModded;
			_projectilePool.Destroy();
		}

		private void Update()
		{
			if (!_chargable || !_isCharging || !CanAttack)
			{
				_chargingTime = 0f;
				return;
			}

			_chargingTime += Time.deltaTime;
		}

		private void OnValidate()
		{
			_flags |= WeaponFlags.Ranged;

			_projectileCount = Mathf.Max(_projectileCount, 0);
			_preFireDelay = Mathf.Max(_preFireDelay, 0);
			_delayPerProjectile = Mathf.Max(_delayPerProjectile, 0);
			_postFireDelay = Mathf.Max(_postFireDelay, 0);
			_effectiveRange = Mathf.Max(_effectiveRange, 0);
			_useRange = Mathf.Max(_useRange, 0);
			_baseDamage = Mathf.Max(_baseDamage, 0);
		}

		private IEnumerator FireRoutine()
		{
			_isFiring = true;
			OnPreFireStart?.Invoke();
			yield return new WaitForSeconds(Mathf.Max(_preFireDelay * _reloadModifier / _timeMod - _chargingTime, 0));
			OnPreFireEnd?.Invoke();

			for (int i = 0; i < _projectileCount; i++)
			{
				var obj = _projectilePool.Get();
				var args = new DamageArgs(_actor, (int)(_baseDamage * _damageModifier), _damageFlags | DamageFlags.Ranged, this);
				obj.Init(_projectilePool, args, (_actor is ITeamProvider prov) ? prov.TeamNumber : 0, GlobalShootPoint, _spreadProvider.GetDirection(ShootDirection), _actor, _speedModifier * _speedModificator);
				OnFire?.Invoke();
				OnAttack?.Invoke();
				if (_delayPerProjectile != 0)
					yield return new WaitForSeconds(_delayPerProjectile * _reloadModifier / _timeMod);
			}

			OnAttackEnded?.Invoke();
			yield return new WaitForSeconds(_postFireDelay * _reloadModifier /_timeMod);
			_isFiring = false;
		}

		public void Init(IActor actor)
		{
			_actor = actor;
			_transform = transform;
			_polarTransform = GetComponent<PolarTransform>();
			_polarTransform.Offset *= _actor.Scale;
			_transform.position = _polarTransform.GetPosition() + (Vector2)transform.position;
			_actor.OnControllerChange += UpdateInput;
			UpdateInput(null, _actor.Controller);

			if(_actor is IProvider<IHealth> prov && !_keepAfterDeath && prov != null && prov.Value != null)
			{
				prov.Value.OnDeath += UnSub;
			}

			if(!ServiceLocator.TryGet<GlobalTimeModifier>(out _timeModificator))
			{
				Debug.LogWarning("Cannot find Global time modificator");
			}
			else
			{
				_timeMod = _actor.Controller == null || _actor.Controller.IsEffectedBySlowMotion ? _timeModificator.TimeModificator : 1f;
				_timeModificator.OnTimeModificated += TimeModded; ;
			}
			_isInited = true;
			OnInit?.Invoke();
		}

		public bool AddEffect(IEffect<IWeapon> effect)
		{
			bool res = effect.CanApply(this);
			if (!res || _types.Contains(effect.Type)) return false;

			_effects.Add(effect);
			_types.Add(effect.Type);
			effect.ApplyEffect(this);
			return true;
		}

		private void TimeModded()
		{
			_timeMod = _actor.Controller == null || _actor.Controller.IsEffectedBySlowMotion ? _timeModificator.TimeModificator : 1f;
		}

		private void UnSub(DamageArgs obj)
		{
			if (_actor is IProvider<IHealth> prov)
			{
				prov.Value.OnDeath -= UnSub;
			}

			if (_actor.Controller == null) return;
			_actor.Controller.ControllerAction -= ReadInput;
		}

		private void UpdateInput(IController arg1, IController arg2)
		{
			if (arg1 != null)
			{
				arg1.ControllerAction -= ReadInput;
			}
			if (arg2 != null)
			{
				arg2.ControllerAction += ReadInput;
			}
		}

		private void ReadInput(ControllerAction obj)
		{
			if (obj == ControllerAction.Fire && CanAttack) StartCoroutine(FireRoutine());

			_isCharging = _chargable && (obj == ControllerAction.Charge || (_isCharging && obj != ControllerAction.BreakCharge));
		}

		public void UpdateEffector(float delta)
		{
			for (int i = 0, length = Effects.Count; i < length; ++i)
			{
				Effects[i].Update(delta);
				if (Effects[i].State == EffectState.Finished)
				{
					_effects.RemoveAt(i);
					_types.Remove(Effects[i].Type);
					i--;
				}
			}
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if(!_debug) return;

			Vector2 selfPos = transform.position;
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(selfPos, selfPos + _shootPoint.Rotate(transform.eulerAngles.z * Mathf.Deg2Rad));
			Gizmos.DrawWireSphere(selfPos + _shootPoint, 0.1f);
		}

#endif

		public bool Contains(EffectType type)
		{
			return _types.Contains(type);
		}

		public PolarTransform PolarTransform => _polarTransform == null ? GetComponent<PolarTransform>() : _polarTransform; //again why do i have it instead of math extension?
		public bool CanFire { get => _canFire; set => _canFire = value; }
		public bool CanAttack => !_isFiring && _canFire;

		public float DeltaBeforeAttack => throw new NotImplementedException();

		public float EffectiveRange => _effectiveRange;

		public float UseRange => _useRange;

		public WeaponFlags Flags => _flags;

		public bool ParryState => false;

		public IActor Actor { set => _actor = value; get => _actor; }

		public Vector2 ShootDirection => _transform.up;
		public Vector2 GlobalShootPoint => (Vector2)transform.position + _shootPoint.Rotate(transform.eulerAngles.z * Mathf.Deg2Rad);

		public float ChargeTime => _chargingTime;
		public IProjectile Projectile => _projectileBuilder.PeekProjectile();
		public float DamageMultiplier { get => _damageModifier; set => _damageModifier = value; }
		public float ReloadMultiplier { get => _reloadModifier; set => _reloadModifier = value; }
		public float SpeedMultiplier { get => _speedModifier; set => _speedModifier = value; }

		public IReadOnlyList<IEffect<IWeapon>> Effects => _effects;

		public bool IsInited => _isInited;
	}
}
