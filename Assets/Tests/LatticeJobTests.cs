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
	public class LatticeJobTests
	{
		public static object[] LatticeJobTestCases =
		{
			new object[]
			{
				new Point[] {
					new Point(4), new Point(), new Point(),

					// ---------------------------
					new Point(4), new Point(), new Point(),
				},
				new LatticeInfo[] {
					new LatticeInfo(true, 0),
					new LatticeInfo(true, 0),
				},
				3,
				new Point[] {
					// ---------------------------
					new Point(4), new Point(1), new Point(),
					new Point(4), new Point(5), new Point(),
					new Point(4), new Point(7), new Point(),
					new Point(4), new Point(3), new Point(),

					// ---------------------------
					new Point(4), new Point(1), new Point(),
					new Point(4), new Point(5), new Point(),
					new Point(4), new Point(7), new Point(),
					new Point(4), new Point(3), new Point(),
				},
				new LatticeInfo[] {
					new LatticeInfo(true, 0),
					new LatticeInfo(true, 0),
					new LatticeInfo(true, 0),
					new LatticeInfo(true, 0),

					// ---------------------------
					new LatticeInfo(true, 0),
					new LatticeInfo(true, 0),
					new LatticeInfo(true, 0),
					new LatticeInfo(true, 0),
				},
			},
		};

		[Test, TestCaseSource("LatticeJobTestCases")]
		public void LatticeJob_Should_GenerateCorrectChildLattices(
			Point[] inputPoints, LatticeInfo[] inputLattices, int size, Point[] expectedOutputPoints, LatticeInfo[] expectedOutputLattices)
		{
			NativeArray<Point> points = new NativeArray<Point>(inputPoints, Allocator.TempJob);
			NativeArray<LatticeInfo> lattices = new NativeArray<LatticeInfo>(inputLattices, Allocator.TempJob);
			NativeArray<Point> ouputPoints = new NativeArray<Point>(points.Length * 4, Allocator.TempJob);
			NativeArray<LatticeInfo> outputLattices = new NativeArray<LatticeInfo>(lattices.Length * 4, Allocator.TempJob);
			NativeArray<bool> parsedInput = new NativeArray<bool>(new bool[] { false, false, false }, Allocator.TempJob);

			LatticeJob latticeJob = new LatticeJob()
			{
				points = points,
				lattices = lattices,
				outputPoints = ouputPoints,
				outputLattices = outputLattices,
				size = size,
				nextIsHydrophobic = false,
				parsedInput = parsedInput,
				currentProteinStringIndex = 1,
			};

			JobHandle jobHandle = latticeJob.Schedule(lattices.Length, 1);
			jobHandle.Complete();

			Assert.That(ouputPoints.ToArray(), Is.EqualTo(expectedOutputPoints));
			Assert.That(outputLattices.ToArray(), Is.EqualTo(expectedOutputLattices));

			points.Dispose();
			lattices.Dispose();
			ouputPoints.Dispose();
			outputLattices.Dispose();
			parsedInput.Dispose();
		}
	}
}
