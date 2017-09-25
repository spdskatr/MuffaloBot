# MuffaloBot Commands Reference

## Discord and Reddit:
Note: For reddit commands, you can call up to 3 of them on the same post. Any commands after that will be ignored. 

| Command | Desc | Discord Example | Reddit Example |
| --- | --- | --- | --- |
| `wikisearch <term>` | Searches the [RimWorld wiki](http://rimworldwiki.com/) for the specified search term | `!wikisearch alpaca wool` | `{{wikisearch alpaca wool}}` |
| `wshopsearch <term>` | Searches the [Steam workshop](http://steamcommunity.com/workshop/browse/?appid=294100) for the specified search item. | `!wshopsearch damage indicators mod` | `{{wshopsearch damage indicators mod}}` |
| `basestats <thingdef>` | Searches ThingDefs for the specified item, and returns its base stats. | `!basestats alpaca wool` | `{{basestats alpaca wool}}` |
| `stuffstats <stuff>` | Searches ThingDefs for the specified item, and returns the stat offsets/factors for things made out of it. | `!stuffstats alpaca wool` | `{{stuffstats alpaca wool}}` |
| `xpath <xpath>` | Selects xpath from the core defs and returns OuterXML of resulting nodes. Case sensitive, so will mess up sometimes when the devs start some fields with caps | `!xpath */ThingDef[defName="WoodLog"]/statBases/MaxHitPoints` | `{{xpath */ThingDef[defName="WoodLog"]/statBases/MaxHitPoints}}` |
| `field <field> <item>` | Selects field from ThingDef item and returns its value | `!field stacklimit chemfuel` | `{{field stacklimit chemfuel}}` |

## Discord only (mostly in-jokes and random things):
| Command | Desc | Discord Example |
| --- | --- | --- |
| `wiki` | Website for the wiki | `!wiki` |
| `muffalo` | *Hello? MuffaloBot? Are you there?* | `!muffalo` |
| `wolfy` | Awoo~ | `!wolfy` |
| `rm -rf /` | I don't care. Muffy's got nothing to hide anyway | `!rm -rf /` |
| `del C:\Windows\System32` | See above | `!del C:\Windows\System32` |
| ??? | Secret command (maybe multiple secret commands?) | ??? | 

## Misc
`{{MuffaloBotIgnore}}` on Reddit tells MuffaloBot to ignore any commands in the post. Aliases: `{{mbi}}`, `{{muffalobotignore}}`, `{{mbignore}}`
