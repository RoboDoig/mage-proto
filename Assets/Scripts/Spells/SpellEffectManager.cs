using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffectManager : MonoBehaviour
{
    public List<SpellEffect> spellEffects;

    public Dictionary<string, SpellEffect> spellEffectsDict = new Dictionary<string, SpellEffect>();

    void Awake() {
        foreach (SpellEffect spellEffect in spellEffects) {
            spellEffectsDict.Add(spellEffect.effectName, spellEffect);
        }
    }

}
