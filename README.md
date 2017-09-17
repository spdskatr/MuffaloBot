# MuffaloBot source code

MuffaloBot is an all-purpose bot for the RimWorld discord and subreddit. It uses the libraries [DSharpPlus](https://github.com/NaamloosDT/DSharpPlus) and [RedditSharp](https://github.com/CrustyJew/RedditSharp).

# How to set up

Build the project in mode `Release`. It should generate a folder "bin" next to your solution folder. Put in the "bin" folder a `config.json` file that looks like:
```json
{
     "disc" : "Your Discord bot token",
     "redd" : "Your Reddit user refresh token (do not use access token)",
     "redd-appid" : "Your Reddit app ID",
     "stea" : "Your steam web-api id used in api search requests."
}
```

If any of these fields are left empty, their respective functions will be disabled.

# MuffaloBotInterface

This console project is for interacting with most Discord components of MuffaloBot. I call it "an open-ended unit tester"