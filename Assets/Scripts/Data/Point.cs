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
		public int index;
		public bool isHydrophobic;

		public Point(int index, bool isHydrophobic) : this()
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
