using UnityEngine;

using YaEm.Weapons;

using YaEm.Health;

namespace YaEm.Effects
{
	[RequireComponent(typeof(ShockWave))]
	public class ProjectileShockwave : MonoBehaviour
	{
		[SerializeField] private Projectile _projectile;
		[SerializeField] private bool _adaptColors;
		private ShockWave _wave;

		private void Start()
		{
			_projectile.OnHit += Hit;
			_projectile.OnInit += Init;
			_wave = GetComponent<ShockWave>();
			AdaptColors();
		}

		private void Init()
		{
			AdaptColors();
			_wave.Stop();
		}

		private void AdaptColors()
		{
			if (_adaptColors)
			{
				//todo: make it work with more complex gradient
				var teamColor = ColorTable.GetColor(_projectile.TeamNumber);
				var keys = new GradientColorKey[2];
				keys[0].color = new Color(teamColor.r, teamColor.g, teamColor.b, _wave.Gradient.colorKeys[0].color.a);
				keys[1].color = new Color(teamColor.r, teamColor.g, teamColor.b, _wave.Gradient.colorKeys[1].color.a);
				_wave.Gradient.colorKeys = keys;
			}
		}
		private void Hit(RaycastHit2D arg1, IDamageReactable arg2)
		{
			AdaptColors();
			_wave.Play();
		}
	}
}