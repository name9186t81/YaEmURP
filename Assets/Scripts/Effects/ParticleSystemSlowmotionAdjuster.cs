using UnityEngine;

using YaEm;

public class ParticleSystemSlowmotionAdjuster : MonoBehaviour
{
	[SerializeField] private ParticleSystem _system;
	private GlobalTimeModifier _timeModifier;

	private void OnEnable()
	{
		_timeModifier = ServiceLocator.Get<GlobalTimeModifier>();
		_timeModifier.OnTimeModificated += UpdateParticleSystem;
		UpdateParticleSystem();
	}

	private void OnDisable()
	{
		_timeModifier.OnTimeModificated -= UpdateParticleSystem;
	}

	private void UpdateParticleSystem()
	{
		var main = _system.main;
		main.simulationSpeed = _timeModifier.TimeModificator;
	}
}
