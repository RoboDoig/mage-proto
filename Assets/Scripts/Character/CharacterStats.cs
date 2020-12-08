using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public Dictionary<string, float> stats {get; private set;}
    private UIPlayerCanvas uIPlayerCanvas;

    void Start() {
        uIPlayerCanvas = GetComponentInChildren<UIPlayerCanvas>();
    }

    public void SetStats(Dictionary<string, float> _stats) {
        stats = new Dictionary<string, float>(_stats);
    }

    public void ApplyEffect(string stat, float amount) {
        stats[stat] += amount;

        uIPlayerCanvas.SetHealth(stats["health"]);
        uIPlayerCanvas.SetMana(stats["mana"]);
    }

    void CheckConditions() {
        
    }

    void Update() {
        if (stats["mana"] < 100f) {
            stats["mana"] += Time.deltaTime;
            uIPlayerCanvas.SetMana(stats["mana"]);
        }
    }
}
