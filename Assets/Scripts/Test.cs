using System.IO;

using UnityEditor;

using UnityEngine;

#if UNITY_EDITOR
namespace Global
{
	public sealed class Test : MonoBehaviour
	{
		private void Awake()
		{
			Texture2D tex = new Texture2D(512, 512);
			tex.filterMode = FilterMode.Point;
			for(int y = 0; y < 512; y++)
			{
				for(int x = 0; x < 512; x++)
				{
					float xDelta = (float)x / 512;
					float yDelta = (float)y / 512;

					tex.SetPixel(x, y, Color.HSVToRGB(xDelta, 1, 1));
				}
			}

			var bytes = tex.EncodeToPNG();
			if(!Directory.Exists(Application.dataPath + "/RenderOutput"))
			{
				Directory.CreateDirectory(Application.dataPath + "/RenderOutput");
			}

			File.WriteAllBytes(Application.dataPath + "/RenderOutput/HSVSquareBright.png", bytes);
			Debug.Log(Application.dataPath + "/RenderOutput//HSVSquareBright.png");
			AssetDatabase.Refresh();
		}
	}
}
#endif