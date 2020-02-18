using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using ProteinFolding;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
	public class LatticeTests
	{
		//Lattice lattice;

		//[Test]
		//public void IsHydrophobicTrue()
		//{
		//	lattice = new Lattice(2);
		//	lattice.PlaceInitPoint(true);

		//	Assert.IsTrue(lattice.GetIsHydrophobic(lattice.lastX, lattice.lastY));
		//}

		//[Test]
		//public void IsHydrophobicFalse()
		//{
		//	lattice = new Lattice(5);

		//	Assert.IsFalse(lattice.GetIsHydrophobic(1, 1));

		//	lattice.PlaceInitPoint(false);

		//	int initX = lattice.lastX;
		//	int initY = lattice.lastY;

		//	lattice.PlacePoint(true, Direction.Up);
		//	lattice.PlacePoint(true, Direction.Right);
		//	lattice.PlacePoint(true, Direction.Down);
		//	lattice.PlacePoint(true, Direction.Down);
		//	lattice.PlacePoint(true, Direction.Left);
		//	lattice.PlacePoint(true, Direction.Left);

		//	Assert.IsFalse(lattice.GetIsHydrophobic(initX, initY));
		//}

		//[Test]
		//public void IsOccupiedTrue()
		//{
		//	lattice = new Lattice(5);
		//	lattice.PlaceInitPoint(false);

		//	int initX = lattice.lastX;
		//	int initY = lattice.lastY;

		//	lattice.PlacePoint(true, Direction.Right);
		//	lattice.PlacePoint(false, Direction.Up);

		//	Assert.IsTrue(lattice.IsOccupied(initX, initY));
		//}
		//[Test]
		//public void IsOccupiedFalse()
		//{
		//	lattice = new Lattice(5);
		//	lattice.PlaceInitPoint(false);
		//	lattice.PlacePoint(true, Direction.Up);
		//	lattice.PlacePoint(true, Direction.Right);
		//	lattice.PlacePoint(true, Direction.Down);
		//	lattice.PlacePoint(true, Direction.Down);
		//	lattice.PlacePoint(true, Direction.Left);
		//	lattice.PlacePoint(true, Direction.Left);

		//	Assert.IsFalse(lattice.IsOccupied(1, 1));
		//}

		//[Test]
		//public void OppositeDirectionTest()
		//{
		//	lattice = new Lattice(1);

		//	Assert.AreEqual(lattice.GetOppositeDirection(Direction.Up), Direction.Down);
		//	Assert.AreEqual(lattice.GetOppositeDirection(Direction.Right), Direction.Left);
		//	Assert.AreEqual(lattice.GetOppositeDirection(Direction.Down), Direction.Up);
		//	Assert.AreEqual(lattice.GetOppositeDirection(Direction.Left), Direction.Right);
		//	Assert.AreEqual(lattice.GetOppositeDirection(Direction.None), Direction.None);
		//}

		//[Test]
		//public void EnergyCalculationTest()
		//{
		//	lattice = new Lattice(5);
		//	lattice.PlaceInitPoint(true);
		//	lattice.PlacePoint(true, Direction.Up);
		//	lattice.PlacePoint(false, Direction.Up);
		//	lattice.PlacePoint(false, Direction.Left);
		//	lattice.PlacePoint(true, Direction.Down);
		//	lattice.PlacePoint(true, Direction.Down);

		//	Assert.That(lattice.CalculateEnergy(), Is.EqualTo(-2));
		//}


		//[Test]
		//public void GetChildLattices_Should()
		//{
		//	Lattice resultingLattice = new Lattice();

		//	try
		//	{
		//		lattice = new Lattice(4);
		//		lattice.PlaceInitPoint(true);

		//		resultingLattice = lattice.GetChildLattice(true, Direction.Up);

		//		Assert.That(resultingLattice.GetPoint(2, 1), Is.EqualTo(3));
		//		Assert.That(resultingLattice.GetIsHydrophobic(2, 1), Is.EqualTo(true));
		//	}
		//	finally
		//	{
		//		resultingLattice.Dispose();
		//	}
		//}

		//[Test]
		//public void GetResultingLatticeTest()
		//{
		//	Lattice resultingLattice = new Lattice();

		//	try
		//	{
		//		lattice = new Lattice(4);
		//		lattice.PlaceInitPoint(true);

		//		resultingLattice = lattice.GetChildLattice(true, Direction.Up);

		//		Assert.IsTrue(resultingLattice.IsOccupied(2, 1));
		//		Assert.IsTrue(resultingLattice.GetIsHydrophobic(2, 1));

		//	}
		//	finally
		//	{
		//		resultingLattice.Dispose();
		//	}
		//}

		//[Test]
		//public void GetChildLattice_Should_ReutrnALatticeWithDecreasedEnergy_IfPointIsHydrophobicAndThereAreAdjacentHydrophobicPoints()
		//{
		//	Lattice resultingLattice = new Lattice();

		//	try
		//	{
		//		lattice = new Lattice(4);
		//		lattice.PlaceInitPoint(true);

		//		lattice.PlacePoint(true, Direction.Up);
		//		lattice.PlacePoint(false, Direction.Up);
		//		lattice.PlacePoint(false, Direction.Left);
		//		lattice.PlacePoint(true, Direction.Down);
		//		lattice.energy = lattice.CalculateEnergy();

		//		resultingLattice = lattice.GetChildLattice(true, Direction.Down);

		//		Assert.AreEqual(resultingLattice.energy, resultingLattice.CalculateEnergy());
		//	}
		//	finally
		//	{
		//		resultingLattice.Dispose();
		//	}
		//}

		//[TearDown]
		//public void Teardown()
		//{
		//	lattice.Dispose();
		//}
	}
}
