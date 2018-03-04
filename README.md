Hearts

Online desktop version of the classic card game Hearts.
You can play a single player version, and read rules for example here: https://cardgames.io/hearts/
Project is written in C# with WPF using MVVM pattern and a little bit of MVVM Light Library.
Clients and server are connected via TCP protocol. Server's and client's operations are working in asynchronous model
with async/await keywords and some basics of Task Parallel Library.

This is my first bigger application with online connection, I had to get to know TCP and UDP protocols and decide which to use
in my case, I chose TCP, it can be a little bit more safe than UDP, client-server model is an obvious choice here, as I needed
server role to handle game rules, read and send messages from/to clients, client's apps are just aware of things that server is "willing" to tell them, peer to peer in my opinion wouldn't fit here.

It was also some kind of test for me, to check my MVVM knowladge in  kind of simple "real life" application.
I'm now learning about testing software while developing it, so I think in further projects I need to focus more on that field,
and use some of testing paradigms.

MVVM promotes, separation of concerns and after while, I can defenitely say that not everything in that project is seperated
enough, also I broke some of the SOLID rules there, but looking form afar, I learned a lot, so in further projects I can fix these mistakes.
