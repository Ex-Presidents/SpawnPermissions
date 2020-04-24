using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Core.Logging;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace SpawnPermissions.Commands
{
    public class CommandVehicle : IRocketCommand
    {
        #region Boilerplate
        
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "v";
        public string Help => "Give yourself a vehicle";
        public string Syntax => "<id>";
        public List<string> Aliases => new List<string> { "vehicle" };
        public List<string> Permissions => new List<string>
        {
            "rocket.v",
            "rocket.vehicle"
        };

        #endregion

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer Player = (UnturnedPlayer) caller;

            if (command.Length != 1)
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }

            ushort? VehicleID = command.GetUInt16Parameter(0);
            VehicleAsset VehicleMatch = null;
            
            if (!VehicleID.HasValue)
            {
                string VehicleString = command.GetStringParameter(0);

                if (VehicleString == null)
                {
                    UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                    throw new WrongUsageOfCommandException(caller, this);
                }

                VehicleMatch = (VehicleAsset)Assets.find(EAssetType.VEHICLE).FirstOrDefault(veh => veh.name.ToUpperInvariant().Contains(VehicleString.ToUpperInvariant()));

                if(VehicleMatch != null)
                    VehicleID = VehicleMatch.id;

                if (!VehicleID.HasValue)
                {
                    UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                    throw new WrongUsageOfCommandException(caller, this);
                }
            }

            VehicleMatch ??= (VehicleAsset) Assets.find(EAssetType.VEHICLE, VehicleID.Value);

            if(!SpawnPermissions.Instance.CheckPermission(false, VehicleMatch.id, Player, out bool WaiveCooldwons))
            {
                UnturnedChat.Say(caller, SpawnPermissions.Instance.Translate("vehicle_denied", VehicleMatch.id, VehicleMatch.vehicleName));

                if (WaiveCooldwons)
                    throw new WrongUsageOfCommandException(caller, this);

                return;
            }
            
            if (VehicleTool.giveVehicle(Player.Player, VehicleMatch.id))
            {
                Logger.Log(U.Translate("command_v_giving_console", Player.CharacterName, VehicleMatch.id));
                UnturnedChat.Say(caller, U.Translate("command_v_giving_private", VehicleMatch.vehicleName, VehicleMatch.id));
            }
            else
                UnturnedChat.Say(caller, U.Translate("command_v_giving_failed_private", VehicleMatch.vehicleName, VehicleMatch.id));
        }
    }
}