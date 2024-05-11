using UnityEngine;

using YaEm.GUI;

namespace YaEm.Dialogues
{
	public abstract class DialogueTrigger : MonoBehaviour
	{
		[Header("Main Settings")]
		[SerializeField] private Dialogue _dialogue;
		[SerializeField] private bool _triggeredOnce;

		[Space]
		[Header("Camera interaction")]
		[SerializeField] private bool _triggerCameraFocus;
		[SerializeField] private Transform _target;
		[SerializeField] private float _targetReachTime;
		[SerializeField] private float _stayTime;
		[SerializeField] private bool _stayFocused;

		[Space]
		[Header("Slowmotion interaction")]
		[SerializeField] private bool _triggerSlowmotion;
		[SerializeField] private float _minTimeScale;
		[SerializeField] private float _reachTime;
		private bool _triggered = false;
		private bool _doSlowmotion;
		private float _elapsed;

		private void Awake()
		{
			ServiceLocator.Get<DialogueService>().OnDialogueEnded += Restart;
		}

		protected virtual void Restart(Dialogue obj)
		{
			if (obj != _dialogue) return;

			if (_doSlowmotion)
			{
				ServiceLocator.Get<GlobalTimeModifier>().SetTimeModificator(1f);
				_doSlowmotion = false;
			}

			if(_triggerCameraFocus && _stayFocused && ServiceLocator.TryGet<CameraStateMachine>(out var stateMachine))
			{
				stateMachine.UnFocus();
			}
		}

		protected void Trigger()
		{
			if (_triggeredOnce && _triggered) return;

			_triggered = true;
			if(ServiceLocator.TryGet<DialogueService>(out var service))
			{
				service.TriggerDialogue(_dialogue);
			}

			if (_triggerCameraFocus && ServiceLocator.TryGet<CameraStateMachine>(out var stateMachine))
			{
				stateMachine.Focus(_target, _targetReachTime, _stayTime, _stayFocused);
			}

			_doSlowmotion = _triggerSlowmotion;
			if(_doSlowmotion)
			{
				_elapsed = 0f;
			}
		}

		private void Update()
		{
			if (_doSlowmotion)
			{
				_elapsed += Time.deltaTime;
				float delta = _elapsed / _reachTime;

				float slowmotion = Mathf.Lerp(1f, _minTimeScale, Mathf.Ceil((delta * delta - 1 / 8f) * 4) / 4);
				ServiceLocator.Get<GlobalTimeModifier>().SetTimeModificator(slowmotion);
			}
		}

		protected virtual void EffectUpdate(float dt) { }
	}
}