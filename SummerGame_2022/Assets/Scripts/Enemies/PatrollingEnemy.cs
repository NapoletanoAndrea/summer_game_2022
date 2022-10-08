using System;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemy : MovingEnemy {
    public List<PatrolTile> patrolTiles = new();
    private PatrolTile lastTile;

    [Serializable]
    public class PatrolTile {
        public Vector2 tilePosition;
        public List<Vector2> adjacentTiles = new();
        public int index;

        public bool IsAdjacentTileFree(int index = -1) {
            if (index < 0) {
                index = this.index;
            }
            var cellSize = GridManager.Instance.grid.cellSize;
            var result = Physics2D.OverlapBox(adjacentTiles[index], cellSize, 0, GridManager.Instance.obstacleMask);
            return result == null;
        }

        public void IncrementIndex() {
            index++;
            if (index >= adjacentTiles.Count) {
                index = 0;
            }
        }
    }

    protected override void OnEnemyTurn() {
        if (patrolTiles.Count < 2) {
            Moved?.Invoke();
            return;
        }
        
        Vector2 nextTilePos = GetNextTilePosition();
        
        Vector2 enemyPosition = transform.position;
        lastTile = GetPatrolTile(enemyPosition);
        lastTile.IncrementIndex();
        
        Move(nextTilePos - enemyPosition);
    }

    private Vector2 GetNextTilePosition() {
        Vector2 enemyPosition = transform.position;
        var currentTile = GetPatrolTile(enemyPosition);

        int firstIndex = currentTile.index;

        Vector2 lastTilePos = Vector2.positiveInfinity;
        if (lastTile != null) {
            lastTilePos = lastTile.tilePosition;
        }
        bool canReturnLastTilePos = false;
        
        int i = firstIndex;
        do {
            var nextTilePos = currentTile.adjacentTiles[currentTile.index];
            if (currentTile.IsAdjacentTileFree()) {
                if (nextTilePos != lastTilePos) {
                    return nextTilePos;
                }
                else {
                    canReturnLastTilePos = true;
                }
            }
            currentTile.IncrementIndex();
            i = currentTile.index;
        } 
        while (i != firstIndex);
        
        return canReturnLastTilePos ? lastTilePos : currentTile.tilePosition;
    }
    
    public void AddPatrolTile(Vector2 pos) {
        Vector2 cellSize = FindObjectOfType<Grid>().cellSize;
        var patrolTile = GetPatrolTile(pos);
        if (patrolTile == null) {
            PatrolTile toAdd = new ()
            {
                tilePosition = pos
            };
            Vector2[] directions = { Vector2.up * cellSize.y, Vector2.down * cellSize.y, Vector2.left * cellSize.x, Vector2.right * cellSize.x };
            foreach (var direction in directions) {
                var adjTile = GetPatrolTile(pos + direction);
                if (adjTile != null) {
                    adjTile.adjacentTiles.Add(toAdd.tilePosition);
                    toAdd.adjacentTiles.Add(adjTile.tilePosition); 
                }
            }
            patrolTiles.Add(toAdd);
        }
    }

    public void RemovePatrolTile(Vector2 pos) {
        Vector2 cellSize = FindObjectOfType<Grid>().cellSize;
        var patrolTile = GetPatrolTile(pos);
        if (patrolTile != null) {
            foreach (var adjTilePosition in patrolTile.adjacentTiles) {
                var adjTile = GetPatrolTile(adjTilePosition);
                adjTile.adjacentTiles.Remove(patrolTile.tilePosition);
            }
            patrolTiles.Remove(patrolTile);
        }
    }

    public void AddTileConnection(Vector2 tilePos1, Vector2 tilePos2) {
        PatrolTile tile1 = GetPatrolTile(tilePos1);
        PatrolTile tile2 = GetPatrolTile(tilePos2);
        if (tile1 != null && tile2 != null && tile1 != tile2) {
            if (!tile1.adjacentTiles.Contains(tilePos2)) {
                tile1.adjacentTiles.Add(tilePos2);
            }
            if (!tile2.adjacentTiles.Contains(tilePos1)) {
                tile2.adjacentTiles.Add(tilePos1);
            }
        }
    }

    public void RemoveTileConnection(Vector2 tilePos1, Vector2 tilePos2) {
        PatrolTile tile1 = GetPatrolTile(tilePos1);
        PatrolTile tile2 = GetPatrolTile(tilePos2);
        if (tile1 != null && tile2 != null) {
            if (tile1.adjacentTiles.Contains(tilePos2)) {
                tile1.adjacentTiles.Remove(tilePos2);
            }
            if (tile2.adjacentTiles.Contains(tilePos1)) {
                tile2.adjacentTiles.Remove(tilePos1);
            }
        }
    }

    public PatrolTile GetPatrolTile(Vector2 tilePosition) {
        return patrolTiles.Find(t => t.tilePosition == tilePosition);
    }
}
