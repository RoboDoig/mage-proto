using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterface : MonoBehaviour
{

    public CharacterControl targetCharacter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            targetCharacter.LookAtTarget(hit.point);

            if (Input.GetMouseButtonDown(0)) {
                targetCharacter.GoToTarget(hit.point);
            }
        }
    }
}
