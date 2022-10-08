using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CombatUIManager : MonoBehaviour {
    [SerializeField] private GameObject combatGraphics;
    [SerializeField] private SpriteRenderer enemyRenderer;

    [SerializeField] private Transform fightBarParent;
    [SerializeField] private SpriteRenderer fightBar;
    [SerializeField] private Transform selector;

    private Dictionary<ElementType, Sprite> elementSprites = new();

    private Vector2 barLimit;
    private List<GameObject> elementSpritesInstances = new();

    private void Start() {
        BattleManager.Instance.BattleStarted += Setup;
    }

    private void OnDisable() {
        BattleManager.Instance.BattleStarted -= Setup;
    }

    private void Setup() {
        elementSprites = DataSprites.Instance.dataSprites.elementSprites;
        enemyRenderer.sprite = BattleManager.Instance.enemyMoveSelector.combatSprite;
        GenerateSprites();
        combatGraphics.SetActive(true);
    }

    private void GenerateSprites() {
        Vector2 center = fightBar.transform.localPosition;
        float scale = fightBar.size.x / 2 * fightBar.transform.localScale.x;
        barLimit = new Vector2(center.x - scale, center.x + scale);
        Debug.Log(barLimit);

        List<Vector2> excludedLimits = new();
        foreach (var elementSprite in elementSprites) {
            var spriteInstance = new GameObject();
            spriteInstance.transform.parent = fightBarParent;
            var spriteRenderer = spriteInstance.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = elementSprite.Value;
            spriteRenderer.sortingLayerID = SortingLayer.NameToID("Fight");
            spriteRenderer.sortingOrder = 11;
            float randomX = 0;
            Vector2 randomLimit = Vector2.zero;
            bool flag = false;
            while (!flag) {
                flag = true;
                randomX = Random.Range(barLimit.x, barLimit.y);
                foreach (var limit in excludedLimits) {
                    randomLimit = new Vector2(randomX - spriteInstance.transform.localScale.x / 2, randomX + spriteInstance.transform.localScale.x / 2);
                    if (randomLimit.x >= limit.x && randomLimit.y <= limit.y) {
                        flag = false;
                    }
                }
            }
            excludedLimits.Add(randomLimit);
            spriteInstance.transform.localPosition = new Vector2(randomX, center.y);
            elementSpritesInstances.Add(spriteInstance);
        }
    }
}
