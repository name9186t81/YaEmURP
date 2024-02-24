using UnityEngine;

using YaEm.Core;

using static UnityEngine.Rendering.HableCurve;

namespace YaEm.Effects
{
	//todo use Graphics.Draw methods to not create tons of gameObjects
	public sealed class SpriteTrail : MonoBehaviour, IActorComponent
	{
		private class Segment
		{
			public Transform Transform;
			public Vector2 StartPosition;
			public SpriteRenderer Renderer;
			public float Elapsed;
			public float Rotation;
			public bool Activated;
		}

		[SerializeField] private Sprite _base;
		[SerializeField] private Gradient _colorOverTrail;
		[SerializeField] private Material _material;
		[SerializeField] private float _duration;
		[SerializeField] private bool _changeTeamColor;
		[SerializeField] private int _count;
		private Gradient _grad;
		private IActor _actor;
		private Segment[] _segments;
		private bool _isActive = false;
		private Transform _cached;

		public IActor Actor { set => _actor = value; }

		private void Awake()
		{
			_segments = new Segment[_count];
			_grad = _colorOverTrail;
			_cached = transform;
			for (int i = 0; i < _count; i++)
			{
				var obj = new GameObject();
				obj.transform.SetParent(transform);
				obj.name = "Segment " + i;

				var renderer = obj.AddComponent<SpriteRenderer>();
				renderer.sharedMaterial = _material;
				renderer.sprite = _base;

				float delta = i.Delta(_count);
				_segments[i] = new Segment()
				{
					StartPosition = Vector2.zero,
					Elapsed = -delta * _duration,
					Renderer = renderer,
					Transform = obj.transform
				};
				renderer.enabled = false;
			}
		}

		private void Update()
		{
			if(!_isActive) return;

			for(int i = 0; i < _count; ++i)
			{
				var segment = _segments[i];
				segment.Elapsed += Time.deltaTime;
				if(segment.Elapsed > _duration)
				{
					segment.Renderer.enabled = true;
					segment.Activated = true;
					segment.StartPosition = _cached.position;
					segment.Rotation = _cached.eulerAngles.z;
					segment.Elapsed = 0f;
				}

				if (segment.Activated)
				{
					float delta = segment.Elapsed.Delta(_duration);
					segment.Renderer.color = _grad.Evaluate(delta);
					segment.Transform.position = (Vector3)segment.StartPosition + Vector3.forward * delta;
					segment.Transform.rotation = Quaternion.Euler(Vector3.forward * segment.Rotation);
				}
			}
		}

		public void Deactivate()
		{
			_isActive = false;
			for(int i = 0; i < _count; i++)
			{
				_segments[i].Activated = false;
				_segments[i].Renderer.enabled = false;
				_segments[i].StartPosition = _cached.position;
				_segments[i].Elapsed = i.Delta(_count) * _duration;
			}
		}

		public void Activate()
		{
			_isActive = true;
		}

		public void Init(IActor actor)
		{
			_actor = actor;

			if(_changeTeamColor && actor is ITeamProvider prov)
			{
				prov.OnTeamNumberChange += Changed;
				UpdateGradient(prov.TeamNumber);
			}
		}

		private void UpdateGradient(int team)
		{
			_grad = ColorTable.GetColoredGradient(_colorOverTrail, team);
		}
		private void Changed(int arg1, int arg2)
		{
			UpdateGradient(arg2);
		}
	}
}