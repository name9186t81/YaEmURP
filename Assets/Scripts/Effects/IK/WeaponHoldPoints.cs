using UnityEngine;

namespace YaEm.Effects
{
	public sealed class WeaponHoldPoints : MonoBehaviour
	{
		[SerializeField] private Transform _rightPoint;
		[SerializeField] private Transform _leftPoint;

		public int PointsCount => _leftPoint == null ? 1 : 2;
		public Transform RightPoint => _rightPoint;
		public Transform LeftPoint => _leftPoint;
	}
}