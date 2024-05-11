using System;

using UnityEngine;

namespace YaEm.GUI
{
	[Obsolete]
	public class CameraFollower : GUIElement
	{
		//[SerializeField] private Transform _camera;
		//[SerializeField] private Joystick _stick;
		//[SerializeField] private float _strength;
		//private Vector2 _offset;

		//private void Update()
		//{
		//	if (Player != null && !Player.Equals(null))
		//	{	
		//		Vector2 offset = Vector2.zero;
		//		if(_stick != null)
		//		{
		//			offset = _stick.Direction * _strength;
		//		}
		//		_offset = Vector2.Lerp(_offset, offset, 0.01f);
		//		_camera.position = new Vector3(Player.Position.x + _offset.x, Player.Position.y + _offset.y, _camera.position.z);
		//	}
		//}
	}
}
