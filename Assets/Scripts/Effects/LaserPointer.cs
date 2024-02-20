using UnityEngine;

using YaEm.Weapons;

[RequireComponent(typeof(LineRenderer))]
public class LaserPointer : MonoBehaviour
{
	[SerializeField] private RangedWeapon _weapon;
	[SerializeField] private float _maxRange;
	[SerializeField] private LayerMask _mask;
	private LineRenderer _lineRenderer;

	private void Awake()
	{
		_lineRenderer = GetComponent<LineRenderer>();
		_lineRenderer.positionCount = 2;
		_lineRenderer.useWorldSpace = true;
	}

	private void Update()
	{
		var ray = Physics2D.Raycast(_weapon.GlobalShootPoint, _weapon.ShootDirection, _maxRange, _mask);
		if (ray)
		{
			_lineRenderer.SetPosition(1, ray.point);
		}
		else
		{
			_lineRenderer.SetPosition(1, _weapon.ShootDirection * _maxRange + _weapon.GlobalShootPoint);
		}
		_lineRenderer.SetPosition(0, _weapon.GlobalShootPoint);
	}
}
