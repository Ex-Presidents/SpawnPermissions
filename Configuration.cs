using Rocket.API;

namespace SpawnPermissions
{
    public class Configuration : IRocketPluginConfiguration
    {
        public bool EnableVehicleWhitelist;
        public bool EnableVehicleBlacklist;
        public bool EnableItemWhitelist;
        public bool EnableItemBlacklist;
        public bool WaiveCooldowns;
        public bool OverrideAdmin;
        
        public void LoadDefaults()
        {
            EnableVehicleWhitelist = false;
            EnableVehicleBlacklist = false;
            EnableItemWhitelist = false;
            EnableItemBlacklist = false;
            WaiveCooldowns = false;
            OverrideAdmin = false;
        }
    }
}