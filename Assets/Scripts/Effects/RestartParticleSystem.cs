using UnityEngine;

namespace Global
{
	[DisallowMultipleComponent(), RequireComponent(typeof(ParticleSystem))]
	//i love when unity devs cant combine 2 functions so i need to write 22 lines code
	public sealed class RestartParticleSystem : MonoBehaviour
	{
		private ParticleSystem _system;

		private void Awake()
		{
			_system = GetComponent<ParticleSystem>();
		}

		public void Restart()
		{
			_system.Stop();
			_system.Clear();
			_system.Play();
		}
	}
}