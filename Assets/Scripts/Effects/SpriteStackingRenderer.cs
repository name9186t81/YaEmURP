using UnityEngine;
using System;
using System.Collections.Generic;

[DisallowMultipleComponent()]
public class SpriteStackingRenderer : MonoBehaviour
{
	[SerializeField] private Sprite[] _sprites;
	[SerializeField] private float _distance;
	[SerializeField] private float _scale = 1f;
	[SerializeField] private Transform _tracked;
	[SerializeField] private Material _mat;
	[SerializeField] private bool _reverse;
	private Vector2 _direction = Vector2.up;
	private Transform _transform;
	private static Material _material;
	private static bool _isInited = false;
	private static Dictionary<Sprite, InstancedDrawing> _instancedDrawings = new Dictionary<Sprite, InstancedDrawing>();
	private List<InstancedDrawing> _instances;

	[NonSerialized] public float Rotation;

	private void Awake()
	{
		_transform = transform;
		if (!_isInited) Init();

		_instances = new List<InstancedDrawing>();
		for (int i = 0; i < _sprites.Length; i++)
		{
			var instanced = GetInstancedDrawing(_sprites[i]);
			if (!_instances.Contains(instanced))
			{
				_instances.Add(instanced);
			}
		}
	}

	public Vector2 Direction
	{
		get
		{
			return _direction;
		}
		set
		{
			float length = value.magnitude;
			if (length > 1f)
				value /= length;
			_direction = value;
		}
	}

	private InstancedDrawing GetInstancedDrawing(Sprite sprite)
	{
		InstancedDrawing result = default;
		if (_instancedDrawings.TryGetValue(sprite, out result))
		{
			return result;
		}

		//Debug.Log(sprite.rect + " " + sprite.name + " " + sprite.rect.x / (float)sprite.texture.width + ", " + sprite.rect.y / (float)sprite.texture.height + ", " + sprite.rect.width / (float)sprite.texture.width + ", "+ sprite.rect.height / (float)sprite.texture.height);
		result = new InstancedDrawing(GetSpriteMesh(sprite), _material, sprite.texture, 0);
		_instancedDrawings.Add(sprite, result);
		return result;
	}

	private void Init()
	{
		_material = new Material(_mat);
		_material.enableInstancing = true;

		_isInited = true;
	}

	private Mesh GetSpriteMesh(Sprite sprite)
	{
		var mesh = new Mesh();
		Vector3[] verticies = new Vector3[4];
		Vector2[] uvs = new Vector2[4];
		int[] triangles = new int[6];

		float ratio = sprite.rect.width / (float)sprite.rect.height;
		verticies[0] = new Vector2(-0.5f * _scale * ratio, -0.5f * _scale);
		verticies[1] = new Vector2(0.5f * _scale * ratio, -0.5f * _scale);
		verticies[2] = new Vector2(-0.5f * _scale * ratio, 0.5f * _scale);
		verticies[3] = new Vector2(0.5f * _scale * ratio, 0.5f * _scale);

		float lowX = sprite.rect.x / (float)sprite.texture.width;
		float highX = sprite.rect.width / (float)sprite.texture.width + lowX;
		float lowY = sprite.rect.y / (float)sprite.texture.height;
		float highY = sprite.rect.height / (float)sprite.texture.height + lowY;
		uvs[0] = new Vector2(lowX, lowY);
		uvs[1] = new Vector2(highX, lowY);
		uvs[2] = new Vector2(lowX, highY);
		uvs[3] = new Vector2(highX, highY);


		triangles[0] = 0;
		triangles[1] = 2;
		triangles[2] = 1;
		triangles[3] = 1;
		triangles[4] = 2;
		triangles[5] = 3;

		mesh.SetVertices(verticies);
		mesh.SetUVs(0, uvs);
		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateNormals();
		return mesh;
	}

	private void Update()
	{
		float modifier = 0f;
		float delta = _distance / _sprites.Length;
		Direction = Position - (Vector2)_tracked.position;
		var angle = Quaternion.Euler(0, 0, ZAngle + Rotation);

		for (int i = 0, length = _sprites.Length; i < length; ++i, modifier += _distance)
		{
			Vector2 offset = _reverse ? (_direction * length * _distance - _direction * modifier) : (_direction * modifier);

			var matrix = Matrix4x4.TRS((Vector3)(Position + offset) - Vector3.forward * _distance * (length - i) * delta, angle, Scale);
			var instanced = GetInstancedDrawing(_sprites[i]);
			instanced.AddMatrix(matrix);
			//Graphics.DrawMesh(_squareMesh, matrix, _material, 0);
		}
	}

	private void LateUpdate()
	{
		for (int i = 0; i < _instances.Count; i++)
		{
			_instances[i].Complete();
		}
	}

	private float ZAngle => _transform.eulerAngles.z;
	private Vector2 Position => _transform.position;
	private Vector3 Scale => _transform.lossyScale;

	private class InstancedDrawing
	{
		private const int MAX_BUFFER_SIZE = 1023;
		private Matrix4x4[] _matrices;
		private Material _material;
		private Texture _texture;
		private Mesh _mesh;
		private int _count;
		private int _layer;

		public InstancedDrawing(Mesh mesh, Material material, Texture texture, int layer)
		{
			_matrices = new Matrix4x4[MAX_BUFFER_SIZE];
			_material = material;
			_mesh = mesh;
			_layer = layer;
			_texture = texture;
			_count = 0;
		}

		public void AddMatrix(in Matrix4x4 matrix)
		{
			_matrices[_count++] = matrix;
			if (_count == MAX_BUFFER_SIZE)
			{
				DrawBuffer();
			}
		}

		private void DrawBuffer()
		{
			if (_count == 0) return;

			MaterialPropertyBlock prob = new MaterialPropertyBlock();
			prob.SetTexture("_MainTex", _texture);
			Graphics.DrawMeshInstanced(_mesh, 0, _material, _matrices, _count, prob, UnityEngine.Rendering.ShadowCastingMode.Off, false, _layer, null, UnityEngine.Rendering.LightProbeUsage.Off, null);
			//Graphics.DrawMesh(_mesh, _matrices[1], _material, _layer, null, 0, prob);
			_count = 0;
		}

		public void Complete()
		{
			DrawBuffer();
		}
	}
}
