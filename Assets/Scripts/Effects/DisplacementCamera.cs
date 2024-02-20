using UnityEngine;

//just marks camera as displacement camera
[RequireComponent(typeof(Camera))]
public sealed class DisplacementCamera : MonoBehaviour
{
	private void Awake()
	{
		var cam = GetComponent<Camera>();
		cam.targetTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0);
	}
}
