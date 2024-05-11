using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace YaEm.GUI
{
	public sealed class FadeInText : MonoBehaviour
	{
		[SerializeField] private float _timePerCharacter;
		[SerializeField] private float _scale;
		[SerializeField] private TextMeshProUGUI _text;
		private Rect[] _scales;
		private float _totalTime;
		private float _pausedTime;
		private bool _wasPaused;
		private int _index;
		private float _elapsed;
		private float _speedModifier = 1f;

		public bool Paused => _pausedTime > 0;
		public float SpeedModififer { get => _speedModifier; set => _speedModifier = value; }
		public int Index => _index;

		public void Pause(float time)
		{
			_pausedTime = time;
		}
		public void UnPause()
		{
			_pausedTime = 0f;
		}

		private void Start()
		{
			_text.ForceMeshUpdate();

			var info = _text.textInfo;
			_scales = new Rect[info.characterCount];//todo use sprite hash map to store scales

			for(int i = 0, length = info.characterCount; i < length; i++)
			{
				var charInfo = info.characterInfo[i];
				var verts = info.meshInfo[charInfo.materialReferenceIndex].vertices;

				Vector2 diff = (verts[charInfo.vertexIndex + 2] - verts[charInfo.vertexIndex]);
				Vector2 pos = (Vector2)verts[charInfo.vertexIndex] + diff / 2;
				_scales[i] = new Rect(pos.x, pos.y, diff.x, diff.y);
			}
		}

		public void ForceUpdate()
		{
			var info = _text.textInfo;

			if (_index >= _scales.Length || _index < 0)
			{
				//State = EffectState.Dead; //there is no need to keep this component after its done its job
				return;
			}

			while (!info.characterInfo[_index].isVisible) _index++;

			_pausedTime -= Time.deltaTime;
			_elapsed += Time.deltaTime * _speedModifier * (Paused ? 0f : _wasPaused ? 1000f : 1f);
			_wasPaused = Paused;
			float delta = _elapsed / _timePerCharacter;
			delta = Mathf.Clamp01(delta);

			float scale = 0f;
			if(delta < 0.5f)
			{
				scale = Mathf.Lerp(0, _scale, delta * 2f);
			}
			else
			{
				float t = (delta - 0.5f) * 2;
				scale = Mathf.Lerp(_scale, 1f, t * t * t);
			}
			Vector2 originalSize = _scales[_index].size;
			Vector2 halfSize = (originalSize / 2) * scale;

			var charInfo = info.characterInfo[_index];
			var verts = info.meshInfo[charInfo.materialReferenceIndex].vertices;

			verts[charInfo.vertexIndex] = _scales[_index].position - halfSize;
			verts[charInfo.vertexIndex + 1] = _scales[_index].position + new Vector2(-halfSize.x, halfSize.y);
			verts[charInfo.vertexIndex + 2] = _scales[_index].position + halfSize;
			verts[charInfo.vertexIndex + 3] = _scales[_index].position + new Vector2(halfSize.x, -halfSize.y);

			for(int i = _index + 1; i < info.characterCount; ++i)
			{
				var cInfo = info.characterInfo[i];

				if (!cInfo.isVisible) continue;

				var vert = info.meshInfo[cInfo.materialReferenceIndex].vertices;

				for (int j = 0; j < 4; ++j)
				{
					vert[cInfo.vertexIndex + j] = _scales[i].position;
				}
			}

			if (delta == 1f)
			{
				_index++;
				_elapsed = 0f;
			}

			//_text.ForceMeshUpdate();
			//var info = _text.textInfo;

			//for(int i = 0, length = info.characterCount; i < length; ++i)
			//{
			//	var charInfo = info.characterInfo[i];

			//	if (!charInfo.isVisible) continue;

			//	var verts = info.meshInfo[charInfo.materialReferenceIndex].vertices;

			//	for(int j = 0; j < 4; ++j)
			//	{
			//		var original = verts[charInfo.vertexIndex + j];

			//		verts[charInfo.vertexIndex + j] = original + Vector3.up * Mathf.Sin(Time.time * 2f + original.x * 0.01f) * 10f;
			//	}
			//}

			//for(int i = 0, length = info.meshInfo.Length; i < length ; ++i)
			//{
			//	var meshInfo = info.meshInfo[i];
			//	meshInfo.mesh.vertices = meshInfo.vertices;
			//	_text.UpdateGeometry(meshInfo.mesh, i);
			//}
		}

		public void Reset()
		{
			_elapsed = 0f;
			_index = 0;
			_totalTime = 0f;

			var info = _text.textInfo;

			_scales = new Rect[info.characterCount];//todo use sprite hash map to store scales
			
			for (int i = 0, length = info.characterCount; i < length; i++)
			{
				var charInfo = info.characterInfo[i];
				var verts = info.meshInfo[charInfo.materialReferenceIndex].vertices;

				_totalTime += _timePerCharacter;
				if (!charInfo.isVisible) continue;

				Vector2 diff = (verts[charInfo.vertexIndex + 2] - verts[charInfo.vertexIndex]);
				Vector2 pos = (Vector2)verts[charInfo.vertexIndex] + diff / 2;
				_scales[i] = new Rect(pos.x, pos.y, diff.x, diff.y);

				for (int j = 0; j < 4; ++j)
				{
					verts[charInfo.vertexIndex + j] = Vector3.zero;
				}
			}
		}

		public float TotalTime => _totalTime;
	}
}