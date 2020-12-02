using System;
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
        public PlayerManager(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {

        }

        public override bool ThreadSafe => false;

        public override Version Version => new Version(1, 0, 0);
    }
}
