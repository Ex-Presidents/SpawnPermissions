# SpawnPermissions
Free Version of AviAjvi but with a name that makes sense

![](https://i.imgur.com/ZNxWGDs.png)

## Features

- Permissions based
- Translatable
- /i whitelisting
- /i blacklisting
- /v whitelisting
- /v blacklisting
- Works on admins
- Configurable cooldowns
- Not $8

## Config

- EnableVehicleWhitelist
  - If enabled, players will only be able to spawn vehicles that are whitelisted with the correct permission
- EnableVehicleBlacklist
  - If enabled, players can spawn any vehicle except those blacklisted with the correct permission
- EnableItemWhitelist
  - If enabled, players will only be able to spawn specific items that are whitelisted with the correct permission
- EnableItemBlacklist
  - If enabled, players can spawn any item except those blacklisted with the correct permission
- WaiveCooldowns
  - If enabled, players won't have a cooldown for /i or /v when they try to spawn a blacklisted/not whitelisted item or vehicle
- OverrideAdmin
  - If enabled, admins will still have blacklisted/whitelisted items/vehicles unless they have the bypass permission
  
## Note
  
This does override rocket item/vehicle blacklisting. However, it does work with the max spawn limit.
  
## Permissions
  
Replace ID with vehicle id or item id
  
`spawnpermissions.v.blist.{ID}` Causes the vehicle to be blacklisted for the permission group

`spawnpermissions.v.wlist.{ID}` Causes the vehicle to be whitelisted for the permission group

`spawnpermissions.i.blist.{ID}` Causes the item to be blacklisted for the permission group  

`spawnpermissions.i.wlist.{ID}` Causes the item to be whitelisted for the permission group

`spawnpermissions.bypass` Anyone with this permission can spawn any item/vehicle
