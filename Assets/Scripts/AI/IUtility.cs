namespace YaEm.AI
{
	public interface IUtility
	{
		StateType StateType { get; }
		void Init(AIController controller);
		/// <summary>
		/// Guaranteed to get called every frame/every ai tick
		/// </summary>
		float GetEffectivness();
		/// <summary>
		/// Called once before Execute is called
		/// </summary>
		void PreExecute();
		void Execute();
		void Undo();
	}
}
