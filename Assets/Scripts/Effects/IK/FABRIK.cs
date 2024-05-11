using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using static IK2D.FABRIK;

namespace IK2D
{
	public sealed class FABRIK
	{
		//[0]-->[1]-->[2]-->[3]
		private readonly Point[] r_pointsArray;
		private readonly bool r_pointsNormalized = true;
		private readonly bool r_limitAngles;
		private Vector2 _start;
		private Vector2 _globalPosition;

		public FABRIK(IEnumerable<Vector2> points)
		{
			if (points == null)
			{
				throw new ArgumentException();
			}

			r_pointsArray = new Point[points.Count()];
			int ind = 0;
			Vector2 prevPos = Vector2.zero;

			foreach (var point in points)
			{
				r_pointsArray[ind] = new Point()
				{
					Position = point,
					Length = ind == 0 ? 0 : Vector2.Distance(prevPos, point),
				};
				ind++;
				prevPos = point;
			}

			_start = r_pointsArray[0].Position;
		}

		public FABRIK(IEnumerable<Vector2> points, Vector2 globalPosition, bool normalizePoints = true)
		{
			if (points == null)
			{
				throw new ArgumentException();
			}

			_globalPosition = globalPosition;
			r_pointsArray = new Point[points.Count()];
			int ind = 0;
			Vector2 prevPos = Vector2.zero;

			foreach (var point in points)
			{
				r_pointsArray[ind] = new Point()
				{
					Position = point,
					Length = ind == 0 ? 0 : Vector2.Distance(prevPos, point),
				};
				ind++;
				prevPos = normalizePoints ? point - globalPosition : point;
			}

			_start = r_pointsArray[0].Position;
		}

		/// <summary>
		/// Angles should be in radians
		/// </summary>
		/// <param name="nodes"></param>
		/// <exception cref="ArgumentException"></exception>
		public FABRIK(IEnumerable<(Vector2 pos, float angle)> nodes)
		{
			if (nodes == null)
			{
				throw new ArgumentException();
			}
			
			//r_limitAngles = true;
			r_pointsArray = new Point[nodes.Count()];
			int ind = 0;
			Vector2 prevPos = Vector2.zero;

			foreach (var point in nodes)
			{
				r_pointsArray[ind] = new Point()
				{
					Position = point.pos,
					Length = ind == 0 ? 0 : Vector2.Distance(prevPos, point.pos),
					MaxAngle = point.angle
				};
				ind++;
				prevPos = point.pos;
			}

			for (int i = 0, length = r_pointsArray.Length; i < length - 1; i++)
			{
				r_pointsArray[i].Direction = (r_pointsArray[i + 1].Position - r_pointsArray[i].Position).normalized;
			}

			_start = r_pointsArray[0].Position;
		}


		public void UpdateGlobalPosition(in Vector2 newPosition)
		{
			Vector2 difference = newPosition - _globalPosition;
			_globalPosition = newPosition;

			for (int i = 0, length = r_pointsArray.Length; i < length; i++)
			{
				r_pointsArray[i].Position += difference;
			}

			_start = r_pointsArray[0].Position;
		}

		public void ForceUpdatePositions(IEnumerable<Vector2> positions, Vector2 globalPosition, bool overrideGlobalPosition = true, bool updateLength = true)
		{
			if (positions.Count() != r_pointsArray.Length) throw new ArgumentException();
			int counter = 0;

			if(overrideGlobalPosition) _globalPosition = globalPosition;

			foreach (var position in positions)
			{
				Vector2 pos = overrideGlobalPosition ? position - _globalPosition : position;

				if (updateLength && counter > 0)
				{
					r_pointsArray[counter].Length = Vector2.Distance(pos, r_pointsArray[counter - 1].Position);
				}

				r_pointsArray[counter++].Position = pos;
			}
		}

		public void Resolve(Vector2 point, bool globalPosition = true)
		{
			if (globalPosition && r_pointsNormalized) point -= _globalPosition;

			//backward
			int length = r_pointsArray.Length;
			r_pointsArray[length - 1].Position = point;

			for (int i = length - 2; i >= 0; i--)
			{
				Vector2 direction = (r_pointsArray[i].Position - r_pointsArray[i + 1].Position).normalized;
				if (r_limitAngles && i > 0)
				{
					
				}
				r_pointsArray[i].Position = direction * r_pointsArray[i].Length + r_pointsArray[i + 1].Position;
			}

			//forward
			r_pointsArray[0].Position = _start;
			for (int i = 1; i < length; i++)
			{
				Vector2 direction = (r_pointsArray[i].Position - r_pointsArray[i - 1].Position).normalized;
				if (r_limitAngles)
				{
					float half = r_pointsArray[i - 1].MaxAngle / 2;
					float dirAngle = Mathf.Atan2(r_pointsArray[i - 1].Direction.y, r_pointsArray[i - 1].Direction.x);
					float lowerAngle = -half + dirAngle;
					float highAngle = half + dirAngle;
					
					float curAngle = Mathf.Atan2(direction.y, direction.x);

					if (curAngle < lowerAngle || curAngle > highAngle)
					{
						float clamped = Mathf.Clamp(curAngle, lowerAngle, highAngle);
						direction = new Vector2(Mathf.Cos(clamped), Mathf.Sin(clamped));
					}
				}
				r_pointsArray[i].Position = direction * r_pointsArray[i].Length + r_pointsArray[i - 1].Position;
			}
		}

		public IReadOnlyList<IReadOnlyPoint> Positions => r_pointsArray;

		private class Point : IReadOnlyPoint
		{
			public Vector2 Direction;
			public Vector2 Position;
			public float Length;
			public float MaxAngle = 360;

			Vector2 IReadOnlyPoint.Position => Position;
		}

		public interface IReadOnlyPoint
		{
			Vector2 Position{ get; }
		}
	}
}