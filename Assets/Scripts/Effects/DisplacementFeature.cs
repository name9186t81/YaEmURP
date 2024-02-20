using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace YaEm.Effects
{
	public class DisplacementFeature : ScriptableRendererFeature
	{
		[SerializeField] private float _intensity;
		[SerializeField] private Shader _displacementShader;
		private DisplacementPostProcessing _pass;

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			renderer.EnqueuePass(_pass);
		}

		public override void Create()
		{
			_pass = new DisplacementPostProcessing(_intensity, Shader.Find("Shader Graphs/Displacement"));
		}
	}
}