namespace YaEm.Core
{
	public interface IActorComponent
	{
		IActor Actor { set; }
		void Init(IActor actor);
	}
}
