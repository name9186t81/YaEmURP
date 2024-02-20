using UnityEngine;

using YaEm.Core;
using YaEm.Health;
using YaEm.Effects;

public class OnFireEffectApplier : MonoBehaviour
{
	[SerializeField] private float _ticksPerSecond;
	[SerializeField] private int _damage;
	[SerializeField] private float _life;
	[SerializeField] private GameObject _spawnedOnTarget;
	private EffectBuilder<IHealth> _fireBuilder;
	public GameObject Spawned;

	private void Awake()
	{
		_fireBuilder = new EffectBuilder<IHealth>();
		_fireBuilder.SetDamage(_damage).SetDuration(_life).SetTicksPerSecond(_ticksPerSecond);
	}

	public void Apply(IActor target, IActor owner)
	{
		if (target is IProvider<IHealth> prov) Apply(prov, owner, target.Position, (target as MonoBehaviour).transform); //remove this hack
	}

	public void Apply(IProvider<IHealth> target, IActor owner, Vector2 position, Transform targetTransform)
	{
		_fireBuilder.SetOwner(owner);
		if (target.Value.AddEffect(_fireBuilder.Build(EffectType.OnFire)))
		{
			var obj = Instantiate(_spawnedOnTarget, position, Quaternion.identity, targetTransform);
			Spawned = obj;
			Destroy(obj, _life);
		}
	}
}
