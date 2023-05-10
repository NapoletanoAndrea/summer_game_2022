using UnityEditor;
using UnityEngine;

public static class Utils {
	
	#if UNITY_EDITOR
	
	public static T[] GetAllInstances<T>() where T : ScriptableObject {
		string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
		var a = new T[guids.Length];
		for (int i = 0; i < guids.Length; i++) {
			string path = AssetDatabase.GUIDToAssetPath(guids[i]);
			a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
		}
		return a;
	}
	
	#endif

	public static int Repeat(int num, int minInclusive, int maxExclusive) {
		return num >= maxExclusive ? minInclusive : num;
	}
}