﻿using System;
using UnityEngine;
using YaEm.Core;
using YaEm.Weapons;

namespace YaEm.Health
{
	public sealed class DamageArgs : EventArgs
	{
		public readonly IActor Sender;
		public readonly int Damage;
		public readonly DamageFlags DamageFlags;
		public readonly IWeapon Weapon;
		public Vector2 HitPosition;
		public Vector2 SourcePosition;

		public DamageArgs(IActor sender, int damage, DamageFlags damageFlags, IWeapon weapon = null)
		{
			Sender = sender;
			Damage = damage;
			DamageFlags = damageFlags;
			Weapon = weapon;
		}
	}

	[Flags]
	public enum DamageFlags
	{
		Unknown = 0,
		Kinetic = 1,
		Explosive = 2,
		Fire = 4,
		Melee = 8,
		Ranged = 16,
		NoWeapon = 32,
		Heal = 64
	}
}