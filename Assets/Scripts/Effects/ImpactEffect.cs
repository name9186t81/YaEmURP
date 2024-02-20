using UnityEngine;

using YaEm.Weapons;

using YaEm;
using YaEm.Health;

public class ImpactEffect : MonoBehaviour
{
	[SerializeField] private Projectile _projectile;
	[SerializeField] private GameObject _impactSFX;
	[SerializeField] private float _lifeTime;
	[SerializeField, Range(0, 1f)] private float _spawnChance;
	private float _elapsed;

	private void Awake()
	{
		_projectile.OnInit += Init;
		_projectile.OnHit += Hit;
	}

	private void Hit(RaycastHit2D arg1, IDamageReactable arg2)
	{
		if (Random.value > _spawnChance) return;
		_impactSFX.SetActive(true);
		if (_impactSFX.TryGetComponent<ParticleSystem>(out var particle)) particle.Play();
		_elapsed = 0f;
		_impactSFX.transform.rotation = Quaternion.Euler(0, 0, arg1.normal.AngleFromVector() - 90);
	}

	private void Update()
	{
		_elapsed += Time.deltaTime;
		if(_elapsed > _lifeTime)
		{
			_impactSFX.SetActive(false);
		}
	}

	private void Init()
	{
		_elapsed = 0f;
		_impactSFX.SetActive(false);
	}
}
