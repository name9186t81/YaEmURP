using System;

namespace YaEm.AI
{
	//todo: this is just an over-engineering... use dictionary or other hash table
	public sealed class AIMemory
	{
		private object[] _memories;
		public AIMemory() { _memories = new object[Enum.GetNames(typeof(AIMemoryKey)).Length]; }

		public void SetMemory(AIMemoryKey key, object value)
		{
			_memories[(int)key] = value;
		}

		public bool TryGetValue(AIMemoryKey key, out object value)
		{
			value = _memories[(int)key];
			return value != null;
		}
	}
}
