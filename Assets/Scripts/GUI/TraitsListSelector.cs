using UnityEngine;
using UnityEngine.UI;

namespace YaEm.GUI
{
	public sealed class TraitsListSelector : MonoBehaviour
	{
		[SerializeField] private TraitsListDisplay _bodyDisplay;
		[SerializeField] private TraitsListDisplay _weaponDisplay;
		[SerializeField] private TraitsListDisplay _abilityDisplay;

		[SerializeField] private Button _bodyButton;
		[SerializeField] private Button _weaponButton;
		[SerializeField] private Button _abilityButton;

		private VerticalLayoutGroup _selected;

		private void Start()
		{
			_weaponDisplay.Group.gameObject.SetActive(false);
			_abilityDisplay.Group.gameObject.SetActive(false);
			_selected = _bodyDisplay.Group;

			_bodyButton.onClick.AddListener(() => { _selected.gameObject.SetActive(false); _bodyDisplay.Group.gameObject.SetActive(true); _selected = _bodyDisplay.Group;  });
			_weaponButton.onClick.AddListener(() => { _selected.gameObject.SetActive(false); _weaponDisplay.Group.gameObject.SetActive(true); _selected = _weaponDisplay.Group; });
			_abilityButton.onClick.AddListener(() => { _selected.gameObject.SetActive(false); _abilityDisplay.Group.gameObject.SetActive(true); _selected = _abilityDisplay.Group; });
		}
	}
}