using UnityEngine;

using YaEm.Weapons;

namespace YaEm.Effects {
	public class OverheatingVFXControl : MonoBehaviour
	{
		[SerializeField] private WeaponOverheating _overheating;
		[SerializeField] private SpriteRenderer _weaponSprite;
		[SerializeField] private ParticleSystem _overheatParticles;
		[SerializeField] private ParticleSystem _dropParticles;

		private void Awake()
		{
			if(_overheatParticles != null)
				_overheating.OnOverheat += Overheat;
			if (_dropParticles != null)
				_overheating.OnOverheatDropped += Drop;
		}

		private void Drop()
		{
			_dropParticles.Play();
		}

		private void Overheat()
		{
			_overheatParticles.Play();
		}

		private void Update()
		{
			if(_weaponSprite != null)
			{
				Color.RGBToHSV(_weaponSprite.color, out var h, out var s, out var v);
				_weaponSprite.color = Color.HSVToRGB(h, 1 - Mathf.Clamp(_overheating.CurrentOverheat * _overheating.CurrentOverheat * _overheating.CurrentOverheat, 0f, 0.99f), v);
				//why 0.01f is min? because if its zero then unity will turn color into red for some reason
			}
		}
	}
}