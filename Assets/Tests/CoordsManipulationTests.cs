using System.Collections;
using NUnit.Framework;
using ProteinFolding;
using UnityEngine.TestTools;

namespace Tests
{
	public class CoordsManipulationTests
	{
		[Test]
		public void AdjacentCoordsTest()
		{
			Lattice lattice = new Lattice(10);

			Assert.AreEqual(lattice.GetAdjacentCoordinates(5, 5, Direction.Up), new System.Tuple<int, int>(5, 4));
			Assert.AreEqual(lattice.GetAdjacentCoordinates(5, 5, Direction.Down), new System.Tuple<int, int>(5, 6));
			Assert.AreEqual(lattice.GetAdjacentCoordinates(5, 5, Direction.Left), new System.Tuple<int, int>(4, 5));
			Assert.AreEqual(lattice.GetAdjacentCoordinates(5, 5, Direction.Right), new System.Tuple<int, int>(6, 5));
		}
	}
}
