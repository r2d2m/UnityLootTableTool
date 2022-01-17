# UnityLootTableTool
A data driven loot table editor tool for Unity 3D.
 
## Background

This projects focus was to create a Loot Table Editor, with a custom utility built into the Unity editor in a period of 2 weeks.
The goal was to allow designers to create item drop tables quickly and easily, in a way that allows changes
to be pushed to a CDN and not require costly game updates through an app store.

There is a small demo included that features a basic inventory system, some wandering enemies and the ability to
attack enemies and collect drops.

[Download Release Build Here](https://github.com/GroverErin/UnityLootTableTool/releases/download/v1.0.0/GroverErin_GAP351_FinalBuild_121621.zip)

## Screenshots

Loot Table Editor Tool:
<p align="center">
  <img src="https://github.com/GroverErin/UnityLootTableTool/blob/main/Screenshots/Editor1.png?raw=true" alt="Loot Table Editor"/>
</p>

Gameplay Start:
<p align="center">
  <img src="https://github.com/GroverErin/UnityLootTableTool/blob/main/Screenshots/Gameplay1.png?raw=true" alt="Game Demo"/>
</p>

Item Dropped on Enemy Death:
<p align="center">
  <img src="https://github.com/GroverErin/UnityLootTableTool/blob/main/Screenshots/Gameplay2.png?raw=true" alt="Item Dropped"/>
</p>

Item Collected and Inventory Display:
<p align="center">
  <img src="https://github.com/GroverErin/UnityLootTableTool/blob/main/Screenshots/Gameplay3.png?raw=true" alt="Item Collected"/>
</p>

## Getting Started

When you open the project, the `Initializer` scene should be opened, as the game menus will not function correctly if the
game is run from any other scene (although most functionality will work fine).

The custom editor can be found under `Tools->LootTableEditor` or with the hotkey `Alt+L` within the Unity Editor.

Gameplay controls are provided in a tutorial menu when the game is run.

## Known Bugs

There is a bug when loading tables sometimes that I think occurs due to write protection on the table `JSON` files.
It doesn't happen when loading tables made during the same "session".

Tables currently don't clamp the total item weights to 1.0, so item spawn chances can be rendered invalid. The sum of
item weights in tables should be manually made to be 1.0 or some items may never drop.

Enemies dont animate. This isn't a bug, I just ran out of time on this project to add that little extra polish.
