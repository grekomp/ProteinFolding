using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Git {
	public class Preprocessor : IPreprocessBuildWithReport {

		public int callbackOrder {
			get { return 0; }
		}

		private static bool IsHeadDetached(string head) {
			return !head.StartsWith("ref:");
		}

		public void OnPreprocessBuild(BuildReport report) {
			var gitRoot = Path.Combine(Directory.GetCurrentDirectory(), ".git");
			if (!Directory.Exists(gitRoot)) {
				Debug.LogError("not fount .git directory");
				return;
			}

			var headPath = Path.Combine(gitRoot, "HEAD");
			var head = File.ReadAllText(headPath);

			string commitId;
			if (IsHeadDetached(head)) {
				commitId = head.Trim();
			} else {
				var headRefPath = Path.Combine(gitRoot, head.Remove(0, 4).Trim());
				commitId = File.ReadAllText(headRefPath);
			}
			Commit commit = Commit.Create(commitId);
			AssetDatabase.CreateAsset(commit, "Assets/Resources/GitHead.asset");
		}
	}

}
