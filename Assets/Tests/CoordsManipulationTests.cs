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
			Assert.AreEqual(Lattice.GetAdjacentX(5, Direction.Up), 5);
			Assert.AreEqual(Lattice.GetAdjacentX(5, Direction.Left), 4);
			Assert.AreEqual(Lattice.GetAdjacentX(5, Direction.Down), 5);
			Assert.AreEqual(Lattice.GetAdjacentX(5, Direction.Right), 6);

			Assert.AreEqual(Lattice.GetAdjacentY(5, Direction.Up), 4);
			Assert.AreEqual(Lattice.GetAdjacentY(5, Direction.Left), 5);
			Assert.AreEqual(Lattice.GetAdjacentY(5, Direction.Down), 6);
			Assert.AreEqual(Lattice.GetAdjacentY(5, Direction.Right), 5);
		}
	}
}
