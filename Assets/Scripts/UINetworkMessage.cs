using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINetworkMessage : MonoBehaviour
{
    public Transform networkMessagePanel;
    public GameObject networkMessagePrefab;

    public int maxMessages = 5;
    List<string> networkMessages = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        DisplayMessages();
    }

    void ClearMessages() {
        foreach (Transform child in networkMessagePanel) {
            Destroy(child.gameObject);
        }
    }

    void DisplayMessages() {
        ClearMessages();

        foreach (string message in networkMessages) {
            GameObject newMessage = Instantiate(networkMessagePrefab);
            newMessage.GetComponent<Text>().text = message;
            newMessage.transform.SetParent(networkMessagePanel);
        }
    }

    public void AddMessage(string message) {
        if (networkMessages.Count == maxMessages) {
            networkMessages.RemoveAt(0);
        }

        networkMessages.Add(message);

        DisplayMessages();
    }
}
