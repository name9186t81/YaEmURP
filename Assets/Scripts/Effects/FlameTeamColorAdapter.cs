using UnityEngine;

using YaEm;
using YaEm.Core;

public class FlameTeamColorAdapter : MonoBehaviour, IActorComponent
{
	[SerializeField] private ParticleSystem _particleSystem;
	[SerializeField] private Gradient _grad;
	[SerializeField] private bool _debug;
	private IActor _actor;

	public IActor Actor { set => _actor = value; }

	public void Init(IActor actor)
	{
		if(actor is ITeamProvider prov)
		{
			prov.OnTeamNumberChange += TeamsUpdated;
			ChangeColors(prov.TeamNumber);
		}
	}

	private void TeamsUpdated(int arg1, int arg2)
	{
		ChangeColors(arg2);
	}

	public void ChangeColors(int team)
	{
		Color col = ColorTable.GetColor(team);
		Color.RGBToHSV(col, out float offset, out _, out _);

		var val = _particleSystem.colorOverLifetime;
		ParticleSystem.MinMaxGradient gradient = val.color;

		Gradient grad = new Gradient();
		GradientColorKey[] keys = new GradientColorKey[val.color.gradient.colorKeys.Length];
		for (int i = 0, length = val.color.gradient.colorKeys.Length; i < length; i++)
		{
			var current = gradient.gradient.colorKeys[i];
			Color.RGBToHSV(gradient.gradient.colorKeys[i].color, out float var, out float s, out float v);
			keys[i] = new GradientColorKey(Color.HSVToRGB(var + offset, s, v), current.time);
		}
		grad.SetKeys(keys, gradient.gradient.alphaKeys);
		if(!_debug) _grad = grad;
		val.color = grad;
		if (_debug) val.color = _grad;
	}
}
