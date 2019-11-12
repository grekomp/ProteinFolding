using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProteinFolding
{
	public class ProteinFoldingSolver : MonoBehaviour
	{
		[Header("Input Variables")]
		public StringReference input;

		[Header("Output Variables")]
		public IntReference outputEnergy;

		public void Execute()
		{
			// Parse input


			// Create initial lattice
			// Place first two monomers arbitrarily
			Lattice initialLattice = new Lattice(2 * input.Value.Length);
			//initialLattice.TryPlacePoint(input.Value.Length, input.Value.Length - 1, GetLatticePointFor(0, BondDirection.None));
			//initialLattice.TryPlacePoint(input.Value.Length - 1, input.Value.Length - 1, GetLatticePointFor(1, BondDirection.Down));

			initialLattice.ConsoleLog();

			// Run recursive placement method


			outputEnergy.Value = initialLattice.GetEnergy();
		}

		private LatticePoint GetLatticePointFor(int inputIndex, BondSourceDirection direction)
		{
			switch (input.Value[inputIndex])
			{
				case 'H':
				case 'h':
					return new LatticePoint(PointType.Hydrophobic, direction);
				case 'P':
				case 'p':
					return new LatticePoint(PointType.Polar, direction);
				default:
					return new LatticePoint(PointType.None, direction);
			}
		}

		private void PlaceNext(int index, Lattice lattice)
		{

		}
	}
}
