using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomMovingEnemy : MovingEnemy
{
	[SerializeField] private bool movesHorizontally;
	[SerializeField] private bool movesVertically;
	[SerializeField] private bool movesDiagonally;

	private List<Vector2> possibleDirections = new();
	private Collider2D col;

	private void Awake() {
		col = GetComponent<Collider2D>();
	}

	protected override void OnEnemyTurn() {
		Move(GetRandomDirection());
	}
	
	private void AddPossibleDirections(float distance, params Vector2[] directions) {
		foreach (var direction in directions) {
			direction.Normalize();
			if (Physics2D.RaycastAll(col.bounds.center, direction, distance, GridManager.Instance.obstacleMask).Length <= 1) {
				possibleDirections.Add(direction * distance);
			}
		}
	}

	private void FillPossibleDirections() {
		Vector2 cellSize = GridManager.Instance.grid.cellSize;
		possibleDirections.Clear();
		
		if (movesHorizontally) {
			AddPossibleDirections(cellSize.y, Vector2.up, Vector2.down);
		}
		if (movesVertically) {
			AddPossibleDirections(cellSize.x, Vector2.left, Vector2.right);
		}
		if (movesDiagonally) {
			Vector2 upRight = Vector2.up * cellSize.y + Vector2.right * cellSize.x;
			Vector2 downRight = Vector2.down * cellSize.y + Vector2.right * cellSize.x;
			Vector2 downLeft = Vector2.down * cellSize.y + Vector2.left * cellSize.x;
			Vector2 upLeft = Vector2.up * cellSize.y + Vector2.left * cellSize.x;
			AddPossibleDirections(upRight.magnitude, upRight, downRight, downLeft, upLeft);
		}
	}
	
	private Vector2 GetRandomDirection() {
		FillPossibleDirections();
		return possibleDirections.Count > 0 ? possibleDirections[Random.Range(0, possibleDirections.Count)] : Vector2.zero;
	}
}
