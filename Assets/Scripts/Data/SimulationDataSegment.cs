using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;

namespace ProteinFolding
{
	[Serializable]
	public class SimulationDataSegment
	{
		public NativeArray<Point> points;
		public NativeArray<LatticeInfo> lattices;
		public SimulationTreeLevel treeLevel;

		public SimulationDataSegment(NativeArray<Point> points, NativeArray<LatticeInfo> lattices, SimulationTreeLevel treeLevel)
		{
			this.points = points;
			this.lattices = lattices;
			this.treeLevel = treeLevel;
		}

		public void Dispose()
		{
			points.Dispose();
			lattices.Dispose();
		}
	}
}
