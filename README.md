# ETDA
A Memory Based Darkages Hunting Bot.

![img](http://s32.postimg.org/ok7drfpqd/etda.png)

I wrote this bot for the challenge of writing a bot without a network based backend.
I wanted to develop a concept used in many other games and bring the same approach to darkages.

THe pros of using a memory base over a networking one is very significant.
in terms of response time and overall performance.

> This application ultilizes ETDA.dll
this is a dll written in c++, prior to being injected it hooks the necessary functions used in the DA Client
and exposes them so that they can be used or called externally, In some cases, they are merely patched/hooked.
and the information detoured to our main application. In this instance (BotCore).

The BotCore manages all the consumer aspects of the Interop Communication that is done via Write/Read Process Memory
as i felt small data would not require a real IPC layer. Having said that, my results have been good so i went with this approach.

ETDA sits between the Client, and Acts as an interface between the external Caller and the invokee.
ETDA is still a work in progress and Many features will become available as progression and development continues.

Currently ETDA supports the following functionality:

*ETDA Core Features*
- Inject Packet Client/Server   ( No encryption / decryption neccessary! )
- Intercept / Filter Packets    ( No encryption / decryption neccessary! )
- Chat Handling
- Freeze free Debugging         ( Never get disconnected )


*ETDA Interaction*
- ClickToMove(Point) Invokation
- SetCursor(Point)
- WalkTo(Point A, Point B)

*ETDA Overlaying*
- GDI, GDI++ Overlay support (by Hooking EndPaint's device context)
- Support for a basic HUD overlay, currently only display's name anc arc over character.
- Support for bitmap
- Text Overlay Support


#BotCore

This bot is in early development.

*Current Roadmap*
- [x] State Machine Based   ( Fast Response  )
- [x] Component Based       ( Modular design )
- [x] Customizable States, Plugins, Components
- [x] No Scripting Necessary
- [ ] Every class Supported, Including Subpaths.
- [ ] Dynamic Path Finding
- [ ] Dynamic Hunting Routes

*Upcoming RoadMap*
- [ ] Remove all static addresse and replace with patterns
- [ ] Replace ActiveBar Component so that a pattern scan is not needed.

# credits

- Shynd, He taught me the basics of a memory based bot years ago and i guess you can say this idea is derived alot from what he taught me.
- ZenLulz, for his fantastic Memorysharp library.
- Acht/Kyle, I stole his neat RegexMessageHandler code which i found in a public bot.
- Huy/Jimmy, Thanks for helping with the states and some of the prelim code.

This program is for educational purposes only and all code posted here is public domain.
however please give credit where credit is due.

