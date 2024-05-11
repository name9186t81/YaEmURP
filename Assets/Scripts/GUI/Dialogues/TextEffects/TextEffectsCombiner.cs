using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

using YaEm.GUI;

namespace Global
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public sealed class TextEffectsCombiner : MonoBehaviour
	{
		[SerializeField] private TextEffect[] _effects;
		private Dictionary<TextEffect.EffectPriority, List<TextEffect>> _sorted;
		private TextMeshProUGUI _text;
		private FadeInText _fadeInEffect;
		private bool _deactivated;

		public void Activate()
		{
			_deactivated = false;
		}

		public void Deactivate()
		{
			_deactivated = true;
		}

		public void ResetEffects()
		{
			StartCoroutine(ThanksUnityForFixing2020Bugs());
		}

		private IEnumerator ThanksUnityForFixing2020Bugs()
		{
			yield return new WaitForEndOfFrame();

			_text.textInfo.linkCount = 0;

			var info = _text.textInfo;

			for (int i = 0; i < _effects.Length; i++)
			{
				_effects[i].Reset();
			}

			for (int j = 0; j < info.meshInfo.Length; j++)
			{
				var meshInfo = info.meshInfo[j];
				meshInfo.mesh.vertices = meshInfo.vertices;
				_text.UpdateGeometry(meshInfo.mesh, j);
			}
		}
		private void Awake()
		{
			_text = GetComponent<TextMeshProUGUI>();
			_sorted = new Dictionary<TextEffect.EffectPriority, List<TextEffect>>();

			for(int i = 0; i < _effects.Length; i++)
			{
				if (_sorted.TryGetValue(_effects[i].Priority, out var list))
				{
					list.Add(_effects[i]);
				}
				else
				{
					_sorted.Add(_effects[i].Priority, new List<TextEffect>()
					{
						_effects[i]
					});
				}

				//its just bad but im too lazy to think how to do it better and i doubt there will be any other time consuming effect
				//if (_effects[i] is FadeInText fadeInText)
				//{
				//	_fadeInEffect = fadeInText;
				//}
			}
		}

		private void Update()
		{
			if (_deactivated) return;

			_text.ForceMeshUpdate();
			var info = _text.textInfo;

			if(_sorted.TryGetValue(TextEffect.EffectPriority.VeryHigh, out var list))
			{
				UpdateList(list);
			}

			if (_sorted.TryGetValue(TextEffect.EffectPriority.High, out var list2))
			{
				UpdateList(list2);
			}

			if (_sorted.TryGetValue(TextEffect.EffectPriority.Medium, out var list3))
			{
				UpdateList(list3);
			}

			if (_sorted.TryGetValue(TextEffect.EffectPriority.Low, out var list4))
			{
				UpdateList(list4);
			}

			if (_sorted.TryGetValue(TextEffect.EffectPriority.VeryLow, out var list5))
			{
				UpdateList(list5);
			}

			for(int j = 0; j < info.meshInfo.Length; j++)
			{
				var meshInfo = info.meshInfo[j];
				meshInfo.mesh.vertices = meshInfo.vertices;
				_text.UpdateGeometry(meshInfo.mesh, j);
			}
		}

		private void UpdateList(List<TextEffect> list)
		{
			for(int i = 0; i < list.Count; i++) 
			{
				if (list[i].State == TextEffect.EffectState.Dead)
				{
					list.RemoveAt(i);
					i--;
					continue;
				}
				list[i].PlayEffect();
			}
		}

		public float TextAppearenceTime => _fadeInEffect == null ? 0f : _fadeInEffect.TotalTime;
	}
}