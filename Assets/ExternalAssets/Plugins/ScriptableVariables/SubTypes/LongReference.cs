using System;
using UnityEngine;
[Serializable]
public class LongReference : ScriptableVariableReference<long, LongVariable> {
    public LongReference() : base() { }
    public LongReference(long value) : base(value) { }
    public LongReference(LongVariable variable) : base(variable) { }
}
