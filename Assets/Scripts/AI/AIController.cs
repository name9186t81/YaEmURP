using System;
using YaEm;
using YaEm.Weapons;
using YaEm.Movement;
using YaEm.Core;
using YaEm.Health;
using YaEm.AI.States;
using System.Collections;
using UnityEngine;
using YaEm.Ability;

namespace YaEm.AI
{
	//todo: ai calculations are pretty heavy so implement global ai updater and set its tick rate to like 0.1 sec
	[RequireComponent(typeof(AIVision)), RequireComponent(typeof(AITargetPicker)), DisallowMultipleComponent(), RequireComponent(typeof(IActor))]
	public class AIController : MonoBehaviour, IController
	{
		//todo: write custom editor for all of this
		private IActor _controlled;
		[SerializeField] private float _fireThreshold;
		[SerializeField] private LayerMask _avoidMask;
		[SerializeField] private float _accelarationTime = 0.2f;
		[SerializeField] private bool _affectedBySlowMotion = true;

		[Header("Profile settings")]
		[SerializeField, Tooltip("set to -1 to disable seed")] private int _seed = -1;
		[SerializeField] private AIProfile _min;
		[SerializeField] private AIProfile _max;

		[Header("Memory settings")]
		[SerializeField] private float _maxTargetRemembranceTime;
		[SerializeField] private bool _debug;

		private Vector2? _lastSavePosition;
		private Transform _transform;
		private AIProfile _mixed;
		private AIMemory _memory;
		private AIVision _vision;
		private UtilityAI _utilityAI;
		private IHealth _health;
		private Motor _motor;
		private Vector2 _direction;
		private float _rotation;
		private bool _isInited = false;
		private IWeapon _weapon;
		private float _braveNess;

		public IHealth TargetHealth { get; private set; }
		public Transform TargetTransform { get; private set; }
		public IActor CurrentTarget { get; private set; }
		public bool IsTargetNull => TargetTransform == null || CurrentTarget == null;

		private void Awake()
		{
			_controlled = GetComponent<IActor>();

			if (!_controlled.TryChangeController(this))
			{
				Debug.LogWarning("Cant change controller on " + _controlled);
				enabled = false;
				return;
			}
			_controlled.OnInit += Init;
			_transform = (_controlled as MonoBehaviour).transform;
		}

		public void UpdateTarget(IActor newTarget, bool saveInMemory = true)
		{
			if(saveInMemory && CurrentTarget != null) RememberLastTarget(CurrentTarget);

			CurrentTarget = newTarget;
			if (newTarget != null)
			{
				TargetTransform = ((MonoBehaviour)newTarget).transform;
				if (CurrentTarget is IProvider<IHealth> prov) TargetHealth = prov.Value;
				else TargetHealth = null;

			}
			else
			{
				TargetTransform = null;
				TargetHealth = null;
			}
		}

		RaycastHit2D[] _cachedHits = new RaycastHit2D[4];
		public void SafeWalk(in Vector2 point)
		{
			Vector2 dir = Position.GetDirectionNormalized(point);
			Vector2 resultDir = Vector2.zero;
			Debug.DrawLine(Position, Position + dir * 2f, Color.red);

			for (int i = 0; i < 8; i++)
			{
				Vector2 currentDir = PhysicsUtils.EightDirections[i];
				float dot = Vector2.Dot(dir, currentDir);
				if(dot < 0)
				{
					continue;
				}

				Debug.DrawLine(Position, Position + currentDir * 1.5f * dot, Color.yellow);
				float size = _controlled.Scale + 0.1f;
				var ray = PhysicsUtils.RaycastIgnoreSelf(_transform, Position, currentDir, size, _avoidMask);
				if (ray)
				{
					resultDir -= currentDir * (ray.distance / (size)) * (1 - dot);
				}
				else
				{
					resultDir += currentDir;
				}
			}
			resultDir = resultDir.normalized;
			_direction += resultDir / _accelarationTime * Time.deltaTime;
			float mag = _direction.magnitude;
			if (mag > 1f) _direction.Normalize();
		}

		public void ForgotTarget(bool saveInMemory)
		{
			if (CurrentTarget != null)
			{
				if(saveInMemory) RememberLastTarget(CurrentTarget);
				TargetTransform = null;
				TargetHealth = null;
				CurrentTarget = null;
			}
		}

		private void Init()
		{
			_vision = GetComponent<AIVision>();
			_vision.Init(this);

			_vision.OnScan += Scanned;

			if (_controlled is IProvider<IHealth> provider)
			{
				_health = provider.Value;
			}
			if (_controlled is IProvider<Motor> motor)
			{
				_motor = motor.Value;
			}
			if (_controlled is IProvider<IWeapon> weapon)
			{
				_weapon = weapon.Value;
			}

			_isInited = true;
			if (_seed == -1)
				_mixed = AIProfile.Mix(_min, _max);
			else
				_mixed = AIProfile.Mix(_min, _max, _seed);

			GetComponent<AITargetPicker>().Init(this);

			bool additionalState = false;
			IUtility state = null;
			if(Actor is IProvider<IAbility> prov && prov.Value != null && prov.Value.AIAbilityInstruction != null)
			{
				additionalState = true;
				state = prov.Value.AIAbilityInstruction;
			}

			_memory = new AIMemory();
			if (additionalState)
			{
				_utilityAI = new UtilityAI(this, new IUtility[]{
				new IdleState(),
				new AttackState(),
				state
			});
			}
			else
			{
				_utilityAI = new UtilityAI(this, new IUtility[]
				{
					new IdleState(),
					new AttackState()
				});
			}
			_braveNess = (_mixed.Aggresivness + _mixed.Experience) / 2f;
		}

