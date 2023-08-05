# Tower defense prototype

This game was created in Unity using Mirror and Steamworks API extensions. It can be played in either singleplayer or multiplayer mode. Multiplayer is only possible through Steam as application uses their API to host server/lobby.

The core game loop was inspired by Warcraft 3 maps Legion TD and Element TD. Player faces waves of increasingly harder enemies and tries to survive as long as possible. Enemies move through a fixed path, along which player needs to build defenses. Defenses are in form of towers which shoots projectiles at nearby enemies. When player is defeated, final score is calculated and displayed.

## Map editor

Game offers map editor which is accessible from the main menu. In editor players can build their own maps from available blocks, save them and then play on them. If during multiplayer session player selects custom map created in editor, it will be automatically shared with the rest of the players in lobby.

<img src="https://github.com/AndrejVysinsky/tower-defense-3d/assets/59775817/d7ad3fbf-17c7-4586-8df3-9567f926c5c7" width="900">


## Multiplayer

Synchronization between clients is handled through Mirror extension. Every action is validated on server (host) to avoid cheating. Connecting to other players is handled through Steamworks API and can be done by either direct invitations through Steam or using steam lobby id which can be found in lobby screen.

Game offers two multiplayer modes: versus and coop.

<img src="https://github.com/AndrejVysinsky/tower-defense-3d/assets/59775817/962cd5aa-bc76-40d6-a401-88a0f748fbf9" width="900">

### Versus

In versus mode, each player is assigned their own map instance where only they can build. The goal of the game is to be the last one to survive.

<img src="https://github.com/AndrejVysinsky/tower-defense-3d/assets/59775817/591dcd71-15b1-43e6-99be-1e532a021ba1" width="900">

### Coop

In coop mode players share their building area and defend the same path. The goal is the same as in singleplayer mode, and that is to survive as long as possible.

<img src="https://github.com/AndrejVysinsky/tower-defense-3d/assets/59775817/d539ca3b-7511-454d-9a18-396bcae80a67" width="900">


