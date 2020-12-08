using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private Dictionary<string, float> stats;
    private UIPlayerCanvas uIPlayerCanvas;

    void Start() {
        uIPlayerCanvas = GetComponentInChildren<UIPlayerCanvas>();
    }

    public void SetStats(Dictionary<string, float> _stats) {
        stats = new Dictionary<string, float>(_stats);
    }

    public void ApplyEffect(string stat, float amount) {
        stats[stat] += amount;
        Debug.Log(stat + ": " + stats[stat].ToString());

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
