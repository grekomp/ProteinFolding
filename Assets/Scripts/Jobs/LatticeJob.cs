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
			baseIndex = executionIndex * size * size;
			outputBaseIndex = executionIndex * size * size * 4;
			outputLatticeIndex = executionIndex * 4;

			TryPlacePoint(nextIsHydrophobic, Direction.Up);
			outputBaseIndex += size * size;
			outputLatticeIndex++;
			TryPlacePoint(nextIsHydrophobic, Direction.Right);
			outputBaseIndex += size * size;
			outputLatticeIndex++;
			TryPlacePoint(nextIsHydrophobic, Direction.Down);
			outputBaseIndex += size * size;
			outputLatticeIndex++;
			TryPlacePoint(nextIsHydrophobic, Direction.Left);
		}


		#region Placing points
		public void TryPlacePoint(bool isHydrophobic, Direction direction)
		{
			// Load lattice info
			LatticeInfo outputLattice = lattices[executionIndex];
			int adjacentIndex = GetAdjacentIndex(outputLattice.lastIndex, direction);

			// Validate the placement is correct
			int originalPointIndex = baseIndex + adjacentIndex;
			if (IsOccupied(originalPointIndex))
			{
				OutputLattice(new LatticeInfo(false, 0, outputLattice.lastPoint + 1, adjacentIndex));
				return;
			}

			// Copy points to output
			CopyPoints(baseIndex, outputBaseIndex);

			// Place point
			int newPointIndex = outputBaseIndex + adjacentIndex;
			outputPoints[newPointIndex] = new Point((byte)++outputLattice.lastPoint);

			// Update lattice info
			outputLattice.lastIndex = adjacentIndex;
			outputLattice.energy += GetOutputEnergyPoint(newPointIndex, isHydrophobic);

			// Add new lattice info to output
			outputLattices[outputLatticeIndex] = outputLattice;
		}

		private void OutputLattice(LatticeInfo latticeInfo)
		{
			outputLattices[outputLatticeIndex] = latticeInfo;
		}

		private bool IsOccupied(int originalPointIndex)
		{
			return points[originalPointIndex].index > 0;
		}
		#endregion


		#region Coords manipulation methods
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Index(int baseIndex, int x, int y)
		{
			return (baseIndex * size + y) * size + x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetAdjacentIndex(int index, Direction direction)
		{
			index += direction == Direction.Left ? -1 : 0;
			index += direction == Direction.Right ? 1 : 0;

			index += direction == Direction.Up ? -size : 0;
			index += direction == Direction.Down ? size : 0;

			return index;
		}


		public int GetOutputEnergyPoint(int index, bool isHydrophobic)
		{
			int pointEnergy = 0;
			pointEnergy += GetOutputEnergySingleDirection(index, Direction.Up, isHydrophobic);
			pointEnergy += GetOutputEnergySingleDirection(index, Direction.Right, isHydrophobic);
			pointEnergy += GetOutputEnergySingleDirection(index, Direction.Down, isHydrophobic);
			pointEnergy += GetOutputEnergySingleDirection(index, Direction.Left, isHydrophobic);
			return pointEnergy;
		}
		public int GetOutputEnergySingleDirection(int index, Direction direction, bool isHydrophobic)
		{
			if (isHydrophobic == false) return 0;

			int adjacentIndex = GetAdjacentIndex(index, direction);
			Point point = outputPoints[index];
			Point adjacentPoint = outputPoints[adjacentIndex];
			if (adjacentPoint.index > 0 && IsHydrophobic(adjacentPoint) && Math.Abs(point.index - adjacentPoint.index) > 1) return -1;

			return 0;
		}
		#endregion


		#region Data access helper methods
		public bool IsHydrophobic(int index)
		{
			return parsedInput[index - 2];
		}
		public bool IsHydrophobic(Point point)
		{
			return IsHydrophobic(point.index);
		}
		#endregion


		#region Copying points
		private void CopyPoints(int inputBaseIndex, int outputBaseIndex)
		{
			points.GetSubArray(inputBaseIndex, size * size).CopyTo(outputPoints.GetSubArray(outputBaseIndex, size * size));
		}
		#endregion
	}
}
