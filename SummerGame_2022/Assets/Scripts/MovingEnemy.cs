using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovingEnemy : EnemyBase {
	[SerializeField] private bool movesHorizontally;
	[SerializeField] private bool movesVertically;
	[SerializeField] private bool movesDiagonally;
	[SerializeField] private bool slow;
	[SerializeField] private float moveSeconds;
	[SerializeField] private LayerMask obstacleMask;
	
	private Collider2D col;

	private List<Vector2> possibleDirections = new();

	private void Awake() {
		col = GetComponent<Collider2D>();
	}

	private void AddPossibleDirections(float distance, params Vector2[] directions) {
		foreach (var direction in directions) {
			direction.Normalize();
			if (!Physics2D.Raycast(col.bounds.center, direction, distance, obstacleMask)) {
				possibleDirections.Add(direction * distance);
			}
		}
	}

	private List<Vector2> GetPossibleDirections() {
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
		return possibleDirections;
	}

	private Vector2 GetRandomDirection() {
		return GetPossibleDirections()[Random.Range(0, possibleDirections.Count)];
	}

	private void Update() {
		Vector2 startPos = col.bounds.center;
		float num = 1;
		Debug.DrawRay(startPos, Vector3.up / num);
		Debug.DrawRay(startPos, Vector3.down / num);
		Debug.DrawRay(startPos, Vector3.left / num);
		Debug.DrawRay(startPos, Vector3.right / num);
		Debug.DrawRay(startPos, Vector3.up + Vector3.right);
	}

	protected override void OnEnemyTurn() {
		if (slow) {
			if (TurnManager.Instance.turnsLeftForMoveExecution == 1) {
				StartCoroutine(MoveCoroutine(GetRandomDirection()));
			}
			return;
		}
		StartCoroutine(MoveCoroutine(GetRandomDirection()));
	}

	private IEnumerator MoveCoroutine(Vector2 vector, bool isLocation = false) {
		float count = 0;
		float t = 0;
		Vector2 startPosition = transform.position;
		
		if (!isLocation) {
			while (count <= moveSeconds) {
				count += Time.deltaTime;
				t += Time.deltaTime / moveSeconds;
				transform.position = Vector3.Lerp(startPosition, startPosition + vector, t);
				yield return null;
			}
			transform.position = startPosition + vector;
		}
		
		Moved?.Invoke();
	}
}
