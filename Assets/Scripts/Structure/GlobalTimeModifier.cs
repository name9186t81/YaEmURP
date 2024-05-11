using System;

public class GlobalTimeModifier : IService
{
	private float _timeModifier = 1f;
	public event Action OnTimeModificated;

	public void ModifyTime(float factor)
	{
		_timeModifier *= factor;
		OnTimeModificated?.Invoke();
	}

	public void SetTimeModificator(float factor)
	{
		_timeModifier = factor;
		OnTimeModificated?.Invoke();
	}

	public void Reset()
	{
		_timeModifier = 1f;
		OnTimeModificated?.Invoke();
	}

	public float TimeModificator => _timeModifier;
}
