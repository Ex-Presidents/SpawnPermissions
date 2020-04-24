using Rocket.API;
using Rocket.API.Collections;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Commands;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace SpawnPermissions
{
    public class SpawnPermissions : RocketPlugin<Configuration>
    {
        public static SpawnPermissions Instance;

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "vehicle_denied", "You do not have permission to spawn vehicle ID: {0} ({1})" },
            { "item_denied", "You do not have permission to spawn item ID: {0} ({1})" }
        };

        protected override void Load()
        {
            Logger.Log("If you need help please go to https://github.com/Ex-Presidents/SpawnPermissions", ConsoleColor.Yellow);
            Logger.Log("There is documentation available and you can create issues if necessary.", ConsoleColor.Yellow);

            Instance = this;
        }

        protected override void Unload()
        {
            base.Unload();
        }

        public bool CheckPermission(bool Item, ushort ID, UnturnedPlayer Player, out bool WaiveCooldowns)
        {
            // Yes this is unnecessary
            WaiveCooldowns = Instance.Configuration.Instance.WaiveCooldowns;

            // Check if OverrideAdmin is enabled
            bool OverrideAdmin = Instance.Configuration.Instance.OverrideAdmin;

            // If the player has permission to bypass restrictions, return true
            if (HasPermission(Player, "spawnpermissions.bypass", OverrideAdmin))
                return true;


            if (!Item)
            {
                // If the vehicle restrictions are disabled, return true
                if (!Instance.Configuration.Instance.EnableVehicleBlacklist && !Instance.Configuration.Instance.EnableVehicleWhitelist)
                    return true;

                // If vehicle blacklisting is enabled, and the player has the ID blacklisted in permissions, return false
                if (Instance.Configuration.Instance.EnableVehicleBlacklist)
                    if (HasPermission(Player, $"spawnpermissions.v.blist.{ID}", OverrideAdmin))
                        return false;

                // If vehicle whitelisting is enabled, and the player doesn't have the ID whitelisted in permissions, return false
                if (Instance.Configuration.Instance.EnableVehicleWhitelist)
                    if (!HasPermission(Player, $"spawnpermissions.v.wlist.{ID}", OverrideAdmin))
                        return false;

                return true;
            }
            else
            {
                // If item spawning restrictions are disabled, return true
                if (!Instance.Configuration.Instance.EnableItemBlacklist && !Instance.Configuration.Instance.EnableItemWhitelist)
                    return true;

                // If item blacklisting is enabled, and the player has the ID blacklisted in permissions, return false
                if (Instance.Configuration.Instance.EnableItemBlacklist)
                    if (HasPermission(Player, $"spawnpermissions.i.blist.{ID}", OverrideAdmin))
                        return false;

                // If item whitelisting is enabled, adn the player doesn't have the ID whitelisted in permissions, return false
                if (Instance.Configuration.Instance.EnableItemWhitelist)
                    if (!HasPermission(Player, $"spawnpermissions.i.wlist.{ID}", OverrideAdmin))
                        return false;

                return true;
            }
        }

        public bool HasPermission(UnturnedPlayer Player, string Permission, bool OverrideAdmin)
        {
            // If OverrideAdmin is disabled, we can just use the original function
            if (!OverrideAdmin)
                return Player.HasPermission(Permission);

            // If not we can actually check if they have permission
            List<Permission> applyingPermissions = R.Permissions.GetPermissions(Player, Permission);

            return applyingPermissions.Count != 0;
        }

        // DeleteCooldownThread and DeleteCooldown are never actually used
        // I wrote them before I realized you can just throw an exception for the same result
        // Maybe it will be useful to whoever is reading this lol
        public void DeleteCooldownThread(IRocketPlayer Player, IRocketCommand Command)
        {
            Thread Thread = new Thread(placeholder => DeleteCooldown(Player, Command));
            Thread.Start();
        }

        private void DeleteCooldown(IRocketPlayer Player, IRocketCommand Command)
        {
            // Wait a second for Rocket to set the cooldown
            // A second is generous, but if someone is sending commands faster than 1/second they are going too fast
            Thread.Sleep(1000);

            // This is the list where cooldowns are stored. We have to use reflection because it's internal
            FieldInfo cooldown = typeof(RocketCommandManager).GetField("cooldown", BindingFlags.NonPublic | BindingFlags.Instance);

            // We have to cast it to a generic List because there are no types with reflection
            IList newCooldowns = (IList)cooldown.GetValue(R.Commands);

            // I guess we have to start at the top of the list. Not sure
            // https://stackoverflow.com/a/16606706
            for (int i = newCooldowns.Count - 1; i >= 0; i--)
            {
                // Element = RocketCommandCooldown
                // https://github.com/RocketMod/Rocket/blob/legacy/Rocket.Core/Commands/RocketCommandCooldown.cs
                object Element = newCooldowns[i];

                // Finally we can get the "Command" property of RocketCommandCooldown and cast it
                IRocketCommand ElementCommand = (IRocketCommand)Element.GetType().GetField("Command", BindingFlags.Instance | BindingFlags.Public).GetValue(Element);

                // If the name of the command that has a cooldown matches the one we are trying to remove, remove it from our new cooldown list
                if (ElementCommand.Name == Command.Name)
                    newCooldowns.RemoveAt(i);
            }

            // Replace Rocket's cooldown list with the new one that has the cooldown removed
            cooldown.SetValue(R.Commands, newCooldowns);
        }
    }
}
