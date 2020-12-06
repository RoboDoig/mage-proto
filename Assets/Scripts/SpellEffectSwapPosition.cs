using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffectSwapPosition : SpellEffect
{   
    public override bool OnEffectEnd() {
        Destroy(currentProjectile);
        currentImpactIndicator = Instantiate(impactIndicator, castTarget, Quaternion.identity);

        WorldObject castObj = null;
        WorldObject initObj = null;

        // Swap objects
        Collider[] hitColliders = Physics.OverlapSphere(castTarget, areaOfEffect);
        foreach (Collider hitCollider in hitColliders) {
            WorldObject worldObj = hitCollider.GetComponent<WorldObject>();
            if (worldObj) {
                castObj = worldObj;
            }
        }

        hitColliders = Physics.OverlapSphere(initiateTarget, areaOfEffect);
        foreach (Collider hitCollider in hitColliders) {
            WorldObject worldObj = hitCollider.GetComponent<WorldObject>();
            if (worldObj) {
                initObj = worldObj;
            }
        }

        if (castObj != null && initObj != null) {
            Vector3 castObjPosition = castObj.transform.position;
            Vector3 initObjPosition = initObj.transform.position;

            initObj.transform.position = castObjPosition;
            castObj.transform.position = initObjPosition;
        }

        Destroy(currentImpactIndicator, 1f);
        return true;
    }
}
