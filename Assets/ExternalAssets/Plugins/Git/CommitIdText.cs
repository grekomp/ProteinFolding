using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CommitIdText : MonoBehaviour {

	TextMeshProUGUI text;

	void Start() {
		text = GetComponent<TextMeshProUGUI>();
		Git.Commit commit = Resources.Load<Git.Commit>("GitHead");
		if (commit != null) {
			text.text += commit.id.Substring(0, 11);
		} else {
			text.text += "UnityEditor";
		}
	}
}
