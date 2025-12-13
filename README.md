# Kartographer

## Game Concept

Kartographer is a co-op extraction-survival fusion game about driving and upgrading a golf cart through a desert wasteland. Players will drive past abandoned buildings where they can stop and enter the buildings to find various parts to either repair or upgrade their cart. As players drive, they will unlock new parts of the map. Eventually, players will pass by extraction points where they can choose to extract and save their cart upgrades and map progress, trade upgrades, or continue driving and risk losing all their progress. However, players must continue driving to outrun the dangerous storm.

___

- [Game Design Document](https://docs.google.com/document/d/14KN_Ifoyv4o1aBq00-vxLoxY2Le4OYspcUA4Ot9exIQ/edit?usp=sharing)

- [Development Timeline](https://github.com/users/Helena-Crowley/projects/1/views/4)

- [Art Board (Temp)](https://docs.google.com/drawings/d/1Wy0cIlKC2e3cyBW-VD-Q6V8-XV-B-d2-cxhqyY7yAwY/edit?usp=sharing)

___

## Setup
- Download [*Kartographer_Demo.zip*](https://drive.google.com/file/d/13usy_mjAU0DBko7Iqkp8fn7ulhdvr4mC/view)
- Extract all files
- Inside folder *Kartographer_Demo*, double click *Kartographer.exe*
- Have fun playing!!

## Features
### Scary Enemy AI
When explorers leave their cart, there is a chance that an enemy Wendigo will spawn.
The Wendigo does not always attack the player, but when it does, explorers must get in their cart and outrun it or be subject to damage. 

### Physics-based driving
The golf cart has realistic friction, sliding feedback, and spring based suspension. 
The golf cart wheels are removable and effect the cart when removed/put on.
Sound effects for starting the cart, sliding on the sand, and slowing/speeding up contribute to an immersive driving experience.

### Building Scanning Effect
LiDar style visual effects help explorers visualize progress for scanning buildings. 
When buildings are scanned, their data is sent to the company via drone.

### Inventory System
Explorers can collect scrap they find in the buildings and store them in their inventory. 
Scrap can be picked up, dropped, or sold for coins by using the recycler. 

## Some Mechanics
### Risk–Reward Exploration
Leaving the cart allows explorers to scan buildings and collect scrap
Being outside the cart increases the chance of triggering a Wendigo encounter
Explorers must decide when it’s worth the risk to explore on foot

### Scan Progression Mechanic
Buildings require time to complete scans
LiDAR visuals indicate scan progress and completion
Completed scans trigger drone deployment and progress the game

### Chase & Escape Mechanic
When the Wendigo attacks, explorers must return to the cart to escape
Survival depends on driver technique
Failing to escape results in player damage or player death