		private void Scanned()
		{
			if(_vision.EnemiesInRangeCount == 0)
			{
				if(!_lastSavePosition.HasValue)
					_lastSavePosition = Actor.Position;
			}
			else
			{
				if (_lastSavePosition.HasValue && !_vision.CanSeePoint(_vision.EnemiesInRange[0].Position, _lastSavePosition.Value, false)) return;
				//Debug.DrawLine(_vision.EnemiesInRange[0].Position, _lastSavePosition.Value, Color.red, 5f);
				_lastSavePosition = null;
			}
		}

		private void Update()
		{
			_utilityAI.Update();
			if (_lastSavePosition.HasValue)
			{
				Debug.DrawLine(Position, _lastSavePosition.Value);
			}
			//Debug.Log(_utilityAI.CurrentUtility.GetType().ToString());
		}

		public bool IsEffectiveToFire(Vector2 point)
		{
			return _weapon != null && (point - Position).sqrMagnitude < (_weapon.EffectiveRange * _weapon.EffectiveRange) && Mathf.Abs(Vector2.Angle((Actor.Rotation + Mathf.PI / 2).VectorFromAngle(), point - Position)) < _fireThreshold;
		}

		public void LookAtPoint(Vector2 point)
		{
			Vector2 dir = Position - point;
			_rotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
		}

		public void InitCommand(ControllerAction key)
		{
			ControllerAction?.Invoke(key);
		}

		public void MoveToThePoint(Vector2 point)
		{
			Vector2 dir = Position.GetDirectionNormalized(point);
			_direction += dir / _accelarationTime * Time.deltaTime;
			float mag = _direction.magnitude;
			if (mag > 1f) _direction.Normalize();
		}

		public Vector2 PredictShootPoint(IActor target)
		{
			if (!(Actor is IProvider<IWeapon> prov) || prov.Value == null || (prov.Value.Flags & WeaponFlags.Ranged) == 0) return target.Position;

			Vector2 pos = target.Position;
			RangedWeapon ranged = prov.Value as RangedWeapon;
			float projSpeed = ranged.Projectile.Speed;
			Vector2 shootPos = ranged.GlobalShootPoint;

			//Vector2 dir = (ranged.ShootDirection.Perpendicular2() * (2 * (1 - Mathf.Cos(Vector2.Angle(ranged.GlobalShootPoint.GetDirection(pos), _transform.up) * Mathf.Deg2Rad)))).Rotate(_transform.eulerAngles.z * Mathf.Deg2Rad);
			Vector2 offset = Vector2.zero;
			if (target is IProvider<Motor> prov2) offset = prov2.Value.MoveDirection * prov2.Value.Speed;

			//Debug.Log(offset + " " + (pos - shootPos).sqrMagnitude / (projSpeed));
			return pos + offset * (2 * (pos - shootPos).sqrMagnitude / (projSpeed  * projSpeed));
		}

		public void StopMoving()
		{
			MoveDirection = Vector2.zero;
		}

		public void RememberLastTarget(IActor target)
		{
			_memory.SetMemory(AIMemoryKey.LastTargetPosition, target.Position);
			_memory.SetMemory(AIMemoryKey.LastTarget, target);
			if(target is IProvider<IHealth> prov) _memory.SetMemory(AIMemoryKey.LastTargetHealth, prov.Value);
			StartCoroutine(TargetClearRoutine());
		}

		public void ForceClearLastTarget()
		{
			_memory.SetMemory(AIMemoryKey.LastTarget, null);
			_memory.SetMemory(AIMemoryKey.LastTargetPosition, null);
			_memory.SetMemory(AIMemoryKey.LastTargetHealth, null);
		}

		private IEnumerator TargetClearRoutine()
		{
			yield return new WaitForSeconds(_maxTargetRemembranceTime);
			ForceClearLastTarget();
		}

		public Vector2 MoveDirection {get { return _direction; } set { _direction = value.normalized; } }
		public Vector2 DesiredMoveDirection => _direction;
		public Vector2 Position => _controlled.Position;
		public IActor Actor => _controlled;

		public float DesiredRotation => _rotation;

		public event Action<ControllerAction> ControllerAction;

		public Vector2? LastSavePosition => _lastSavePosition;
		public StateType State => _utilityAI.CurrentUtility.StateType;
		public AIMemory Memory => _memory;
		public float Aggresivness => _mixed.Aggresivness;
		public float TeamWork => _mixed.TeamWork;
		public float Braveness => _braveNess;
		public float Experience => _mixed.Experience;
		public AIVision Vision => _vision;
		public IWeapon Weapon => _weapon;
		public IHealth Health => _health;
		public Motor Motor => _motor;

		public ControllerType Type => ControllerType.AI;

		public bool IsEffectedBySlowMotion => _affectedBySlowMotion;
	}
}