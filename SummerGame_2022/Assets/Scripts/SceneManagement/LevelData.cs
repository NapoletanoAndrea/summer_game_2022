using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/LevelData")]
public class LevelData : ScriptableObject {
    public UnitySerializedDictionary<int, int> levelBuildIndex;

    [Button]
    public void FillDictionary() {
        for (int i = 0; i < levelBuildIndex.Count; i++) {
            levelBuildIndex[i + 1] = i;
        }
    }

    [Button]
    public void Add() {
        for (int i = 0; i < levelBuildIndex.Count; i++) {
            levelBuildIndex[i + 1]++;
        }
    }

    [Button]
    public void Subtract() {
        for (int i = 0; i < levelBuildIndex.Count; i++) {
            levelBuildIndex[i + 1]++;
        }
    }
}
