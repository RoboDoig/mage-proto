using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterface : MonoBehaviour
{

    public CharacterControl targetCharacter;

    public float xOffset;
    public float yOffset;
    public float zOffset;

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

            if (Input.GetMouseButtonDown(1)) {
                targetCharacter.Attack();
            }
        }

        Quaternion lookRotation = Quaternion.LookRotation(targetCharacter.transform.position - Camera.main.transform.position);
        Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, Time.deltaTime);
        Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, targetCharacter.transform.position + new Vector3(xOffset, yOffset, zOffset), Time.deltaTime);
    }

    void FixedUpdate() {

    }
}
