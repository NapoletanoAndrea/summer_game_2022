using System;
using System.Collections;
using Rewired;
using UnityEngine;
using Action = RewiredConsts.Action;

public class PlayerController : MonoBehaviour {
	[SerializeField] private float moveSeconds;

	private Grid grid;
	private Player rewiredPlayer;
	private bool canMove;

	public event System.Action Moved;

	private void Awake() {
		grid = FindObjectOfType<Grid>();
		rewiredPlayer = ReInput.players.GetPlayer(0);
	}

	private void Start() {
		TurnManager.Instance.PlayerTurn += OnPlayerTurn;
	}

	private void OnEnable() {
		rewiredPlayer.AddInputEventDelegate(OnMove, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, Action.MoveLeft);	
		rewiredPlayer.AddInputEventDelegate(OnMove, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, Action.MoveRight);	
		rewiredPlayer.AddInputEventDelegate(OnMove, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, Action.MoveDown);	
		rewiredPlayer.AddInputEventDelegate(OnMove, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, Action.MoveUp);	
	}

	private void OnDisable() {
		rewiredPlayer.RemoveInputEventDelegate(OnMove, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, Action.MoveLeft);	
		rewiredPlayer.RemoveInputEventDelegate(OnMove, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, Action.MoveRight);	
		rewiredPlayer.RemoveInputEventDelegate(OnMove, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, Action.MoveDown);	
		rewiredPlayer.RemoveInputEventDelegate(OnMove, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, Action.MoveUp);
		TurnManager.Instance.PlayerTurn -= OnPlayerTurn;
	}

	private void OnPlayerTurn() {
		canMove = true;
	}

	private Vector2 GetDirection(int actionId) {
		Vector2 vector = Vector2.zero;

		switch (actionId) {
			case Action.MoveLeft: 
				vector = Vector2.left * grid.cellSize.x;
				break;
			case Action.MoveRight: 
				vector = Vector2.right * grid.cellSize.x;
				break;
			case Action.MoveUp: 
				vector = Vector2.up * grid.cellSize.y;
				break;
			case Action.MoveDown: 
				vector = Vector2.down * grid.cellSize.y;
				break;
		}
		return vector;
	}

	private void OnMove(InputActionEventData data) {
		if (canMove) {
			StartCoroutine(MoveCoroutine(GetDirection(data.actionId)));
		}
	}

	private IEnumerator MoveCoroutine(Vector2 vector, bool isLocation = false) {
		canMove = false;
		
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
