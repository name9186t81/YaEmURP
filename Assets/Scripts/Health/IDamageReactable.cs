﻿using YaEm;
using System;

namespace YaEm.Health
{
	public interface IDamageReactable
	{
		bool CanTakeDamage(DamageArgs args);
		void TakeDamage(DamageArgs args);
		event Action<DamageArgs> OnDamage;
	}
}
