using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacterControl : MonoBehaviour
{
    private Animator animator;
    private SpellCasting spellCasting;
    private Vector3 currentLookTarget;
    private Vector2 animSpeed;

    public void SetRotation(Quaternion _rotation) {
        transform.rotation = _rotation;
    }

    public void SetLookTarget(Vector3 _target) {
        currentLookTarget = _target;
    }

    public void SetPosition(Vector3 _position) {
        transform.position = _position;
    }

    public void SetAnimatorSpeeds(Vector2 _animSpeed) {
        animSpeed.x = _animSpeed.x;
        animSpeed.y = _animSpeed.y;
    }
}
