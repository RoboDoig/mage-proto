using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;

public class NetworkMessenger : MonoBehaviour
{
    public UnityClient client;

    public void SendPlayerMoveMessage(Vector3 position, Quaternion rotation, Vector3 currentLookTarget, Vector2 animSpeeds) {
        using (DarkRiftWriter moveMessageWriter = DarkRiftWriter.Create()) {
            moveMessageWriter.Write(new NetworkPlayerManager.MovementMessage(position, rotation, currentLookTarget, animSpeeds));
            using (Message moveMessage = Message.Create(Tags.MovePlayerTag, moveMessageWriter)) {
                client.SendMessage(moveMessage, SendMode.Reliable);
            }
        }
    }

    public void SendSpellMessage(string spellName, string command) {
        using (DarkRiftWriter spellMessageWriter = DarkRiftWriter.Create()) {
            spellMessageWriter.Write(new NetworkPlayerManager.SpellMessage(spellName, command));
            using (Message spellMessage = Message.Create(Tags.SpellPlayerTag, spellMessageWriter)) {
                client.SendMessage(spellMessage, SendMode.Reliable);
            }
        }
    }

    public void RequestStatsEffectMessage(ushort receiverID, string stat, float amount) {
        using (DarkRiftWriter statsMessageWriter = DarkRiftWriter.Create()) {
            statsMessageWriter.Write(new NetworkPlayerManager.StatMessage(client.ID, receiverID, stat, amount));
            using (Message statsMessage = Message.Create(Tags.ApplyEffectTag, statsMessageWriter)) {
                client.SendMessage(statsMessage, SendMode.Reliable);
            }
        }
    }
}
