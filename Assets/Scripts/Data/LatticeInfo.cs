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
		public int lastPoint;
		public int lastIndex;

		public LatticeInfo(bool isValid, int energy, int lastPoint, int lastIndex)
		{
			this.isValid = isValid;
			this.energy = energy;
			this.lastPoint = lastPoint;
			this.lastIndex = lastIndex;
		}


		#region Overrides
		public override string ToString()
		{
			return $"LatticeInfo( isValid: {isValid}, energy: {energy}, lastPoint: {lastPoint}, lastIndex: {lastIndex})";
		}
		#endregion
	}
}
