using IK2D;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace YaEm.Effects
{
    [DisallowMultipleComponent, RequireComponent(typeof(LineRenderer))]
    public sealed class FABRIKLineRenderer : MonoBehaviour
    {
        [SerializeField] private int _pointsCount;
        [SerializeField] private float _distanceBetweenPoints;
        [SerializeField] private bool _randomizeStartingPositions;
        [SerializeField] private Transform _endPoint;
        private Transform _cached;
        private Vector2 _prevPosition;
        private bool _enabled;
        private LineRenderer _lineRenderer;
        private FABRIK _fabrik;

		private void Start()
		{
            _cached = transform;
			_lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = _pointsCount;
            Vector2[] positions = new Vector2[_pointsCount];
            Vector2 prevPos = Vector2.zero;

            for (int i = 0; i < _pointsCount; i++)
            {
                Vector2 pos = Vector2.up * _distanceBetweenPoints * i;
                Debug.Log(transform.parent.localScale.x + " " + transform.parent.name);
                if (_randomizeStartingPositions) pos = Vector2Extensions.RandomDirection() * _distanceBetweenPoints * i;
				prevPos = positions[i] = pos;
                _lineRenderer.SetPosition(i, pos);
            }

            if (SceneManager.GetActiveScene().buildIndex == 0) Destroy(gameObject); //todo: redo preview system in menu and remove that
            _fabrik = new FABRIK(positions, transform.position, false);
		}
        
		private void Update()
		{
            if(!_enabled) return;
            Vector2 currentPos = _endPoint.position - _cached.position;
            if ((currentPos - _prevPosition).sqrMagnitude < 0.05f * 0.05f) return;

            _prevPosition = currentPos;

            _fabrik.Resolve(((Vector2)(_endPoint.position - _cached.position)).Rotate(-_cached.eulerAngles.z * Mathf.Deg2Rad), false);

            for (int i = 0, length = _fabrik.Positions.Count; i < length; i++)
            {
                _lineRenderer.SetPosition(i, _fabrik.Positions[i].Position);
            }
		}

        public void Enable()
        {
            _enabled = true;
            _lineRenderer.enabled = true;
        }

        public void Disable()
        {
            _enabled = false;
            _lineRenderer.enabled = false;
        }

        public void SetEndPoint(Transform point)
        {
            _endPoint = point;
        }
	}
}