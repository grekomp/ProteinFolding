using System;
using System.Collections;
using System.Collections.Generic;
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

		[Space]
		public LatticePointVisual latticePointPrefab;

		[Header("Runtime Variables")]
		public LatticePointVisual[] spawnedPoints;

		protected override void Init()
		{
			gridRect = gridRect ?? GetComponent<RectTransform>();
			GenerateBaseLatticeVisualization();
		}

		protected override void ApplySet()
		{
			Debug.Log(lattice.Value);

			if (IsCurrentLatticeVisualizationValid() == false) GenerateBaseLatticeVisualization();
			if (lattice.Value.size == 0) return;

			gridSpacing.Value = gridRect.rect.width / lattice.Value.size;

			for (int x = 0; x < lattice.Value.size; x++)
			{
				for (int y = 0; y < lattice.Value.size; y++)
				{
					int i = Index(y, x);

					spawnedPoints[i].x = x;
					spawnedPoints[i].y = y;
					spawnedPoints[i].gridSpacing = gridSpacing;
					spawnedPoints[i].UpdatePositionSize();

					spawnedPoints[i].IsActive = lattice.Value.IsOccupied(x, y);
					spawnedPoints[i].IsHydrophobic = lattice.Value.GetPoint(x, y).isHydrophobic;
					spawnedPoints[i].BindingDirection = lattice.Value.BindingDirection(x, y);

					spawnedPoints[i].value = lattice.Value.GetPoint(x, y).index;
					spawnedPoints[i].index = lattice.Value.Index(x, y);
				}
			}
		}
		private bool IsCurrentLatticeVisualizationValid()
		{
			if (lattice.Value.size == 0 && spawnedPoints != null) return false;
			if (lattice.Value.size > 0 && spawnedPoints == null) return false;

			if (lattice.Value.size == 0 && spawnedPoints == null) return true;
			if (lattice.Value.size * lattice.Value.size != spawnedPoints.Length) return false;

			foreach (var spawnedPoint in spawnedPoints)
			{
				if (spawnedPoint == null) return false;
			}

			return true;
		}
		protected void GenerateBaseLatticeVisualization()
		{
			CleanUpSpawnedPoints();

			if (lattice.Value.size == 0)
			{
				if (spawnedPoints == null) return;
				return;
			}

			spawnedPoints = new LatticePointVisual[lattice.Value.size * lattice.Value.size];

			for (int y = 0; y < lattice.Value.size; y++)
			{
				for (int x = 0; x < lattice.Value.size; x++)
				{
					spawnedPoints[Index(y, x)] = Instantiate(latticePointPrefab, transform);
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

		private int Index(int y, int x)
		{
			return x + y * lattice.Value.size;
		}


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
