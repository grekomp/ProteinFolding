using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace ProteinFolding
{
	[BurstCompile]
	public struct LatticeJob : IJobParallelFor
	{
		// Inputs
		[ReadOnly]
		public NativeArray<Point> points;
		[ReadOnly]
		public NativeArray<LatticeInfo> lattices;
		[ReadOnly]
		public NativeArray<bool> parsedInput;

		public int size;
		public int executionIndex;
		public bool nextIsHydrophobic;
		public int currentProteinStringIndex;

		// Runtime variables
		private int baseIndex;
		private int outputBaseIndex;
		private int outputLatticeIndex;

		// Outputs
		[NativeDisableParallelForRestriction]
		public NativeArray<Point> outputPoints;

		[NativeDisableParallelForRestriction]
		public NativeArray<LatticeInfo> outputLattices;


		public void Execute(int index)
		{
			executionIndex = index;
			baseIndex = executionIndex * parsedInput.Length;
			outputBaseIndex = executionIndex * parsedInput.Length * 4;
			outputLatticeIndex = executionIndex * 4;

			TryPlacePoint(nextIsHydrophobic, Direction.Up);
			outputBaseIndex += parsedInput.Length;
			outputLatticeIndex++;
			TryPlacePoint(nextIsHydrophobic, Direction.Right);
			outputBaseIndex += parsedInput.Length;
			outputLatticeIndex++;
			TryPlacePoint(nextIsHydrophobic, Direction.Down);
			outputBaseIndex += parsedInput.Length;
			outputLatticeIndex++;
			TryPlacePoint(nextIsHydrophobic, Direction.Left);
		}


		#region Placing points
		public void TryPlacePoint(bool isHydrophobic, Direction direction)
		{
			// Load lattice info
			LatticeInfo outputLattice = lattices[executionIndex];

			// Check if position is free
			int lastIndex = points[baseIndex + currentProteinStringIndex - 1].conformationIndex;
			int adjacentIndex = GetAdjacentIndex(lastIndex, direction);
			if (IsOccupied(adjacentIndex))
			{
				OutputLattice(new LatticeInfo(false, 0));
				return;
			}

			// Copy points to output
			CopyPoints(baseIndex, outputBaseIndex);

			// Place point
			outputPoints[outputBaseIndex + currentProteinStringIndex] = new Point((short)adjacentIndex);

			// Update lattice info
			outputLattice.energy += GetOutputEnergyPoint(adjacentIndex, isHydrophobic);

			// Add new lattice info to output
			outputLattices[outputLatticeIndex] = outputLattice;
		}

		private void OutputLattice(LatticeInfo latticeInfo)
		{
			outputLattices[outputLatticeIndex] = latticeInfo;
		}

		private bool IsOccupied(int originalPointIndex)
		{
			return FindPointWithIndex(originalPointIndex).point.conformationIndex > 0;
		}
		#endregion


		#region Coords manipulation methods
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Index(int baseIndex, int x, int y)
		{
			return (baseIndex * size + y) * size + x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetAdjacentIndex(int conformationIndex, Direction direction)
		{
			conformationIndex += direction == Direction.Left ? -1 : 0;
			conformationIndex += direction == Direction.Right ? 1 : 0;

			conformationIndex += direction == Direction.Up ? -size : 0;
			conformationIndex += direction == Direction.Down ? size : 0;

			return conformationIndex;
		}
		#endregion


		#region Energy calculations
		public int GetOutputEnergyPoint(int conformationIndex, bool isHydrophobic)
		{
			int pointEnergy = 0;
			pointEnergy += GetOutputEnergySingleDirection(conformationIndex, Direction.Up, isHydrophobic);
			pointEnergy += GetOutputEnergySingleDirection(conformationIndex, Direction.Right, isHydrophobic);
			pointEnergy += GetOutputEnergySingleDirection(conformationIndex, Direction.Down, isHydrophobic);
			pointEnergy += GetOutputEnergySingleDirection(conformationIndex, Direction.Left, isHydrophobic);
			return pointEnergy;
		}
		public int GetOutputEnergySingleDirection(int conformationIndex, Direction direction, bool isHydrophobic)
		{
			if (isHydrophobic == false) return 0;

			int adjacentIndex = GetAdjacentIndex(conformationIndex, direction);
			IndexedPoint adjacentPoint = FindPointWithIndex(adjacentIndex);
			if (adjacentPoint.point.conformationIndex > 0 && IsHydrophobic(adjacentPoint.proteinStringIndex) && currentProteinStringIndex - adjacentPoint.proteinStringIndex > 1) return -1;

			return 0;
		}
		#endregion


		#region Data access helper methods
		public bool IsHydrophobic(int proteinStringIndex)
		{
			return parsedInput[proteinStringIndex];
		}

		public IndexedPoint FindPointWithIndex(int conformationIndex)
		{
			for (int i = 0; i < currentProteinStringIndex; i++)
			{
				if (points[baseIndex + i].conformationIndex == conformationIndex)
				{
					return new IndexedPoint(points[baseIndex + i], i);
				}
			}

			return new IndexedPoint();
		}
		#endregion


		#region Copying points
		private void CopyPoints(int inputBaseIndex, int outputBaseIndex)
		{
			points.GetSubArray(inputBaseIndex, parsedInput.Length).CopyTo(outputPoints.GetSubArray(outputBaseIndex, parsedInput.Length));
		}
		#endregion
	}
}
