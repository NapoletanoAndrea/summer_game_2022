using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyComponent : ScriptableObject {
    protected EnemyBase enemyBase;

    public virtual void Init(EnemyBase enemy) {
        this.enemyBase = enemyBase;
    }
}
