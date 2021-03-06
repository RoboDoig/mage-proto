﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DarkRift;

namespace MagePlugin
{
    class Player : IDarkRiftSerializable
    {
        public ushort ID { get; set; }
        public ushort teamID { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float rotX { get; set; }
        public float rotY { get; set; }
        public float rotZ { get; set; }
        public float rotW { get; set; }

        public float lookX { get; set; }
        public float lookY { get; set; }
        public float lookZ { get; set; }

        public Dictionary<string, float> stats;

        public Player(ushort id, ushort team, float x, float y, float z)
        {
            ID = id;
            teamID = team;
            X = x;
            Y = y;
            Z = z;

            rotX = 0f;
            rotY = 0f;
            rotZ = 0f;
            rotW = 0f;

            lookX = 0f;
            lookY = 0f;
            lookZ = 0f;

            stats = new Dictionary<string, float>();
            stats.Add("health", 100f);
            stats.Add("mana", 100f);
        }

        public void Deserialize(DeserializeEvent e)
        {
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
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ID);
            e.Writer.Write(teamID);

            e.Writer.Write(X);
            e.Writer.Write(Y);
            e.Writer.Write(Z);

            e.Writer.Write(rotX);
            e.Writer.Write(rotY);
            e.Writer.Write(rotZ);
            e.Writer.Write(rotW);

            e.Writer.Write(lookX);
            e.Writer.Write(lookY);
            e.Writer.Write(lookZ);

            foreach (KeyValuePair<string, float> stat in stats)
            {
                e.Writer.Write(stat.Key);
                e.Writer.Write(stat.Value);
            }
        }
    }
}
