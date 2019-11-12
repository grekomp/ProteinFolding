using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProteinFolding
{
	struct Lattice
	{
		public LatticePoint[,] points;

		public int lastPlacedX;
		public int lastPlacedY;

		private int energy;
		public int Energy { get => energy; private set => energy = value; }

		#region Constructors
		public Lattice(Lattice other)
		{
			points = (LatticePoint[,])other.points.Clone();
			lastPlacedX = other.lastPlacedX;
			lastPlacedY = other.lastPlacedY;
			energy = other.Energy;
		}
		public Lattice(int size)
		{
			points = new LatticePoint[size, size];
			lastPlacedX = size / 2;
			lastPlacedY = size / 2;
			energy = 0;
		}
		#endregion

		public Lattice Copy()
		{
			return new Lattice(this);
		}
		public LatticePoint this[int x, int y] {
			get {
				return points[x, y];
			}
		}

		public bool TryPlacePoint(PointType type, BondSourceDirection direction)
		{
			int x, y;
			x = GetNextX(direction);
			y = GetNextY(direction);

			if (CanPlacePoint(x, y))
			{
				PlacePoint(x, y, new LatticePoint(type, direction));
				return true;
			}

			return false;
		}

		/// <summary>
		/// Checks if a point can be placed at specified coords
		/// </summary>
		public bool CanPlacePoint(int x, int y)
		{
			return AreCoordsAdjacentToLastPlacement(x, y) && points[x, y].type == PointType.None;
		}
		/// <summary>
		/// Places a point without doing any prior validation
		/// </summary>
		public void PlacePoint(int x, int y, LatticePoint point)
		{
			points[x, y] = point;
			lastPlacedX = x;
			lastPlacedY = y;

			// Recalculate energy with newly placed point
			if (point.type == PointType.Hydrophobic)
			{
				GetEnergyForPoint(x, y);
			}
		}

		/// <summary>
		/// Calculates the energy from bonds of a specific point
		/// </summary>
		private int GetEnergyForPoint(int x, int y)
		{
			LatticePoint point = points[x, y];
			int pointEnergy = 0;


			return pointEnergy;
		}
		private int GetEnergyForPointDirection(int x, int y, BondSourceDirection direction)
		{
			if (points[x, y].type == PointType.Hydrophobic && points[x, y].direction != direction)
			{

			}

			if (points[GetNextX(direction, GetNextY)])

				return 0;
		}

		#region Coords
		private bool AreCoordsAdjacentToLastPlacement(int x, int y)
		{
			return Mathf.Abs(x - lastPlacedX) + Mathf.Abs(y - lastPlacedY) == 1;
		}

		public int GetNextX(BondSourceDirection direction)
		{
			switch (direction)
			{
				case BondSourceDirection.Up:
					return lastPlacedX + 1;
				case BondSourceDirection.Down:
					return lastPlacedX - 1;
				case BondSourceDirection.None:
				case BondSourceDirection.Right:
				case BondSourceDirection.Left:
				default:
					return lastPlacedX;
			}
		}
		public int GetNextY(BondSourceDirection direction)
		{
			switch (direction)
			{
				case BondSourceDirection.None:
				case BondSourceDirection.Up:
				case BondSourceDirection.Right:
				default:
					return lastPlacedY;
				case BondSourceDirection.Down:
					return lastPlacedY - 1;
				case BondSourceDirection.Left:
					return lastPlacedY + 1;
			}
		}
		#endregion

		public int GetEnergy()
		{
			int energy = 0;

			for (int i = 0; i < points.GetLength(0) - 1; i++)
			{
				for (int j = 0; j < points.GetLength(1) - 1; j++)
				{
					LatticePoint point = points[i, j];

					if (point.type == PointType.Hydrophobic)
					{
						// Check point to the right
						if (point.direction != BondSourceDirection.Right)
						{
							if (points[i + 1, j].type == PointType.Hydrophobic && points[i + 1, j].direction != BondSourceDirection.Left)
							{
								energy--;
							}
						}

						// Check point to the bottom
						if (point.direction != BondSourceDirection.Down)
						{
							if (points[i, j + 1].type == PointType.Hydrophobic && points[i, j + 1].direction != BondSourceDirection.Up)
							{
								energy--;
							}
						}
					}
				}
			}

			return energy;
		}

		#region Debug
		public void ConsoleLog()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < points.GetLength(0); i++)
			{

				for (int j = 0; j < points.GetLength(1); j++)
				{
					switch (points[i, j].type)
					{
						case PointType.None:
							sb.Append("O");
							break;
						case PointType.Hydrophobic:
							sb.Append("H");
							break;
						case PointType.Polar:
							sb.Append("P");
							break;
						default:
							break;
					}
				}

				sb.Append("\n");
			}

			Debug.Log(sb.ToString());
		}
		#endregion
	}
}
