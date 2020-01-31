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
		/// <summary>
		/// An array of lattice points, represented in the 4 lowest bits of the byte as follows:
		/// xx00 / xx01 - Empty point
		/// xx10 - Polar particle
		/// xx11 - Hydrophobic particle
		/// 
		/// 00xx - No bonds to the right and downwards
		/// 01xx - One bond to the right
		/// 10xx - One bond downwards
		/// 11xx - Both bonds to the right and downwards
		/// </summary>
		public byte[,] points;
		public int Size {
			get {
				return points.GetLength(0);
			}
		}
		public int energy = 0;

		public Lattice(int size)
		{
			points = new byte[size, size];
		}

		public Lattice Copy()
		{
			Lattice newLattice = new Lattice(Size);

			for (int i = 0; i < points.GetLength(0); i++)
			{
				for (int j = 0; j < points.GetLength(1); j++)
				{
					newLattice.points[i, j] = points[i, j];
				}
			}

			newLattice.energy = energy;
			return newLattice;
		}

		public void PlacePoint(int x, int y, bool isHydrophobic, Direction bondDirection)
		{
			// Place point
			byte point = 0;
			point |= isHydrophobic ? (byte)0b0011 : (byte)0b0010;

			if (bondDirection == Direction.Down) point |= 0b1000;
			if (bondDirection == Direction.Right) point |= 0b0100;

			points[x, y] = point;

			// Update bindings
			if (bondDirection == Direction.Up)
			{
				var adjacentCoords = GetAdjacentCoordinates(x, y, bondDirection);
				points[adjacentCoords.Item1, adjacentCoords.Item2] |= 0b1000;
			}
			if (bondDirection == Direction.Left)
			{
				var adjacentCoords = GetAdjacentCoordinates(x, y, bondDirection);
				points[adjacentCoords.Item1, adjacentCoords.Item2] |= 0b0100;
			}
		}



		#region Helper methods
		public Tuple<int, int> GetAdjacentCoordinates(int x, int y, Direction direction)
		{
			switch (direction)
			{
				case Direction.Up:
					y--;
					break;
				case Direction.Right:
					x++;
					break;
				case Direction.Down:
					y++;
					break;
				case Direction.Left:
					x--;
					break;
			}

			return new Tuple<int, int>(x, y);
		}

		public bool IsHydrophobic(int x, int y)
		{
			return (points[x, y] & 0b0011) == 0b0011;
		}
		public bool IsOccupied(int x, int y)
		{
			return (points[x, y] & 0b0010) == 0b0010;
		}
		public bool HasDownBinding(int x, int y)
		{
			return (points[x, y] & 0b1000) == 0b1000;
		}
		public bool HasRightBinding(int x, int y)
		{
			return (points[x, y] & 0b0100) == 0b0100;
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
					if (IsHydrophobic(x, y))
					{
						var downCoords = GetAdjacentCoordinates(x, y, Direction.Down);
						var rightCoords = GetAdjacentCoordinates(x, y, Direction.Right);
						if (HasDownBinding(x, y) == false && IsHydrophobic(downCoords.Item1, downCoords.Item2)) calculatedEnergy--;
						if (HasRightBinding(x, y) == false && IsHydrophobic(rightCoords.Item1, rightCoords.Item2)) calculatedEnergy--;
					}
				}
			}

			return calculatedEnergy;
		}
		#endregion
	}
}
