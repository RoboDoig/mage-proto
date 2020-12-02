using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class WorldObject : MonoBehaviour
{

    Outline outline;

    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Outline>();
        outline.OutlineWidth = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseOver() {
        outline.OutlineWidth = 2;
    }

    void OnMouseExit() {
        outline.OutlineWidth = 0;
    }
}
