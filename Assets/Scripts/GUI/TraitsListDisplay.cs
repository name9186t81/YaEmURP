using System;

using UnityEngine;
using UnityEngine.UI;

namespace YaEm.GUI
{
	[DisallowMultipleComponent()]
	public sealed class TraitsListDisplay : MonoBehaviour
	{
		[Serializable]
		private struct Trait
		{
			public TraitBase Base;
			public string Description;
			public Sprite Icon;
		}
		public enum TraitType
		{
			Body,
			Weapon,
			Ability
		}

		[SerializeField] private Transform _holder;
		[SerializeField] private Trait[] _traits;
		[SerializeField] private VerticalLayoutGroup _layoutPrefab;
		[SerializeField] private SelectableTraitDisplay _traitPrefab;
		[SerializeField] private TraitType _type;
		public event Action<TraitBase> OnTraitChanged;
		private SelectableTraitDisplay _selected;
		private SelectableTraitDisplay[] _displays;
		private VerticalLayoutGroup _group;

		private void Awake()
		{
			_group = Instantiate(_layoutPrefab, _holder);
			_group.name += _type.ToString();
			_displays = new SelectableTraitDisplay[_traits.Length];
			for(int i = 0; i < _traits.Length; i++)
			{
				_displays[i] = Instantiate(_traitPrefab, _group.transform);
				_displays[i].UpdateDisplay(_traits[i].Icon, _traits[i].Description, _traits[i].Base.Name);
				_displays[i].UnSelect();
				_displays[i].OnClick += SelectTrait;
			}
		}

		public void SelectTrait(string name)
		{
			int hash = name.GetHashCode();
			int ind = 0;
			foreach(var trait in _traits)
			{
				if(hash == trait.Base.Name.GetHashCode())
				{
					if(_selected != null)
					{
						_selected.UnSelect();
					}
					_selected = _displays[ind];
					_selected.Select();
					OnTraitChanged?.Invoke(trait.Base);
					return;
				}
				ind++;
			}
		}

		public VerticalLayoutGroup Group => _group;
		public TraitType Type => _type;
	}
}