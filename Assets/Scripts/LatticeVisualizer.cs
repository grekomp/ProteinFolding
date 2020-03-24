using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProteinFolding
{

	public class LatticeVisualizer : ValueSetter
	{
		[Header("Components")]
		public RectTransform gridRect;

		[Header("Options")]
		[HandleChanges] public LatticeReference lattice;
		public FloatReference gridSpacing;
		public FloatReference minUpdateInterval;

		[Space]
		public LatticePointVisual latticePointPrefab;

		[Header("Runtime Variables")]
		public List<LatticePointVisual> spawnedPoints = new List<LatticePointVisual>();
		public DateTime lastUpdateTime = new DateTime(2000, 1, 1);

		protected override void Init()
		{
			gridRect = gridRect ?? GetComponent<RectTransform>();
		}

		protected override void ApplySet()
		{
			if ((DateTime.Now - lastUpdateTime).TotalSeconds < minUpdateInterval) return;
			lastUpdateTime = DateTime.Now;

			if (lattice.Value == null || lattice.Value.size == 0) return;

			DisplayLatticeSubset(lattice.Value.minOccupiedX, lattice.Value.minOccupiedY, lattice.Value.maxSpanXY);
		}
		private bool IsCurrentLatticeVisualizationValid()
		{
			if (lattice.Value == null) return spawnedPoints == null;
			if (lattice.Value.size == 0 && spawnedPoints != null) return false;
			if (lattice.Value.size > 0 && spawnedPoints == null) return false;

			if (lattice.Value.size == 0 && spawnedPoints == null) return true;
			if (lattice.Value.size * lattice.Value.size != spawnedPoints.Count) return false;

			foreach (var spawnedPoint in spawnedPoints)
			{
				if (spawnedPoint == null) return false;
			}

			return true;
		}


		#region Lattice subset display
		public void DisplayLatticeSubset(int minX, int minY, int size)
		{
			GenerateSpawnedPoints(size);

			gridSpacing.Value = gridRect.rect.width / size;

			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					LatticePointVisual point = spawnedPoints[Index(y, x, size)];

					point.x = x;
					point.y = y;

					point.debugOriginalX = x + minX;
					point.debugOriginalY = y + minY;

					point.gridSpacing = gridSpacing;
					point.UpdatePositionSize();

					point.IsActive = lattice.Value.IsOccupied(x + minX, y + minY);
					point.IsHydrophobic = lattice.Value.IsHydrophobic(x + minX, y + minY);
					point.BindingDirection = lattice.Value.BindingDirection(x + minX, y + minY);

					point.value = lattice.Value.GetPoint(x + minX, y + minY).point.conformationIndex;
					point.index = lattice.Value.Index(x + minX, y + minY);
				}
			}
		}
		#endregion


		#region Spawning points
		private void GenerateSpawnedPoints(int size)
		{
			if (size <= 0)
			{
				CleanUpSpawnedPoints();
				return;
			}

			int sizeDelta = size * size - spawnedPoints.Count;

			if (sizeDelta < 0)
			{
				DespawnPoints(-sizeDelta);
			}

			if (sizeDelta > 0)
			{
				SpawnPoints(sizeDelta);
			}
		}

		private void SpawnPoints(int numPointsToSpawn)
		{
			for (int i = 0; i < numPointsToSpawn; i++)
			{
				spawnedPoints.Add(Instantiate(latticePointPrefab, transform));
			}
		}
		private void DespawnPoints(int numPointsToRemove)
		{
			for (int i = 0; i < numPointsToRemove; i++)
			{
				var pointToRemove = spawnedPoints.Last();

				if (pointToRemove)
				{
					spawnedPoints.Remove(pointToRemove);
					Destroy(pointToRemove.gameObject);
				}
				else
				{
					spawnedPoints.RemoveAt(spawnedPoints.Count - 1);
				}
			}
		}
		private void CleanUpSpawnedPoints()
		{
			if (spawnedPoints == null) return;

			foreach (var spawnedPoint in spawnedPoints)
			{
				if (spawnedPoint) Destroy(spawnedPoint.gameObject);
			}
			spawnedPoints = null;
		}

		private int Index(int y, int x, int size)
		{
			return x + y * size;
		}
		#endregion


		#region Debug		
		//[ContextMenu("Test")]
		//public void Test()
		//{
		//	lattice.Value = new Lattice(5);
		//	lattice.Value.PlaceInitPoint(true);
		//	lattice.Value.PlacePoint(false, Direction.Up);
		//	lattice.Value.PlacePoint(true, Direction.Right);
		//	lattice.Value.PlacePoint(true, Direction.Down);
		//	lattice.Value.PlacePoint(false, Direction.Right);

		//	Set();
		//}
		#endregion
	}
}
