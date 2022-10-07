using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MovesOrder : MonoBehaviour {
    [SerializeField] private Transform[] moves;
    [SerializeField] private Vector2 bigScale;
    [SerializeField] private Vector2 smallScale;

    private Dictionary<ElementType, int> movesIndex = new()
    {
        {ElementType.Rock, 1},
        {ElementType.Paper, 4},
        {ElementType.Scissors, 7}
    };

    private void Start() {
        if (Application.isPlaying) {
            TurnManager.Instance.playerElement.FinishedTurn += OnPlayerFinishedTurn;
        }
    }

    private void OnDisable() {
        if (Application.isPlaying) {
            TurnManager.Instance.playerElement.FinishedTurn -= OnPlayerFinishedTurn;
        }
    }

    private void Update() {
        if (!Application.isPlaying) {
            PlayerElement pElement = FindObjectOfType<PlayerElement>();
            if (pElement) {
                for (int i = 0; i < moves.Length; i++) {
                    moves[i].localScale = smallScale;
                    if (i == GetCurrentIndex(pElement)) {
                        moves[i].localScale = bigScale;
                    }
                }
            }
        }
    }

    private int GetCurrentIndex(PlayerElement playerElement) {
        int index = movesIndex[!Application.isPlaying ? playerElement.startingElement : playerElement.currentElement];
        index += playerElement.turnCount;
        if (index > 8) {
            index = 0;
        }
        return index;
    }

    private void OnPlayerFinishedTurn() {
        PlayerElement pElement = TurnManager.Instance.playerElement;
        if (pElement) {
            for (int i = 0; i < moves.Length; i++) {
                moves[i].localScale = smallScale;
                if (i == GetCurrentIndex(pElement)) {
                    moves[i].localScale = bigScale;
                }
            }
        }
    }
}
