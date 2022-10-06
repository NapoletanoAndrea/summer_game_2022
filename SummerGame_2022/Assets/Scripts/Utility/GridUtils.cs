using UnityEngine;

public static class GridUtils 
{
    public static void AttackPosition(MoveData moveData) {
        float radius = GridManager.Instance.grid.cellSize.x / 2;
        radius *= .9f;
        foreach (var targetPosition in moveData.targetPositions) {
            var results = Physics2D.OverlapCircleAll(targetPosition, radius);
            foreach (var c in results) {
                MonoBehaviour[] monos = c.GetComponents<MonoBehaviour>();
                foreach (var mono in monos) {
                    ((ITargetable) mono)?.ReceiveMove(moveData);
                }
            }
        }
    }
    
    public static Vector2[] GetCrossPattern(Vector2 center) {
        float cellSize = GridManager.Instance.grid.cellSize.x;
        Vector2[] crossPattern =
        {
            center + Vector2.up * cellSize,
            center + Vector2.left * cellSize,
            center + Vector2.right * cellSize,
            center + Vector2.down * cellSize
        };
        return crossPattern;
    }
    
    public static float SnapNumber(float x, float halfCellSize) {
        float mulValue = x / halfCellSize;
        mulValue = Mathf.Floor(mulValue);
        if (mulValue % 2 == 0) {
            mulValue++;
        }
        return halfCellSize * mulValue;
    }
}
