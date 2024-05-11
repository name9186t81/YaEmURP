using System;
using System.Collections.Generic;

using YaEm.Core;
using YaEm.Dialogues;

namespace YaEm
{
	public static class ServiceLocator
	{
		private static Dictionary<Type, object> _services = new Dictionary<Type, object>();

		static ServiceLocator()
		{
			_services.Add(typeof(GlobalDeathNotificator), new GlobalDeathNotificator());
			_services.Add(typeof(GlobalTimeModifier), new GlobalTimeModifier());
			_services.Add(typeof(ColorTable), new ColorTable());
			_services.Add(typeof(PlayerChararcterContainer), new PlayerChararcterContainer());
			_services.Add(typeof(DialogueService), new DialogueService());
		}

		public static void Register<T>(T instance) where T : class, IService
		{
			_services.Add(typeof(T), instance);
		}

		public static bool TryGet<T>(out T instance) where T : class, IService
		{
			if (_services.TryGetValue(typeof(T), out object obj))
			{
				instance = obj as T;
				return true;
			}
			instance = default;
			return false;
		}

		public static T Get<T>() where T : class, IService
		{
			if (_services.TryGetValue(typeof(T), out object obj))
			{
				return (T)obj;
			}
			return null;
		}

		public static bool TrySet<T>(T val) where T : class, IService
		{
			bool res = _services.ContainsKey(typeof(T));
			if (res) _services[typeof(T)] = val;
			return res;
		}

		public static bool Remove<T>() where T : class, IService
		{
			return _services.Remove(typeof(T));
		}
	}
}