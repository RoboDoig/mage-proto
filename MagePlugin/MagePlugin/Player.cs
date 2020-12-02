using System;
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

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float rotX { get; set; }
        public float rotY { get; set; }
        public float rotZ { get; set; }
        public float rotW { get; set; }

        public Player(ushort id, float x, float y, float z)
        {
            
        }

        public void Deserialize(DeserializeEvent e)
        {
            throw new NotImplementedException();
        }

        public void Serialize(SerializeEvent e)
        {
            throw new NotImplementedException();
        }
    }
}
