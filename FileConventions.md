# Unity file Conventions


## Table of Contents

- [Asset Naming](#asset-naming)
    - [Folders](#folders)
    - [Source code](#source-code)
    - [Non-code assets](#non-code-assets)
- [Directory/File structure](#Directory/File-Structure)
    - [Assets](#assets)
    - [Scripts](#scripts)
    - [Materials / Models / Textures](#Materials-/-Models-/-Textures)
- [Workflow](#workflow)
    - [Model Export / Import](#Model-Export-/-Import)
    - [Textures](#textures)
    - [Configuration files](#configuration-files)
    - [Localization](#localization)
    - [Audio](#audio)

# Asset Naming

First of all, no **spaces** on file or directory names.

## Folders

`PascalCase`

Prefer a deep folder structure over having long asset names.

Directory names should be as concise as possible, prefer one or two words. If a directory name is too long, it probably makes sense to split it into sub directories.

Try to have only one file type per folder. Use `Textures/Trees`, `Models/Trees` and not `Trees/Textures`, `Trees/Models`. That way its easy to set up root directories for the different software involved, for example, Substance Painter would always be set to save to the Textures directory.

If your project contains multiple environments or art sets, use the asset type for the parent directory: `Trees/Jungle`, `Trees/City` not `Jungle/Trees`, `City/Trees`. Since it makes it easier to compare similar assets from different art sets to ensure continuity across art sets.

### Debug Folders

`[PascalCase]`

This signifies that the folder only contains assets that are not ready for production. For example, having an `[Assets]` and `Assets` folder.

## Source Code

Use the naming convention of the programming language. For C# and shader files use `PascalCase`, as per C# convention.

## Non-Code Assets

Use `tree_small` not `small_tree`. While the latter sound better in English, it is much more effective to group all tree objects together instead of all small objects.

`camelCase` where necessary. Use `weapon_miniGun` instead of `weapon_gun_mini`. Avoid this if possible, for example, `vehicles_fighterJet` should be `vehicles_jet_fighter` if you plan to have multiple types of jets.

Prefer using descriptive suffixes instead of iterative: `vehicle_truck_damaged` not `vehicle_truck_01`. If using numbers as a suffix, always use 2 digits. And **do not** use it as a versioning system! Use `git` or something similar.

### Persistent/Important GameObjects

`_snake_case`

Use a leading underscore to make object instances that are not specific to the current scene stand out.

### Debug Objects

`[SNAKE_CASE]`

Enclose objects that are only being used for debugging/testing and are not part of the release with brackets.

# Directory/File Structure

```
Root
+---Assets
+---Build
\---Tools           # Programs to aid development: compilers, asset managers etc.
```

## Assets

```
Assets
+---Art
|   +---Materials
|   +---Models
|   +---Textures
+---Audio
|   +---Music
|   \---Sound       # Samples and sound effects
+---Code
|   +---Scripts     # C# scripts
|   \---Shaders     # Shader files and shader graphs
+---Docs            # Wiki, concept art, marketing material
+---Level           # Anything related to game design in Unity
|   +---Prefabs
|   +---Scenes
|   \---UI
\---Resources       # Configuration files, localization text and other user files.
```

## Scripts

Use namespaces that match your directory structure.

A Framework directory is great for having code that can be reused across projects.

The Scripts folder varies depending on the project, however, `Environment`, `Framework`, `Tools` and `UI` should be consistent  across projects.

```
Scripts
+---Environment
+---Framework
+---NPC
+---Player
+---Tools
\---UI
```

## Materials / Models / Textures

```
Models
+---Characters
|   +---Ash
|   +---Zoe
|   \---...
+---Modular
|   +---Walls
|   +---Stairs
|   +---Floors
|   \---...
\---Props
|   +---Dynamic
|       +---Weapons
|         \---Claws
|   +---Static
|       +---Vase
|       +---Statue
|       \---...
```

# Workflow

## Model Export / Import
### Export from Maya
*   All transformations freezed (if rigged check that there are no rotations)
*   History deleted (if skinned, non-deformer history deleted)
*   Pivot point at “base” and at 0,0,0
*   UV-mapped correctly (no overlap and in correct UDIM unless using trim)
*   Named correctly

### Import to Unity
*   If static
    *   Uncheck animation, rig, and material creation (import separately)
    *   Check “create lightmap UVs”
*   If non-static (character etc.)
    *   Check rig
    *   Uncheck animation and material creation (import separately)
*   If animation
    *   Only import animation, NOTHING ELSE



## Textures

File extension: `PNG`, `TIFF` or `HDR` used.

We are using `Roughness/Metallic` workflow.

### Texture Suffixes

Suffix | Texture
:------|:-----------------
`_AL`  | Albedo
`_R`   | Roughness
`_MT`  | Metallic
`_N`   | Normal
`_H`   | Height
`_DP`  | Displacement
`_EM`  | Emission
`_AO`  | Ambient Occlusion
`_M`   | Mask

### RGBA Masks

It is good practice to use a single texture to combine black and white masks in a single texture split by each RGB channel. Using this, most textures should have:

```
texture_AL.png  # Albedo
texture_N.png   # Normal Map
texture_M.png   # Mask
```

Channel | Rough/Metal
:-------|:----------------------|
R       |   Metallic            | 
G       |   Ambient Occlusion   |
B       |   Detail mask         |
A       |   Smoothness          |

#### The blue channel can vary depending on the type of material:

 - For character materials use the `B` channel for *subsurface opacity/strength*
 - For anisotropic materials use the `B` channel for the *anisotropic direction map*

## Configuration Files

File extension: `INI`

Fast and easy to parse, clean and easy to tweak.

`XML`, `JSON`, and `YAML` are also good alternatives, pick one and be consistent.

Use binary file formats for files that should not be changed by the player.

## Localization

File extension: `CSV`

Widely used by localization software, makes it trivial to edit strings using spreadsheets.

## Audio

File extension: `WAV` while mixing, `OGG` in game.

Preload small sound clips to memory, load on the fly for longer music and less frequent ambient noise files.

---
