using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProteinFolding
{
	[Serializable]
	public struct IndexedPoint
	{
		public Point point;
		public int proteinStringIndex;

		public IndexedPoint(Point point, int proteinStringIndex)
		{
			this.point = point;
			this.proteinStringIndex = proteinStringIndex;
		}
	}
}
