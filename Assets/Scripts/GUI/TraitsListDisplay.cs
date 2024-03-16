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
			public string Name;
			public TraitType Type;
			public GameObject DisplayPrefab;
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
		private SelectableTraitDisplay _selected;
		private SelectableTraitDisplay[] _displays;
		private VerticalLayoutGroup _group;

		private void Start()
		{
			_group = Instantiate(_layoutPrefab, _holder);
			_group.name += _type.ToString();
			_displays = new SelectableTraitDisplay[_traits.Length];
			for(int i = 0; i < _traits.Length; i++)
			{
				_displays[i] = Instantiate(_traitPrefab, _group.transform);
				_displays[i].UpdateDisplay(_traits[i].Icon, _traits[i].Description, _traits[i].Name, _traits[i].DisplayPrefab);
				_displays[i].UnSelect();
				_displays[i].OnClick += SelectTrait;
			}
		}

		public void SelectTrait(string name)
		{
			Debug.Log(name);
			int hash = name.GetHashCode();
			int ind = 0;
			foreach(var trait in _traits)
			{
				if(hash == trait.Name.GetHashCode())
				{
					if(_selected != null)
					{
						_selected.UnSelect();
					}
					_selected = _displays[ind];
					_selected.Select();
				}
				ind++;
			}
		}

		public TraitType Type => _type;
	}
}