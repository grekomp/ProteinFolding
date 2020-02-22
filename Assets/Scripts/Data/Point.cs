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
		public short conformationIndex;

		public Point(short proteinStringIndex) : this()
		{
			this.conformationIndex = proteinStringIndex;
		}


		#region Overrides
		public override string ToString()
		{
			return $"Point({conformationIndex})";
		}
		#endregion
	}
}
