using UnityEngine;

using YaEm.Weapons;

namespace YaEm.Effects
{
	public sealed class ClearTrailOnRestore : MonoBehaviour
	{
		[SerializeField] private Projectile _projectile;
		[SerializeField] private TrailRenderer _renderer;

		private void Awake()
		{
			_projectile.OnInit += Clear;
		}

		private void Clear()
		{
			_renderer.Clear();
		}
	}
}