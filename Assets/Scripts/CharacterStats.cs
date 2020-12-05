using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private Dictionary<string, float> stats = new Dictionary<string, float>();

    void Awake() {
        stats.Add("health", 100f);
        stats.Add("mana", 100f);
    }

    public void ApplyEffect(string stat, float amount) {
        stats[stat] += amount;
    }

    void CheckConditions() {
        
    }

    // private float health = 100f;

    // public float speed = 3.5f;

    // public void ApplyDamage(float amount) {
    //     health -= amount;
    //     if (health <= 0f) {
    //         Die();
    //     }
    // }

    // void Die() {
    //     Destroy(this.gameObject);
    // }
}
