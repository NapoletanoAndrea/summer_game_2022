using UnityEngine;

public class DataSprites : MonoBehaviour {
    public static DataSprites Instance;
    public DataSpritesSO dataSprites;

    private void Awake() {
        Instance = this;
    }
}
