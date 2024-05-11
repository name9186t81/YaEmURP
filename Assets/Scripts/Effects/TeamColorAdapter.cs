using YaEm;
using YaEm.Core;

using UnityEngine;
using Global;
using System;

public class TeamColorAdapter : MonoBehaviour
{
	[SerializeField] private ActorGraphics _actorGraphics;
	[SerializeField, Obsolete, Tooltip("this field is old use ActorGraphics component instead")] 
	private SpriteRenderer[] _renderers;
	[SerializeField] private LineRenderer[] _lines;
	[SerializeField] private TrailRenderer[] _trails;
	[SerializeField] private ParticleSystem[] _particles;

	private ITeamProvider _provider;

	private void Awake()
	{
		if(!TryGetComponent<ITeamProvider>(out var prov))
		{
			Debug.LogWarning("Cannot find team provider on " + gameObject.name);
			return;
		}

		_provider = prov;
		prov.OnTeamNumberChange += UpdateColors;
		UpdateColors(0, prov.TeamNumber);

		if(_actorGraphics != null)
		{
			_actorGraphics.OnGraphicsAdded += SpriteAdded;
		}
	}

	private void SpriteAdded(ActorGraphics.SpriteInfo obj)
	{
		if (obj.Renderer != null)
		{
			obj.Renderer.color = ServiceLocator.Get<ColorTable>().GetColor(_provider.TeamNumber);
		}

		if(obj.ParticleRenderer != null)
		{
			obj.ParticleRenderer.startColor = ServiceLocator.Get<ColorTable>().GetColor(_provider.TeamNumber);
		}
	}

	private void UpdateColors(int arg1, int arg2)
	{
		var color = ServiceLocator.Get<ColorTable>().GetColor(arg2);
		for (int i = 0; i < _renderers.Length; i++)
		{
			if (_renderers[i] == null) continue;
			_renderers[i].color = new Color(color.r, color.g, color.b, _renderers[i].color.a);
		}
		for (int i = 0; i < _lines.Length; i++)
		{
			if (_lines[i] == null) continue;
			_lines[i].startColor = color;
			_lines[i].endColor = new Color(color.r, color.g, color.b, _lines[i].endColor.a);
			//_lines[i].material.color = color;
		}
		for (int i = 0; i < _trails.Length; i++)
		{
			if (_trails[i] == null) continue;
			_trails[i].startColor = color;
			_trails[i].endColor = new Color(color.r, color.g, color.b, _trails[i].endColor.a);
		}
		for (int i = 0; i < _particles.Length; i++)
		{
			if (_particles[i] == null) continue;
			_particles[i].startColor = color;
		}

		if(_actorGraphics != null)
		{
			foreach(var info in _actorGraphics.Infos)
			{
				if((info.Flags & ActorGraphics.GraphicsFlags.NoTeamChange) != 0) continue;

				if (info.Renderer != null) info.Renderer.color = color;
				if(info.ParticleRenderer != null) info.ParticleRenderer.startColor = color;
			}
		}
	}
}
