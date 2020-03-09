using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProteinFolding
{
	[Serializable]
	public struct LatticeInfo
	{
		public bool isValid;
		public int energy;

		public LatticeInfo(bool isValid, int energy)
		{
			this.isValid = isValid;
			this.energy = energy;
		}


		#region Overrides
		public override string ToString()
		{
			return $"LatticeInfo( isValid: {isValid}, energy: {energy})";
		}
		#endregion
	}
}
