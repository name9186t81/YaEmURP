using UnityEngine;

using YaEm.Core;
using YaEm.Weapons;

using static UnityEngine.UI.CanvasScaler;

namespace YaEm.Effects
{
	[DisallowMultipleComponent(), RequireComponent(typeof(IActorComponent))]
	public class UnitHandsController : MonoBehaviour, IActorComponent
	{
		[SerializeField] private FABRIKLineRenderer _rightHandRenderer;
		[SerializeField] private FABRIKLineRenderer _leftHandRenderer;
		private WeaponHoldPoints _holders;
		private bool _dirty;
		private IWeapon _weapon;
		private IActor _unit;

		public IActor Actor { set => _unit = value	; }

		public void Init(IActor actor)
		{
			_unit = actor;
			if ((_unit as MonoBehaviour).TryGetComponentInChildren<WeaponHoldPoints>(out var points))
			{
				_holders = points;
			}
		}


		private void Update()
		{
			if (_holders == null) return;

			if(!_unit.IsVisible && _dirty)
			{
				_rightHandRenderer.Disable();
				_leftHandRenderer.Disable();
				_dirty = false;
			}
			else if(_unit.IsVisible && !_dirty)
			{
				_rightHandRenderer.Enable();
				_leftHandRenderer.Enable();
				_dirty = true;
			}

			if(_holders == null && (_unit as MonoBehaviour).TryGetComponentInChildren<WeaponHoldPoints>(out var points))
			{
				_holders = points;
			}

			if(_holders.PointsCount == 2)
			{
				_rightHandRenderer.SetEndPoint(_holders.RightPoint);
				_leftHandRenderer.SetEndPoint(_holders.LeftPoint);
			}
			else
			{
				_rightHandRenderer.SetEndPoint(_holders.RightPoint);
			}
		}
	}
}