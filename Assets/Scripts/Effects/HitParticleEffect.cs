using YaEm.Core;
using YaEm.Health;

using System.Collections;

using UnityEngine;

namespace YaEm.Effects
{
	public sealed class HitParticleEffect : MonoBehaviour
	{
		[SerializeField] private ParticleSystem _hitEffectPrefab;
		[SerializeField] private DamageFlags _excludedTags;
		[SerializeField] private bool _applyTeamColor = true;
		private Pool<ParticleSystem> _systemPool;
		private IActor _owner;

		private void Awake()
		{
			if(!TryGetComponent<IProvider<IHealth>>(out var prov))
			{
				Debug.LogError("Cannot find health provider");
				Destroy(gameObject);
				return;
			}

			_owner = GetComponent<IActor>();
			_owner.OnInit += Init;
		}

		private void Init()
		{
			(_owner as IProvider<IHealth>).Value.OnDamage += Damaged;
			_systemPool = new Pool<ParticleSystem>(() => Instantiate(_hitEffectPrefab, Vector3.zero, Quaternion.identity, transform));
			_owner.OnInit -= Init;
		}

		private void Damaged(DamageArgs obj)
		{
			if ((obj.DamageFlags & _excludedTags) != 0) return;
			var system = _systemPool.Get();
			if (_applyTeamColor)
			{
				system.startColor = ServiceLocator.Get<ColorTable>().GetColor(_owner == null || !(_owner is ITeamProvider prov) ? 0 : prov.TeamNumber);
			}
			system.Play();
			system.transform.position = obj.HitPosition;
			system.transform.rotation = Quaternion.Euler(0, 0, transform.position.GetDirection(obj.HitPosition).AngleFromVector() - 90);
			StartCoroutine(DespawnRoutine(system));
		}

		private IEnumerator DespawnRoutine(ParticleSystem system)
		{
			yield return new WaitForSeconds(system.duration);
			_systemPool.ReturnToPool(system);
		}
	}
}