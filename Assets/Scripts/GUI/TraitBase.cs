using UnityEngine;

namespace YaEm.GUI
{
	public abstract class TraitBase : ScriptableObject
	{
		public enum TraitType
		{
			Body,
			Weapon,
			Ability
		}

		[SerializeField] private string _name;
		[SerializeField] private TraitType _type;

		public string Name => _name;
		public TraitType Type => _type;
	}
}