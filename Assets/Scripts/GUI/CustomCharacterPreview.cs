using Global;

using UnityEngine;

using YaEm.Ability;
using YaEm.AI;
using YaEm.Core;
using YaEm.Weapons;

namespace YaEm.GUI
{
	public sealed class CustomCharacterPreview : MonoBehaviour
	{
		[SerializeField] private BodyTrait _defaultBody;
		[SerializeField] private WeaponTrait _defaultWeapon;

		[SerializeField] private TraitsListDisplay _bodyList;
		[SerializeField] private TraitsListDisplay _weaponList;
		[SerializeField] private TraitsListDisplay _abilityList;

		[SerializeField] private GameObject _parent;
		[SerializeField] private float _scale = 1;

		[SerializeField] private float _changeTime;
		[SerializeField] private int _maxTeam;

		private int _team = 1;
		private float _elapsed;
		private AbilityTrait _ability;
		private WeaponTrait _weaponTrait;
		private Unit _body;
		private RangedWeapon _weapon;
		private PolarTransform _initedAbility;

		private void Start()
		{
			_bodyList.OnTraitChanged += BodyTraitChanged;
			_weaponList.OnTraitChanged += WeaponTraitChanged;
			_abilityList.OnTraitChanged += AbilityTraitChanged;

			_weaponTrait = _defaultWeapon;
			InitBody(_defaultBody);
		}

		private void Update()
		{
			_elapsed += Time.deltaTime;
			if (_elapsed > _changeTime)
			{
				_elapsed = 0;
				_team++;
				if(_team == _maxTeam)
				{
					_team = 1;
				}

				_body.TryChangeTeamNumber(_team);
			}
			_weapon.transform.localPosition = new Vector3(_weapon.transform.localPosition.x, _weapon.transform.localPosition.y, 0);
		}

		private void AbilityTraitChanged(TraitBase obj)
		{
			if (obj.Type != TraitBase.TraitType.Ability || obj is not AbilityTrait trait)
			{
				Debug.LogError("Ability trait: " + obj + " is not ability trait type!");
				return;
			}

			_ability = trait;
			InitAbility(_ability);
		}

		private void WeaponTraitChanged(TraitBase obj)
		{
			if (obj.Type != TraitBase.TraitType.Weapon || obj is not WeaponTrait trait)
			{
				Debug.LogError("Weapon trait: " + obj + " is not weapon trait type!");
				return;
			}

			_weaponTrait = trait;
			InitWeapon(trait, true);
		}

		private void BodyTraitChanged(TraitBase obj)
		{
			if(obj.Type != TraitBase.TraitType.Body || obj is not BodyTrait trait)
			{
				Debug.LogError("Body trait: " + obj + " is not body trait type!");
				return;
			}

			InitBody(trait);
			InitWeapon(_weaponTrait, false);
			InitAbility(_ability);
		}

		private void InitBody(BodyTrait trait)
		{
			if (_body != null) Destroy(_body.gameObject);

			_body = Instantiate(trait.Unit, Vector3.zero, Quaternion.identity, _parent.transform);
			_body.transform.localPosition = Vector3.zero;
			_body.SetScale(0.5f * _scale / 35.5f * _body.Scale / 0.54f); //???
			_body.transform.localScale = Vector3.one * _scale;
			if (_body.TryGetComponent<AIController>(out var controller))
			{
				controller.Disable();
			}
			_body.TryChangeTeamNumber(_team);

			ServiceLocator.Get<PlayerChararcterContainer>().Body = trait;
		}

		private void InitWeapon(WeaponTrait trait, bool reinit)
		{
			if(_body == null || !_body.isActiveAndEnabled)
			{
				Debug.LogError("Trying to change weapon while body is not inited");
				return;
			}

			if (_weapon != null)
			{
				if (_weapon.TryGetComponent<ActorGraphics>(out var graphics) && _body.TryGetComponent<ActorGraphics>(out var graph))
				{
					graph.Separate(graphics);
				}
				Destroy(_weapon.gameObject);
			}

			_weapon = Instantiate(trait.Weapon, _body.transform.position, Quaternion.identity, _body.transform);
			//_weapon.transform.localPosition = Vector3.zero;
			if(reinit) 
				_weapon.Init(_body);

			if (_weapon.TryGetComponent<ActorGraphics>(out var gr1) && _body.TryGetComponent<ActorGraphics>(out var gr2))
			{
				gr2.Merge(gr1);
			}

			ServiceLocator.Get<PlayerChararcterContainer>().Weapon = trait;
		}

		private void InitAbility(AbilityTrait trait)
		{
			if (_body == null || !_body.isActiveAndEnabled)
			{
				Debug.LogError("Trying to change ability while body is not inited");
				return;
			}

			if(_initedAbility != null)
			{
				if(_initedAbility.TryGetComponent<ActorGraphics>(out var graphics) && _body.TryGetComponent<ActorGraphics>(out var graph))
				{
					graph.Separate(graphics);
				}
				Destroy(_initedAbility.gameObject);
			}

			if (trait == null || trait.Visual == null) return;

			_initedAbility = Instantiate(trait.Visual, _body.transform.position, Quaternion.identity, _body.transform);
			_initedAbility.Offset *= _body.Scale;
			_initedAbility.transform.position = _initedAbility.GetPosition() + (Vector2)_body.transform.position;

			if (_initedAbility.TryGetComponent<ActorGraphics>(out var gr1) && _body.TryGetComponent<ActorGraphics>(out var gr2))
			{
				gr2.Merge(gr1);
			}

			ServiceLocator.Get<PlayerChararcterContainer>().Ability = trait;
		}
	}
}