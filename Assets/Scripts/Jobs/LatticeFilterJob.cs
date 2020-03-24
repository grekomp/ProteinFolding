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
	public struct LatticeFilterJob : IJobParallelForFilter
	{
		[ReadOnly]
		public NativeArray<LatticeInfo> lattices;
		[ReadOnly]
		public NativeArray<float> random;
		[ReadOnly]
		public float pruningProbability01;
		[ReadOnly]
		public float pruningProbability02;
		[ReadOnly]
		public NativeArray<float> averageEnergy;
		[ReadOnly]
		public NativeArray<int> bestEnergy;

		public bool lastPointIsHydrophobic;

		public bool Execute(int index)
		{
			if (lattices[index].isValid)
			{
				if (lastPointIsHydrophobic) return true;

				if (lattices[index].energy <= bestEnergy[0]) return true;

				if (lattices[index].energy <= averageEnergy[0])
				{
					return random[index] > pruningProbability01;
				}
				else
				{
					return random[index] > pruningProbability02;
				}
			}
			else
			{
				return false;
			}
		}
	}
}
