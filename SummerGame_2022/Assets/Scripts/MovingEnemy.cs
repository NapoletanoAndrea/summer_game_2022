using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovingEnemy : EnemyBase {
	[SerializeField] private float moveSeconds;
	[SerializeField] private LayerMask obstacleMask;

	private Grid grid;
	private Collider2D col;

	private void Awake() {
		grid = FindObjectOfType<Grid>();
		col = GetComponent<Collider2D>();
	}

	private Vector2 GetRandomDirection() {
		List<Vector2> possibleDirections = new();
		for (int i = 0; i < 4; i++) {
			Vector2 raycastDir = Vector2.zero;
			float distance = 1;
			switch (i) {
				case 0:
					raycastDir = Vector2.up;
					distance = grid.cellSize.y;
					break;
				case 1:
					raycastDir = Vector2.down * grid.cellSize.y;
					distance = grid.cellSize.y;
					break;
				case 2:
					raycastDir = Vector2.left * grid.cellSize.x;
					distance = grid.cellSize.x;
					break;
				case 3:
					raycastDir = Vector2.right * grid.cellSize.x;
					distance = grid.cellSize.x;
					break;
			}
			if (!Physics2D.Raycast(col.bounds.center, raycastDir, distance / 1.5f, obstacleMask)) {
				possibleDirections.Add(raycastDir * distance);
			}
		}
		if (possibleDirections.Count == 0) {
			return Vector2.zero;
		}
		return possibleDirections[Random.Range(0, possibleDirections.Count)];
	}

	private void Update() {
		Vector2 startPos = col.bounds.center;
		float num = 1.5f;
		Debug.DrawRay(startPos, Vector3.up / num);
		Debug.DrawRay(startPos, Vector3.down / num);
		Debug.DrawRay(startPos, Vector3.left / num);
		Debug.DrawRay(startPos, Vector3.right / num);
	}

	protected override void OnEnemyTurn() {
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
