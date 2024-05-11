using UnityEngine;

namespace YaEm
{
	public sealed class UpdateLineScale : MonoBehaviour
	{
		[SerializeField] private Unit _unit;
		[SerializeField] private float _defaultScale = 0.54f;
		[SerializeField] private LineRenderer _renderer;

		private void Start()
		{
			_renderer.widthMultiplier = _unit.Scale / _defaultScale;
		}
	}
}