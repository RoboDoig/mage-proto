using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;

public class NetworkPlayerManager : MonoBehaviour
{
    const byte SPAWN_TAG = 0;

    [SerializeField]
    UnityClient client;

    [SerializeField]
    [Tooltip("The controllable player prefab")]
    GameObject controllablePrefab;

    [SerializeField]
    [Tooltip("The network controllable player prefab")]
    GameObject networkPrefab;

    Dictionary<ushort, CharacterControl> networkPlayers = new Dictionary<ushort, CharacterControl>();

    void Awake() {
        if (client == null) {
            Debug.LogError("Client unassigned in NetworkPlayerManager");
            Application.Quit();
        }

        if (controllablePrefab == null) {
            Debug.LogError("Player prefab unassigned in NetworkPlayerManager");
            Application.Quit();
        }

        if (networkPrefab == null) {
            Debug.LogError("Network player unassigned in NetworkPlayerManager");
            Application.Quit();
        }

        client.MessageReceived += MessageReceived;
    }

    void MessageReceived(object sender, MessageReceivedEventArgs e) {
        using (Message message = e.GetMessage() as Message) {
            Debug.Log(message.Tag);
            if (message.Tag == Tags.SpawnPlayerTag) {
                SpawnPlayer(sender, e);
            } else if (message.Tag == Tags.DespawnPlayerTag) {
                DespawnPlayer(sender, e);
            }
        }
    }

    void SpawnPlayer(object sender, MessageReceivedEventArgs e) {
        using (Message message = e.GetMessage()) 
        using (DarkRiftReader reader= message.GetReader()) {
            while (reader.Position < reader.Length) {
                ushort id = reader.ReadUInt16();
                Vector3 position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Quaternion rotation = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                GameObject gameObject;
                if (id == client.ID) {
                    gameObject = Instantiate(controllablePrefab, position, Quaternion.identity);
                } else {
                    gameObject = Instantiate(networkPrefab, position, Quaternion.identity);
                }

                networkPlayers.Add(id, gameObject.GetComponent<CharacterControl>());
            }
        }
    }   

    void DespawnPlayer(object sender, MessageReceivedEventArgs e) {
        using (Message message = e.GetMessage())
        using (DarkRiftReader reader = message.GetReader()) {
            DestroyPlayer(reader.ReadUInt16());
        }
    }

    void DestroyPlayer(ushort id) {
        CharacterControl p = networkPlayers[id];
        Destroy(p.gameObject);
        networkPlayers.Remove(id);
    }

    // class PlayerMessage : IDarkRiftSerializable {
    //     public ushort ID;
    //     public float X {get; set;}
    //     public float Y {get; set;}
    //     public float Z {get; set;}
    //     public float rotX {get; set;}
    //     public float rotY {get; set;}
    //     public float rotZ {get; set;}
    //     public float rotW {get; set;}

    //     public void Deserialize(DeserializeEvent e) {
    //         ID = e.Reader.ReadUInt16();
    //         X = e.Reader.ReadSingle();
    //         Y = e.Reader.ReadSingle();
    //         Z = e.Reader.ReadSingle();
    //         rotX = e.Reader.ReadSingle();
    //         rotY = e.Reader.ReadSingle();
    //         rotZ = e.Reader.ReadSingle();
    //         rotW = e.Reader.ReadSingle();
    //     }

    //     public void Serialize(SerializeEvent e) {
    //         throw new System.NotImplementedException();
    //     }
    // }   
}
