# ETDA   i may never finish this... but i continue chugging away at it...


> Status: Working for USDA Darkages Clients (www.darkages.com) *7.41*
[DarkAges741single.exe] (https://s3.amazonaws.com/kru-downloads/da/DarkAges741single.exe)

![img](http://s32.postimg.org/ok7drfpqd/etda.png)


ETDA, a memory based darkages hunting bot/application... i may never finish this... but i continue chugging away at it...
This Project is still in development, The current release forecast is unknown for the current development roadmap, Feel free to submit contributions to further push this project along the way. currently, The framework is in working condition but is missing many features listed on the roadmap for ETDA. the Bot that uses the framework, BotCore is in early stages of development and has a long way to go, however having said that, it has a lot done, so check out the code and have fun!.
A Memory Based Darkages Hunting Bot.
Status: Working for USDA Darkages Clients (www.darkages.com) 7.41 DarkAges741single.exe
 
I wrote this bot for the challenge of writing a bot without a network based backend. I wanted to develop a concept used in many other games and bring the same approach to darkages.
The advantages of using a memory base backend over a networking layer is very significant. in terms of response time and overall performance, because there is much less overhead retrieving and sending information. For instance, A proxy will need an additional socket layer ontop of your clients socket layer to send and retrieve information, on top of this, it also needs to parse the raw packet bytes, append buffers, then sequentially decrypt them and then handle them again to extract the information, and then encrypt them again and send them back to the clients socket layer, this is a lot of stuff happening, Stuff that is done already by the clients own socket layer, so ultiizing memory, we can bypass all of this and jump straight to the clients own function and call it directly, inline, allowing the client to do the work. I've written plenty of network bots in the past, UDBot (Proboably the First bot ever written), DABasherBot (2004-2006, Private Bot, specifically written for bashers in Nobis), Bot2008, (written in 2006, released 2008), Bot2009 (Released 2009), Wrenbot (Post 2009, never completed) just to give you a history. So While those network bots did a decent job of collecting XP, they were often unreliable for long periods of botting sessions, would unpredictably disconnect, and so the idea behind ETDA is to make it light as possible, and fast as possible and reliable as possible.
This application utilizes ETDA.dll this is a dll written in c++, prior to being injected it hooks the necessary functions used in the DA Client and exposes them so that they can be used or called externally, In some cases, they are merely patched/hooked. and the information detoured to our main application. In this instance (BotCore).
The BotCore manages all the consumer aspects of the Interop Communication that is done via Write/Read Process Memory as i felt small data would not require a real IPC layer. Having said that, my results have been good so i went with this approach.
ETDA sits between the Client, and Acts as an interface between the external Caller and the invokee. ETDA is still a work in progress and Many features will become available as progression and development continues.
Currently ETDA supports the following functionality:


#ETDA Core Features
•	Inject Packet Client/Server ( No encryption / decryption necessary! )
•	Intercept / Filter Packets ( No encryption / decryption necessary! )
•	Chat Handling
•	Freeze free Debugging ( Never get disconnected )

#ETDA Interaction
•	ClickToMove(Point) Invocation
•	SetCursor(Point)
•	WalkTo(Point A, Point B)

#ETDA Overlaying
•	GDI, GDI++ Overlay support (by Hooking End Paint’s device context)
•	Support for a basic HUD overlay, currently only display's name and arc over character.
•	Support for bitmap
•	Text Overlay Support

#BotCore
This bot is in early development.

Current Roadmap
•	  State Machine Based ( Fast Response )
•	  Component Based ( Modular design )
•	  Customizable States, Plugins, Components
•	  No Scripting Necessary
•	  Packet Editor
•	  Every class Supported, Including Subpaths.
•	  Dynamic Path Finding
•	  Dynamic Hunting Routes
•	  Packet Editor Dialog Support

Upcoming RoadMap
•	  Remove all static addresses and replace with patterns
•	  Replace ActiveBar Component so that a pattern scan is not needed.

#credits
•	Shynd, He taught me the basics of a memory based bot years ago and i guess you can say this idea is derived a lot from what he taught me. DA Procuration!
•	ZenLulz, for his fantastic Memorysharp library.
•	Acht/Kyle, I stole his neat RegexMessageHandler code which i found in a public bot.
•	Huy/Jimmy, Thanks for helping with the states and some of the prelim code.
This program is for educational purposes only and all code posted here is public domain. however please give credit where credit is due.


