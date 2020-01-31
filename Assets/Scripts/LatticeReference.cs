using ProteinFolding;
using System;
using UnityEngine;
[Serializable]
public class LatticeReference : ScriptableVariableReference<Lattice, LatticeVariable>
{
	public LatticeReference() : base() { }
	public LatticeReference(Lattice value) : base(value) { }
	public LatticeReference(LatticeVariable variable) : base(variable) { }
}
