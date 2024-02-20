using UnityEngine;
using YaEm;
using YaEm.Core;
using System;

public class DebugController : MonoBehaviour, IController
{
	[SerializeField] private bool _affectedBySlowMotion;
	private Vector2 _moveDir;
	private float _rotation;
	public ControllerType Type => ControllerType.Player;

	public Vector2 DesiredMoveDirection => _moveDir;

	public float DesiredRotation => _rotation;

	public event Action<ControllerAction> ControllerAction;

	[SerializeField] private Unit _target;
	public Unit Target => _target;

	public bool IsEffectedBySlowMotion => _affectedBySlowMotion;

	private void Start()
	{
		if(_target != null)
		{
			_target.TryChangeController(this);
		}
	}
	private void Update()
	{
		Vector2 moveDir = Vector2.zero;
		if (Input.GetKey(KeyCode.W)) moveDir += Vector2.up;
		if (Input.GetKey(KeyCode.S)) moveDir += Vector2.down;
		if (Input.GetKey(KeyCode.A)) moveDir += Vector2.left;
		if (Input.GetKey(KeyCode.D)) moveDir += Vector2.right;

		if (Input.GetKey(KeyCode.E) && ServiceLocator.TryGet<GlobalTimeModifier>(out var mod)) mod.SetTimeModificator(0.25f); 
		if (Input.GetKey(KeyCode.R) && ServiceLocator.TryGet<GlobalTimeModifier>(out var mod2)) mod2.SetTimeModificator(1f); 

		if (Input.GetMouseButton(0)) ControllerAction?.Invoke(YaEm.Core.ControllerAction.Fire);
		if (Input.GetMouseButtonDown(1)) ControllerAction?.Invoke(YaEm.Core.ControllerAction.Charge);
		if (Input.GetMouseButtonUp(1)) ControllerAction?.Invoke(YaEm.Core.ControllerAction.BreakCharge);

		_rotation = (transform.position.GetDirection(Camera.main.ScreenToWorldPoint(Input.mousePosition))).AngleFromVector() - 90;
		_moveDir = moveDir;
	}
}
