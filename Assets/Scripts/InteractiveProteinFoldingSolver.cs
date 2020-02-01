using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProteinFolding
{
	public class InteractiveProteinFoldingSolver : MonoBehaviour
	{
		[Header("Variables")]
		public StringReference inputString;

		protected List<bool> parsedInput;


		#region Initialization
		[ContextMenu("Init")]
		public void Init()
		{

		}
		#endregion


		#region Helper methods
		protected List<bool> ParseInput()
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
