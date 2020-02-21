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
		public float averageEnergy;
		[ReadOnly]
		public int bestEnergy;

		public bool lastPointIsHydrophobic;

		public bool Execute(int index)
		{
			if (lattices[index].isValid)
			{
				if (lastPointIsHydrophobic) return true;

				if (lattices[index].energy <= bestEnergy) return true;

				if (lattices[index].energy <= averageEnergy)
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
