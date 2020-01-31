using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProteinFolding
{
	public class LatticePlacementProposition
	{
		public Lattice baseLattice;
		public int x;
		public int y;
		public bool isHydrophobic = false;

		/// <summary>
		/// In which direction, as seen from the (x, y) coordinates, is the point's binding
		/// </summary>
		public Direction direction;

		public bool IsEnergyCalculated {
			get; private set;
		}
		private int _energy;
		public int Energy {
			get {
				if (IsEnergyCalculated == false) _energy = GetEnergy();

				return _energy;
			}
		}

		public bool IsValid {
			get {
				return baseLattice.IsOccupied(x, y) == false;
			}
		}

		public LatticePlacementProposition(Lattice baseLattice, int x, int y, bool isHydrophobic, Direction direction)
		{
			this.baseLattice = baseLattice;
			this.x = x;
			this.y = y;
			this.isHydrophobic = isHydrophobic;
			this.direction = direction;
		}

		private int GetEnergy()
		{
			if (isHydrophobic == false) return baseLattice.energy;

			int energy = baseLattice.energy;

			energy += GetSingleEnergy(Direction.Up);
			energy += GetSingleEnergy(Direction.Right);
			energy += GetSingleEnergy(Direction.Down);
			energy += GetSingleEnergy(Direction.Left);

			return energy;
		}
		private int GetSingleEnergy(Direction checkedDirection)
		{
			if (checkedDirection == Direction.None) return 0;
			if (direction == checkedDirection) return 0;

			var adjacentCoords = baseLattice.GetAdjacentCoordinates(x, y, checkedDirection);

			if (AreCoordsWithinRange(adjacentCoords) == false) return 0;

			if (baseLattice.IsHydrophobic(adjacentCoords.Item1, adjacentCoords.Item2))
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}

		private bool AreCoordsWithinRange(Tuple<int, int> adjacentCoords)
		{
			if (adjacentCoords.Item1 < 0 || adjacentCoords.Item2 < 0) return false;
			if (adjacentCoords.Item1 >= baseLattice.points.GetLength(0) || adjacentCoords.Item2 >= baseLattice.points.GetLength(1)) return false;

			return true;
		}

		public Lattice GetResultingLattice()
		{
			Lattice newLattice = baseLattice.Copy();
			newLattice.PlacePoint(x, y, isHydrophobic, direction);
			newLattice.energy = Energy;
			return newLattice;
		}
		public LatticePlacementProposition[] GetChildLatticePlacementPropositions(bool nextIsHydrophobic)
		{
			return GetLatticePlacementPropositions(baseLattice, x, y, nextIsHydrophobic);
		}

		#region Creating placement propositions
		public static LatticePlacementProposition[] GetLatticePlacementPropositions(Lattice baseLattice, int x, int y, bool isHydrophobic)
		{
			LatticePlacementProposition[] propositions = new LatticePlacementProposition[4];

			propositions[0] = GetLatticePlacementProposition(baseLattice, x, y, isHydrophobic, Direction.Up);
			propositions[1] = GetLatticePlacementProposition(baseLattice, x, y, isHydrophobic, Direction.Right);
			propositions[2] = GetLatticePlacementProposition(baseLattice, x, y, isHydrophobic, Direction.Down);
			propositions[3] = GetLatticePlacementProposition(baseLattice, x, y, isHydrophobic, Direction.Left);

			return propositions;
		}
		public static LatticePlacementProposition GetLatticePlacementProposition(Lattice baseLattice, int originX, int originY, bool isHydrophobic, Direction moveDirection)
		{
			var adjacentCoords = baseLattice.GetAdjacentCoordinates(originX, originY, moveDirection);
			return new LatticePlacementProposition(baseLattice, adjacentCoords.Item1, adjacentCoords.Item2, isHydrophobic, baseLattice.GetOppositeDirection(moveDirection));
		}
		#endregion
	}
}

