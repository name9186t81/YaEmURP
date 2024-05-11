using UnityEngine;

using YaEm.Core;

namespace YaEm.Effects
{
	public sealed class ActorSpriteTrails : MonoBehaviour, IActorComponent
	{
		[SerializeField] private SpriteTrail _prefab;
		private IActor _actor;
		public IActor Actor { set => _actor = value; }

		public void Init(IActor actor)
		{
			_actor = actor;

			if((_actor as MonoBehaviour).TryGetComponent<ActorGraphics>(out var graphics))
			{
				foreach(var info in graphics.Infos)
				{
					if ((info.Flags & ActorGraphics.GraphicsFlags.NoEffects) != 0 || info.Renderer == null) continue;

					var go = Instantiate(_prefab, Vector3.zero, info.Renderer.transform.localRotation,info.Renderer.transform);
					go.name = info.Renderer.sprite.name + "_trail";
					go.SetSprite(info.Renderer.sprite);
					go.Init(_actor);
					go.transform.localPosition = Vector3.zero;

					if(go.TryGetComponent<SpriteTrailOnAbilityUse>(out var usage))
					{
						usage.Init(_actor);
					}
				}
			}
			else
			{
				Debug.LogError("Cannot find actor graphics...");
			}
		}
	}
}