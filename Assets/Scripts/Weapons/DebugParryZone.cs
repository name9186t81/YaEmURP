using YaEm.Core;
using YaEm;

using System;

using UnityEngine;

using YaEm.Weapons;

public class DebugParryZone : MonoBehaviour, IActor, ITeamProvider
{
	[SerializeField] private Vector2 _boxSize;
	[SerializeField] private Vector2 _boxOffset;
	[SerializeField] private LayerMask _layerMask;
	[SerializeField] private int _teamNumber;
	[SerializeField] private bool _reflect;

	public event Action OnInit;
	public event Action<IController, IController> OnControllerChange;
	public event Action<int, int> OnTeamNumberChange;
	public event Action<ControllerAction> OnAction;

	public Vector2 Position => throw new NotImplementedException();

	public float Rotation => throw new NotImplementedException();

	public float Scale => throw new NotImplementedException();

	public IController Controller => throw new NotImplementedException();

	public string Name => throw new NotImplementedException();

	public int TeamNumber => _teamNumber;
	private Collider2D[] _colliders = new Collider2D[32];
	private void Update()
	{
		Vector2 pos = transform.position;
		Vector2 point1 = pos + (_boxOffset + _boxSize / 2).Rotate(transform.eulerAngles.z * Mathf.Deg2Rad);
		Vector2 point2 = pos + (_boxOffset - _boxSize / 2).Rotate(transform.eulerAngles.z * Mathf.Deg2Rad);
		int size = Physics2D.OverlapAreaNonAlloc(point1, point2, _colliders);

		if (_colliders != null && _colliders.Length > 0)
		{
			foreach (var p in _colliders)
			{
				if (p != null && p.TryGetComponent<IProjectile>(out var parriable))
				{
					if (_reflect)
					{
						parriable.TryChangeDirection(Vector2.Reflect(parriable.Direction, pos.GetDirection(parriable.Position).x < 0 ? Vector2.right : Vector2.left));
					}

					if ((parriable.ProjectileFlags & ProjectileFlags.Parriable) != 0)
						parriable.Parry(this, transform.position.GetDirectionNormalized(parriable.StartPosition));
					else
						parriable.Destroy();
				}
			}
		}
	}

	private void OnDrawGizmos()
	{
		Vector2 selfPos = transform.position;
		float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
		Vector2 halfedBox = _boxSize / 2;

		Vector2 point1 = (new Vector2(halfedBox.x, halfedBox.y) + _boxOffset).Rotate(angle) + selfPos;
		Vector2 point2 = (new Vector2(halfedBox.x, -halfedBox.y) + _boxOffset).Rotate(angle) + selfPos;
		Vector2 point3 = (new Vector2(-halfedBox.x, -halfedBox.y) + _boxOffset).Rotate(angle) + selfPos;
		Vector2 point4 = (new Vector2(-halfedBox.x, halfedBox.y) + _boxOffset).Rotate(angle) + selfPos;

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(point1, point2);
		Gizmos.DrawLine(point2, point3);
		Gizmos.DrawLine(point3, point4);
		Gizmos.DrawLine(point4, point1);
	}

	public bool TryChangeController(in IController controller)
	{
		return false;
	}

	public bool TryChangeTeamNumber(int newTeamNumber)
	{
		throw new NotImplementedException();
	}
}
