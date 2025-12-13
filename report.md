## Challenges and Fixes
### Multiplayer
This project was originally intended to be co-op, however time constraints limited our ability to fully port all functionality to be multiplayer compatible. Many features do work with two players, however we encountered some issues with maintaining multiplayer syncing and moving players to and from the outpost as well as handling the event where one player dies and the other survives. These issues led us to limit this demo to single player, however we plan to continue development for Kartographer in hopes of polishing and distributing this project. 

### Cactus Physics
When a player hits them with a cart, the cacti randomly placed throughout the map will explode apart. This effect is done by simulating the forces on the cactus pieces to create the explosion in real time. Originally, each cactus was spawned in on start with all necessary components for explosion. This method was extremely resource intensive and required an overhaul. In order to fix this issue, we decided to instantiate the physics-effected duplicate cactus upon contact with the cart, instead of spawning each cactus from the beginning with physics. 

### Cart Physics
The cart took a very long time to create and fine tune. The theory was covered by a youtube video, which we implemented in C# using real spring and friction equations to govern the behaviour of the cart. The cart system was also created to be highly customizable and able to be adapted to any car that suits it. In the current build, there are some issues with the cart suspension that were introduced after the feature to add/remove the wheels was implemented. The cart had an issue in the early stages of the game where it was not properly interacting with jumps and gravity. This issue was resolved by limiting the bounds of the suspension spring, which was causing the cart to be dragged down too abruptly. 

