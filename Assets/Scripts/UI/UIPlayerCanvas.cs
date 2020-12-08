using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerCanvas : MonoBehaviour
{
    Camera viewCamera;
    public Slider healthSlider;
    public Slider manaSlider;

    void Awake() {
        viewCamera = Camera.main;
    }

    void Update() {
        transform.LookAt(viewCamera.transform);
        transform.Rotate(0, 180, 0);
    }

    public void SetHealth(float amount) {
        healthSlider.value = amount / 100f;
    }

    public void SetMana(float amount) {
        manaSlider.value = amount / 100f;
    }
}
