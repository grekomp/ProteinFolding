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
			lattice.PlacePoint(1, 1, true, Direction.None);

			Assert.IsTrue(lattice.IsHydrophobic(1, 1));
		}

		[Test]
		public void IsHydrophobicFalse()
		{
			Lattice lattice = new Lattice(2);

			Assert.IsFalse(lattice.IsHydrophobic(1, 1));

			lattice.PlacePoint(1, 1, false, Direction.None);
			lattice.PlacePoint(0, 1, true, Direction.None);
			lattice.PlacePoint(1, 0, true, Direction.None);

			Assert.IsFalse(lattice.IsHydrophobic(1, 1));
		}

		[Test]
		public void IsOccupiedTrue()
		{
			Lattice lattice = new Lattice(3);
			lattice.PlacePoint(1, 1, false, Direction.None);
			lattice.PlacePoint(0, 1, false, Direction.Right);
			lattice.PlacePoint(2, 1, false, Direction.Right);

			Assert.IsTrue(lattice.IsOccupied(1, 1));
		}
		[Test]
		public void IsOccupiedFalse()
		{
			Lattice lattice = new Lattice(3);
			lattice.PlacePoint(0, 0, false, Direction.None);
			lattice.PlacePoint(0, 1, false, Direction.Up);
			lattice.PlacePoint(0, 2, false, Direction.Up);
			lattice.PlacePoint(1, 2, false, Direction.Left);
			lattice.PlacePoint(2, 2, true, Direction.Left);
			lattice.PlacePoint(2, 1, true, Direction.Down);

			Assert.IsFalse(lattice.IsOccupied(1, 1));
		}

		[Test]
		public void HasDownBindingTrue01()
		{
			Lattice lattice = new Lattice(3);
			lattice.PlacePoint(1, 1, false, Direction.None);
			lattice.PlacePoint(1, 2, false, Direction.Up);

			Assert.IsTrue(lattice.HasDownBinding(1, 1));
		}
		[Test]
		public void HasDownBindingTrue02()
		{
			Lattice lattice = new Lattice(3);
			lattice.PlacePoint(1, 1, false, Direction.None);
			lattice.PlacePoint(1, 0, false, Direction.Down);

			Assert.IsTrue(lattice.HasDownBinding(1, 0));
		}
		[Test]
		public void HasDownBindingFalse()
		{
			Lattice lattice = new Lattice(3);
			lattice.PlacePoint(1, 1, false, Direction.None);
			lattice.PlacePoint(1, 2, false, Direction.Up);

			Assert.IsFalse(lattice.HasDownBinding(1, 2));
		}
		[Test]
		public void HasRightBindingTrue01()
		{
			Lattice lattice = new Lattice(3);
			lattice.PlacePoint(1, 1, false, Direction.None);
			lattice.PlacePoint(0, 1, false, Direction.Right);

			Assert.IsTrue(lattice.HasRightBinding(0, 1));
		}
		[Test]
		public void HasRightBindingTrue02()
		{
			Lattice lattice = new Lattice(3);
			lattice.PlacePoint(1, 1, false, Direction.None);
			lattice.PlacePoint(2, 1, false, Direction.Left);

			Assert.IsTrue(lattice.HasRightBinding(1, 1));
		}
		[Test]
		public void HasRightBindingFalse()
		{
			Lattice lattice = new Lattice(3);
			lattice.PlacePoint(1, 1, false, Direction.None);
			lattice.PlacePoint(0, 1, false, Direction.Right);

			Assert.IsFalse(lattice.HasRightBinding(1, 1));
		}

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
			Lattice lattice = new Lattice(3);
			lattice.PlacePoint(0, 0, true, Direction.None);
			lattice.PlacePoint(0, 1, true, Direction.Up);
			lattice.PlacePoint(0, 2, false, Direction.Up);
			lattice.PlacePoint(1, 2, false, Direction.Left);
			lattice.PlacePoint(1, 1, true, Direction.Down);
			lattice.PlacePoint(1, 0, true, Direction.Down);

			Assert.AreEqual(lattice.CalculateEnergy(), -2);
		}
	}
}
