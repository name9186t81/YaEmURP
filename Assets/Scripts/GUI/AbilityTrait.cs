using UnityEngine;

using YaEm.Ability;

namespace YaEm.GUI
{
	[CreateAssetMenu(fileName = "Ability trait", menuName = "YaEm/Ability Trait")]
	public sealed class AbilityTrait : TraitBase
	{
		[SerializeField] private AbilityBuilder _builder;
		[SerializeField] private PolarTransform _visual;

		/// <summary>
		/// Visual representation of ability. CAN be null.
		/// </summary>
		public PolarTransform Visual => _visual;
		public AbilityBuilder Builder => _builder;
	}
}