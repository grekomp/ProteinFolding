//using System.Collections;
//using System.Collections.Generic;
//using NUnit.Framework;
//using ProteinFolding;
//using UnityEngine;
//using UnityEngine.TestTools;

//namespace Tests
//{
//	public class LatticePlacementPropositionTests
//	{
//		Lattice lattice;

//		[Test]
//		public void GetLatticePlacementPropositionTest()
//		{
//			lattice = new Lattice(4);
//			lattice.PlaceInitPoint(true);

//			LatticePlacementProposition latticePlacementProposition = lattice.GetPlacementPropositions(true)[0];

//			Assert.AreEqual(latticePlacementProposition.x, 2);
//			Assert.AreEqual(latticePlacementProposition.y, 1);
//			Assert.AreEqual(latticePlacementProposition.isHydrophobic, true);
//		}

//		[Test]
//		public void GetResultingLatticeTest()
//		{
//			Lattice resultingLattice = new Lattice();

//			try
//			{
//				lattice = new Lattice(4);
//				lattice.PlaceInitPoint(true);

//				LatticePlacementProposition latticePlacementProposition = lattice.GetPlacementPropositions(true)[0];

//				resultingLattice = latticePlacementProposition.GetResultingLattice();

//				Assert.IsTrue(resultingLattice.IsOccupied(2, 1));
//				Assert.IsTrue(resultingLattice.GetIsHydrophobic(2, 1));

//			}
//			finally
//			{
//				resultingLattice.Dispose();
//			}
//		}

//		[Test]
//		public void EnergyTest()
//		{
//			Lattice resultingLattice = new Lattice();

//			try
//			{
//				lattice = new Lattice(4);
//				lattice.PlaceInitPoint(true);

//				lattice.PlacePoint(true, Direction.Up);
//				lattice.PlacePoint(false, Direction.Up);
//				lattice.PlacePoint(false, Direction.Left);
//				lattice.PlacePoint(true, Direction.Down);
//				lattice.energy = lattice.CalculateEnergy();

//				var placementPropositions = lattice.GetPlacementPropositions(true);

//				LatticePlacementProposition latticePlacementProposition = lattice.GetPlacementPropositions(true)[2];
//				resultingLattice = latticePlacementProposition.GetResultingLattice();

//				Assert.AreEqual(latticePlacementProposition.Energy, resultingLattice.CalculateEnergy());
//			}
//			finally
//			{
//				resultingLattice.Dispose();
//			}
//		}

//		[TearDown]
//		public void Teardown()
//		{
//			lattice.Dispose();
//		}
//	}
//}
