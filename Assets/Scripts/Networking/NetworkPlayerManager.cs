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

    // UI
    public UINetworkMessage uiNetworkMessage;

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

        uiNetworkMessage.AddMessage("Connected to Network");
    }

    void MessageReceived(object sender, MessageReceivedEventArgs e) {
        using (Message message = e.GetMessage() as Message) {
            if (message.Tag == Tags.SpawnPlayerTag) {
                SpawnPlayer(sender, e);
                uiNetworkMessage.AddMessage("Player Spawned");
            } else if (message.Tag == Tags.DespawnPlayerTag) {
                DespawnPlayer(sender, e);
                uiNetworkMessage.AddMessage("Player Despawned");
            } else if (message.Tag == Tags.MovePlayerTag) {
                MovePlayer(sender, e);
            } else if (message.Tag == Tags.SpellPlayerTag) {
                PlayerSpell(sender, e);
            } else if (message.Tag == Tags.ApplyEffectTag) {
                ApplyEffect(sender, e);
                uiNetworkMessage.AddMessage("Add Effect");
            }
        }
    }

    void SpawnPlayer(object sender, MessageReceivedEventArgs e) {
        using (Message message = e.GetMessage()) 
        using (DarkRiftReader reader= message.GetReader()) {
            PlayerMessage playerMessage = reader.ReadSerializable<PlayerMessage>();

            Vector3 position = new Vector3(playerMessage.X, playerMessage.Y, playerMessage.Z);
            GameObject obj;

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

            // Spawn player at appropriate spawn point
            foreach (SpawnLocation spawnLocation in SpawnLocation.spawnLocations) {
                if (spawnLocation.team == playerMessage.teamID) {
                    obj.transform.position = spawnLocation.transform.position;
                }
            }
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
                MovementMessage moveMessage = reader.ReadSerializable<MovementMessage>();

                if (networkPlayers.ContainsKey(moveMessage.ID)) {
                    NetworkCharacterControl networkCharacterControl = networkPlayers[moveMessage.ID].GetComponent<NetworkCharacterControl>();
                    networkCharacterControl.SetPosition(moveMessage.position);
                    networkCharacterControl.SetRotation(moveMessage.rotation);
                    networkCharacterControl.SetLookTarget(moveMessage.lookTarget);
                    networkCharacterControl.SetAnimatorSpeeds(moveMessage.animSpeeds);
                }
            }
        }
    }

    void PlayerSpell(object sender, MessageReceivedEventArgs e) {
        using (Message message= e.GetMessage() as Message) {
            using (DarkRiftReader reader = message.GetReader()) {
                SpellMessage spellMessage = reader.ReadSerializable<SpellMessage>();

                if (networkPlayers.ContainsKey(spellMessage.ID)) {
                    NetworkCharacterControl networkCharacterControl = networkPlayers[spellMessage.ID].GetComponent<NetworkCharacterControl>();
                    if (spellMessage.command == "spellInitiate") {
                        uiNetworkMessage.AddMessage("Spell Initiate");
                        networkCharacterControl.InitiateSpell(spellMessage.spellName);
                    } else if (spellMessage.command == "spellRelease") {
                        uiNetworkMessage.AddMessage("Spell Release");
                        networkCharacterControl.ReleaseSpell();
                    }
                }
            }
        }
    }

    void ApplyEffect(object sender, MessageReceivedEventArgs e) {
        using (Message message = e.GetMessage() as Message) {
            using (DarkRiftReader reader = message.GetReader()) {
                StatMessage statMessage = reader.ReadSerializable<StatMessage>();

                if (networkPlayers.ContainsKey(statMessage.receiverID)) {
                    networkPlayers[statMessage.receiverID].GetComponent<CharacterStats>().ApplyEffect(statMessage.stat, statMessage.amount);
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
        public ushort teamID;
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
            teamID = e.Reader.ReadUInt16();
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
        public ushort ID {get; set;}
        public Vector3 position {get; set;}
        public Quaternion rotation {get; set;}
        public Vector3 lookTarget {get; set;}
        public Vector2 animSpeeds {get; set;}

        public MovementMessage() {

        }

        public MovementMessage(Vector3 _position, Quaternion _rotation, Vector3 _lookTarget, Vector2 _animSpeeds) {
            position = _position;
            rotation = _rotation;
            lookTarget = _lookTarget;
            animSpeeds = _animSpeeds;
        }

        public void Deserialize(DeserializeEvent e) {
            ID = e.Reader.ReadUInt16();
            position = new Vector3(e.Reader.ReadSingle(), e.Reader.ReadSingle(), e.Reader.ReadSingle());
            rotation = new Quaternion(e.Reader.ReadSingle(), e.Reader.ReadSingle(), e.Reader.ReadSingle(), e.Reader.ReadSingle());
            lookTarget = new Vector3(e.Reader.ReadSingle(), e.Reader.ReadSingle(), e.Reader.ReadSingle());
            animSpeeds = new Vector2(e.Reader.ReadSingle(), e.Reader.ReadSingle());
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
        public ushort ID {get; set;}
        public string spellName {get; set;}
        public string command {get; set;}

        public SpellMessage() {

        }

        public SpellMessage(string _spellName, string _command) {
            spellName = _spellName;
            command = _command;
        }

        public void Deserialize(DeserializeEvent e) {
            ID = e.Reader.ReadUInt16();
            spellName = e.Reader.ReadString();
            command = e.Reader.ReadString();
        }

        public void Serialize(SerializeEvent e) {
            e.Writer.Write(spellName);
            e.Writer.Write(command);
        }
    }

    public class StatMessage : IDarkRiftSerializable {
        public ushort requesterID {get; set;}
        public ushort receiverID {get; set;}
        public string stat {get; set;}
        public float amount {get; set;}

        public StatMessage() {

        }

        public StatMessage(ushort _requesterID, ushort _receiverID, string _stat, float _amount) {
            requesterID = _requesterID;
            receiverID = _receiverID;
            stat = _stat;
            amount = _amount;
        }

        public void Deserialize(DeserializeEvent e) {
            requesterID = e.Reader.ReadUInt16();
            receiverID = e.Reader.ReadUInt16();
            stat = e.Reader.ReadString();
            amount = e.Reader.ReadSingle();
        }

        public void Serialize(SerializeEvent e) {
            e.Writer.Write(requesterID);
            e.Writer.Write(receiverID);
            e.Writer.Write(stat);
            e.Writer.Write(amount);
        }
    }
}
