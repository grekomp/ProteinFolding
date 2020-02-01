using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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

		public List<LatticePlacementProposition> placementPropositions = new List<LatticePlacementProposition>();
		public int currentMonomerIndex = 0;

		public int prunedTrees01 = 0;
		public int prunedTrees02 = 0;

		[ContextMenu("Execute")]
		public void Execute()
		{
			// Parse input
			parsedInput = ParseInput();


			// Init
			energyValuesCount = 0;
			bestEnergyValue = 1;
			averageEnergyValue = 0;
			prunedTrees01 = 0;
			prunedTrees02 = 0;
			placementPropositions.Clear();


			// Create initial lattice
			// Place first two monomers arbitrarily
			if (parsedInput.Count == 0) return;
			int latticeSize = 2 * parsedInput.Count + 2;
			Lattice initialLattice = new Lattice(latticeSize);
			initialLattice.PlaceInitPoint(parsedInput[0]);

			if (parsedInput.Count == 1) return;
			initialLattice.PlacePoint(parsedInput[1], Direction.Down);

			if (parsedInput.Count == 2) return;
			placementPropositions.AddRange(initialLattice.GetPlacementPropositions(parsedInput[2]));


			// Place the rest of points
			currentMonomerIndex = 2;

			Debug.Log("Started execution");
		}

		[ContextMenu("PlaceNext")]
		private async void PlaceNext()
		{
			if (currentMonomerIndex >= parsedInput.Count)
			{
				Debug.Log("All monomers placed");
				return;
			}

			bestEnergyValue = 1;
			averageEnergyValue = 0;
			energyValuesCount = 0;

			List<LatticePlacementProposition> lastPlacementPropositions = new List<LatticePlacementProposition>(placementPropositions);
			placementPropositions.Clear();
			foreach (var placementProposition in lastPlacementPropositions)
			{
				EvaluatePlacementProposition(placementProposition);
			}

			outputEnergy.Value = outputLattice.Value.energy;
			currentMonomerIndex++;
		}

		private void EvaluatePlacementProposition(LatticePlacementProposition placementProposition)
		{
			if (placementProposition.IsValid == false)
			{
				return;
			}

			bool addCurrentPlacement = false;

			if (placementProposition.Energy <= bestEnergyValue)
			{
				addCurrentPlacement = true;
			}
			else
			{
				if (placementProposition.Energy < averageEnergyValue)
				{
					if (UnityEngine.Random.Range(0f, 1f) >= pruningProbability02)
					{
						addCurrentPlacement = true;
					}
					else
					{
						prunedTrees02++;
					}
				}
				else
				{
					if (UnityEngine.Random.Range(0f, 1f) >= pruningProbability01)
					{
						addCurrentPlacement = true;
					}
					else
					{
						prunedTrees01++;
					}
				}
			}

			if (addCurrentPlacement)
			{
				UpdateEnergyValues(placementProposition);

				// Queue next
				if (currentMonomerIndex < parsedInput.Count - 1)
				{
					placementPropositions.AddRange(placementProposition.GetChildLatticePlacementPropositions(parsedInput[currentMonomerIndex + 1]));
				}
			}
		}

		private void UpdateEnergyValues(LatticePlacementProposition placementProposition)
		{
			if (placementProposition.Energy < bestEnergyValue)
			{
				bestEnergyValue = placementProposition.Energy;
				outputLattice.Value = placementProposition.GetResultingLattice();
			}

			averageEnergyValue = ((averageEnergyValue * energyValuesCount) + placementProposition.Energy) / (energyValuesCount + 1);
			energyValuesCount++;
		}

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



	}
}
