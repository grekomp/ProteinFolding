using UnityEngine;

namespace Git {

	public class Commit : ScriptableObject {

		public string id;

		public static Commit Create(string commitId) {
			var instance = CreateInstance<Commit>();
			instance.id = commitId;
			return instance;
		}
	}
}
