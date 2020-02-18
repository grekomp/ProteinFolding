using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
	public class JobTests
	{
		struct DoubleCopyJob : IJobParallelFor
		{
			[ReadOnly]
			public NativeArray<int> input;
			[ReadOnly]
			public int size;

			[WriteOnly]
			[NativeDisableParallelForRestriction]
			public NativeArray<int> output;


			public void Execute(int index)
			{
				int inputBaseIndex = index * size;
				int outputBaseIndex = index * size * 2;

				Copy(inputBaseIndex, outputBaseIndex);

				outputBaseIndex += size;

				Copy(inputBaseIndex, outputBaseIndex);
			}

			private void Copy(int inputBaseIndex, int outputBaseIndex)
			{
				for (int i = 0; i < size; i++)
				{
					output[outputBaseIndex + i] = input[inputBaseIndex + i];
				}
			}
		}


		struct FilteredCopyJob : IJobParallelFor
		{
			[ReadOnly]
			public NativeArray<int> input;
			[ReadOnly]
			public NativeArray<int> indices;
			[ReadOnly]
			public int size;

			[WriteOnly]
			[NativeDisableParallelForRestriction]
			public NativeArray<int> output;


			public void Execute(int index)
			{
				int inputBaseIndex = indices[index] * size;
				int outputBaseIndex = index * size;

				Copy(inputBaseIndex, outputBaseIndex);
			}

			private void Copy(int inputBaseIndex, int outputBaseIndex)
			{

				input.GetSubArray(inputBaseIndex, size).CopyTo(output.GetSubArray(outputBaseIndex, size));

				//for (int i = 0; i < size; i++)
				//{
				//	output[outputBaseIndex + i] = input[inputBaseIndex + i];
				//}
			}
		}


		[Test]
		[TestCase(new int[] { 1, 2, 3, 4, 5, 6 }, new int[12] { 1, 2, 3, 1, 2, 3, 4, 5, 6, 4, 5, 6 }, 3)]
		[TestCase(new int[] { 1, 2, 3, 4, 5, 6 }, new int[12] { 1, 2, 3, 4, 5, 1, 2, 3, 4, 5, 0, 0 }, 5)]
		public void DoubleCopyJob_Should_CreateTwoCopiesOfTheInputArraySliceOfSpecifiedSizeInOutputArray(
			int[] inputArray, int[] expectedOutputArray, int size)
		{
			NativeArray<int> input = new NativeArray<int>(inputArray, Allocator.TempJob);
			NativeArray<int> output = new NativeArray<int>(inputArray.Length * 2, Allocator.TempJob);

			DoubleCopyJob doubleCopyJob = new DoubleCopyJob()
			{
				input = input,
				output = output,
				size = size
			};

			JobHandle jobHandle = doubleCopyJob.Schedule(inputArray.Length / size, 1);
			jobHandle.Complete();

			Assert.That(output.ToArray(), Is.EqualTo(expectedOutputArray));

			input.Dispose();
			output.Dispose();
		}


		[Test]
		[TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 2, new int[] { 0, 2, 3, 2 }, new int[] { 1, 2, 5, 6, 7, 8, 5, 6 })]
		public void FilteredCopyJob_Should_CopySlicesOfInputThatCorrespondToSpecifiedIndices(
			int[] inputArray, int size, int[] indicesArray, int[] expectedOutputArray)
		{
			NativeArray<int> input = new NativeArray<int>(inputArray, Allocator.TempJob);
			NativeArray<int> indices = new NativeArray<int>(indicesArray, Allocator.TempJob);
			NativeArray<int> output = new NativeArray<int>(indicesArray.Length * size, Allocator.TempJob);

			FilteredCopyJob doubleCopyJob = new FilteredCopyJob()
			{
				input = input,
				indices = indices,
				output = output,
				size = size
			};

			JobHandle jobHandle = doubleCopyJob.Schedule(indicesArray.Length, 1);
			jobHandle.Complete();

			Assert.That(output.ToArray(), Is.EqualTo(expectedOutputArray));

			input.Dispose();
			indices.Dispose();
			output.Dispose();
		}
	}
}
