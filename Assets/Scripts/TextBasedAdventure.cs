using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class TextBasedAdventure : MonoBehaviour
{
    public enum TileType
    {
        Invalid,
        Empty,
        Item,
        Enemy,
        Exit,
        Teleporter,
        Blockade,
    }

    private List<(int Row, int Col)> tilesVisited = new List<(int Row, int Col)>(); 

    private string[,] tileNames = { { "Dark Cave"              /* 0,0 */,  "Mossy Tunnel"         /* 0,1 */,   "Crystal Room"            /* 0,2 */,  "Dev Room"        /* 0,3 */},
                                    { "(Blockade)"            /* 1,0 */,  "Flooded Hall"         /* 1,1 */,   "Iron Gate"               /* 1.2 */,  "Treasure Room"   /* 1,3 */},
                                    { "Goblin Den",          /* 2.0 */   "Armory",              /* 2.1 */     "Throne Room"            /* 2.2 */,  "Furnace Kitchen" /* 2,3 */},
                                    { "Bone Chamber",       /* 3.0 */   "Ritual Room",         /* 3.1 */     "(Blockade)"             /* 3.2 */,  "Laboratory"      /* 3,3 */}
                                    };

    private TileType[,] tileTypes = { { TileType.Empty, TileType.Item,  TileType.Teleporter, TileType.Enemy},
                                      { TileType.Blockade, TileType.Empty, TileType.Exit, TileType.Item},
                                      { TileType.Enemy, TileType.Teleporter, TileType.Item, TileType.Empty},
                                      { TileType.Enemy, TileType.Empty, TileType.Blockade, TileType.Item},
                                      };

    private string[,] tileDescriptions = { { "It's humid, you can hear a faint dripping of water far into the cave.", /* 0,0 */
                                             "The floor is slippery with the moss, the tunnel appears endless.", /* 0,1 */
                                             "Blinding false lights reflect into your eyes making it hard to view the beautiful room.", /* 0,2 */
                                             "A room filled with easter eggs and funny images- a dev room is always needed in a game."}, /* 0,3 */

                                           { "The path is blocked, turn around.", /* 1,0 */
                                             "Your shoes are soaked with the mysterious liquid, a squelch is made with each step.", /* 1,1 */
                                             "A light slips through the bars of the large iron gate, relief overcomes you.", /* 1,2 */
                                             "A chest stands in the middle of the room on a pile of gold and other shimmering objects."},/* 1,3 */

                                           { "It appears to be an abandoned den which seemed to house goblins back in its day.", /* 2,0 */
                                             "You flinch instinctively seeing a body in front of you before realizing it's just an armorstand.", /* 2,1 */
                                             "The room opens into an expansive throne room, a luxurious rug leading up to the throne forward.", /* 2,2 */
                                             "A heavy heat washes over you with the omnious glow of the large furnace."},/* 2,3 */

                                           { "Pale bones decorate the large chamber.", /* 3,0 */
                                             "An omnious hum reverberates around you with the faint red glow from the candles surrounding a drawn symbol on the ground.", /* 3,1 */ 
                                             "The path is blocked, turn around.", /* 3,2 */
                                             "Wires are strewn from the ceiling, way too close to the open containers of chemicals. A broken flickering light makes it difficult to see."} /* 3,3 */
                                           };

    private int playerRow = 0;
    private int playerCol = 0;
    private int playerHealth = 10;
    private int enemyDamage = 1;
    private int itemHealAmount = 2;
    private bool teleporterActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OutputTileInformation();
    }

    // Update is called once per frame
    void Update()
    {
        bool wasKeyPressed = HandleInput(out int newRow, out int newCol);
        if (!wasKeyPressed)
        {
            return;
        }
        SetPlayerPosition(newRow, newCol);
        OutputTileInformation();
    }

    private void OutputTileInformation()
    {
        Debug.Log("You are in: " + tileNames[playerRow, playerCol]);
        if (!tilesVisited.Contains((playerRow, playerCol))) //Checking if the player has visited this tile, if not, the description is shown
        {
            Debug.Log(tileDescriptions[playerRow, playerCol]);
        }
        tilesVisited.Add((playerRow, playerCol)); //Putting the addition of the list here so the player gets the description first THEN it counts as being visited

        switch (tileTypes[playerRow, playerCol])
        {
            case TileType.Empty:
                teleporterActive = false;
                Debug.Log("There is nothing here.");
                break;
            case TileType.Enemy:
                teleporterActive = false;
                Debug.Log("Oooo a spooky ghost");
                EncounterEnemy();
                break;
            case TileType.Item:
                teleporterActive = false;
                Debug.Log("You see a shiny object");
                ItemPickup();
                break;
            case TileType.Exit:
                teleporterActive = false;
                Debug.Log("You see a way out");
                break;
            case TileType.Teleporter:
                Debug.Log("A warping sound is echoing.");
                AtTeleporter(playerRow, playerCol);
                break;
            case TileType.Blockade:
                //Doesnt need the bool because player cant be on it
                Debug.Log("It's blocked off."); //Wont be seen because Player isnt allowed onto tile
                break;
            default:
                Debug.LogError("Invalid TileType");
                break;
        }
    }
    private void AtTeleporter(int Row, int Col)
    {
        teleporterActive = true;
        Debug.Log("Press T to use teleporter.");
    }
    private void EncounterEnemy()
    {
        PlayerTakeDamage(enemyDamage);
    }
    
    private void ItemPickup()
    {
        PlayerHeal(itemHealAmount);
    }

    private void PlayerHeal(int heal)
    {
        playerHealth += heal;
        Debug.Log("You get healed. Your health is now " + playerHealth);
    }

    private void PlayerTakeDamage(int damage)
    {
        playerHealth -= damage;
        Debug.Log("You get hit. Your health is now " + playerHealth);
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Debug.Log("You are dead");
        }
    }

    /// <summary>
    /// Sets the player position to a new row and column position
    /// </summary>
    /// <param name="newRow"></param>
    /// <param name="newCol"></param>
    private void SetPlayerPosition(int newRow, int newCol)
    {
        if (CheckIfNewPositionInTileBounds(newRow, newCol))
        {
            playerRow = newRow;
            playerCol = newCol;
        }
        else
        {
            Debug.Log("Can't go that way");
        }
    }

    /// <summary>
    /// Determine if the new row and column position are within the bounds of the tiles
    /// </summary>
    /// <param name="newRow"></param>
    /// <param name="newCol"></param>
    /// <returns>True if it is within the bounds, false if not</returns>
    private bool CheckIfNewPositionInTileBounds(int newRow, int newCol)
    {
        if ((newRow == 1 && newCol == 0) || (newRow == 3 && newCol == 2)) //This checks tiles (1,0) and (3,2) These are the ones that are blocked.
        {
            return false;
        }
        return (newRow >= 0 && newRow < tileNames.GetLength(0)) && (newCol >= 0 && newCol < tileNames.GetLength(1));
    }

    /// <summary>
    /// Handles the player's input and sets potential new position in the tileNames array
    /// </summary>
    /// <param name="newRow">new row position</param>
    /// <param name="newCol">new column position</param>
    /// <returns>True if an input was pressed, false if not</returns>
    private bool HandleInput(out int newRow, out int newCol)
    {
        bool hasMoved = true;

        newRow = playerRow;
        newCol = playerCol;

        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("You pressed " + KeyCode.D);
            newCol++;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("You pressed " + KeyCode.A);
            newCol--;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("You pressed " + KeyCode.W);
            newRow--;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("You pressed " + KeyCode.S);
            newRow++;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("You look around.");
            Debug.Log(tileDescriptions[playerRow, playerCol]);
            hasMoved = false; //This is because this bool is to check if the player has moved- the player does not move but just looks
        }
        else if (Input.GetKeyDown(KeyCode.T) && teleporterActive == true)
        {
            if (newRow == 0 && newCol == 2) //Teleports player to other teleporter
            {
                newRow = 2;
                newCol = 1;
            }
            else if (newRow == 2 && newCol == 1) //Teleports player to other teleporter
            {
                newRow = 0;
                newCol = 2;
            }
        }
        else
        {
            hasMoved = false;
        }
        return hasMoved;
    }

}
