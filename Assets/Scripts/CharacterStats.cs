using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private Dictionary<string, float> stats;

    public void SetStats(Dictionary<string, float> _stats) {
        stats = new Dictionary<string, float>(_stats);
    }

    public void ApplyEffect(string stat, float amount) {
        stats[stat] += amount;
        Debug.Log(stat + ": " + stats[stat].ToString());
    }

    void CheckConditions() {
        
    }
}
