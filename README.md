Instructions to use the Maze Generator:

1] Create a Maze Generator Data scriptable object and enter the fields of data(These prefabs will be used as templates to generate the maze). Right click in Assets-> Create -> 
2] Populating the trophy prefab has been deactivated for now, as for the game's sense, the trophy was placed inside the room to add challenge to the player.
3] After the creation of Maze Generator data File, from the File Menu select Custom Tools -> Maze Generator to open the tool. 
4] Add the Maze Generator Data file to the tool and hit on create. 
5] Ensure to add your player and camera to place in the level (Assigning player location is not handled on purpose to ensure more control of the level)
6] Open Window -> AI -> Navigation window, select Bake-> and Bake to create the Navmesh. AI Experimental package was used before for runtime navmesh generation, but it was causing the navmesg agent to become corrupt at times, hence the code for it has been commented out. The Baked Agent Size settings are given below to set:
 
7] Play the Game, if you create the build, ensure to add Menu Scene at Index 0 (currently in the settings), the Menu scene has Scene Manager which handles scene/level restart component
# MazeRunner
