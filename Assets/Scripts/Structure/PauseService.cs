using System;

public class PauseService : IService
{
	public event Action OnPause;
	public event Action OnUnPause;
	public bool IsPaused { get; private set; } = false;
	public void Pause()
	{
		OnPause?.Invoke();
		IsPaused = true;
	}
	public void UnPause()
	{
		OnUnPause?.Invoke();
		IsPaused = false;
	}
}
