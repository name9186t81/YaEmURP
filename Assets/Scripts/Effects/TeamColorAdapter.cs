using YaEm;
using YaEm.Core;

using UnityEngine;

public class TeamColorAdapter : MonoBehaviour
{
	[SerializeField] private SpriteRenderer[] _renderers;
	[SerializeField] private LineRenderer[] _lines;
	[SerializeField] private TrailRenderer[] _trails;
	[SerializeField] private ParticleSystem[] _particles;

	private void Awake()
	{
		if(!TryGetComponent<ITeamProvider>(out var prov))
		{
			Debug.LogError("Cannot find team provider on " + gameObject.name);
			return;
		}

		prov.OnTeamNumberChange += UpdateColors;
		UpdateColors(0, prov.TeamNumber);
	}

	private void UpdateColors(int arg1, int arg2)
	{
		var color = ColorTable.GetColor(arg2);
		for (int i = 0; i < _renderers.Length; i++)
		{
			_renderers[i].color = new Color(color.r, color.g, color.b, _renderers[i].color.a);
		}
		for (int i = 0; i < _lines.Length; i++)
		{
			_lines[i].startColor = color;
			_lines[i].endColor = new Color(color.r, color.g, color.b, _lines[i].endColor.a);
			//_lines[i].material.color = color;
		}
		for (int i = 0; i < _trails.Length; i++)
		{
			_trails[i].startColor = color;
			_trails[i].endColor = new Color(color.r, color.g, color.b, _trails[i].endColor.a);
		}
		for (int i = 0; i < _particles.Length; i++)
		{
			_particles[i].startColor = color;
		}
	}
}
