using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffectBlink : SpellEffect
{
    public override bool OnEffectEnd()
    {
        Destroy(currentProjectile);
        currentImpactIndicator = Instantiate(impactIndicator, castTarget, Quaternion.identity);

        caster.transform.position = castTarget;
        CharacterControl characterControl = caster.GetComponent<CharacterControl>();
        if (characterControl != null)
            characterControl.GoToTarget(castTarget);

        Destroy(currentImpactIndicator, 1f);
        Destroy(this.gameObject);
        
        return true;
    }
}
