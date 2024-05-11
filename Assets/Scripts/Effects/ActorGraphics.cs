using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace YaEm
{
	public sealed class ActorGraphics : MonoBehaviour
	{
		[Flags]
		public enum GraphicsFlags
		{
			None = 0,
			NoEffects = 1,
			NoTeamChange = 2
		}

		[Serializable]
		public sealed class SpriteInfo
		{
			public SpriteInfo(SpriteRenderer renderer, GraphicsFlags flags)
			{
				Renderer = renderer;
				Flags = flags;
			}

			[field: SerializeField] public SpriteRenderer Renderer { get; private set; }
			[field: SerializeField] public ParticleSystem ParticleRenderer { get; private set; }
			[field: SerializeField] public GraphicsFlags Flags { get; private set; }
		}

		[SerializeField] private List<SpriteInfo> _infos;
		public event Action<SpriteInfo> OnGraphicsAdded;
		public event Action<SpriteInfo> OnGraphicsRemoved;

		public void AddSprite(SpriteRenderer renderer, GraphicsFlags flags)
		{
			var info = new SpriteInfo(renderer, flags);
			_infos.Add(info);
			OnGraphicsAdded?.Invoke(info);
		}

		public bool RemoveSprite(SpriteRenderer renderer)
		{
			var info = _infos.FirstOrDefault((SpriteInfo inf) => inf.Renderer == renderer);
			if (info != default(SpriteInfo))
			{
				OnGraphicsRemoved?.Invoke(info);
				_infos.Remove(info);
				return true;
			}
			return false;
		}

		public void Merge(ActorGraphics other)
		{
			foreach(var info in other.Infos)
			{
				_infos.Add(info);
				OnGraphicsAdded?.Invoke(info);
			}
		}

		public void Separate(ActorGraphics other)
		{
			foreach (var info in other.Infos)
			{
				if (_infos.Remove(info))
				{
					OnGraphicsRemoved?.Invoke(info);
				}
			}
		}

		public IReadOnlyList<SpriteInfo> Infos => _infos;
	}
}