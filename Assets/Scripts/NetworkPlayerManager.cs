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

    Dictionary<ushort, NetworkEntity> networkPlayers = new Dictionary<ushort, NetworkEntity>();

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
            if (message.Tag == Tags.SpawnPlayerTag) {
                SpawnPlayer(sender, e);
            } else if (message.Tag == Tags.DespawnPlayerTag) {
                DespawnPlayer(sender, e);
            } else if (message.Tag == Tags.MovePlayerTag) {
                MovePlayer(sender, e);
            } else if (message.Tag == Tags.SpellPlayerTag) {
                PlayerSpell(sender, e);
            } else if (message.Tag == Tags.ApplyEffectTag) {
                ApplyEffect(sender, e);
            }
        }
    }

    void SpawnPlayer(object sender, MessageReceivedEventArgs e) {
        using (Message message = e.GetMessage()) 
        using (DarkRiftReader reader= message.GetReader()) {
            PlayerMessage playerMessage = reader.ReadSerializable<PlayerMessage>();

            Vector3 position = new Vector3(playerMessage.X, playerMessage.Y, playerMessage.Z);
            GameObject obj;

            // Important TODO - all players, local and network, must be in the networkPlayers dict. Need this for if server needs to send message to all entities
            if (playerMessage.ID == client.ID) {
                obj = Instantiate(controllablePrefab, position, Quaternion.identity);
                obj.GetComponent<NetworkMessenger>().client = client;
                obj.GetComponent<NetworkEntity>().networkID = client.ID;
            } else {
                obj = Instantiate(networkPrefab, position, Quaternion.identity);
                obj.GetComponent<NetworkEntity>().networkID = playerMessage.ID;
            }

            obj.GetComponent<CharacterStats>().SetStats(playerMessage.stats);
            networkPlayers.Add(playerMessage.ID, obj.GetComponent<NetworkEntity>());
        }
    }   

    void DespawnPlayer(object sender, MessageReceivedEventArgs e) {
        using (Message message = e.GetMessage())
        using (DarkRiftReader reader = message.GetReader()) {
            DestroyPlayer(reader.ReadUInt16());
        }
    }

    void MovePlayer(object sender, MessageReceivedEventArgs e) {
        using (Message message = e.GetMessage() as Message) {
            using (DarkRiftReader reader = message.GetReader()) {
                ushort id = reader.ReadUInt16();
                Vector3 newPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Quaternion newRotation = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Vector3 newLookTarget = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Vector2 newAnimSpeeds = new Vector2(reader.ReadSingle(), reader.ReadSingle());

                if (networkPlayers.ContainsKey(id)) {
                    NetworkCharacterControl networkCharacterControl = networkPlayers[id].GetComponent<NetworkCharacterControl>();
                    networkCharacterControl.SetPosition(newPosition);
                    networkCharacterControl.SetRotation(newRotation);
                    networkCharacterControl.SetLookTarget(newLookTarget);
                    networkCharacterControl.SetAnimatorSpeeds(newAnimSpeeds);
                }
            }
        }
    }

    void PlayerSpell(object sender, MessageReceivedEventArgs e) {
        using (Message message= e.GetMessage() as Message) {
            using (DarkRiftReader reader = message.GetReader()) {
                ushort id = reader.ReadUInt16();
                string spellName = reader.ReadString();
                string command = reader.ReadString();

                if (networkPlayers.ContainsKey(id)) {
                    NetworkCharacterControl networkCharacterControl = networkPlayers[id].GetComponent<NetworkCharacterControl>();
                    if (command == "spellInitiate") {
                        networkCharacterControl.InitiateSpell(spellName);
                    } else if (command == "spellRelease") {
                        networkCharacterControl.ReleaseSpell();
                    }
                }
            }
        }
    }

    void ApplyEffect(object sender, MessageReceivedEventArgs e) {
        using (Message message = e.GetMessage() as Message) {
            using (DarkRiftReader reader = message.GetReader()) {
                ushort requesterID = reader.ReadUInt16();
                ushort receiverID = reader.ReadUInt16();
                string stat = reader.ReadString();
                float amount = reader.ReadSingle();

                if (networkPlayers.ContainsKey(receiverID)) {
                    networkPlayers[receiverID].GetComponent<CharacterStats>().ApplyEffect(stat, amount);
                }
            }
        }
    }

    void DestroyPlayer(ushort id) {
        NetworkCharacterControl p = networkPlayers[id].GetComponent<NetworkCharacterControl>();
        Destroy(p.gameObject);
        networkPlayers.Remove(id);
    }

    public class PlayerMessage : IDarkRiftSerializable {
        public ushort ID;
        public float X {get; set;}
        public float Y {get; set;}
        public float Z {get; set;}
        public float rotX {get; set;}
        public float rotY {get; set;}
        public float rotZ {get; set;}
        public float rotW {get; set;}
        public float lookX {get; set;}
        public float lookY {get; set;}
        public float lookZ {get; set;}
        public Dictionary<string, float> stats = new Dictionary<string, float>();

        public void Deserialize(DeserializeEvent e) {
            ID = e.Reader.ReadUInt16();
            X = e.Reader.ReadSingle();
            Y = e.Reader.ReadSingle();
            Z = e.Reader.ReadSingle();
            rotX = e.Reader.ReadSingle();
            rotY = e.Reader.ReadSingle();
            rotZ = e.Reader.ReadSingle();
            rotW = e.Reader.ReadSingle();
            lookX = e.Reader.ReadSingle();
            lookY = e.Reader.ReadSingle();
            lookZ = e.Reader.ReadSingle();

            while (e.Reader.Position < e.Reader.Length) {
                string stat = e.Reader.ReadString();
                float value = e.Reader.ReadSingle();
                stats.Add(stat, value);
            }
        }

        public void Serialize(SerializeEvent e) {
            throw new System.NotImplementedException();
        }
    }

    public class MovementMessage : IDarkRiftSerializable {
        Vector3 position;
        Quaternion rotation;
        Vector3 lookTarget;
        Vector2 animSpeeds;

        public MovementMessage(Vector3 _position, Quaternion _rotation, Vector3 _lookTarget, Vector2 _animSpeeds) {
            position = _position;
            rotation = _rotation;
            lookTarget = _lookTarget;
            animSpeeds = _animSpeeds;
        }

        public void Deserialize(DeserializeEvent e) {
            throw new System.NotImplementedException();
        }

        public void Serialize(SerializeEvent e) {
            e.Writer.Write(position.x);
            e.Writer.Write(position.y);
            e.Writer.Write(position.z);
            e.Writer.Write(rotation.x);
            e.Writer.Write(rotation.y);
            e.Writer.Write(rotation.z);
            e.Writer.Write(rotation.w);
            e.Writer.Write(lookTarget.x);
            e.Writer.Write(lookTarget.y);
            e.Writer.Write(lookTarget.z);
            e.Writer.Write(animSpeeds.x);
            e.Writer.Write(animSpeeds.y);
        }
    }

    public class SpellMessage : IDarkRiftSerializable {
        string spellName;
        string command;

        public SpellMessage(string _spellName, string _command) {
            spellName = _spellName;
            command = _command;
        }

        public void Deserialize(DeserializeEvent e) {
            throw new System.NotImplementedException();
        }

        public void Serialize(SerializeEvent e) {
            e.Writer.Write(spellName);
            e.Writer.Write(command);
        }
    }

    public class StatMessage : IDarkRiftSerializable {
        ushort requesterID;
        ushort receiverID;
        string stat;
        float amount;

        public StatMessage(ushort _requesterID, ushort _receiverID, string _stat, float _amount) {
            requesterID = _requesterID;
            receiverID = _receiverID;
            stat = _stat;
            amount = _amount;

            Debug.Log(requesterID);
            Debug.Log(receiverID);
        }

        public void Deserialize(DeserializeEvent e) {
            throw new System.NotImplementedException();
        }

        public void Serialize(SerializeEvent e) {
            e.Writer.Write(requesterID);
            e.Writer.Write(receiverID);
            e.Writer.Write(stat);
            e.Writer.Write(amount);
        }
    }
}
