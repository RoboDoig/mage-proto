using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterface : MonoBehaviour
{
    public CharacterControl targetCharacter;
    public UIManager uiManager;
    public SpellEffectManager spellEffectManager;
    private SpellCasting characterSpellCasting;

    public float xOffset;
    public float yOffset;
    public float zOffset;

    public void SelectCharacter(CharacterControl characterControl) {
        targetCharacter = characterControl;

        Camera.main.transform.position = targetCharacter.transform.position + new Vector3(xOffset, yOffset, zOffset);
        Camera.main.transform.LookAt(targetCharacter.transform.position);

        uiManager.PopulateEffectPanel(spellEffectManager.spellEffects);
        uiManager.SelectEffect(0);

        characterSpellCasting = targetCharacter.GetComponent<SpellCasting>();
        characterSpellCasting.SelectSpellEffect(spellEffectManager.spellEffects[0]);
    }

    void Start() {
        Screen.SetResolution(1920, 1080, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (targetCharacter != null) {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                int selectionIndex = 0;
                uiManager.SelectEffect(selectionIndex);
                characterSpellCasting.SelectSpellEffect(spellEffectManager.spellEffects[selectionIndex]);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                int selectionIndex = 1;
                uiManager.SelectEffect(selectionIndex);
                characterSpellCasting.SelectSpellEffect(spellEffectManager.spellEffects[selectionIndex]);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                int selectionIndex = 2;
                uiManager.SelectEffect(selectionIndex);
                characterSpellCasting.SelectSpellEffect(spellEffectManager.spellEffects[selectionIndex]);
            }


            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                targetCharacter.LookAtTarget(hit.point);

                if (Input.GetMouseButtonDown(0)) {
                    targetCharacter.GoToTarget(hit.point);
                }

                if (Input.GetMouseButtonDown(1)) {
                    targetCharacter.InitiateSpell();
                }

                if (Input.GetMouseButtonUp(1)) {
                    targetCharacter.ReleaseSpell();
                }
            }

            Quaternion lookRotation = Quaternion.LookRotation(targetCharacter.transform.position - Camera.main.transform.position);
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, Time.deltaTime);
            Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, targetCharacter.transform.position + new Vector3(xOffset, yOffset, zOffset), Time.deltaTime);
        }
    }
}
