using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using Random = UnityEngine.Random;

public class CombatUIManager : MonoBehaviour {
    [SerializeField] private GameObject combatGraphics;
    [SerializeField] private SpriteRenderer enemyRenderer;

    [SerializeField] private SpriteRenderer elementBalloonSprite;

    [SerializeField] private Transform fightBarParent;
    [SerializeField] private SpriteRenderer fightBar;
    [SerializeField] private Transform selector;
    [SerializeField] private float selectorSpeed;
    [SerializeField] private int bounceTimes;

    public LayerMask layerMask;

    private Dictionary<ElementType, Sprite> elementSprites = new();

    private Vector2 barLimit;
    private Dictionary<GameObject, ElementType> elementTypeInstances = new();
    private bool canSelect;

    private Player rewiredPlayer;

    private void Awake() {
        rewiredPlayer = ReInput.players.GetPlayer(0);
    }

    private void Start() {
        BattleManager.Instance.BattleStarted += Setup;
        BattleManager.Instance.BattleTurnStarted += OnBattleTurnStarted;
        BattleManager.Instance.BattleFinished += OnBattleFinished;
    }

    private void OnEnable() {
        rewiredPlayer.AddInputEventDelegate(Select, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, RewiredConsts.Action.Select);
    }

    private void OnDisable() {
        BattleManager.Instance.BattleStarted -= Setup;
        BattleManager.Instance.BattleTurnStarted -= OnBattleTurnStarted;
        BattleManager.Instance.BattleFinished -= OnBattleFinished;
        rewiredPlayer.RemoveInputEventDelegate(Select, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, RewiredConsts.Action.Select);
    }

    private void Setup() {
        elementSprites = DataSprites.Instance.dataSprites.elementSprites;
        enemyRenderer.sprite = BattleManager.Instance.enemyMoveSelector.combatSprite;
        combatGraphics.SetActive(true);
    }

    private void OnBattleTurnStarted(ElementType enemyMoveType) {
        elementBalloonSprite.sprite = elementSprites[enemyMoveType];
        GenerateSprites();
        StartCoroutine(MoveSelectorCoroutine());
    }

    private void OnBattleFinished() {
        combatGraphics.SetActive(false);
    }

    private void GenerateSprites() {
        foreach (var sprite in elementTypeInstances) {
            Destroy(sprite.Key);
        }
        elementTypeInstances.Clear();
        
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
                randomLimit = new Vector2(randomX - spriteInstance.transform.localScale.x / 2, randomX + spriteInstance.transform.localScale.x / 2);
                foreach (var limit in excludedLimits) {
                    if ((randomLimit.x >= limit.x && randomLimit.x <= limit.y) || (randomLimit.y >= limit.x && randomLimit.y <= limit.y)) {
                        flag = false;
                    }
                }
            }
            excludedLimits.Add(randomLimit);
            spriteInstance.transform.localPosition = new Vector2(randomX, center.y);
            spriteInstance.layer = 6;
            var col = spriteInstance.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            elementTypeInstances.Add(spriteInstance, elementSprite.Key);
        }
    }

    private IEnumerator MoveSelectorCoroutine() {
        Vector2 center = fightBar.transform.localPosition;
        Vector2 pos1 = new Vector2(barLimit.x, center.y);
        Vector2 pos2 = new Vector2(barLimit.y, center.y);

        int rng = Random.Range(0, 2);
        Vector2 startLocalPos = rng == 0 ? pos1 : pos2;
        Vector2 finalLocalPos = rng == 1 ? pos1 : pos2;
        selector.localPosition = startLocalPos;

        canSelect = true;
        for (int i = 0; i < bounceTimes; i++) {
            float count = 0;
            while ((Vector2) selector.localPosition != finalLocalPos) {
                count += Time.deltaTime;
                selector.localPosition = Vector2.Lerp(startLocalPos, finalLocalPos, selectorSpeed * count);
                yield return null;
            }
            (startLocalPos, finalLocalPos) = (finalLocalPos, startLocalPos);
        }
        canSelect = false;
        BattleManager.Instance.SelectElement(ElementType.Rock, true);
    }

    private void Select(InputActionEventData inputActionEventData) {
        if (canSelect) {
            StopAllCoroutines();
            canSelect = false;
            Collider2D col = selector.GetComponent<Collider2D>();
            var results = Physics2D.OverlapCircleAll(selector.transform.position, col.bounds.extents.x, layerMask);
            if (results.Length == 0) {
                BattleManager.Instance.SelectElement(ElementType.Rock, true);
            }
            else if (results.Length == 1) {
                BattleManager.Instance.SelectElement(elementTypeInstances[results[0].gameObject], false);
            }
            else {
                results = results.OrderBy(r => (r.transform.position - selector.transform.position).sqrMagnitude).ToArray();
                BattleManager.Instance.SelectElement(elementTypeInstances[results[0].gameObject], false);
            }
        }
    }
}
