using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProteinFolding
{
	public enum PointType
	{
		None,
		Hydrophobic,
		Polar
	}
	public enum BondSourceDirection
	{
		None,
		Up,
		Right,
		Down,
		Left
	}

	struct LatticePoint
	{
		public PointType type;
		public BondSourceDirection direction;

		public LatticePoint(PointType content, BondSourceDirection direction)
		{
			this.type = content;
			this.direction = direction;
		}
	}
}
