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
		public int energyValuesCount;
		public int bestEnergyValue;
		public float averageEnergyValue;
		public LatticeReference outputLattice;
		[Space]
		public List<bool> parsedInput;


		[Header("Runtime Variables")]
		Coroutine runningExecution;
		public int latticeSize = 0;

		public NativeArray<Point> points;
		public NativeArray<LatticeInfo> lattices;

		public int currentTreeLevel = 0;

		public int prunedTrees01 = 0;
		public int prunedTrees02 = 0;


		#region Executing simulation
		[ContextMenu("Start Execution")]
		public void StartExecution()
		{
			// Parse input
			parsedInput = ParseInput();

			// Init
			energyValuesCount = 0;
			bestEnergyValue = 0;
			averageEnergyValue = 0;
			prunedTrees01 = 0;
			prunedTrees02 = 0;

			// Start coroutine
			Debug.Log("Started execution");
			if (runningExecution != null) StopCoroutine(runningExecution);
			runningExecution = StartCoroutine(Execute());
		}

		public IEnumerator Execute()
		{
			if (parsedInput.Count > 2)
			{
				InitializeExecution();

				// Place the rest of points
				currentTreeLevel = 2;
				while (currentTreeLevel < parsedInput.Count)
				{
					ExecuteTreeLevel();

					currentTreeLevel++;
					yield return null;
				}

				CreateOutputLattice();
			}

			yield return null;
		}

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

			outputLattice.Value = new Lattice(
				points.GetSubArray(latticeSize * latticeSize

				* bestIndex, latticeSize * latticeSize).ToArray(),
				bestLatticeInfo.energy,
				latticeSize);

			points.Dispose();
			lattices.Dispose();
		}

		private void InitializeExecution()
		{
			// Create initial lattice
			latticeSize = 2 * parsedInput.Count + 2;

			points = new NativeArray<Point>(latticeSize * latticeSize, Allocator.Persistent);
			lattices = new NativeArray<LatticeInfo>(1, Allocator.Persistent);

			// Place first two monomers arbitrarily
			int initialIndex = latticeSize * ((latticeSize - 1) / 2) + (latticeSize / 2);
			points[initialIndex] = new Point(2, parsedInput[0]);
			points[initialIndex + 1] = new Point(3, parsedInput[1]);
			lattices[0] = new LatticeInfo(true, 0, 3, initialIndex + 1);
		}
		private void ExecuteTreeLevel()
		{
			// Generate children
			NativeArray<Point> childPoints = new NativeArray<Point>(lattices.Length * latticeSize * latticeSize * 4, Allocator.TempJob);
			NativeArray<LatticeInfo> childLattices = new NativeArray<LatticeInfo>(lattices.Length * 4, Allocator.TempJob);
			Debug.Log($"Generating child lattices: {childLattices.Length}.");
			GenerateChildLattices(childPoints, childLattices);

			points.Dispose();
			lattices.Dispose();

			// Calculate average energy
			NativeArray<float> averageEnergy = new NativeArray<float>(1, Allocator.TempJob);
			NativeArray<int> bestEnergy = new NativeArray<int>(1, Allocator.TempJob);
			CalculateAverageEnergy(childLattices, averageEnergy, bestEnergy);

			// Generate random pruning values
			NativeArray<float> randomValues = new NativeArray<float>(childLattices.Length, Allocator.TempJob);
			GenerateRandomElements(ref randomValues);

			// Generate filtered indices
			NativeList<int> indices = new NativeList<int>(childLattices.Length, Allocator.TempJob);
			GenerateIndicesFilter(ref childLattices, averageEnergy[0], bestEnergy[0], indices, randomValues);

			// Generate filtered output 
			points = new NativeArray<Point>(latticeSize * latticeSize * indices.Length, Allocator.TempJob);
			lattices = new NativeArray<LatticeInfo>(indices.Length, Allocator.TempJob);
			FilterIndices(childPoints, childLattices, ref indices);

			bestEnergy.Dispose();
			averageEnergy.Dispose();
			indices.Dispose();
			childPoints.Dispose();
			childLattices.Dispose();
			randomValues.Dispose();

			Debug.Log($"Executed tree level {currentTreeLevel}.");
		}

		private void FilterIndices(NativeArray<Point> childPoints, NativeArray<LatticeInfo> childLattices, ref NativeList<int> indices)
		{
			JobHandle jobHandle;
			LatticeFilteredCopyJob filteredCopyJob = new LatticeFilteredCopyJob()
			{
				points = childPoints,
				lattices = childLattices,
				indices = indices,
				size = latticeSize,
				outputPoints = points,
				outputLattices = lattices
			};
			jobHandle = filteredCopyJob.Schedule(indices.Length, 1);
			jobHandle.Complete();
		}


		private void GenerateChildLattices(NativeArray<Point> childPoints, NativeArray<LatticeInfo> childLattices)
		{
			LatticeJob initialLatticeJob = new LatticeJob()
			{
				points = points,
				lattices = lattices,
				size = latticeSize,
				nextIsHydrophobic = parsedInput[currentTreeLevel],
				outputPoints = childPoints,
				outputLattices = childLattices
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
				pruningProbability02 = pruningProbability02.Value
			};
			jobHandle = latticeFilterJob.ScheduleAppend(indices, childLattices.Length, 1);
			jobHandle.Complete();

			Debug.Log($"Generated filtered indices - filtered {childLattices.Length - indices.Length}/{childLattices.Length}.");
		}
		private static void CalculateAverageEnergy(NativeArray<LatticeInfo> childLattices, NativeArray<float> averageEnergy, NativeArray<int> bestEnergy)
		{
			JobHandle jobHandle;
			LatticesCalculateAverageEnergyJob latticesCalculateAverageEnergyJob = new LatticesCalculateAverageEnergyJob()
			{
				lattices = childLattices,
				averageEnergy = averageEnergy,
				bestEnergy = bestEnergy
			};
			jobHandle = latticesCalculateAverageEnergyJob.Schedule();
			jobHandle.Complete();

			Debug.Log($"Calculated average energy: {averageEnergy[0]}, bestEnergy: {bestEnergy[0]}.");
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
