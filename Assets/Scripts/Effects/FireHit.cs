using UnityEngine;

using YaEm.Weapons;

using YaEm.Health;

using YaEm.Core;

public class FireHit : MonoBehaviour
{
	[SerializeField] private Projectile _projectile;
	[SerializeField] private OnFireEffectApplier _effector;

	private void Awake()
	{
		_projectile.OnHit += Hit;
	}

	private void Hit(RaycastHit2D arg1, IDamageReactable arg2)
	{
		if (arg2 != null && arg1.transform.TryGetComponent<IProvider<IHealth>>(out var react) && react.Value.CanTakeDamage(_projectile.Args))
		{
			_effector.Apply(react, _projectile.Source, arg1.transform.position, arg1.transform);
			if(_effector.Spawned != null && _effector.Spawned.TryGetComponent<FlameTeamColorAdapter>(out var adapter))
			{
				adapter.ChangeColors(_projectile.TeamNumber);
			}
		}
	}
}
