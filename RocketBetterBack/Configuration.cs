using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rocket.API;

namespace RocketBetterBack
{
    public class Configuration : IRocketPluginConfiguration
    {
        public byte Delay;

        public void LoadDefaults()
        {
            Delay = 5;
        }
    }
}
