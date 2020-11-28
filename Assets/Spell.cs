using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell
{
    public List<SpellElement> spellElements;

    public SpellElement currentElement;
    private int elementCounter;

    public Spell(List<SpellElement> _spellElements) {
        spellElements = _spellElements;
        elementCounter = 0;
        currentElement = spellElements[elementCounter];
    }

    void Start() {
        currentElement = spellElements[elementCounter];
    }

    public void NextElement() {
        elementCounter++;
        currentElement = spellElements[elementCounter];
    }

    public void ResetSpell() {
        elementCounter = 0;
        currentElement = spellElements[elementCounter];
    }

    public bool CanAdvance() {
        if ((elementCounter + 1) < spellElements.Count) {
            return true;
        } else {
            return false;
        }
    }
}
