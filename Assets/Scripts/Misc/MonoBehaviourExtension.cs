using UnityEngine;

namespace YaEm
{
	public static class MonoBehaviourExtension
	{
		public static bool TryGetComponentInChildren<T>(this Component beh, out T component)
		{
			component = beh.GetComponentInChildren<T>();
			return component != null;
		}
	}
}
