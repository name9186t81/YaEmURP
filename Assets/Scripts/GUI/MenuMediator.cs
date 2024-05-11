using System;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace YaEm.GUI
{
	public sealed class MenuMediator : MonoBehaviour
	{
		public enum SoundKey
		{
			Click = 1,
		}

		[Serializable]
		private class SoundInfo
		{
			[SerializeField] private AudioClip _clip;
			[SerializeField] private SoundKey _key;

			//todo implement pool support
			public void Play(AudioSource source)
			{
				source.clip = _clip;
				source.Play();
			}

			public SoundKey Key => _key;
		}
		[SerializeField] private GameObject _mainPanel;
		[SerializeField] private GameObject _customizationPanel;
		[SerializeField] private GameObject _teamColorPanel;
		[SerializeField] private Button _exitButton;
		[SerializeField] private SoundInfo[] _infos;
		[SerializeField] private AudioSource _source;
		private GameObject _selected;

		private void Start()
		{
			_selected = _mainPanel;
			_customizationPanel.SetActive(false);
			_teamColorPanel.SetActive(false);
			_exitButton.onClick.AddListener(() => Application.Quit());
		}

		public void ActivateMain()
		{
			Activate(_mainPanel);
		}

		public void ActivateCustomization()
		{
			Activate(_customizationPanel);
		}

		public void ActivateTeamSettings()
		{
			Activate(_teamColorPanel);
		}

		public void LoadScene(string sceneName)
		{
			SceneManager.LoadScene(sceneName);
		}
		private void Activate(GameObject go)
		{
			_selected.SetActive(false);
			_selected = go;
			_selected.SetActive(true);
		}

		public void Exit()
		{
			Application.Quit();
		}

		public void PlaySound(SoundKey key)
		{
			for(int i = 0, length = _infos.Length; i < length; i++)
			{
				if (_infos[i].Key == key)
				{
					_infos[i].Play(_source);
					break;
				}
			}
		}

		public void PlayClickSound() => PlaySound(SoundKey.Click);
	}
}