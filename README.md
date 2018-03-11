# LanguageServerRobot

For the [TypeCobol project](https://github.com/TypeCobolTeam/TypeCobol), its source code editing plugin under Eclipse RDz and in the implementation of the completion functionality, intelligent editing using the [Microsoft Language Server protocol](https://github.com/Microsoft/language-server-protocol); it is essential to have a means of recording and carrying out tests in order to:



- Validate the implementation of the protocol. 



- Validate intelligent editing features by testing the exchanged data, when executing queries and notifications using the JSON data representation format.

# Architecture overview
## Projects
The solution contains 5 projects- **TypeCobol.LanguageServer.JsonRPC** is a C# class library that implements [JSON RPC](https://en.wikipedia.org/wiki/JSON-RPC) protocol based on the Producer/Consumer Pattern.- **TypeCobol.LanguageServer.Protocol** is a C# class library that implements [Microsoft Language Server](https://github.com/Microsoft/language-server-protocol) protocol's types and signatures.- **TypeCobol.LanguageServer.Robot.Common** is a C# class library that implements core and common Labguage Server Robot features.- **TypeCobol.LanguageServerRobot** is a C# implementation of the Language Server Robot(LSR) application.- **TypeCobol.LanguageServer.Robot.Monitor** is a C#/WPF application for monitoring LSR sessions and scenario recording and replay.
## Projects Dependencies
