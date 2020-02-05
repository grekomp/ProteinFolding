using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatticeVisualizer : ValueSetter
{
	[Header("Components")]
	public RectTransform gridRect;

	[Header("Options")]
	public LatticeReference lattice;
	public FloatReference gridSpacing;

	[Space]
	public LatticePointVisual latticePointPrefab;

	[Header("Runtime Variables")]
	public LatticePointVisual[,] spawnedPoints;

	protected override void Init()
	{
		gridRect = gridRect ?? GetComponent<RectTransform>();
	}

	protected override void ApplySet()
	{
		if (IsCurrentLatticeVisualizationValid() == false) GenerateBaseLatticeVisualization();
		if (lattice.Value == null) return;

		gridSpacing.Value = gridRect.rect.width / (lattice.Value.Size - 1);

		for (int x = 0; x < lattice.Value.Size; x++)
		{
			for (int y = 0; y < lattice.Value.Size; y++)
			{
				spawnedPoints[x, y].x = x;
				spawnedPoints[x, y].y = y;
				spawnedPoints[x, y].gridSpacing = gridSpacing;
				spawnedPoints[x, y].UpdatePositionSize();

				spawnedPoints[x, y].IsActive = lattice.Value.IsOccupied(x, y);
				spawnedPoints[x, y].IsHydrophobic = lattice.Value.IsHydrophobic(x, y);
				spawnedPoints[x, y].BindingDirection = lattice.Value.BindingDirection(x, y);
			}
		}
	}


	private bool IsCurrentLatticeVisualizationValid()
	{
		if (lattice.Value == null && spawnedPoints != null) return false;
		if (lattice.Value != null && spawnedPoints == null) return false;

		if (lattice.Value == null && spawnedPoints == null) return true;
		if (lattice.Value.Size != spawnedPoints.GetLength(0)) return false;

		return true;
	}
	protected void GenerateBaseLatticeVisualization()
	{
		if (lattice.Value == null)
		{
			if (spawnedPoints == null) return;
			CleanUpSpawnedPoints();
			return;
		}

		spawnedPoints = new LatticePointVisual[lattice.Value.Size, lattice.Value.Size];

		for (int x = 0; x < spawnedPoints.GetLength(0); x++)
		{
			for (int y = 0; y < spawnedPoints.GetLength(1); y++)
			{
				spawnedPoints[x, y] = Instantiate(latticePointPrefab, transform) as LatticePointVisual;
			}
		}
	}

	private void CleanUpSpawnedPoints()
	{
		for (int x = 0; x < spawnedPoints.GetLength(0); x++)
		{
			for (int y = 0; y < spawnedPoints.GetLength(1); y++)
			{
				if (spawnedPoints[x, y]) Destroy(spawnedPoints[x, y]);
			}
		}

		spawnedPoints = null;
	}
}
