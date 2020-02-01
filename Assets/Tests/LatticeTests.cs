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
		[Test]
		public void IsHydrophobicTrue()
		{
			Lattice lattice = new Lattice(2);
			lattice.PlaceInitPoint(true);

			Assert.IsTrue(lattice.IsHydrophobic(lattice.lastX, lattice.lastY));
		}

		[Test]
		public void IsHydrophobicFalse()
		{
			Lattice lattice = new Lattice(5);

			Assert.IsFalse(lattice.IsHydrophobic(1, 1));

			lattice.PlaceInitPoint(false);

			int initX = lattice.lastX;
			int initY = lattice.lastY;

			lattice.PlacePoint(true, Direction.Up);
			lattice.PlacePoint(true, Direction.Right);
			lattice.PlacePoint(true, Direction.Down);
			lattice.PlacePoint(true, Direction.Down);
			lattice.PlacePoint(true, Direction.Left);
			lattice.PlacePoint(true, Direction.Left);

			Assert.IsFalse(lattice.IsHydrophobic(initX, initY));
		}

		[Test]
		public void IsOccupiedTrue()
		{
			Lattice lattice = new Lattice(5);
			lattice.PlaceInitPoint(false);

			int initX = lattice.lastX;
			int initY = lattice.lastY;

			lattice.PlacePoint(true, Direction.Right);
			lattice.PlacePoint(false, Direction.Up);

			Assert.IsTrue(lattice.IsOccupied(initX, initY));
		}
		[Test]
		public void IsOccupiedFalse()
		{
			Lattice lattice = new Lattice(5);
			lattice.PlaceInitPoint(false);
			lattice.PlacePoint(true, Direction.Up);
			lattice.PlacePoint(true, Direction.Right);
			lattice.PlacePoint(true, Direction.Down);
			lattice.PlacePoint(true, Direction.Down);
			lattice.PlacePoint(true, Direction.Left);
			lattice.PlacePoint(true, Direction.Left);

			Assert.IsFalse(lattice.IsOccupied(1, 1));
		}

		//[Test]
		//public void HasDownBindingTrue01()
		//{
		//	Lattice lattice = new Lattice(3);
		//	lattice.PlacePoint(1, 1, false, Direction.None);
		//	lattice.PlacePoint(1, 2, false, Direction.Up);

		//	Assert.IsTrue(lattice.HasDownBinding(1, 1));
		//}
		//[Test]
		//public void HasDownBindingTrue02()
		//{
		//	Lattice lattice = new Lattice(3);
		//	lattice.PlacePoint(1, 1, false, Direction.None);
		//	lattice.PlacePoint(1, 0, false, Direction.Down);

		//	Assert.IsTrue(lattice.HasDownBinding(1, 0));
		//}
		//[Test]
		//public void HasDownBindingFalse()
		//{
		//	Lattice lattice = new Lattice(3);
		//	lattice.PlacePoint(1, 1, false, Direction.None);
		//	lattice.PlacePoint(1, 2, false, Direction.Up);

		//	Assert.IsFalse(lattice.HasDownBinding(1, 2));
		//}
		//[Test]
		//public void HasRightBindingTrue01()
		//{
		//	Lattice lattice = new Lattice(3);
		//	lattice.PlacePoint(1, 1, false, Direction.None);
		//	lattice.PlacePoint(0, 1, false, Direction.Right);

		//	Assert.IsTrue(lattice.HasRightBinding(0, 1));
		//}
		//[Test]
		//public void HasRightBindingTrue02()
		//{
		//	Lattice lattice = new Lattice(3);
		//	lattice.PlacePoint(1, 1, false, Direction.None);
		//	lattice.PlacePoint(2, 1, false, Direction.Left);

		//	Assert.IsTrue(lattice.HasRightBinding(1, 1));
		//}
		//[Test]
		//public void HasRightBindingFalse()
		//{
		//	Lattice lattice = new Lattice(3);
		//	lattice.PlacePoint(1, 1, false, Direction.None);
		//	lattice.PlacePoint(0, 1, false, Direction.Right);

		//	Assert.IsFalse(lattice.HasRightBinding(1, 1));
		//}

		[Test]
		public void OppositeDirectionTest()
		{
			Lattice lattice = new Lattice(1);

			Assert.AreEqual(lattice.GetOppositeDirection(Direction.Up), Direction.Down);
			Assert.AreEqual(lattice.GetOppositeDirection(Direction.Right), Direction.Left);
			Assert.AreEqual(lattice.GetOppositeDirection(Direction.Down), Direction.Up);
			Assert.AreEqual(lattice.GetOppositeDirection(Direction.Left), Direction.Right);
			Assert.AreEqual(lattice.GetOppositeDirection(Direction.None), Direction.None);
		}

		[Test]
		public void EnergyCalculationTest()
		{
			Lattice lattice = new Lattice(5);
			lattice.PlaceInitPoint(true);
			lattice.PlacePoint(true, Direction.Up);
			lattice.PlacePoint(false, Direction.Up);
			lattice.PlacePoint(false, Direction.Left);
			lattice.PlacePoint(true, Direction.Down);
			lattice.PlacePoint(true, Direction.Down);

			Assert.That(lattice.CalculateEnergy(), Is.EqualTo(-2));
		}
	}
}
