using UnityEngine;

using YaEm;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class UnitDisolveEffect : MonoBehaviour
{
	[SerializeField] private float _playTime;
	[SerializeField] private float _speed;
	[SerializeField] private AnimationCurve _growCurve;
	private Transform _cached;
	private float _elapsed;
	private Material _mat;

	private Vector2 _randomDirection;
	[HideInInspector] public Vector2 Direction;
	[HideInInspector] public float RandomVelocity;
	[HideInInspector] public float RotationVelocity;

	private void Start()
	{
		_mat = new Material(GetComponent<SpriteRenderer>().material);
		GetComponent<SpriteRenderer>().material = _mat;
		_cached = transform;
		_randomDirection = (Random.Range(0, 360f) * Mathf.Deg2Rad).VectorFromAngle();
		Destroy(gameObject, _playTime);
	}

	private void Update()
	{
		_elapsed += Time.deltaTime;
		_cached.position += (Vector3)(Direction + _randomDirection * RandomVelocity) * _speed * Time.deltaTime;
		_cached.rotation = Quaternion.Euler(0, 0, RotationVelocity * Time.deltaTime + _cached.eulerAngles.z);

		float factor = _growCurve.Evaluate(_elapsed / _playTime);
		_mat.SetFloat("_Progress", factor);
	}
}
