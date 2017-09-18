# To Do List:
+ Complete Commands documentation

+ Integrate Reddit component
  + ~~Connect successfully~~
  + Basic commands: wshopsearch wikisearch basestats (needs testing - spdskatr)
  + ~~Change command prefix to something better~~ (used double braces `{{ }}`)
+ Add command `xpath` for CoreDefDatabase, ~~hopefully phase out ThingDefDatabase~~ ThingDefDatabase can stay
  + ~~Truncate OuterXML when 100 chars are reached~~
+ Possible command ideas:
  + ~~`xpath <xpath>`: Return the OuterXml of the selected nodes (max of 5?)~~
  + `children <xpath>`: Return the children for selected nodes
  + `assemblysearch <name>`: Searches `Assembly-CSharp` for the specified type/member
+ Misc bugs:

+ Away with the fairies:
  + ThingDef-style command loading