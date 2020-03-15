using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ProteinFolding
{
	public class ProteinFoldingSolver : MonoBehaviour
	{
		[Header("Input Variables")]
		public StringReference inputString;
		[Space]
		public FloatReference pruningProbability01;
		public FloatReference pruningProbability02;

		[Header("Output Variables")]
		public IntReference outputEnergy;
		[Space]
		public LatticeReference outputLattice;
		[Space]
		public bool[] parsedInputPersistent;
		public int outputLatticeIndex;

		public bool suspendExecution = false;
		public bool stepByStepExecution = false;


		[Header("Runtime Variables")]
		public int latticeSize = 0;

		//public Point[] pointsPersistent;
		//public LatticeInfo[] latticesPresistent;
		public float[] randomValuesPersistent;

		public NativeArray<Point>[] segmentedPoints;
		public NativeArray<LatticeInfo>[] segmentedLattices;

		public NativeArray<Point> points;
		public NativeArray<LatticeInfo> lattices;

		[Space]
		public int currentTreeLevel = 0;
		public int lastBestEnergy = 0;

		Coroutine runningExecution;


		#region Executing simulation
		[ContextMenu("Start Execution")]
		public void StartExecution()
		{
			// Parse input
			parsedInputPersistent = ParseInput().ToArray();

			// Start coroutine
			Debug.Log("Started execution");
			if (runningExecution != null) StopCoroutine(runningExecution);
			runningExecution = StartCoroutine(Execute());
		}

		public IEnumerator Execute()
		{
			lastBestEnergy = 0;

			if (parsedInputPersistent.Length > 2)
			{
				InitializeExecution();

				// Place the rest of points
				currentTreeLevel = 2;
				while (currentTreeLevel < parsedInputPersistent.Length)
				{
					if (suspendExecution)
					{
						yield return null;
						continue;
					}
					if (stepByStepExecution)
					{
						suspendExecution = true;
					}

					ExecuteTreeLevel();

					currentTreeLevel++;
					yield return null;
				}

				CreateOutputLattice();
			}

			yield return null;
			points.Dispose();
			lattices.Dispose();
		}

		private void InitializeExecution()
		{
			// Create initial lattice
			latticeSize = 2 * parsedInputPersistent.Length + 2;

			points = new NativeArray<Point>(latticeSize * latticeSize, Allocator.Persistent);
			lattices = new NativeArray<LatticeInfo>(1, Allocator.Persistent);

			// Place first two monomers arbitrarily
			int initialIndex = latticeSize * ((latticeSize - 1) / 2) + (latticeSize / 2);
			points[0] = new Point((short)initialIndex);
			points[1] = new Point((short)(initialIndex + 1));
			lattices[0] = new LatticeInfo(true, 0);
		}
		private void ExecuteTreeLevel()
		{
			// Generate children
			NativeArray<Point> childPoints = new NativeArray<Point>(lattices.Length * parsedInputPersistent.Length * 4, Allocator.TempJob);
			NativeArray<LatticeInfo> childLattices = new NativeArray<LatticeInfo>(lattices.Length * 4, Allocator.TempJob);
			NativeArray<bool> parsedInput = new NativeArray<bool>(parsedInputPersistent, Allocator.TempJob);
			Debug.Log($"Generating child lattices: {childLattices.Length}.");
			GenerateChildLattices(childPoints, childLattices, parsedInput);

			points.Dispose();
			lattices.Dispose();

			// Calculate average energy
			NativeArray<float> averageEnergy = new NativeArray<float>(1, Allocator.TempJob);
			NativeArray<int> bestEnergy = new NativeArray<int>(1, Allocator.TempJob);
			CalculateAverageEnergy(childLattices, averageEnergy, bestEnergy);
			lastBestEnergy = bestEnergy[0];

			// Generate random values
			NativeArray<float> randomValues = new NativeArray<float>(childLattices.Length, Allocator.TempJob);
			GenerateRandomElements(ref randomValues);
			//NativeArray<float> randomValues = new NativeArray<float>(randomValuesPersistent, Allocator.TempJob);

			// Generate filtered indices
			NativeList<int> indices = new NativeList<int>(childLattices.Length, Allocator.TempJob);
			GenerateIndicesFilter(ref childLattices, averageEnergy[0], bestEnergy[0], indices, randomValues);

			// Generate filtered output 
			points = new NativeArray<Point>(parsedInput.Length * indices.Length, Allocator.Persistent);
			lattices = new NativeArray<LatticeInfo>(indices.Length, Allocator.Persistent);
			FilterIndices(childPoints, childLattices, ref indices, points, lattices);

			parsedInput.Dispose();
			bestEnergy.Dispose();
			averageEnergy.Dispose();
			indices.Dispose();
			childPoints.Dispose();
			childLattices.Dispose();
			randomValues.Dispose();

			//pointsPersistent = new Point[points.Length];
			//latticesPresistent = new LatticeInfo[lattices.Length];

			//points.CopyTo(pointsPersistent);
			//lattices.CopyTo(latticesPresistent);

			Debug.Log($"Executed tree level {currentTreeLevel}.");
		}

		private void EvaluateSegment(NativeArray<Point> points, NativeArray<LatticeInfo> lattices, out NativeArray<Point> outputPoints, out NativeArray<LatticeInfo> outputLattices, ref float averageEnergy, ref int bestEnergy, ref int averageEnergyWeight)
		{
			// Generate children
			NativeArray<Point> childPoints = new NativeArray<Point>(lattices.Length * parsedInputPersistent.Length * 4, Allocator.TempJob);
			NativeArray<LatticeInfo> childLattices = new NativeArray<LatticeInfo>(lattices.Length * 4, Allocator.TempJob);
			NativeArray<bool> parsedInput = new NativeArray<bool>(parsedInputPersistent, Allocator.TempJob);
			Debug.Log($"Generating child lattices: {childLattices.Length}.");
			GenerateChildLattices(childPoints, childLattices, parsedInput);


			// Calculate average energy
			NativeArray<float> averageEnergyNative = new NativeArray<float>(1, Allocator.TempJob);
			NativeArray<int> averageEnergyWeightNative = new NativeArray<int>(1, Allocator.TempJob);
			NativeArray<int> bestEnergyNative = new NativeArray<int>(1, Allocator.TempJob);

			averageEnergyNative[0] = averageEnergy;
			averageEnergyWeightNative[0] = averageEnergyWeight;
			bestEnergyNative[0] = bestEnergy;

			CalculateAverageEnergy(childLattices, averageEnergyNative, averageEnergyWeightNative, bestEnergyNative);

			averageEnergy = averageEnergyNative[0];
			averageEnergyWeight = averageEnergyWeightNative[0];
			bestEnergy = bestEnergyNative[0];


			// Generate indices filter
			NativeArray<float> randomValues = new NativeArray<float>(childLattices.Length, Allocator.TempJob);
			GenerateRandomElements(ref randomValues);
			NativeList<int> indices = new NativeList<int>(childLattices.Length, Allocator.TempJob);
			GenerateIndicesFilter(ref childLattices, averageEnergyNative[0], bestEnergyNative[0], indices, randomValues);


			// Filter data
			outputPoints = new NativeArray<Point>(parsedInput.Length * indices.Length, Allocator.Persistent);
			outputLattices = new NativeArray<LatticeInfo>(indices.Length, Allocator.Persistent);
			FilterIndices(childPoints, childLattices, ref indices, outputPoints, outputLattices);
		}
		#endregion


		#region Jobs
		private void FilterIndices(NativeArray<Point> childPoints, NativeArray<LatticeInfo> childLattices, ref NativeList<int> indices, NativeArray<Point> outputPoints, NativeArray<LatticeInfo> outputLattices)
		{
			JobHandle jobHandle;
			LatticeFilteredCopyJob filteredCopyJob = new LatticeFilteredCopyJob()
			{
				points = childPoints,
				lattices = childLattices,
				indices = indices,
				singleLatticePointsCount = parsedInputPersistent.Length,
				outputPoints = outputPoints,
				outputLattices = outputLattices
			};
			jobHandle = filteredCopyJob.Schedule(indices.Length, 1);
			jobHandle.Complete();
		}
		private void GenerateChildLattices(NativeArray<Point> childPoints, NativeArray<LatticeInfo> childLattices, NativeArray<bool> parsedInput)
		{
			LatticeJob initialLatticeJob = new LatticeJob()
			{
				points = points,
				lattices = lattices,
				parsedInput = parsedInput,
				size = latticeSize,
				nextIsHydrophobic = parsedInput[currentTreeLevel],
				outputPoints = childPoints,
				outputLattices = childLattices,
				currentProteinStringIndex = currentTreeLevel
			};

			JobHandle jobHandle = initialLatticeJob.Schedule(lattices.Length, 100);
			jobHandle.Complete();
		}
		private void GenerateIndicesFilter(ref NativeArray<LatticeInfo> childLattices, float averageEnergy, int bestEnergy, NativeList<int> indices, NativeArray<float> randomValues)
		{
			JobHandle jobHandle;
			LatticeFilterJob latticeFilterJob = new LatticeFilterJob()
			{
				lattices = childLattices,
				averageEnergy = averageEnergy,
				bestEnergy = bestEnergy,
				random = randomValues,
				pruningProbability01 = pruningProbability01.Value,
				pruningProbability02 = pruningProbability02.Value,
				lastPointIsHydrophobic = parsedInputPersistent[currentTreeLevel],
			};
			jobHandle = latticeFilterJob.ScheduleAppend(indices, childLattices.Length, 1);
			jobHandle.Complete();

			Debug.Log($"Generated filtered indices - filtered {childLattices.Length - indices.Length}/{childLattices.Length}.");
		}
		private static void CalculateAverageEnergy(NativeArray<LatticeInfo> childLattices, NativeArray<float> averageEnergy, NativeArray<int> averageEnergyWeight, NativeArray<int> bestEnergy)
		{
			LatticesCalculateAverageEnergyJob latticesCalculateAverageEnergyJob = new LatticesCalculateAverageEnergyJob()
			{
				lattices = childLattices,
				averageEnergy = averageEnergy,
				averageEnergyWeight = averageEnergyWeight,
				bestEnergy = bestEnergy
			};
			latticesCalculateAverageEnergyJob.Schedule().Complete();

			Debug.Log($"Calculated average energy: {averageEnergy[0]}, bestEnergy: {bestEnergy[0]}.");
		}
		#endregion


		#region Random values generation
		[ContextMenu("Regenerate random values array")]
		public void RegenerateRandomValuesArray()
		{
			// Generate random pruning values
			NativeArray<float> randomValues = new NativeArray<float>(10000, Allocator.TempJob);
			GenerateRandomElements(ref randomValues);

			randomValuesPersistent = new float[10000];
			randomValues.CopyTo(randomValuesPersistent);

			randomValues.Dispose();
		}
		public static void GenerateRandomElements(ref NativeArray<float> elements)
		{
			GenerateRandomElementsJob generateRandomElementsJob = new GenerateRandomElementsJob()
			{
				random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(0, 645241)),
				randomElements = elements
			};
			generateRandomElementsJob.Schedule().Complete();
		}
		#endregion


		#region Output
		[ContextMenu("Output best lattice")]
		private void CreateOutputLattice()
		{
			LatticeInfo bestLatticeInfo = new LatticeInfo();
			int bestIndex = 0;
			for (int i = 0; i < lattices.Length; i++)
			{
				if (lattices[i].energy < bestLatticeInfo.energy)
				{
					bestLatticeInfo = lattices[i];
					bestIndex = i;
				}
			}

			OutputLattice(bestIndex);
		}
		private void OutputLattice(int index)
		{
			outputLatticeIndex = index;

			outputLattice.Value = new Lattice(
							points.GetSubArray(parsedInputPersistent.Length * index, parsedInputPersistent.Length).ToArray(),
							parsedInputPersistent,
							lattices[index].energy,
							latticeSize);
		}

		[ContextMenu("Output next lattice")]
		public void OutputNext()
		{
			OutputLattice(++outputLatticeIndex);
		}
		[ContextMenu("Output prev lattice")]
		public void OutputPrevious()
		{
			OutputLattice(--outputLatticeIndex);
		}
		#endregion


		#region Parsing input
		private List<bool> ParseInput()
		{
			List<bool> result = new List<bool>();

			string cleanedUpInput = inputString.Value.Trim().ToLower();
			foreach (char ch in cleanedUpInput)
			{
				if (ch == 'h') result.Add(true);
				if (ch == 'p') result.Add(false);
			}

			return result;
		}
		#endregion
	}
}
