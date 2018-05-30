using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;

namespace RocketBetterBack
{
    public class BackCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "Back";
        public string Help => "Returns you to the position of your death";
        public string Syntax => "/back";

        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>()
        {
            "rocketbetterback.back"
        };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            RocketBetterBack.BackRequest request = RocketBetterBack.Instance.Requests.FirstOrDefault(a => a.Player == player.Player);

            if(request != null)
            {
                byte length = (byte)(RocketBetterBack.Config.Delay - (DateTime.Now - request.Executed).TotalSeconds);

                UnturnedChat.Say(player, RocketBetterBack.Instance.Translate("teleporting", length));
                return;
            }
            if (!RocketBetterBack.Instance.LastPosition.ContainsKey(player.Player))
            {
                UnturnedChat.Say(player, RocketBetterBack.Instance.Translate("no_location"));
                return;
            }

            RocketBetterBack.Instance.Requests.Add(new RocketBetterBack.BackRequest(player.Player, RocketBetterBack.Instance.LastPosition[player.Player]));
            UnturnedChat.Say(player, RocketBetterBack.Instance.Translate("teleporting", RocketBetterBack.Config.Delay));
        }
    }
}
