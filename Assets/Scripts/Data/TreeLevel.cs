using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProteinFolding
{
	[Serializable]
	public class SimulationTreeLevel
	{
		public int levelNumber;

		public float averageEnergy;
		public int averageEnergyWeight;
		public int bestEnergy;

		public SimulationTreeLevel(int levelNumber)
		{
			this.levelNumber = levelNumber;
		}
	}
}
