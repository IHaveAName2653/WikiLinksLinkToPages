# PermissionControl

A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com/).
This mod fixes the resolution of wiki links in-game so they point to the correct wiki page. It fixes the issue of pseudo-generics that point to wiki links that don't exist. Example: Avg_Float2 -> Avg, OR_Multi_Bool -> MultiOR, Add_ColorX_Float -> Add.

## Installation

1. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
1. Place [WikiLinkRedirectionWorking.dll](https://todo) into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create this folder for you.
1. Start the game. If you want to verify that the mod is working you can check your Resonite logs.

## Known Issues

I could not find an easy solution for:

- ComposeBits_*
- ExtractBits_*
- ComposeTRS_*
- Compose_Scale_*
- Compose_Rotation_*
