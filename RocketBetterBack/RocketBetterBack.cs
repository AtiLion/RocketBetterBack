using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rocket.Unturned.Player;
using Rocket.Core.Plugins;

using SDG.Unturned;
using UnityEngine;
using Steamworks;
using Rocket.API.Collections;

namespace RocketBetterBack
{
    public class RocketBetterBack : RocketPlugin<Configuration>
    {
        #region Variables
        private Dictionary<Player, Vector3> _LastPosition = new Dictionary<Player, Vector3>();
        private List<BackRequest> _Requsts = new List<BackRequest>();
        #endregion

        #region Properties
        public static RocketBetterBack Instance { get; private set; }
        public static Configuration Config => Instance.Configuration.Instance;

        public List<BackRequest> Requests => _Requsts;
        public Dictionary<Player, Vector3> LastPosition => _LastPosition;

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "teleporting", "Teleporting you back in {0} seconds" },
            { "no_location", "No previous teleport location has been found" }
        };
        #endregion

        protected override void Load()
        {
            Instance = this;

            Provider.onEnemyConnected += new Provider.EnemyConnected(delegate (SteamPlayer player)
            {
                player.player.life.onHurt += OnHurt;
            });
            Provider.onEnemyDisconnected += new Provider.EnemyDisconnected(delegate (SteamPlayer player)
            {
                if (_LastPosition.ContainsKey(player.player))
                    _LastPosition.Remove(player.player);
                BackRequest req = _Requsts.FirstOrDefault(a => a.Player == player.player);
                if (req != null)
                    _Requsts.Remove(req);
            });
        }

        protected override void Unload()
        {
            Instance = null;
            _LastPosition.Clear();
            _Requsts.Clear();
        }

        #region Unity Functions
        void FixedUpdate()
        {
            if (Requests.Count < 1)
                return;
            if((DateTime.Now - Requests[0].Executed).TotalSeconds >= Config.Delay)
            {
                BackRequest req = Requests[0];

                req.Player.sendTeleport(req.Position, req.Player.look.angle);
                _LastPosition.Remove(req.Player);

                Requests.RemoveAt(0);
            }
        }
        #endregion

        #region Event Functions
        void OnHurt(Player player, byte damage, Vector3 force, EDeathCause cause, ELimb limb, CSteamID killer)
        {
            if(player.life.health <= 0)
            {
                if (_LastPosition.ContainsKey(player))
                    _LastPosition[player] = player.transform.position;
                else
                    _LastPosition.Add(player, player.transform.position);
            }
        }
        #endregion

        #region SubClasses
        public class BackRequest
        {
            public Player Player { get; private set; }
            public Vector3 Position { get; private set; }
            public DateTime Executed { get; private set; }

            public BackRequest(Player player, Vector3 position)
            {
                this.Player = player;
                this.Position = position;
                this.Executed = DateTime.Now;
            }
        }
        #endregion
    }
}
