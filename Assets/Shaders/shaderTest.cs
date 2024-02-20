using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class shaderTest : MonoBehaviour
{
	[SerializeField] private Camera _cam;
	[SerializeField] private Material _mat;

	private void Update()
	{
		if(_mat == null || _cam == null) return;

		_mat.SetTexture("_MainTex", _cam.targetTexture);
	}
}
