Basic Description
------------
- This mod adapts the stat-based learning system from GeneticArtifact to the variant selection system of VarianceAPI
- This doesn't evaluate the performance of each variant, but instead suggests variants based on the stats they have
- With the current math, each variant can be recommended anywhere between half and twice as often (subject to change)
- Note: this mod does NOT require the Artifact of Genetics to be active to function, as it only runs IF the artifact is enabled

How it Works
------------
- Each variant in VarianceAPI has 5 stat multipliers, and 4 of these match GeneticArtifacts' choices
- As the genetic learning system chooses which stats to prioritize, this mod will keep track of how well those match the variants' multipliers
- If it discovers a close match, that variant will have it's spawn rate increased - poor matches will have theirs reduced

Known Issues/Planned Updates
------------
- This is a prototype build of this concept, so balance and polish haven't been taken care of yet
- If you run into any bugs or have feedback, either reach out in the discord or contribute a issue on the github
- Have a better idea for an icon? I'll gladly take it since the current one is not great

Changelog
-----------
```
0.4.0
- Updated for Variance v2.2 internal catalog names

0.3.0
- Updated for VarianceAPI v2 (Nebby my beloved, make the catalog public)
  - Due to the new VAPI structure, the method of adjusting spawn rate isn`t super safe and may break on conflict
- Added config option to set the maximum effect this mod can have on each spawn rate (globally)
  - Default value is 0.5 - variants will be up to 50% more/less common than their base rate
- Fitness evaluation is much stricter now and uses fancier maths for the spawn rate tweaks
  - Simple variants are much more likely to spawn early into runs, but taper off as the algorithm learns
  - Extreme variants are unlikly to spawn early, but can dramatically increase later on

0.2.0
- Added ability to prevent variants from recieving the genetic bonuses it`s standard version would recieve
  - Added configuration option to enable/disable the above (enabled by default)
- Added configuration option to enable/disable the dynamic variant chance tweaking (enabled by default)

0.1.0
- Prototype release, expect things to be broken/unpolished
```

Installation
------------
Place the .dll in Risk of Rain 2\BepInEx\plugins or use a mod manager.

Contact
------------
If you have issues/suggestions leave them on the github as an issue/suggestion or reach out to Rico#6416 on the modding Discord.
