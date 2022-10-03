using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable {
    public abstract void ReceiveMove(MoveData move);
}
