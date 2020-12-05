using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacterControl : MonoBehaviour
{
    private Animator animator;
    private SpellCasting spellCasting;
    private Vector3 currentLookTarget;
    private Vector2 animSpeed;

    void Awake() {
        animator = GetComponent<Animator>();
        spellCasting = GetComponent<SpellCasting>();
    }

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
        animator.SetFloat("speedX", _animSpeed.x);
        animator.SetFloat("speedY", _animSpeed.y);
    }
}
