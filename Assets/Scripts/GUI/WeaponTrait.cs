using UnityEngine;

using YaEm.Weapons;

namespace YaEm.GUI
{
	[CreateAssetMenu(fileName = "Weapon Trait", menuName = "YaEm/WeaponTrait")]
	public sealed class WeaponTrait : TraitBase
	{
		[SerializeField] private RangedWeapon _weapon;

		public RangedWeapon Weapon => _weapon;
	}
}