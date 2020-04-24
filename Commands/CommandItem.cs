using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpawnPermissions.Commands
{
    public class CommandItem : IRocketCommand
    {
        #region Boilerplate

        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "i";
        public string Help => "Give yourself an item";
        public string Syntax => "<id> [amount]";
        public List<string> Aliases => new List<string> { "item" };
        public List<string> Permissions => new List<string>
        {
            "rocket.item",
            "rocket.i"
        };

        #endregion

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer Player = (UnturnedPlayer)caller;

            if (command.Length == 0 || command.Length > 2)
            {
                UnturnedChat.Say(Player, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }

            ushort ItemID = 0;
            byte ItemAmount = 1;
            ItemAsset ItemAsset = null;

            string ItemString = command[0].ToString();

            if (!ushort.TryParse(ItemString, out ItemID))
            {
                if (String.IsNullOrEmpty(ItemString.Trim()))
                {
                    UnturnedChat.Say(Player, U.Translate("command_generic_invalid_parameter"));
                    throw new WrongUsageOfCommandException(caller, this);
                }

                List<ItemAsset> sortedAssets = new List<ItemAsset>(Assets.find(EAssetType.ITEM).Cast<ItemAsset>());
                ItemAsset MatchingAsset = sortedAssets.OrderBy(i => i.itemName.Length).FirstOrDefault(i => i.itemName.ToUpperInvariant().Contains(ItemString.ToUpperInvariant()));
                
                if (MatchingAsset == null)
                {
                    UnturnedChat.Say(Player, U.Translate("command_generic_invalid_parameter"));
                    throw new WrongUsageOfCommandException(caller, this);
                }

                ItemID = MatchingAsset.id;
                ItemAsset = MatchingAsset;
            }

            if(ItemAsset == null)
                ItemAsset = (ItemAsset)Assets.find(EAssetType.ITEM, ItemID);

            if (command.Length == 2 && !byte.TryParse(command[1].ToString(), out ItemAmount) || ItemAsset == null)
            {
                UnturnedChat.Say(Player, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }

            if(!SpawnPermissions.Instance.CheckPermission(true, ItemID, Player, out bool WaiveCooldowns))
            {
                UnturnedChat.Say(Player, SpawnPermissions.Instance.Translate("item_denied", ItemID, ItemAsset.itemName));

                if (WaiveCooldowns)
                    throw new WrongUsageOfCommandException(caller, this);

                return;
            }

            if (U.Settings.Instance.EnableItemSpawnLimit && !Player.HasPermission("itemspawnlimit.bypass"))
            {
                if (ItemAmount > U.Settings.Instance.MaxSpawnAmount)
                {
                    UnturnedChat.Say(Player, U.Translate("command_i_too_much", U.Settings.Instance.MaxSpawnAmount));
                    return;
                }
            }

            string ItemName = ItemAsset.itemName;

            if (Player.GiveItem(ItemID, ItemAmount))
            {
                Logger.Log(U.Translate("command_i_giving_console", Player.DisplayName, ItemID, ItemAmount));
                UnturnedChat.Say(Player, U.Translate("command_i_giving_private", ItemAmount, ItemName, ItemID));
            }
            else
            {
                UnturnedChat.Say(Player, U.Translate("command_i_giving_failed_private", ItemAmount, ItemName, ItemID));
            }
        }
    }
}