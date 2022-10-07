using UnityEngine;

public class GridManager : MonoBehaviour {
    public static GridManager Instance;
    
    public Grid grid;
    public LayerMask obstacleMask;

    private void Awake() {
        Instance = this;
        if (!grid) {
            grid = FindObjectOfType<Grid>();
        }
    }
}
