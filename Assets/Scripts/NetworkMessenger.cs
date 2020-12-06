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
        // Message moveMessage = Message.Create(Tags.MovePlayerTag,
        //     new NetworkPlayerManager.MovementMessage(position, rotation, currentLookTarget, animSpeeds));

        // client.SendMessage(moveMessage, SendMode.Unreliable);
    }

    public void SendSpellMessage(string spellName, string command) {
        Message spellMessage = Message.Create(Tags.SpellPlayerTag,
            new NetworkPlayerManager.SpellMessage(spellName, command));

        client.SendMessage(spellMessage, SendMode.Reliable);
    }

    public void RequestStatsEffectMessage(ushort receiverID, string stat, float amount) {
        Message statMessage = Message.Create(Tags.ApplyEffectTag,
            new NetworkPlayerManager.StatMessage(client.ID, receiverID, stat, amount));

        client.SendMessage(statMessage, SendMode.Reliable);
    }
}
