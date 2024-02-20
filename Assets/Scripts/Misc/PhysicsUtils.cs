using UnityEngine;

namespace YaEm
{
	public static class PhysicsUtils
	{
		private static RaycastHit2D[] _cachedHits = new RaycastHit2D[32];

		public static readonly Vector2[] EightDirections;

		static PhysicsUtils()
		{
			EightDirections = new Vector2[8]
			{
				new Vector2(0, 1),
				new Vector2(0.71f, 0.71f),
				new Vector2(1, 0),
				new Vector2(0.71f, -0.71f),
				new Vector2(0, -1),
				new Vector2(-0.71f, -0.71f),
				new Vector2(-1, 0),
				new Vector2(-0.71f, 0.71f)
			};
		}
		public static RaycastHit2D RaycastIgnoreSelf(Transform compare, Vector2 position, Vector2 direction, float distance, LayerMask mask)
		{
			int hitsCount = Physics2D.RaycastNonAlloc(position, direction, _cachedHits, distance, mask);

			if(hitsCount == 1)
			{
				if (_cachedHits[0].transform == compare) return default;
				return _cachedHits[0];
			}
			else if(hitsCount > 1)
			{
				return _cachedHits[1];
			}
			return default;
		}
	}
}
