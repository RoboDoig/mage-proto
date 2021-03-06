﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DarkRift.Server;
using DarkRift;

namespace MagePlugin
{
    public class PlayerManager : Plugin
    {
        public override bool ThreadSafe => false;

        public override Version Version => new Version(1, 0, 0);

        Dictionary<IClient, Player> players = new Dictionary<IClient, Player>();

        public ushort teamCounter = 0;

        public PlayerManager(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            ClientManager.ClientConnected += ClientConnected;
            ClientManager.ClientDisconnected += ClientDisconnected;
        }

        void ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            // When client connects, generate new player data
            Player newPlayer = new Player(
                e.Client.ID,
                teamCounter,
                0f,
                0.5f,
                0f
            );

            // for now, we just alternate teams between 0 and 1 every time a new player joins
            if (teamCounter == 0)
            {
                teamCounter = 1;
            } else
            {
                teamCounter = 0;
            }

            // Write player data and tell other connected clients about this client player
            using (DarkRiftWriter newPlayerWriter = DarkRiftWriter.Create())
            {
                newPlayerWriter.Write(newPlayer);

                using (Message newPlayerMessage = Message.Create(Tags.SpawnPlayerTag, newPlayerWriter))
                {
                    foreach (IClient client in ClientManager.GetAllClients().Where(x => x != e.Client))
                    {
                        client.SendMessage(newPlayerMessage, SendMode.Reliable);
                    }
                }
            }

            // Add new player to player dict
            players.Add(e.Client, newPlayer);

            // Tell the client player about all connected players
            foreach (Player player in players.Values)
            {
                Message playerMessage = Message.Create(Tags.SpawnPlayerTag, player);
                e.Client.SendMessage(playerMessage, SendMode.Reliable);
            }

            // When this client sends a message, we should also fire the movement handler
            e.Client.MessageReceived += MovementMessageReceived;
            e.Client.MessageReceived += SpellMessageReceived;
            e.Client.MessageReceived += StatsMessageReceived;
        }

        void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            players.Remove(e.Client);

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(e.Client.ID);

                using (Message message = Message.Create(Tags.DespawnPlayerTag, writer))
                {
                    foreach (IClient client in ClientManager.GetAllClients())
                    {
                        client.SendMessage(message, SendMode.Reliable);
                    }
                }
            }
        }

        void StatsMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage() as Message)
            {
                if (message.Tag == Tags.ApplyEffectTag)
                {
                    using (DarkRiftReader reader = message.GetReader())
                    {
                        ushort requesterID = reader.ReadUInt16();
                        ushort receiverID = reader.ReadUInt16();
                        string stat = reader.ReadString();
                        float amount = reader.ReadSingle();

                        Player receivingPlayer = players[ClientManager.GetClient(receiverID)];
                        receivingPlayer.stats[stat] += amount;

                        using (DarkRiftWriter writer = DarkRiftWriter.Create())
                        {
                            writer.Write(requesterID);
                            writer.Write(receiverID);
                            writer.Write(stat);
                            writer.Write(amount);

                            foreach (IClient c in ClientManager.GetAllClients())
                                c.SendMessage(message, SendMode.Reliable);
                        }
                    }
                }
            }
        }

        void SpellMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage() as Message)
            {
                if (message.Tag == Tags.SpellPlayerTag)
                {
                    using (DarkRiftReader reader = message.GetReader())
                    {
                        string spellName = reader.ReadString();
                        string command = reader.ReadString();
                        
                        using (DarkRiftWriter writer = DarkRiftWriter.Create())
                        {
                            writer.Write(e.Client.ID);
                            writer.Write(spellName);
                            writer.Write(command);
                            message.Serialize(writer);
                        }

                        foreach (IClient c in ClientManager.GetAllClients().Where(x => x != e.Client))
                            c.SendMessage(message, SendMode.Reliable);
                    }
                }
            }
        }

        void MovementMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage() as Message)
            {
                if (message.Tag == Tags.MovePlayerTag)
                {
                    using (DarkRiftReader reader = message.GetReader())
                    {
                        // what updated position did we receive?
                        float newX = reader.ReadSingle();
                        float newY = reader.ReadSingle();
                        float newZ = reader.ReadSingle();

                        float rotX = reader.ReadSingle();
                        float rotY = reader.ReadSingle();
                        float rotZ = reader.ReadSingle();
                        float rotW = reader.ReadSingle();

                        float lookX = reader.ReadSingle();
                        float lookY = reader.ReadSingle();
                        float lookZ = reader.ReadSingle();

                        float animSpeedX = reader.ReadSingle();
                        float animSpeedY = reader.ReadSingle();

                        // update specified player with this information
                        Player player = players[e.Client];

                        player.X = newX;
                        player.Y = newY;
                        player.Z = newZ;

                        player.rotX = rotX;
                        player.rotY = rotY;
                        player.rotZ = rotZ;
                        player.rotW = rotW;

                        player.lookX = lookX;
                        player.lookY = lookY;
                        player.lookZ = lookZ;

                        // send this player's updated position back to all clients except the client that sent the message
                        using (DarkRiftWriter writer = DarkRiftWriter.Create())
                        {
                            writer.Write(player.ID);
                            writer.Write(player.X);
                            writer.Write(player.Y);
                            writer.Write(player.Z);
                            writer.Write(player.rotX);
                            writer.Write(player.rotY);
                            writer.Write(player.rotZ);
                            writer.Write(player.rotW);
                            writer.Write(player.lookX);
                            writer.Write(player.lookY);
                            writer.Write(player.lookZ);

                            writer.Write(animSpeedX);
                            writer.Write(animSpeedY);

                            message.Serialize(writer);
                        }

                        foreach (IClient c in ClientManager.GetAllClients().Where(x => x != e.Client))
                            c.SendMessage(message, e.SendMode);
                    }
                }
            }
        }
    }
}
