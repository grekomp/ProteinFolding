using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace ProteinFolding
{
	[BurstCompile]
	public struct LatticesCalculateAverageEnergyJob : IJob
	{
		[ReadOnly]
		public NativeArray<LatticeInfo> lattices;

		[WriteOnly]
		public NativeArray<float> averageEnergy;
		[WriteOnly]
		public NativeArray<int> bestEnergy;

		public void Execute()
		{
			int bestEnergyInternal = 0;
			int energySum = 0;
			int validElements = 0;

			for (int i = 0; i < lattices.Length; i++)
			{
				if (lattices[i].isValid)
				{
					validElements++;
					energySum += lattices[i].energy;
					bestEnergyInternal = math.min(bestEnergyInternal, lattices[i].energy);
				}
			}

			averageEnergy[0] = energySum / (float)validElements;
			bestEnergy[0] = bestEnergyInternal;
		}
	}
}
