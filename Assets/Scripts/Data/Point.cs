using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProteinFolding
{
	[Serializable]
	public struct Point
	{
		public byte index;
		public bool isHydrophobic;

		public Point(byte index, bool isHydrophobic) : this()
		{
			this.index = index;
			this.isHydrophobic = isHydrophobic;
		}


		#region Overrides
		public override string ToString()
		{
			return $"Point({index}, {isHydrophobic})";
		}
		#endregion
	}
}
