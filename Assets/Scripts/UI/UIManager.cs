using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Transform spellPanel;
    public GameObject effectPanel;

    private int currentSelectedIndex = 0;
    private List<GameObject> effectPanels = new List<GameObject>();

    public void PopulateEffectPanel(List<SpellEffect> spellEffects) {
        for (int x=0; x<spellEffects.Count; x++) {
            GameObject newEffectPanel = Instantiate(effectPanel);
            newEffectPanel.transform.SetParent(spellPanel);
            EffectPanel effectPanelScript = newEffectPanel.GetComponent<EffectPanel>();

            effectPanelScript.effectSprite.sprite = spellEffects[x].icon;
            effectPanelScript.panelSprite.color = new Color(effectPanelScript.panelSprite.color.r, effectPanelScript.panelSprite.color.g, effectPanelScript.panelSprite.color.b, 0.5f);
            newEffectPanel.GetComponentInChildren<Text>().text = (x+1).ToString();

            effectPanels.Add(newEffectPanel);
        }
    }

    void DeselectEffect(int index) {
        EffectPanel effectPanelScript = effectPanels[currentSelectedIndex].GetComponent<EffectPanel>();
        effectPanelScript.panelSprite.color = new Color(effectPanelScript.panelSprite.color.r, effectPanelScript.panelSprite.color.g, effectPanelScript.panelSprite.color.b, 0.5f);
    }

    public void SelectEffect(int index) {
        DeselectEffect(currentSelectedIndex);

        EffectPanel effectPanelScript = effectPanels[index].GetComponent<EffectPanel>();
        effectPanelScript.panelSprite.color = new Color(effectPanelScript.panelSprite.color.r, effectPanelScript.panelSprite.color.g, effectPanelScript.panelSprite.color.b, 1f);

        currentSelectedIndex = index;
    }
}
