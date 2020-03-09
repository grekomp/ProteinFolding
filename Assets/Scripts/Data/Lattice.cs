using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace ProteinFolding
{
	[Serializable]
	public class Lattice : SerializableWideClass
	{
		public Point[] points;
		public bool[] parsedInput;

		public int size;
		public int energy;


		#region Creating Lattices
		public Lattice(Point[] points, bool[] parsedInput, int energy, int size)
		{
			this.points = points;
			this.parsedInput = parsedInput;
			this.size = size;
			this.energy = energy;
		}
		#endregion


		#region Accessors
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IndexedPoint GetPoint(int x, int y)
		{
			return FindPoint(Index(x, y));
		}
		public IndexedPoint FindPoint(int index)
		{
			for (int i = 0; i < points.Length; i++)
			{
				if (points[i].conformationIndex == index)
				{
					return new IndexedPoint(points[i], i);
				}
			}

			return new IndexedPoint();
		}
		#endregion


		#region Helper methods
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsValid() => size > 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Index(int x, int y)
		{
			return x + y * size;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsHydrophobic(int x, int y)
		{
			return parsedInput[GetPoint(x, y).proteinStringIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsOccupied(int x, int y)
		{
			return GetPoint(x, y).point.conformationIndex > 0;
		}
		public Direction BindingDirection(int x, int y)
		{
			if (IsOccupied(x, y) == false) return Direction.None;

			if (HasBindingInDirection(x, y, Direction.Up)) return Direction.Up;
			if (HasBindingInDirection(x, y, Direction.Right)) return Direction.Right;
			if (HasBindingInDirection(x, y, Direction.Down)) return Direction.Down;
			if (HasBindingInDirection(x, y, Direction.Left)) return Direction.Left;

			return Direction.None;
		}
		public bool HasBindingInDirection(int x, int y, Direction direction)
		{
			int adjacentX = GetAdjacentX(x, direction);
			int adjacentY = GetAdjacentY(y, direction);

			if (IsValidX(adjacentX) == false || IsValidY(adjacentY) == false) return false;

			int searchedIndex = GetPoint(x, y).proteinStringIndex - 1;
			return IsOccupied(adjacentX, adjacentY) && GetPoint(adjacentX, adjacentY).proteinStringIndex == searchedIndex;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsValidX(int x)
		{
			return x >= 0 && x < size;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsValidY(int y)
		{
			return y >= 0 && y < size;
		}

		public Direction GetOppositeDirection(Direction direction)
		{
			switch (direction)
			{
				case Direction.None:
					return Direction.None;
				case Direction.Up:
					return Direction.Down;
				case Direction.Right:
					return Direction.Left;
				case Direction.Down:
					return Direction.Up;
				case Direction.Left:
					return Direction.Right;
			}

			return Direction.None;
		}

		/// <summary>
		/// This method calculates the energy of the entire lattice. 
		/// It should only be used for testing, as calculating the energy difference when placing single points is much faster.
		/// </summary>
		public int CalculateEnergy()
		{
			int calculatedEnergy = 0;

			for (int x = 0; x < size - 1; x++)
			{
				for (int y = 0; y < size - 1; y++)
				{
					calculatedEnergy += GetEnergyPoint(x, y, IsHydrophobic(x, y));
				}
			}

			return calculatedEnergy / 2;
		}

		public int GetEnergyPoint(int x, int y, bool isHydrophobic)
		{
			int pointEnergy = 0;
			pointEnergy += GetEnergySingleDirection(x, y, Direction.Up, isHydrophobic);
			pointEnergy += GetEnergySingleDirection(x, y, Direction.Right, isHydrophobic);
			pointEnergy += GetEnergySingleDirection(x, y, Direction.Down, isHydrophobic);
			pointEnergy += GetEnergySingleDirection(x, y, Direction.Left, isHydrophobic);
			return pointEnergy;
		}
		public int GetEnergySingleDirection(int x, int y, Direction direction, bool isHydrophobic)
		{
			if (isHydrophobic == false) return 0;

			int adjacentX = GetAdjacentX(x, direction);
			int adjacentY = GetAdjacentY(y, direction);
			if (IsHydrophobic(adjacentX, adjacentY) && Math.Abs(GetPoint(x, y).point.conformationIndex - GetPoint(adjacentX, adjacentY).point.conformationIndex) > 1) return -1;

			return 0;
		}

		public static int GetAdjacentX(int x, Direction direction)
		{
			x += direction == Direction.Left ? -1 : 0;
			x += direction == Direction.Right ? 1 : 0;

			return x;
		}
		public static int GetAdjacentY(int y, Direction direction)
		{
			y += direction == Direction.Up ? -1 : 0;
			y += direction == Direction.Down ? 1 : 0;

			return y;
		}
		#endregion


		#region Overrides
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					IndexedPoint indexedPoint = GetPoint(x, y);

					if (indexedPoint.point.conformationIndex > 0)
					{
						sb.Append(parsedInput[indexedPoint.proteinStringIndex] ? "H" : "P");
					}
					else
					{
						sb.Append("O");
					}
				}
				sb.Append("\n");
			}

			return sb.ToString();
		}
		#endregion
	}
}
