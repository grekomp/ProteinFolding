using ProteinFolding;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/Lattice")]
public class LatticeVariable : ScriptableVariable<Lattice>
{
	public static LatticeVariable New(Lattice value = default)
	{
		var createdVariable = CreateInstance<LatticeVariable>();
		createdVariable.Value = value;
		return createdVariable;
	}
}
