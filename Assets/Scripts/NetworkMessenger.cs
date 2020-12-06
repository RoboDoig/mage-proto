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
        Message moveMessage = Message.Create(Tags.MovePlayerTag,
            new NetworkPlayerManager.MovementMessage(position, rotation, currentLookTarget, animSpeeds));

        client.SendMessage(moveMessage, SendMode.Unreliable);
    }

    public void SendSpellMessage(string spellName, string command) {
        Message spellMessage = Message.Create(Tags.SpellPlayerTag,
            new NetworkPlayerManager.SpellMessage(spellName, command));

        client.SendMessage(spellMessage, SendMode.Unreliable);
    }

    public void RequestStatsEffectMessage(string stat, float amount) {

    }
}
