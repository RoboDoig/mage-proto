using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFPSCounter : MonoBehaviour
{

    Text fpsText;
    float deltaTime;

    // Start is called before the first frame update
    void Start()
    {
        fpsText = GetComponent<Text>();
        deltaTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime = 1.0f / Time.smoothDeltaTime;
        fpsText.text = "fps: " + ((int)deltaTime).ToString();
    }
}
