using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace ProteinFolding
{
	[BurstCompile]
	public struct CopySegmentDataJob : IJob
	{
		[ReadOnly]
		public NativeArray<Point> inputPoints;
		[ReadOnly]
		public NativeArray<LatticeInfo> inputLattices;

		public int minIndex;
		public int latticeSize;
		public int latticesToCopy;

		[WriteOnly]
		public NativeArray<Point> outputPoints;
		[WriteOnly]
		public NativeArray<LatticeInfo> outputLattices;

		public void Execute()
		{
			inputLattices.GetSubArray(minIndex, latticesToCopy).CopyTo(outputLattices.GetSubArray(0, latticesToCopy));
			inputPoints.GetSubArray(minIndex * latticeSize, latticeSize * latticesToCopy).CopyTo(outputPoints.GetSubArray(0, latticeSize * latticesToCopy));
		}
	}
}
