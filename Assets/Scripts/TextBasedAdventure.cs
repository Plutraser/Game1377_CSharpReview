using UnityEngine;

public class TextBasedAdventure : MonoBehaviour
{
    enum TileType
    {
        Invalid,
        Empty,
        Item,
        Enemy,
        Exit,
    }

    string[,] tileNames = { { "Dark Cave",      "Mossy Tunnel",     "Crystal Room" },
                            { "Bone Chamber",   "Flooded Hall",     "Iron Gate"    },
                            { "Goblin Den",     "Armory",           "Throne Room"  }
                            };

    TileType[,] tileTypes = {{ TileType.Empty, TileType.Item,  TileType.Empty},
                             { TileType.Enemy, TileType.Empty, TileType.Exit},
                             { TileType.Empty, TileType.Enemy, TileType.Item }
                            };

    int playerRow = 0;
    int playerCol = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("You are in: " + tileNames[playerRow, playerCol]);
    }

    // Update is called once per frame
    void Update()
    {
        int newRow = playerRow;
        int newCol = playerCol;
        if(Input.GetKeyDown(KeyCode.D))
        {
            newRow++;
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            newRow--;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            newCol++;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            newCol--;
        }
        else
        {
            return;
        }
        if ((newRow >= 0 && newRow < tileNames.GetLength(0)) && (newCol >= 0 && newCol < tileNames.GetLength(1)))
        {
            playerRow = newRow;
            playerCol = newCol;
        }
        else
        {
            Debug.Log("Can't go that way");
        }
        Debug.Log("You are in: " + tileNames[playerRow, playerCol]);

        switch (tileTypes[playerRow, playerCol])
        {
            case TileType.Empty:
                Debug.Log("There is nothing here.");
                break;
            case TileType.Enemy:
                Debug.Log("Oooo a spooky ghost");
                break;
            case TileType.Item:
                Debug.Log("You see a shiny object");
                break;
            case TileType.Exit:
                Debug.Log("You see a way out");
                break;
            default:
                Debug.LogError("Invalid TileType");
                break;
        }
    }
}
