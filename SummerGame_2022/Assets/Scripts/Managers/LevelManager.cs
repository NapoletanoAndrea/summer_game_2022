using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance;
    public int currentLevel;
    public List<Key> foundKeys = new();
    public List<EnemyElement> enemiesKilled = new();

    private void Awake() {
        Instance = this;
    }
}
