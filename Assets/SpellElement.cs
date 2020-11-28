using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spells", menuName = "ScriptableObjects/SpellElement", order = 1)]
public class SpellElement : ScriptableObject
{
    // Properties
    public string elementName;
    public Sprite icon;
    public float damage = 1f;
    public float lifeTime;
    public GameObject impactIndicator;
    private GameObject currentIndicator;

    bool spellStarted = false;

    public bool OnElementStart(CharacterControl caster, Vector3 target) {
        currentIndicator = Instantiate(impactIndicator, target, Quaternion.identity);
        return true;
    }

    public bool ElementRun(CharacterControl caster, Vector3 target, float timer) {
        if (timer < lifeTime) {
            return false;
        } else {
            return true;
        }
    }

    public bool OnElementEnd(CharacterControl caster, Vector3 target) {
        Destroy(currentIndicator);
        return true;
    }

}
