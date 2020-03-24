using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/Long")]
public class LongVariable : ScriptableVariable<long> {
    public static LongVariable New(long value = default) {
        var createdVariable = CreateInstance<LongVariable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
