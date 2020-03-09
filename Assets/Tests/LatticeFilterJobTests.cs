using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using ProteinFolding;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
	public class LatticeFilterJobTests
	{
		public static object[] FilterJobTestCases =
		{
			new object[]
			{
				new LatticeInfo[] {
					new LatticeInfo(true, 0),
					new LatticeInfo(false, 0),
					new LatticeInfo(true, 0),
				},
				new int[] {0, 2},
				new float[] {1, 1, 1},
			}
		};

		[Test, TestCaseSource("FilterJobTestCases")]
		public void FilterJob_Should_GenerateIndicesCorrespondingToValidLattices(
			LatticeInfo[] inputLattices, int[] expectedOutputIndices, float[] randomValues)
		{
			NativeArray<LatticeInfo> lattices = new NativeArray<LatticeInfo>(inputLattices, Allocator.TempJob);
			NativeList<int> indices = new NativeList<int>(lattices.Length, Allocator.TempJob);
			NativeArray<float> random = new NativeArray<float>(randomValues, Allocator.TempJob);

			LatticeFilterJob latticeFilterJob = new LatticeFilterJob()
			{
				random = random,
				lattices = lattices,
			};
			JobHandle jobHandle = latticeFilterJob.ScheduleAppend(indices, lattices.Length, 1);
			jobHandle.Complete();

			Assert.That(indices.ToArray(), Is.EqualTo(expectedOutputIndices));

			lattices.Dispose();
			indices.Dispose();
			random.Dispose();
		}
	}
}
