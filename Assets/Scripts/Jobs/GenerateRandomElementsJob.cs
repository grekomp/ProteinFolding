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
	public struct GenerateRandomElementsJob : IJob
	{
		public Unity.Mathematics.Random random;

		[WriteOnly]
		public NativeArray<float> randomElements;

		public void Execute()
		{
			for (int i = 0; i < randomElements.Length; i++)
			{
				randomElements[i] = random.NextFloat();
			}
		}
	}
}
