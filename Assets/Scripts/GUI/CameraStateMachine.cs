using UnityEngine;

namespace YaEm.GUI
{
	[RequireComponent(typeof(Camera))]
	public sealed class CameraStateMachine : GUIElement, IService
	{
		public enum State
		{
			None,
			FollowingPlayer,
			Focus
		}

		[SerializeField] private bool _zoomOutFromStart;
		[SerializeField] private float _zoomOutTime;
		[SerializeField] private float _unFocusTime;
		[SerializeField] private float _focusedZoom;
		[SerializeField] private Transform _camera;
		[SerializeField] private Transform _target;
		private float _originalSize;
		private Camera _cameraComponent;
		private Vector2 _startPosition;
		private float _targetReachTime;
		private float _followTargetTime;
		private float _focusScale;
		private float _elapsed;
		private bool _targetReached;
		private bool _stayFocused;
		private bool _wasFocused;
		private State _state;

		public Transform Target { get => _target; set => _target = value; }

		private void Awake()
		{
			if(ServiceLocator.TryGet<CameraStateMachine>(out var stateMachine))
			{
				stateMachine._camera = _camera;
				stateMachine._focusedZoom = _focusedZoom;
				return;
			}

			_state = State.FollowingPlayer;
			ServiceLocator.Register(this);
			_cameraComponent = GetComponent<Camera>();

			_originalSize = _cameraComponent.orthographicSize;
		}

		public void Focus(Transform target, float reachTime, float followTime, bool stayFocused = false, float focusScale = 1f)
		{
			_target = target;
			_targetReachTime = reachTime;
			_followTargetTime = followTime;
			_targetReached = false;
			_focusScale = focusScale;
			_stayFocused = stayFocused;
			_startPosition = _camera.position;
			_state = State.Focus;

			_elapsed = 0f;
		}

		public void UnFocus()
		{
			_stayFocused = false;
		}

		private void Update()
		{
			if (_zoomOutFromStart)
			{
				_elapsed += Time.deltaTime;
				float delta = _elapsed / _zoomOutTime;

				float factor = -2 * delta + 2;
				_cameraComponent.orthographicSize = Mathf.Lerp(0f, _originalSize, delta < 0.5f ? 4 * delta * delta * delta : 1 - factor * factor * factor / 2);
				_zoomOutFromStart = delta < 1f;
			}

			switch (_state)
			{
				case State.FollowingPlayer: { FollowPlayer(); break; }
				case State.Focus: { Focus(); break; }
			}
		}

		private void FollowPlayer()
		{
			if (IsPlayerNull) return;

			_camera.position = (Vector3)Vector2.Lerp(_camera.position, Player.Position, 0.85f) + Vector3.forward * _camera.position.z;

			if (_wasFocused)
			{
				_elapsed += Time.deltaTime;
				float delta = _elapsed / _unFocusTime;

				float factor = 1 - delta;
				_cameraComponent.orthographicSize = Mathf.Lerp(_originalSize * _focusedZoom * _focusScale, _originalSize, 1 - factor * factor * factor);

				if(delta > 1f)
				{
					_elapsed = 0f;
					_wasFocused = false;
				}
			}
		}

		private void Focus()
		{
			if (_targetReached)
			{
				_elapsed += Time.deltaTime;
				float delta = _elapsed / _followTargetTime;

				float factor = 1 - delta;
				_cameraComponent.orthographicSize = Mathf.Lerp(_originalSize, _originalSize * _focusedZoom * _focusScale, 1 - factor * factor * factor);

				_camera.position = Vector3.Lerp(_camera.position, new Vector3(_target.position.x, _target.position.y, _camera.position.z), 0.65f);
				if(delta > 1f && !_stayFocused)
				{
					_elapsed = 0f;
					_state = State.FollowingPlayer;
				}
			}
			else
			{
				_elapsed += Time.deltaTime;
				float delta = _elapsed / _targetReachTime;

				float factor = -2 * delta + 2;
				_camera.position = (Vector3)Vector2.Lerp(_startPosition, _target.position, delta < 0.5f ? 4 * delta * delta * delta : 1 - factor * factor * factor / 2) + Vector3.forward * _camera.position.z;
				if(delta > 1f)
				{
					_wasFocused = _targetReached = true;
					_elapsed = 0f;
				}
			}
		}

		private void OnDestroy()
		{
			ServiceLocator.Remove<CameraStateMachine>();
		}
	}
}