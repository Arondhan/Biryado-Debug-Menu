# Byriado Debug Menu

A small fan-made BepInEx mod that adds a debug menu to the **Biryado** demo (Unity, Mono). Made just for fun, to mess around with artefact combinations.

Press **F1** in game.

> **Disclaimer:** Biryado and all of its content, code, art and trademarks belong to the game's developers (Les Gros Shlags). This is an unofficial, non-commercial mod, not affiliated with or endorsed by them, and it contains no game files. See [LICENSE](LICENSE).

## Features
- **Run tab:** money (add, subtract, set), round +/-1, shots +1, artefact slots (-1, +1, +5), save/load run, restart scene, jump between scenes, game speed 0-5x.
- **Artefacts tab:** every artefact with its icon, rarity and description, search, one-click add.

## Install
1. Download **BepInEx 5 (x64)** from <https://github.com/BepInEx/BepInEx/releases> (`BepInEx_win_x64_5.x.zip`) and extract it into the game folder (next to `Biryado.exe`).
2. Launch the game once, then close it.
3. Put `ByriadoDebugMenu.dll` from [Releases](../../releases) into `BepInEx\plugins`.
4. Launch the game and press F1.

## Uninstall
Delete `ByriadoDebugMenu.dll` from `BepInEx\plugins`. To remove BepInEx completely, delete the `BepInEx` folder and `winhttp.dll`, `doorstop_config.ini`, `.doorstop_version`, `changelog.txt` from the game folder.

## Build
Requires the .NET SDK. Pass your game path as a property:

```
dotnet build -c Release -p:GameDir="C:\...\Biryado Demo"
```

The DLL is copied to `BepInEx\plugins` automatically. The build references the game's own files (`Assembly-CSharp.dll`, `UnityEngine*.dll`); they are not included in this repository.

## Compatibility
Made for demo v0.6 (Unity 2022.3). Game updates may break the mod because it calls the game's internal classes (`GlobalVariables`, `ArtefactManager`, `RunSaveManager`).

## License
Mod source code: MIT. Everything related to the game itself belongs to its developers. Details in [LICENSE](LICENSE).
