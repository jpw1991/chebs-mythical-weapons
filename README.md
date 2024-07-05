# Cheb's Mythical Weapons

Cheb's Mythical Weapons adds powerful weapons, eg. Excalibur & the Bow of Apollo, to Valheim. The weapons are found in special locations; eg. Excalibur must be found somewhere in the world and only someone with 100 sword skill can remove it from the stone and use it.

Formerly called Cheb's Sword in the Stone, the mod has been renamed, at the request of the commissioner, due of the addition of the Bow of Apollo and upcoming other mythical weapons.

This mod was commissioned by Discord user Mintymintos. If you would like me to create a mod for you, please get in touch.

## About Me

[![image1](https://imgur.com/Fahi6sP.png)](https://necrobase.chebgonaz.com)
[![image2](https://imgur.com/X18OyQs.png)](https://ko-fi.com/chebgonaz)
[![image3](https://imgur.com/4e64jQ8.png)](https://www.patreon.com/chebgonaz?fan_landing=true)

I'm a YouTuber/Game Developer/Modder who is interested in all things necromancy and minion-related. Please check out my [YouTube channel](https://www.youtube.com/channel/UCPlZ1XnekiJxKymXbXyvkCg) and if you like the work I do and want to give back, please consider supporting me on [Patreon](https://www.patreon.com/chebgonaz?fan_landing=true) or throwing me a dime on [Ko-fi](https://ko-fi.com/chebgonaz). You can also check out my [website](https://necrobase.chebgonaz.com) where I host information on all known necromancy mods, games, books, videos and also some written reviews/guides.

Thank you and I hope you enjoy the mod! If you have questions or need help please join my [Discord](https://discord.gg/BweDFym6sc).

### Bisect Hosting

I'm partnered with [Bisect Hosting](https://bisecthosting.com/chebgonaz) to give you a discount when you use promocode `chebgonaz`.

![bisectbanner](https://www.bisecthosting.com/partners/custom-banners/b2629ae1-293a-4094-9d2d-002d14529a82.webp)

## Reporting Bugs & Requesting Features

If you would like to report a bug or request a feature, the best way to do it (in order from most preferable to least preferable) is:

a) Create an issue on my [GitHub](hhttps://github.com/jpw1991/chebs-mythical-weapons).

b) Write to me on [Discord](https://discord.gg/BweDFym6sc).

## Requirements

- Valheim
- BepInEx
- Jotunn
- Cheb's Valheim Library (included)

## Installation (manual)

- Extract the contents of the `plugins` folder to your BepInEx plugins folder in the Valheim directory.

A correct installation looks like:

```sh
plugins/
├── Translations
├── chebsmythicalweapons
├── chebsmythicalweapons.manifest
├── ChebsMythicalWeapons.dll
├── ChebsValheimLibrary.dll
└── ... other mods ...
```

## Features

Detailed info in the [wiki](https://github.com/jpw1991/chebs-mythical-weapons/wiki). Here's the short version:

- New locations such as a Sword in the Stone or Statue of Apollo are sprinkled throughout the world.
- If the player is worthy (has sufficient skill) they can take the mythical weapon from the location.
- Sword in the Stone
	- Excalibur:
		- Cannot be crafted.
		- Suffers no durability loss.
		- Deals 100 slashing damage and 50 spirit damage.
	- The sword must be found and taken from the stone. It requires 100 sword skill to remove the sword from the stone.
- Statue of Apollo
	- The Bow of Apollo:
		- Cannot be crafted.
		- Deals 90 piercing damage and 40 spirit damage.
	- The bow must be found and taken from the statue. It requires 100 bow skill to be deemed worthy of the bow.
- Blade of Olympus
	+ Crafted weapon.
	+ Deals 100 slashing damage and 50 lightning damage.
	+ Can only be made during a thunderstorm.
	+ One-handed sword.
- Greatsword of Olympus
	+ Crafted weapon.
	+ Deals 140 slashing damage and 50 lightning damage
	+ Can only be made during a thunderstorm.
	+ Two-handed sword.
- Joyce Axe
	+ Carried by custom minotaur enemy.
	+ Deals 100 slashing damage and 50 fire damage.
- Aegis
	+ The shield of Athena
	+ Crafted at the forge.

### Config

**Attention:** To edit the config, the [Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager/releases) is the most user friendly way. This is a separate mod. Please download and install it.

Press **F1** to open the mod's configuration panel.

You can also edit the configs manually. Almost everything can be tweaked to your liking. For a complete list of all configuration options, please look [here](https://github.com/jpw1991/chebs-mythical-weapons/wiki/Configs).

### Known Issues

- Quantity doesn't seem to be exact, rather approximate. This will hopefully be locked down and fixed in time.
- If you use [CLLC](https://valheim.thunderstore.io/package/Smoothbrain/CreatureLevelAndLootControl/), the minotaur might drop a bazillion axes. Check your CLLC configuration to stop it from changing the minotaur's drops. Its drop is set to be one, and only one, axe.

## Source

You can find the github [here](https://github.com/jpw1991/chebs-mythical-weapons).

## Special Thanks

- MintyMintos for commissioning the mod.
- Shawesome for commissioning the Blade of Olympus (and setting it up in Unity).
- [System 6G](https://opengameart.org/users/system-g6) for providing the [Medieval Weapons Pack](https://opengameart.org/content/medieval-weapon-pack) from which I took the Excalibur model.
- [qubodup](https://opengameart.org/users/qubodup) for the [Wisdom Magic Sound Effect Pack](https://opengameart.org/content/wisdom-magic) which I use for the looting sound effect. [CC-BY 3.0](https://creativecommons.org/licenses/by/3.0/) stipulates I must indicate whether changes were made: no changes were made.
- [ArsGothica](https://www.artstation.com/arsgothica) for the Apollo bow model and texture; also for the Blade of Olympus model and texture.
- [Behrtron](https://opengameart.org/users/behrtron) for the [white marble](https://opengameart.org/content/4k-seamless-white-marble-stone-textures-public-domain) texture for the Apollo statue.
- [Yughues](https://opengameart.org/users/yughues) for the [Free Pillars Models](https://opengameart.org/content/free-pillars-models) which I used for the pedestal upon which the Statue of Apollo rests (downscaled textures from HD to low-D to fit into Valheim aesthetic).
- Brandon Morris for the [Minotaur sound effects](https://opengameart.org/content/osare-minotaur-sounds)
- [cggroup](https://assetstore.unity.com/publishers/24906) for the [Minotaur Warriors](https://assetstore.unity.com/packages/3d/characters/minotaur-warriors-80621) pack.

## Changelog

<details>
<summary>2023</summary>

 Date | Version | Notes 
--- | --- | ---
06/07/2024 | 4.5.3 | Fix problems like item costs not updating from config; add highest upgrade to Joyce axe
04/07/2024 | 4.5.2 | Change from requiring Flametal to requiring FlametalNew
03/07/2024 | 4.5.1 | Set up Aegis; add dynamic reflection of config changes to items; add a further upgrade level that takes Flametal
25/04/2024 | 4.5.0 | Prepare for ashlands; upgrade CVL
23/01/2023 | 4.4.0 | Add Aegis for evaluation
15/01/2023 | 4.3.4 | Update CVL and Jotunn
30/11/2023 | 4.3.3 | Fix issue of configs not syncing reliably
12/11/2023 | 4.3.1 | Fix crash when blocking with Apollo bow
08/09/2023 | 4.2.4 | expose Joyce axe's chop damage values to config
06/09/2023 | 4.2.3 | expose Joyce axe's tool tier to config
03/09/2023 | 4.2.2 | expose additional bow of apollo velocity values to config
31/08/2023 | 4.2.1 | update CVL & readme
23/08/2023 | 4.2.0 | update for new valheim patch
21/07/2023 | 4.1.0 | add two special attacks to minotaur; fix blade of olympus to be properly one handed
19/07/2023 | 4.0.0 | Make Blade of Olympus a one handed weapon; add Greatsword of Olympus as 2h weapon with appropriate stats; make dynamic Minotaur icon on minimap; streamline configs
19/07/2023 | 3.0.2 | expose gravity adjustment of Bow of Apollo projectiles to config
16/07/2023 | 3.0.1 | Fix bug of Joyce being craftable; update readme
16/07/2023 | 3.0.0 | Add Minotaur and Joyce axe
13/07/2023 | 2.1.0 | Add Blade of Olympus
06/07/2023 | 2.0.1 | Permit optional removal of projectile gravity from projectiles fired with the Bow of Apollo
05/07/2023 | 2.0.0 | Rename to Cheb's Mythical Weapons; Add bow & statue of Apollo; Add Sun Arrows
30/06/2023 | 1.1.1 | Implement the differing material requirements per level of upgrade as requested
25/06/2023 | 1.1.0 | Expose most sword stats to config
25/06/2023 | 1.0.1 | Permit upgrading of Excalibur
24/06/2023 | 1.0.0 | Release

</details>

