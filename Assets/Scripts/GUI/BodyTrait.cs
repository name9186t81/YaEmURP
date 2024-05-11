using UnityEngine;

namespace YaEm.GUI
{
	[CreateAssetMenu(fileName = "Body trait", menuName = "YaEm/Body Trait")]
	public sealed class BodyTrait : TraitBase
	{
		[SerializeField] private Unit _unit;

		public Unit Unit => _unit;
	}
}