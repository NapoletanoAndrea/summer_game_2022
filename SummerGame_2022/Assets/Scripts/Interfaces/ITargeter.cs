using UnityEngine;

public struct MoveData {
    public ElementType moveType;
    public ITargeter sender;
    public Vector2[] targetPositions;

    public MoveData(ElementType moveType, ITargeter sender, Vector2[] targetPositions = null) {
        this.moveType = moveType;
        this.sender = sender;
        this.targetPositions = targetPositions;
    }

    public void SetTargetPositions(params Vector2[] targetPositions) {
        this.targetPositions = targetPositions;
    }
}

public interface ITargeter {
    public abstract void SendMove();
}
