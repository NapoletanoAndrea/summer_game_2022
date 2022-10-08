using UnityEngine;

public abstract class EnemyMoveSelector : MonoBehaviour {
    public Sprite combatSprite;
    protected int moveCount;

    public abstract ElementType GetMove();
}
