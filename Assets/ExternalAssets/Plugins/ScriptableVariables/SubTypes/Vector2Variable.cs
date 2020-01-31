using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/Vector2")]
public class Vector2Variable : ScriptableVariable<Vector2> {
    public static Vector2Variable New(Vector2 value = default) {
        var createdVariable = CreateInstance<Vector2Variable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
