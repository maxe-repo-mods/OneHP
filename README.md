# OneHP

Forces every player's max health to 1 (configurable). Any damage kills.

R.E.P.O. BepInEx mod. Host-side only.

## Features

- Max health and current health of every player are forced to the configured value (default 1)
- Healing (health packs, truck heal, revive) cannot raise health above the limit
- Health upgrades are overridden back to the limit
- Only the host needs the mod — other players are affected even without it

## How it works

Each player's client owns its own health. The host sends the game's own `UpdateHealthRPC`
(which the host is allowed to send for any player) to set max health and health on every
client. Once max health is the limit, the game's own heal logic clamps to it.

## Installation

Requires [BepInEx 5.x](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/).

Place `OneHP.dll` in `BepInEx/plugins/`.

Configuration file is generated at `BepInEx/config/maxenterme.OneHP.cfg` on first launch.

## Configuration

Settings can also be changed in-game with [REPOConfig](https://thunderstore.io/c/repo/p/nickklmao/REPOConfig/)
and take effect within a second. Turning `Enabled` off stops forcing health; players keep their
current values until the next level loads their normal max health.

| Section | Key | Default | Range | Description |
|---------|-----|---------|-------|-------------|
| General | Enabled | true | - | Force max health on every player |
| General | MaxHealth | 1 | 1-100 | Max health forced on every player |

## Build

```bash
dotnet build -c Release
```
