using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace YaEm.Effects
{
	public class DisplacementPostProcessing : ScriptableRenderPass
	{
		private Shader _shader;
		private Material _material;
		private int _sampleID;
		private int _mainID;
		private Camera _displacementCamera;
		private const string PASS_NAME = "DisplacementPass";
		private RenderTargetIdentifier _colorBuffer, _temporaryBuffer;

		public DisplacementPostProcessing(float strength, Shader shader)
		{
			renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

			_shader = shader;
			_material = new Material(_shader);
			_material.SetFloat("_Strength", strength);
			_mainID = _shader.FindPropertyIndex("_MainTex");
			_sampleID = _shader.FindPropertyIndex("_Sample");
		}

		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

			descriptor.depthBufferBits = 0;

			_colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
			cmd.GetTemporaryRT(_mainID, descriptor, FilterMode.Bilinear);
			_temporaryBuffer = new RenderTargetIdentifier(_mainID);
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer cmd = CommandBufferPool.Get(PASS_NAME);

			if (_displacementCamera == null)
			{
				var obj = MonoBehaviour.FindObjectOfType<DisplacementCamera>();
				if (obj == null) _displacementCamera = Camera.main;
				else _displacementCamera = obj.GetComponent<Camera>();
			}

			if (_material == null) return;
			_material.SetTexture("_Sample", _displacementCamera.targetTexture);
			
			using (new ProfilingScope(cmd, new ProfilingSampler(PASS_NAME)))
			{
				Blit(cmd, _colorBuffer, _temporaryBuffer, _material);
				Blit(cmd, _temporaryBuffer, _colorBuffer);
			}

			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}
	}
}