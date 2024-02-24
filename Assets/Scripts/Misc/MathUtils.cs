namespace YaEm
{
	public static class MathUtils
	{
		public static float Delta(this int val, int length) => (float)val / length;
		public static float Delta(this float val, float length) => val / length;
	}
}
