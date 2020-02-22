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
	public struct LatticeFilteredCopyJob : IJobParallelFor
	{
		// Inputs
		[ReadOnly]
		public NativeArray<Point> points;
		[ReadOnly]
		public NativeArray<LatticeInfo> lattices;
		[ReadOnly]
		public NativeArray<int> indices;
		[ReadOnly]
		public int singleLatticePointsCount;


		// Outputs
		[WriteOnly]
		[NativeDisableParallelForRestriction]
		public NativeArray<Point> outputPoints;
		[WriteOnly]
		[NativeDisableParallelForRestriction]
		public NativeArray<LatticeInfo> outputLattices;


		public void Execute(int index)
		{
			int inputBaseIndex = indices[index] * singleLatticePointsCount;
			int outputBaseIndex = index * singleLatticePointsCount;

			outputLattices[index] = lattices[indices[index]];
			CopyPoints(inputBaseIndex, outputBaseIndex);
		}

		private void CopyPoints(int inputBaseIndex, int outputBaseIndex)
		{
			points.GetSubArray(inputBaseIndex, singleLatticePointsCount).CopyTo(outputPoints.GetSubArray(outputBaseIndex, singleLatticePointsCount));
		}
	}
}
