using YaEm.GUI;

namespace YaEm.Core
{
	public sealed class PlayerChararcterContainer : IService
	{
		public BodyTrait Body { get; set; }
		public WeaponTrait Weapon { get; set; }
		public AbilityTrait Ability { get; set; }
	}
}