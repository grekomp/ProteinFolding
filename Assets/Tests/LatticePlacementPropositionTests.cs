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
			lattice.PlaceInitPoint(true);

			LatticePlacementProposition latticePlacementProposition = lattice.GetPlacementPropositions(true)[0];

			Assert.AreEqual(latticePlacementProposition.x, 2);
			Assert.AreEqual(latticePlacementProposition.y, 1);
			Assert.AreEqual(latticePlacementProposition.isHydrophobic, true);
			Assert.AreSame(latticePlacementProposition.baseLattice, lattice);
		}

		[Test]
		public void GetResultingLatticeTest()
		{
			Lattice lattice = new Lattice(4);
			lattice.PlaceInitPoint(true);

			LatticePlacementProposition latticePlacementProposition = lattice.GetPlacementPropositions(true)[0];

			Lattice resultingLattice = latticePlacementProposition.GetResultingLattice();

			Assert.IsTrue(resultingLattice.IsOccupied(2, 1));
			Assert.IsTrue(resultingLattice.IsHydrophobic(2, 1));
		}

		[Test]
		public void EnergyTest()
		{
			Lattice lattice = new Lattice(4);
			lattice.PlaceInitPoint(true);

			lattice.PlacePoint(true, Direction.Up);
			lattice.PlacePoint(false, Direction.Up);
			lattice.PlacePoint(false, Direction.Left);
			lattice.PlacePoint(true, Direction.Down);
			lattice.energy = lattice.CalculateEnergy();

			var placementPropositions = lattice.GetPlacementPropositions(true);

			LatticePlacementProposition latticePlacementProposition = lattice.GetPlacementPropositions(true)[2];
			Lattice resultingLattice = latticePlacementProposition.GetResultingLattice();

			Assert.AreEqual(latticePlacementProposition.Energy, resultingLattice.CalculateEnergy());
		}
	}
}
