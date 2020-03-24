using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		public DoubleReference pruningProbability01;
		public DoubleReference pruningProbability02;
		[Space]
		public IntReference maxPointsInDataSegment;
		public IntReference maxFrameTimeMs;
		public IntReference performanceMeasurementInterval = new IntReference(100);


		[Header("Output Variables")]
		public IntReference outputEnergy;
		public LatticeReference outputLattice;
		[Space]
		public DoubleReference executionTimeSeconds;
		public BoolReference isSimulationRunning;


		[Header("Runtime Variables")]
		[SerializeField] [Disabled] protected int latticeSize = 0;
		[SerializeField] [Disabled] protected long processedLattices = 0;
		public LongReference latticesPerSecond;

		public float[] randomValuesPersistent;

		public Stack<SimulationDataSegment> dataSegments = new Stack<SimulationDataSegment>();
		public SimulationTreeLevel[] treeLevels;

		protected bool[] parsedInputPersistent;
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
		[ContextMenu("Stop Execution")]
		public void StopExecution()
		{
			if (runningExecution != null)
			{
				Debug.Log("Interrupting execution");
				StopCoroutine(runningExecution);
				isSimulationRunning.Value = false;
			}
			else
			{
				Debug.Log("Cannot stop execution - simulation is already stopped.");
			}
		}

		public IEnumerator Execute()
		{
			if (parsedInputPersistent.Length > 2)
			{
				InitializeSimulation();


				System.Diagnostics.Stopwatch fullExecutionTime = new System.Diagnostics.Stopwatch();
				long lastMeasurementTime = 0;
				long lastMeasuredProcessedLatticesCount = 0;
				fullExecutionTime.Start();
				isSimulationRunning.Value = true;


				while (dataSegments.Count > 0)
				{
					// Measure performance
					if (fullExecutionTime.ElapsedMilliseconds >= lastMeasurementTime + performanceMeasurementInterval)
					{
						int smoothingWeight = 4;
						latticesPerSecond.Value = (smoothingWeight * latticesPerSecond + ((processedLattices - lastMeasuredProcessedLatticesCount) * 1000 / performanceMeasurementInterval)) / (smoothingWeight + 1);
						lastMeasuredProcessedLatticesCount = processedLattices;
						lastMeasurementTime = fullExecutionTime.ElapsedMilliseconds;

						executionTimeSeconds.Value = fullExecutionTime.Elapsed.TotalSeconds;
					}

					// Execute next portion of data segments
					System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
					stopWatch.Start();
					while (stopWatch.ElapsedMilliseconds < maxFrameTimeMs.Value && dataSegments.Count > 0)
					{
						ExecuteNextSegment();
					}
					stopWatch.Stop();

					yield return null;
				}

				fullExecutionTime.Stop();
				isSimulationRunning.Value = false;
				Debug.Log($"Execution finished, time elapsed: {fullExecutionTime.Elapsed}");
			}

			yield return null;
			CleanUpDataSegments();
		}

		private void InitializeSimulation()
		{
			outputEnergy.Value = 0;
			processedLattices = 0;
			outputLattice.Value = new Lattice(null, null, 0, 0);

			// Create initial lattice
			latticeSize = 2 * parsedInputPersistent.Length + 2;

			NativeArray<Point> points = new NativeArray<Point>(parsedInputPersistent.Length, Allocator.Persistent);
			NativeArray<LatticeInfo> lattices = new NativeArray<LatticeInfo>(1, Allocator.Persistent);

			treeLevels = new SimulationTreeLevel[parsedInputPersistent.Length + 1];

			for (int i = 0; i <= parsedInputPersistent.Length; i++)
			{
				treeLevels[i] = new SimulationTreeLevel(i);
			}


			// Place first two monomers arbitrarily
			int initialIndex = latticeSize * ((latticeSize - 1) / 2) + (latticeSize / 2);
			points[0] = new Point((short)initialIndex);
			points[1] = new Point((short)(initialIndex + 1));
			lattices[0] = new LatticeInfo(true, 0);

			// Create initial data segment
			dataSegments.Push(new SimulationDataSegment(points, lattices, treeLevels[2]));
		}
		private void ExecuteNextSegment()
		{
			SimulationDataSegment currentDataSegment = dataSegments.Pop();
			//Debug.Log($"Evaluating next segment, tree level: {currentDataSegment.treeLevel.levelNumber}.");
			if (currentDataSegment.treeLevel.levelNumber == parsedInputPersistent.Length)
			{
				FinalizeSegment(currentDataSegment);
			}
			else
			{
				EvaluateSegment(currentDataSegment);
			}
		}
		private void EvaluateSegment(SimulationDataSegment dataSegment)
		{
			// Generate children
			NativeArray<Point> childPoints = new NativeArray<Point>(dataSegment.lattices.Length * parsedInputPersistent.Length * 4, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<LatticeInfo> childLattices = new NativeArray<LatticeInfo>(dataSegment.lattices.Length * 4, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<bool> parsedInput = new NativeArray<bool>(parsedInputPersistent, Allocator.TempJob);
			JobHandle generateChildLatticesJobHandle = GenerateChildLattices(dataSegment.points, dataSegment.lattices, childPoints, childLattices, parsedInput, dataSegment.treeLevel.levelNumber);


			// Calculate average energy
			NativeArray<float> averageEnergyNative = new NativeArray<float>(1, Allocator.TempJob);
			NativeArray<int> averageEnergyWeightNative = new NativeArray<int>(1, Allocator.TempJob);
			NativeArray<int> bestEnergyNative = new NativeArray<int>(1, Allocator.TempJob);
			averageEnergyNative[0] = dataSegment.treeLevel.averageEnergy;
			averageEnergyWeightNative[0] = dataSegment.treeLevel.averageEnergyWeight;
			bestEnergyNative[0] = dataSegment.treeLevel.bestEnergy;
			JobHandle averageEnergyJobHandle = CalculateAverageEnergy(generateChildLatticesJobHandle, childLattices, averageEnergyNative, averageEnergyWeightNative, bestEnergyNative);


			// Generate indices filter
			NativeArray<float> randomValues = new NativeArray<float>(childLattices.Length, Allocator.TempJob);
			JobHandle generateRandomElementsJobHandle = GenerateRandomElementsArray(ref randomValues);
			NativeList<int> indices = new NativeList<int>(childLattices.Length, Allocator.TempJob);
			JobHandle generateIndicesFilterJobHandle = GenerateIndicesFilter(JobHandle.CombineDependencies(averageEnergyJobHandle, generateRandomElementsJobHandle), ref childLattices, averageEnergyNative, bestEnergyNative, indices, randomValues, dataSegment.treeLevel.levelNumber);
			generateIndicesFilterJobHandle.Complete();


			// Filter data
			NativeArray<Point> outputPoints = new NativeArray<Point>(parsedInput.Length * indices.Length, Allocator.Persistent);
			NativeArray<LatticeInfo> outputLattices = new NativeArray<LatticeInfo>(indices.Length, Allocator.Persistent);
			FilterIndices(childPoints, childLattices, ref indices, outputPoints, outputLattices);


			// Segmentalize output data
			SimulationTreeLevel nextTreeLevel = treeLevels[dataSegment.treeLevel.levelNumber + 1];
			SegmentalizeOutputData(outputPoints, outputLattices, nextTreeLevel);


			// Update treeLevel data
			dataSegment.treeLevel.averageEnergy = averageEnergyNative[0];
			dataSegment.treeLevel.averageEnergyWeight = averageEnergyWeightNative[0];
			dataSegment.treeLevel.bestEnergy = bestEnergyNative[0];


			// Update debug progress data
			checked
			{
				processedLattices += dataSegment.lattices.Length;
			}


			// Clean up
			childPoints.Dispose();
			childLattices.Dispose();
			parsedInput.Dispose();
			averageEnergyNative.Dispose();
			averageEnergyWeightNative.Dispose();
			bestEnergyNative.Dispose();
			randomValues.Dispose();
			indices.Dispose();
			dataSegment.Dispose();
			outputPoints.Dispose();
			outputLattices.Dispose();
		}
		private void FinalizeSegment(SimulationDataSegment dataSegment)
		{
			int bestLatticeIndex = FindBestLattice(dataSegment);
			if (outputLattice.Value == null || dataSegment.lattices[bestLatticeIndex].energy <= outputLattice.Value.energy)
			{
				OutputLattice(dataSegment, bestLatticeIndex);
			}

			dataSegment.Dispose();
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
		private JobHandle GenerateChildLattices(NativeArray<Point> points, NativeArray<LatticeInfo> lattices, NativeArray<Point> childPoints, NativeArray<LatticeInfo> childLattices, NativeArray<bool> parsedInput, int currentTreeLevel)
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

			return initialLatticeJob.Schedule(lattices.Length, 10);
		}
		private JobHandle GenerateIndicesFilter(JobHandle dependencies, ref NativeArray<LatticeInfo> childLattices, NativeArray<float> averageEnergy, NativeArray<int> bestEnergy, NativeList<int> indices, NativeArray<float> randomValues, int currentTreeLevel)
		{
			LatticeFilterJob latticeFilterJob = new LatticeFilterJob()
			{
				lattices = childLattices,
				averageEnergy = averageEnergy,
				bestEnergy = bestEnergy,
				random = randomValues,
				pruningProbability01 = (float)pruningProbability01.Value,
				pruningProbability02 = (float)pruningProbability02.Value,
				lastPointIsHydrophobic = parsedInputPersistent[currentTreeLevel],
			};
			return latticeFilterJob.ScheduleAppend(indices, childLattices.Length, 1, dependencies);
			//Debug.Log($"Generated filtered indices - filtered {childLattices.Length - indices.Length}/{childLattices.Length}.");
		}
		private static JobHandle CalculateAverageEnergy(JobHandle dependencies, NativeArray<LatticeInfo> childLattices, NativeArray<float> averageEnergy, NativeArray<int> averageEnergyWeight, NativeArray<int> bestEnergy)
		{
			LatticesCalculateAverageEnergyJob latticesCalculateAverageEnergyJob = new LatticesCalculateAverageEnergyJob()
			{
				lattices = childLattices,
				averageEnergy = averageEnergy,
				averageEnergyWeight = averageEnergyWeight,
				bestEnergy = bestEnergy
			};
			return latticesCalculateAverageEnergyJob.Schedule(dependencies);

			//Debug.Log($"Calculated average energy: {averageEnergy[0]}, bestEnergy: {bestEnergy[0]}.");
		}
		private void SegmentalizeOutputData(NativeArray<Point> unsegmentedPoints, NativeArray<LatticeInfo> unsegmentedLattices, SimulationTreeLevel nextTreeLevel)
		{
			// Calculate data segment lattices count
			int latticesInSingleSegment = maxPointsInDataSegment / latticeSize;
			int outputSegmentsCount = unsegmentedLattices.Length / latticesInSingleSegment;
			if (outputSegmentsCount * latticesInSingleSegment != unsegmentedLattices.Length) outputSegmentsCount++;

			NativeArray<JobHandle> copyJobHandles = new NativeArray<JobHandle>(outputSegmentsCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

			int latticesLeftToCopy = unsegmentedLattices.Length;
			for (int i = 0; i < outputSegmentsCount; i++)
			{
				int latticesInThisSegment = Math.Min(latticesInSingleSegment, latticesLeftToCopy);

				// Create segment
				SimulationDataSegment dataSegment = new SimulationDataSegment(new NativeArray<Point>(parsedInputPersistent.Length * latticesInThisSegment, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
																  new NativeArray<LatticeInfo>(latticesInThisSegment, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
																  nextTreeLevel);

				// Copy data
				CopySegmentDataJob copySegmentDataJob = new CopySegmentDataJob()
				{
					inputPoints = unsegmentedPoints,
					inputLattices = unsegmentedLattices,
					minIndex = i * latticesInSingleSegment,
					latticeSize = parsedInputPersistent.Length,
					latticesToCopy = latticesInThisSegment,
					outputPoints = dataSegment.points,
					outputLattices = dataSegment.lattices
				};
				copyJobHandles[i] = copySegmentDataJob.Schedule();

				// Schedule segment for execution
				dataSegments.Push(dataSegment);

				latticesLeftToCopy -= latticesInSingleSegment;
			}

			JobHandle.CompleteAll(copyJobHandles);
			copyJobHandles.Dispose();
		}
		public static JobHandle GenerateRandomElementsArray(ref NativeArray<float> elements)
		{
			GenerateRandomElementsJob generateRandomElementsJob = new GenerateRandomElementsJob()
			{
				random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(100, 645241)),
				randomElements = elements
			};
			return generateRandomElementsJob.Schedule();
		}
		#endregion


		#region Output
		private int FindBestLattice(SimulationDataSegment dataSegment)
		{
			LatticeInfo[] lattices = dataSegment.lattices.ToArray();

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

			return bestIndex;
		}

		[ContextMenu("Output best lattice")]
		private void CreateOutputLattice()
		{
			LatticeInfo bestLatticeInfo = new LatticeInfo();
			int bestIndex = 0;
			for (int i = 0; i < dataSegments.Peek().lattices.Length; i++)
			{
				if (dataSegments.Peek().lattices[i].energy < bestLatticeInfo.energy)
				{
					bestLatticeInfo = dataSegments.Peek().lattices[i];
					bestIndex = i;
				}
			}

			OutputLattice(dataSegments.Peek(), bestIndex);
		}
		private void OutputLattice(SimulationDataSegment dataSegment, int index)
		{
			outputLattice.Value = new Lattice(
							dataSegment.points.GetSubArray(parsedInputPersistent.Length * index, parsedInputPersistent.Length).ToArray(),
							parsedInputPersistent,
							dataSegment.lattices[index].energy,
							latticeSize);

			outputEnergy.Value = outputLattice.Value.energy;
		}
		#endregion


		#region Cleanup
		private void OnDestroy()
		{
			CleanUpDataSegments();
		}
		private void CleanUpDataSegments()
		{
			while (dataSegments.Count > 0)
			{
				dataSegments.Pop()?.Dispose();
			}
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
