using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProteinFolding
{
	[Serializable]
	public class Lattice
	{
		public int[,] points;
		public bool[,] pointIsHydrophobic;

		public int Size {
			get {
				return points.GetLength(0);
			}
		}
		public int energy = 0;
		public int lastIndex = 0;

		public int lastX = -1;
		public int lastY = -1;

		#region Creating Lattices
		public Lattice(int size)
		{
			points = new int[size, size];
			pointIsHydrophobic = new bool[size, size];
		}

		public Lattice Copy()
		{
			Lattice newLattice = new Lattice(Size);

			for (int i = 0; i < points.GetLength(0); i++)
			{
				for (int j = 0; j < points.GetLength(1); j++)
				{
					newLattice.points[i, j] = points[i, j];
					newLattice.pointIsHydrophobic[i, j] = pointIsHydrophobic[i, j];
				}
			}

			newLattice.energy = energy;
			newLattice.lastIndex = lastIndex;

			newLattice.lastX = lastX;
			newLattice.lastY = lastY;

			return newLattice;
		}
		#endregion


		#region Placing points
		public void PlaceInitPoint(bool isHydrophobic)
		{
			int initX, initY;
			initX = initY = Size / 2;

			points[initX, initY] = ++lastIndex;
			pointIsHydrophobic[initX, initY] = isHydrophobic;

			energy = 0;
			lastX = initX;
			lastY = initY;
		}

		public void PlacePoint(bool isHydrophobic, Direction direction, bool calculateEnergy = true)
		{
			// Place point
			PlacePoint(GetAdjacentX(lastX, direction), GetAdjacentY(lastY, direction), isHydrophobic);

			// Calculating energy
			if (calculateEnergy)
			{
				energy += GetEnergyPoint(lastX, lastY, isHydrophobic);
			}
		}
		protected void PlacePoint(int x, int y, bool isHydrophobic)
		{
			lastX = x;
			lastY = y;

			points[x, y] = ++lastIndex;
			pointIsHydrophobic[x, y] = isHydrophobic;
		}
		public void ApplyPlacementProposition(LatticePlacementProposition placementProposition)
		{
			PlacePoint(placementProposition.x, placementProposition.y, placementProposition.isHydrophobic);
			energy = placementProposition.Energy;
		}
		#endregion


		#region Lattice Placement Propositions
		public LatticePlacementProposition[] GetPlacementPropositions(bool isHydrophobic)
		{
			LatticePlacementProposition[] result = new LatticePlacementProposition[4];

			result[0] = GetPlacementProposition(isHydrophobic, Direction.Up);
			result[1] = GetPlacementProposition(isHydrophobic, Direction.Right);
			result[2] = GetPlacementProposition(isHydrophobic, Direction.Down);
			result[3] = GetPlacementProposition(isHydrophobic, Direction.Left);

			return result;
		}

		private LatticePlacementProposition GetPlacementProposition(bool isHydrophobic, Direction direction)
		{
			int adjacentX = GetAdjacentX(lastX, direction);
			int adjacentY = GetAdjacentY(lastY, direction);

			if (IsOccupied(adjacentX, adjacentY)) return null;

			return new LatticePlacementProposition(this, adjacentX, adjacentY, isHydrophobic);
		}
		#endregion


		#region Helper methods
		public bool IsHydrophobic(int x, int y)
		{
			return pointIsHydrophobic[x, y];
		}
		public bool IsOccupied(int x, int y)
		{
			return points[x, y] > 0;
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

			for (int x = 0; x < points.GetLength(0) - 1; x++)
			{
				for (int y = 0; y < points.GetLength(1) - 1; y++)
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
			if (pointIsHydrophobic[adjacentX, adjacentY] && Math.Abs(points[x, y] - points[adjacentX, adjacentY]) > 1) return -1;

			return 0;
		}

		public static int GetAdjacentX(int x, Direction direction)
		{
			switch (direction)
			{
				case Direction.Left:
					x--;
					break;
				case Direction.Right:
					x++;
					break;
			}

			return x;
		}
		public static int GetAdjacentY(int y, Direction direction)
		{
			switch (direction)
			{
				case Direction.Up:
					y--;
					break;
				case Direction.Down:
					y++;
					break;
			}

			return y;
		}
		#endregion
	}
}
