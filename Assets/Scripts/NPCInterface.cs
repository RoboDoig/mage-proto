using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInterface : MonoBehaviour
{
    private CharacterControl characterControl;
    private Vector3 moveTarget = Vector3.zero;

    // Start is called before the first frame update
    void Awake()
    {
        characterControl = GetComponent<CharacterControl>();
    }

    void Start() {
        characterControl.GoToTarget(moveTarget);
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - moveTarget).magnitude < 1f) {
            moveTarget = new Vector3(Random.Range(-40f, 40f), 0f, Random.Range(-40f, 40f));
            characterControl.GoToTarget(moveTarget);
        }

        characterControl.LookAtTarget(moveTarget);
    }
}
