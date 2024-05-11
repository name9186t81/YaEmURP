using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

using YaEm.Core;

using static UnityEngine.Rendering.HableCurve;

namespace YaEm.Effects
{
	//todo use Graphics.Draw methods to not create tons of gameObjects
	public sealed class SpriteTrail : MonoBehaviour, IActorComponent
	{
		private interface IRenderer
		{
			public Sprite Sprite { get; set; }
			public Color Color { get; set; }
			public Material Material { get; set; }
			public bool Enabled { get; set; }
		}
		private class SpriteRendererVariant : IRenderer
		{
			private readonly SpriteRenderer _renderer;

			public SpriteRendererVariant(SpriteRenderer renderer)
			{
				_renderer = renderer;
			}

			public Sprite Sprite { get => _renderer.sprite; set => _renderer.sprite = value; }
			public Color Color { get => _renderer.color; set => _renderer.color = value; }
			public Material Material { get => _renderer.sharedMaterial; set => _renderer.sharedMaterial = value; }
			public bool Enabled { get => _renderer.enabled; set => _renderer.enabled = value; }
		}
		private class ImageVariant : IRenderer
		{
			private readonly Image _image;

			public ImageVariant(Image image)
			{
				_image = image;
			}

			public Sprite Sprite { get => _image.sprite; set => _image.sprite = value; }
			public Color Color { get => _image.color; set => _image.color = value; }
			public Material Material { get => _image.material; set => _image.material = value; }
			public bool Enabled { get => _image.enabled; set => _image.enabled = value; }
		}
		private class Segment
		{
			public Transform Transform;
			public Vector2 StartPosition;
			public IRenderer Renderer;
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
		[SerializeField] private bool _activeFromStart = false;
		[SerializeField] private bool _affectedByTimeScale = true;
		private GlobalTimeModifier _timeMod;
		private Gradient _grad;
		private IActor _actor;
		private Segment[] _segments;
		private bool _isActive = false;
		private Transform _cached;

		public IActor Actor { set => _actor = value; }

		public void SetSprite(Sprite sprite)
		{
			_base = sprite;
		}

		private void Start()
		{
			_segments = new Segment[_count];
			_grad = _colorOverTrail;
			_cached = transform;
			bool isRect = transform.TryGetComponent<RectTransform>(out var selfRect);

			if(_affectedByTimeScale && !ServiceLocator.TryGet<GlobalTimeModifier>(out _timeMod))
			{
				Debug.LogWarning("Cannot find global time mod");
			}

			if(isRect && !TryGetComponent<Canvas>(out _))
			{
				var c = gameObject.AddComponent<Canvas>();
				c.overrideSorting = true;
			}
			for (int i = 0; i < _count; i++)
			{
				var obj = new GameObject();
				obj.transform.SetParent(transform);
				obj.name = "Segment " + i;

				IRenderer renderer = isRect ? new ImageVariant(obj.AddComponent<Image>()) : new SpriteRendererVariant(obj.AddComponent<SpriteRenderer>());
				if (isRect)
				{
					var canvas = obj.AddComponent<Canvas>();
					canvas.overrideSorting = true;
					canvas.sortingOrder = -i;
					var rect = obj.GetComponent<RectTransform>();
					rect.localScale = Vector3.one;
					obj.GetComponent<Image>().preserveAspect = true;
					rect.sizeDelta = new Vector2(selfRect.rect.width, selfRect.rect.height);
				}
				renderer.Material = _material;
				renderer.Sprite = _base;

				float delta = i.Delta(_count);
				_segments[i] = new Segment()
				{
					StartPosition = Vector2.zero,
					Elapsed = -delta * _duration,
					Renderer = renderer,
					Transform = obj.transform
				};
				renderer.Enabled = false;
			}
			_isActive = _activeFromStart;
		}

		private void Update()
		{
			if(!_isActive) return;

			for(int i = 0; i < _count; ++i)
			{
				var segment = _segments[i];
				segment.Elapsed += Time.deltaTime * (_timeMod != null ? _timeMod.TimeModificator : 1f);
				if(segment.Elapsed > _duration)
				{
					segment.Renderer.Enabled = true;
					segment.Activated = true;
					segment.StartPosition = _cached.position;
					segment.Rotation = _cached.eulerAngles.z;
					segment.Elapsed = 0f;
				}

				if (segment.Activated)
				{
					float delta = segment.Elapsed.Delta(_duration);
					segment.Renderer.Color = _grad.Evaluate(delta);
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
				_segments[i].Renderer.Enabled = false;
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
			_grad = ServiceLocator.Get<ColorTable>().GetColoredGradient(_colorOverTrail, team);
		}
		private void Changed(int arg1, int arg2)
		{
			UpdateGradient(arg2);
		}
	}
}