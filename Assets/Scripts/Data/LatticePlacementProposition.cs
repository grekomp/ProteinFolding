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

		public LatticePlacementProposition(Lattice baseLattice, int x, int y, bool isHydrophobic)
		{
			this.baseLattice = baseLattice;
			this.x = x;
			this.y = y;
			this.isHydrophobic = isHydrophobic;
		}


		public Lattice GetResultingLattice()
		{
			Lattice newLattice = baseLattice.Copy();
			newLattice.ApplyPlacementProposition(this);
			return newLattice;
		}
		public LatticePlacementProposition[] GetChildLatticePlacementPropositions(bool nextIsHydrophobic)
		{
			return baseLattice.GetPlacementPropositions(isHydrophobic);
		}


		#region Helper methods
		private int GetEnergy()
		{
			if (isHydrophobic == false) return baseLattice.energy;

			int energy = baseLattice.energy + baseLattice.GetEnergyPoint(x, y, isHydrophobic);

			return energy;
		}
		private bool AreCoordsWithinRange(Tuple<int, int> adjacentCoords)
		{
			if (adjacentCoords.Item1 < 0 || adjacentCoords.Item2 < 0) return false;
			if (adjacentCoords.Item1 >= baseLattice.points.GetLength(0) || adjacentCoords.Item2 >= baseLattice.points.GetLength(1)) return false;

			return true;
		}
		#endregion
	}
}

