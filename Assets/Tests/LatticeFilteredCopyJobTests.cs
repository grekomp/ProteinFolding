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
	public class LatticeFilteredCopyJobTests
	{

		public static object[] FilteredCopyJobTestCases =
		{
			new object[]
			{
				new Point[] {
					new Point(), new Point(2, false), new Point(3, true), new Point(),
					new Point(), new Point(2, true), new Point(), new Point(),
					new Point(), new Point(3, false), new Point(2, false), new Point(),
				},
				new LatticeInfo[] {
					new LatticeInfo(true, 0, 3, 2),
					new LatticeInfo(true, 0, 2, 1),
					new LatticeInfo(true, 0, 3, 1),
				},
				2,
				new int[] {0, 2},
				new Point[] {
					new Point(), new Point(2, false), new Point(3, true), new Point(),
					new Point(), new Point(3, false), new Point(2, false), new Point(),
				},
				new LatticeInfo[] {
					new LatticeInfo(true, 0, 3, 2),
					new LatticeInfo(true, 0, 3, 1),
				},
			}
		};

		[Test, TestCaseSource("FilteredCopyJobTestCases")]
		public void FilteredCopyJob_Should_CopySlicesOfInputThatCorrespondToSpecifiedIndices(
			Point[] inputPoints, LatticeInfo[] inputLattices, int size, int[] indicesArray, Point[] expectedOutputPoints, LatticeInfo[] expectedOutputLattices)
		{
			NativeArray<Point> points = new NativeArray<Point>(inputPoints, Allocator.TempJob);
			NativeArray<LatticeInfo> lattices = new NativeArray<LatticeInfo>(inputLattices, Allocator.TempJob);
			NativeArray<int> indices = new NativeArray<int>(indicesArray, Allocator.TempJob);
			NativeArray<Point> ouputPoints = new NativeArray<Point>(indicesArray.Length * size * size, Allocator.TempJob);
			NativeArray<LatticeInfo> outputLattices = new NativeArray<LatticeInfo>(indicesArray.Length, Allocator.TempJob);

			LatticeFilteredCopyJob doubleCopyJob = new LatticeFilteredCopyJob()
			{
				points = points,
				lattices = lattices,
				indices = indices,
				outputPoints = ouputPoints,
				outputLattices = outputLattices,
				size = size
			};

			JobHandle jobHandle = doubleCopyJob.Schedule(indicesArray.Length, 1);
			jobHandle.Complete();

			Assert.That(ouputPoints.ToArray(), Is.EqualTo(expectedOutputPoints));
			Assert.That(outputLattices.ToArray(), Is.EqualTo(expectedOutputLattices));

			points.Dispose();
			lattices.Dispose();
			indices.Dispose();
			ouputPoints.Dispose();
			outputLattices.Dispose();
		}
	}
}
