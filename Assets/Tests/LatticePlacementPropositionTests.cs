using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using ProteinFolding;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
	public class LatticePlacementPropositionTests
	{
		[Test]
		public void GetLatticePlacementPropositionTest()
		{
			Lattice lattice = new Lattice(4);
			lattice.PlacePoint(2, 2, true, Direction.None);

			LatticePlacementProposition latticePlacementProposition = LatticePlacementProposition.GetLatticePlacementProposition(lattice, 2, 2, true, Direction.Up);

			Assert.AreEqual(latticePlacementProposition.x, 2);
			Assert.AreEqual(latticePlacementProposition.y, 1);
			Assert.AreEqual(latticePlacementProposition.isHydrophobic, true);
			Assert.AreEqual(latticePlacementProposition.direction, Direction.Down);
			Assert.AreSame(latticePlacementProposition.baseLattice, lattice);
		}

		[Test]
		public void GetResultingLatticeTest()
		{
			Lattice lattice = new Lattice(4);
			lattice.PlacePoint(2, 2, true, Direction.None);

			LatticePlacementProposition latticePlacementProposition = LatticePlacementProposition.GetLatticePlacementProposition(lattice, 2, 2, true, Direction.Up);

			Lattice resultingLattice = latticePlacementProposition.GetResultingLattice();

			Assert.IsTrue(resultingLattice.IsOccupied(2, 1));
			Assert.IsTrue(resultingLattice.IsHydrophobic(2, 1));
			Assert.IsTrue(resultingLattice.HasDownBinding(2, 1));
		}

		[Test]
		public void EnergyTest()
		{
			Lattice lattice = new Lattice(4);
			lattice.PlacePoint(2, 2, true, Direction.None);

			lattice.PlacePoint(0, 0, true, Direction.None);
			lattice.PlacePoint(0, 1, true, Direction.Up);
			lattice.PlacePoint(0, 2, false, Direction.Up);
			lattice.PlacePoint(1, 2, false, Direction.Left);
			lattice.PlacePoint(1, 1, true, Direction.Down);
			lattice.energy = lattice.CalculateEnergy();

			LatticePlacementProposition latticePlacementProposition = LatticePlacementProposition.GetLatticePlacementProposition(lattice, 1, 1, true, Direction.Up);

			Assert.AreEqual(latticePlacementProposition.Energy, latticePlacementProposition.GetResultingLattice().CalculateEnergy());
		}
	}
}
