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

		public Point(byte index) : this()
		{
			this.index = index;
		}


		#region Overrides
		public override string ToString()
		{
			return $"Point({index})";
		}
		#endregion
	}
}
