namespace YaEm.Core
{
	public interface IProvider<T>
	{
		ref T Value { get; }
	}
}
