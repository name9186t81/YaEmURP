using Global;

using UnityEngine;

using YaEm.Core;

namespace YaEm.Weapons
{
	[DisallowMultipleComponent(), RequireComponent(typeof(Projectile)), RequireComponent(typeof(ActorGraphics))]
	public sealed class DProjectile : MonoBehaviour
	{
		[SerializeField, Range(0, 1f)] private float _chillSpeed;
		[SerializeField] private float _chillTime;
		[SerializeField] private float _scanFrequancy = 0.5f;
		[SerializeField] private float _agroTime;
		[SerializeField] private LayerMask _scanMask;
		[SerializeField] private LayerMask _blockMask;
		[SerializeField] private float _agroRadius;
		[SerializeField] private float _maxTurnAngle;
		[SerializeField] private Color _agroColor;
		private bool _chill;
		private float _elapsed;
		private Color _originalColor;
		private Projectile _projectile;
		private ActorGraphics _graphics;
		private Transform _target;

		private static Collider2D[] _cachedColliders = new Collider2D[32];

		private void Awake()
		{
			_projectile = GetComponent<Projectile>();
			_graphics = GetComponent<ActorGraphics>();

			_originalColor = _graphics.Infos[0].Renderer.color;
			_projectile.OnInit += Inited;
			_chill = true;
		}

		private void Inited()
		{
			_chill = true;
			_elapsed = 0f;
			_target = null;
		}

		private void Update()
		{
			if (_chill)
			{
				_elapsed += Time.deltaTime;

				float delta = _elapsed / _chillTime;
				var list = _graphics.Infos;
				var lerped = Color.Lerp(_originalColor, _agroColor, delta);
				_projectile.TryChangeDirection(_projectile.Direction.SetVectorLength(Mathf.Max(1 - delta, 0.01f) * _chillSpeed));

				for (int i = 0, length = list.Count; i < length; ++i)
				{
					if (list[i].ParticleRenderer != null)
					{
						list[i].ParticleRenderer.startColor = lerped;
					}
					if (list[i].Renderer != null)
					{
						list[i].Renderer.color = lerped;
					}
				}

				if(delta > 1)
				{
					_chill = false;
					_elapsed = 0f;
				}
				return;
			}

			_elapsed += Time.deltaTime;
			if(_elapsed > _scanFrequancy && _target == null)
			{
				var overlap = Physics2D.OverlapCircleNonAlloc(_projectile.Position, _agroRadius, _cachedColliders, _scanMask);
				for(int i = 0; i < overlap; ++i)
				{
					var transform = _cachedColliders[i].transform;
					var linecast = Physics2D.Linecast(_projectile.Position, transform.position, _blockMask);
					if (linecast) continue;

					if (transform.TryGetComponent<IActorComponent>(out var actor) && actor is ITeamProvider prov && prov.TeamNumber != _projectile.TeamNumber)
					{
						_target = transform; 
						_projectile.TryChangeDirection(_projectile.Position.GetDirectionNormalized(_target.position) * 0.01f);
						break;
					}
				}

				_elapsed = 0f;
			}

			if (_target == null) return;
			float delta2 = _elapsed / _agroTime;

			var direction = _projectile.Position.GetDirectionNormalized(_target.position);
			var projDirection = _projectile.Direction.normalized;
			float angle = Mathf.Clamp(Vector2.SignedAngle(projDirection, direction), -_maxTurnAngle, _maxTurnAngle);
			_projectile.TryChangeDirection(projDirection.Rotate(angle * Mathf.Deg2Rad * Time.deltaTime).SetVectorLength(Mathf.Lerp(0.01f, 1, Mathf.Min(delta2, 1))));
		}
	}
}