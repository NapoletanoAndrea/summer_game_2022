using UnityEngine;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance;
    public bool hasKey;

    private void Awake() {
        Instance = this;
    }
    
}
