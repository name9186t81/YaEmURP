using YaEm.Effects;
using YaEm.Health;
using YaEm.Core;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace YaEm.Weapons
{
	[RequireComponent(typeof(PolarTransform)), DisallowMultipleComponent()]
	public sealed class MeleeWeapon : MonoBehaviour, IWeapon, IDamageReactable, IParriable
	{
		private IActor _actor;
		[SerializeField] WeaponFlags _flags;
		[SerializeField] private Vector2 _attackBox;
		[SerializeField] private Vector2 _parryHitBox;
		[SerializeField] private Vector2 _parryHitBoxOffset;
		[SerializeField] private Vector2 _attackBoxOffset;
		[SerializeField] private int _baseDamage;
		[SerializeField] private float _attackDelay;
		private float _elapsedTime;
		private float _damageMultiplier = 1f;
		private float _reloadMultiplier = 1f;
		private float _speedMultiplier = 1f;
		private IList<IEffect<IWeapon>> _effects;
		private readonly HashSet<EffectType> _types = new HashSet<EffectType>();

		[Space]
		[SerializeField] private bool _canParryBullets;
		[SerializeField, Range(0, 1)] private float _parryReloadReduction;
		private bool _readyToParry;

		[Space]
		[SerializeField] private bool _debug;

		private bool _isInited = false;
		public event Action<DamageArgs> OnDamage;
		public event Action OnAttack;
		public event Action OnInit;
		public event Action OnAttackEnded;
		public event Action<DamageArgs> OnPreDamage;

		public IActor Actor
		{
			set
			{
				if (_actor == null)
				{
					_actor = value;
					_actor.OnControllerChange += UpdateController;
					if(_actor.Controller != null) UpdateController(null, _actor.Controller);
					return;
				}

				_actor.OnControllerChange -= UpdateController;
				_actor = value;
				_actor.OnControllerChange += UpdateController;
			}
		}

		private void UpdateController(IController arg1, IController arg2)
		{
			if (arg1 != null) arg1.ControllerAction -= ReadAction;
			if (arg2 != null) arg2.ControllerAction += ReadAction;
		}

		public bool CanAttack => _elapsedTime < 0;

		public float DeltaBeforeAttack => _elapsedTime / _attackDelay;

		public float EffectiveRange => UseRange;

		public float UseRange => _attackBox.y / 4 + _attackBoxOffset.y + _actor.Scale / 2;

		public WeaponFlags Flags => _flags;

		public bool ParryState => CanParry && _readyToParry;

		private void ReadAction(ControllerAction obj)
		{
			if (_elapsedTime > 0) return;

			//todo: implement parry into separete class, to allow melee weapons to not only parry
			switch (obj)
			{
				case ControllerAction.Fire:
					{
						Attack();
						break;
					}
				case ControllerAction.AltFire:
					{
						ParryBullets();
						break;
					}
			}
		}

		private void Update()
		{
			_elapsedTime -= Time.deltaTime;
			_readyToParry = _readyToParry && _elapsedTime > 0;

			UpdateEffector(Time.deltaTime);
		}

		private void Attack()
		{
			_elapsedTime = _attackDelay;

			IDamageReactable reactable = _actor as IDamageReactable;
			var targets = GetTInMeleeBox<IDamageReactable>(_attackBox, _attackBoxOffset);
			OnAttack?.Invoke();

			if (targets == null) return;

			for (int i = 0, length = targets.Count; i < length; i++)
			{
				Debug.Log(targets[i].CanTakeDamage(Args));
				if (targets[i].CanTakeDamage(Args) && ((reactable != null && reactable != targets[i])))
				{
					targets[i].TakeDamage(Args);
				}
			}
		}

		private void ParryBullets()
		{
			if(!CanParry) return;

			_elapsedTime = _attackDelay;
			_readyToParry = true;

			var targets = GetTInMeleeBox<IProjectile>(_parryHitBox, _parryHitBoxOffset);
			if(targets == null) return;

			Debug.Log("Bullets found");
			for (int i = 0, length = targets.Count; i < length; i++)
			{
				if (_canParryBullets && (targets[i].ProjectileFlags | ProjectileFlags.Parriable) != ProjectileFlags.None)
				{
					Debug.Log("uh parry?");
					targets[i].Parry(_actor, targets[i].Position.GetDirectionNormalized(targets[i].Source.Position));
					_elapsedTime = Mathf.Lerp(0f, _attackDelay, 1 - _parryReloadReduction);
				}
			}
		}

		private void OnValidate()
		{
			_flags |= WeaponFlags.Melee;

			_attackBox = _attackBox.ClampValues(0, float.MaxValue);
			_attackDelay = Mathf.Max(0f, _attackDelay);
			_baseDamage = Mathf.Max(0, _baseDamage);
		}

		private IList<T> GetTInMeleeBox<T>(in Vector2 box, in Vector2 offset)
		{
			Vector2 point = _actor.Position + (offset - box / 2).Rotate(_actor.Rotation);
			Vector2 point2 = _actor.Position + (offset + box / 2).Rotate(_actor.Rotation);

			var targets = Physics2D.OverlapAreaAll(point, point2);
			int length = targets.Length;
			Debug.Log(length);

			if (length == 0) return null;

			List<T> actors = new List<T>();
			for (int i = 0; i < length; i++)
			{
				if (targets[i].TryGetComponent<T>(out var actor))
				{
					actors.Add(actor);
				}
			}

			return actors;
		}

		public bool CanTakeDamage(DamageArgs args) => _readyToParry && (args.DamageFlags | DamageFlags.Melee) != 0;

		public void TakeDamage(DamageArgs args)
		{
			if ((args.DamageFlags | DamageFlags.NoWeapon) != 0) return;

			if ((args.Weapon.Flags | WeaponFlags.Melee) != 0 && (args.Weapon is IParriable par)
				&& (par.Flags | ParriableFlags.NeverParried) != ParriableFlags.None)
			{
				par.Parry(_actor, this);
			}
		}

		public void Parry(IActor source, IWeapon weapon)
		{
			_attackDelay = 0;
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (!_debug) return;

			Vector2 selfPos = transform.position;
			float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
			Vector2 halfedBox = _attackBox / 2;

			Vector2 point1 = (new Vector2(halfedBox.x, halfedBox.y) + _attackBoxOffset).Rotate(angle) + selfPos;
			Vector2 point2 = (new Vector2(halfedBox.x, -halfedBox.y) + _attackBoxOffset).Rotate(angle) + selfPos;
			Vector2 point3 = (new Vector2(-halfedBox.x, -halfedBox.y) + _attackBoxOffset).Rotate(angle) + selfPos;
			Vector2 point4 = (new Vector2(-halfedBox.x, halfedBox.y) + _attackBoxOffset).Rotate(angle) + selfPos;

			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(point1, point2);
			Gizmos.DrawLine(point2, point3);
			Gizmos.DrawLine(point3, point4);
			Gizmos.DrawLine(point4, point1);

			halfedBox = _parryHitBox / 2;

			point1 = (new Vector2(halfedBox.x, halfedBox.y) + _parryHitBoxOffset).Rotate(angle) + selfPos;
			point2 = (new Vector2(halfedBox.x, -halfedBox.y) + _parryHitBoxOffset).Rotate(angle) + selfPos;
			point3 = (new Vector2(-halfedBox.x, -halfedBox.y) + _parryHitBoxOffset).Rotate(angle) + selfPos;
			point4 = (new Vector2(-halfedBox.x, halfedBox.y) + _parryHitBoxOffset).Rotate(angle) + selfPos;

			Gizmos.color = Color.green;
			Gizmos.DrawLine(point1, point2);
			Gizmos.DrawLine(point2, point3);
			Gizmos.DrawLine(point3, point4);
			Gizmos.DrawLine(point4, point1);
		}
#endif
		public void UpdateEffector(float delta)
		{
			//todo remove so much allocations
			List<int> toDelete = new List<int>();
			List<EffectType> typesToDelete = new List<EffectType>();
			for (int i = 0, length = Effects.Count; i < length; i++)
			{
				Effects[i].Update(delta);
				if (Effects[i].State == EffectState.Finished)
				{
					toDelete.Add(i);
					typesToDelete.Add(Effects[i].Type);
				}
			}

			for (int i = 0, length = toDelete.Count; i < length; i++)
			{
				_effects.RemoveAt(i);
				_types.Remove(typesToDelete[i]);
			}
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

		public void Init(IActor actor)
		{
			Actor = actor;
			_effects = new List<IEffect<IWeapon>>();
			transform.position = GetComponent<PolarTransform>().GetPosition() + (Vector2)transform.position;

			_isInited = true;
			OnInit?.Invoke();
		}

		public bool Contains(EffectType type)
		{
			return _types.Contains(type);
		}

		private DamageArgs Args => new DamageArgs(_actor, _baseDamage, DamageFlags.Melee, this);
		private bool CanParry => (_flags | WeaponFlags.CanParry) != WeaponFlags.None;

		ParriableFlags IParriable.Flags => ParriableFlags.Weapon;

		public float DamageMultiplier { get => _damageMultiplier; set => _damageMultiplier = Mathf.Max(0, value); }
		public float ReloadMultiplier { get => _reloadMultiplier; set => _reloadMultiplier = Mathf.Max(0, value); }
		public float SpeedMultiplier { get => _speedMultiplier; set => _speedMultiplier = Mathf.Max(0, value); }

		public IReadOnlyList<IEffect<IWeapon>> Effects => (IReadOnlyList<IEffect<IWeapon>>)_effects;

		public bool IsInited => _isInited;
	}
}
