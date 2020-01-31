using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatticeVisualizer : DataSource
{
	[Header("Variables")]
	public LatticeReference lattice;
	public FloatReference gridSpacing;

	[Header("Data Keys")]
	public DataKey pointEnabledKey;
	public DataKey pointIsPolarKey;
	public DataKey rightBindingEnabledKey;
	public DataKey downwardsBindingEnabledKey;
	public DataKey pointPositionKey;

	protected override void ApplySet()
	{
		DataBundleCollection dataBundleCollection = new DataBundleCollection();

		for (int i = 0; i < lattice.Value.points.GetLength(0); i++)
		{
			for (int j = 0; j < lattice.Value.points.GetLength(1); j++)
			{
				DataBundle pointDataBundle = DataBundle.New((i * lattice.Value.Size + j).ToString());

				pointDataBundle.Set(pointEnabledKey, BoolVariable.New(lattice.Value.IsOccupied(i, j)));
				pointDataBundle.Set(pointIsPolarKey, BoolVariable.New(lattice.Value.IsHydrophobic(i, j) == false));
				pointDataBundle.Set(rightBindingEnabledKey, BoolVariable.New(lattice.Value.HasRightBinding(i, j)));
				pointDataBundle.Set(downwardsBindingEnabledKey, BoolVariable.New(lattice.Value.HasDownBinding(i, j)));
				pointDataBundle.Set(pointPositionKey, Vector3Variable.New(new Vector3(i * gridSpacing, j * gridSpacing, 0)));

				dataBundleCollection.Add(pointDataBundle);
			}
		}

		Output(dataBundleCollection);
	}
}
