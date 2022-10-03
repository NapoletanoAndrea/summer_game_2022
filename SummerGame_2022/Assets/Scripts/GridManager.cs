using UnityEngine;

public class GridManager : MonoBehaviour {
    public static GridManager Instance;
    
    public Grid grid;

    private void Awake() {
        if (!grid) {
            grid = GetComponent<Grid>();
        }
    }
}
