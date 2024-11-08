using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMaze : MonoBehaviour
{
    [SerializeField]
    GameObject roomPrefab;

    [SerializeField]
    GameObject goalPrefab; // Prefab del sprite con el tag "Goal"

    // The grid.
    Room[,] rooms = null;

    [SerializeField]
    int numX = 10;
    [SerializeField]
    int numY = 10;

    // The room width and height.
    float roomWidth;
    float roomHeight;

    // The stack for backtracking.
    Stack<Room> stack = new Stack<Room>();

    bool generating = false;

    private void GetRoomSize()
    {
        SpriteRenderer[] spriteRenderers =
            roomPrefab.GetComponentsInChildren<SpriteRenderer>();

        Vector3 minBounds = Vector3.positiveInfinity;
        Vector3 maxBounds = Vector3.negativeInfinity;

        foreach (SpriteRenderer ren in spriteRenderers)
        {
            minBounds = Vector3.Min(
                minBounds,
                ren.bounds.min);

            maxBounds = Vector3.Max(
                maxBounds,
                ren.bounds.max);
        }

        roomWidth = maxBounds.x - minBounds.x;
        roomHeight = maxBounds.y - minBounds.y;
    }

    private void SetCamera()
    {
        Camera.main.transform.position = new Vector3(
            numX * (roomWidth - 1) / 2,
            numY * (roomHeight - 1) / 2,
            -100.0f);

        float min_value = Mathf.Min(numX * (roomWidth - 1), numY * (roomHeight - 1));
        Camera.main.orthographicSize = min_value * 0.75f;
    }

private void Start()
{
    GetRoomSize();

    rooms = new Room[numX, numY];

    for (int i = 0; i < numX; ++i)
    {
        for (int j = 0; j < numY; ++j)
        {
            GameObject room = Instantiate(roomPrefab,
                new Vector3(i * roomWidth, j * roomHeight, 0.0f),
                Quaternion.identity);

            room.name = "Room_" + i.ToString() + "_" + j.ToString();
            rooms[i, j] = room.GetComponent<Room>();
            rooms[i, j].Index = new Vector2Int(i, j);
        }
    }

    SetCamera();

    // Genera el laberinto automáticamente al iniciar la escena
    CreateMaze();

    // Coloca al jugador fuera del laberinto, por ejemplo, en (0, -2) (por debajo de la entrada)
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
        // Posición inicial del jugador fuera del laberinto, por debajo de la entrada
        Vector3 playerStartPosition = new Vector3(0 * roomWidth, -0.5f * roomHeight, 0f);
        player.transform.position = playerStartPosition;
    }
    else
    {
        Debug.LogError("No se encontró el objeto con el tag 'Player'.");
    }

    // Coloca el Goal fuera del final del laberinto
    GameObject goal = GameObject.FindGameObjectWithTag("Goal");
    if (goal != null)
    {
        // Coloca el Goal justo afuera de la última sala (en el borde derecho)
        Vector3 goalPosition = new Vector3((numX -1) * roomWidth + roomWidth / 2, 
                                            (numY - 1.5f) * roomHeight + roomHeight / 2 + 1.0f,  // Ajuste en Y para acercarlo
                                            0f);

        goal.transform.position = goalPosition;
    }
    else
    {
        Debug.LogError("No se encontró el objeto con el tag 'Goal'.");
    }
}

    private void PlaceGoal()
    {
        // Se coloca fuera de la última habitación, en este caso, por encima de la última celda
        Vector3 goalPosition = new Vector3((numX - 1) * roomWidth + roomWidth / 2, numY * roomHeight + roomHeight / 2, 0);

        // Instanciamos el sprite con el tag "Goal" en esa posición
        Instantiate(goalPrefab, goalPosition, Quaternion.identity);
    }

    private void RemoveRoomWall(
        int x,
        int y,
        Room.Directions dir)
    {
        if (dir != Room.Directions.NONE)
        {
            rooms[x, y].SetDirFlag(dir, false);
        }

        Room.Directions opp = Room.Directions.NONE;
        switch (dir)
        {
            case Room.Directions.TOP:
                if (y < numY - 1)
                {
                    opp = Room.Directions.BOTTOM;
                    ++y;
                }
                break;
            case Room.Directions.RIGHT:
                if (x < numX - 1)
                {
                    opp = Room.Directions.LEFT;
                    ++x;
                }
                break;
            case Room.Directions.BOTTOM:
                if (y > 0)
                {
                    opp = Room.Directions.TOP;
                    --y;
                }
                break;
            case Room.Directions.LEFT:
                if (x > 0)
                {
                    opp = Room.Directions.RIGHT;
                    --x;
                }
                break;
        }
        if (opp != Room.Directions.NONE)
        {
            rooms[x, y].SetDirFlag(opp, false);
        }
    }

    public List<Tuple<Room.Directions, Room>> GetNeighboursNotVisited(
        int cx, int cy)
    {
        List<Tuple<Room.Directions, Room>> neighbours =
            new List<Tuple<Room.Directions, Room>>();

        foreach (Room.Directions dir in Enum.GetValues(
            typeof(Room.Directions)))
        {
            int x = cx;
            int y = cy;

            switch (dir)
            {
                case Room.Directions.TOP:
                    if (y < numY - 1)
                    {
                        ++y;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(Room.Directions.TOP, rooms[x, y]));
                        }
                    }
                    break;
                case Room.Directions.RIGHT:
                    if (x < numX - 1)
                    {
                        ++x;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(Room.Directions.RIGHT, rooms[x, y]));
                        }
                    }
                    break;
                case Room.Directions.BOTTOM:
                    if (y > 0)
                    {
                        --y;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(Room.Directions.BOTTOM, rooms[x, y]));
                        }
                    }
                    break;
                case Room.Directions.LEFT:
                    if (x > 0)
                    {
                        --x;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(Room.Directions.LEFT, rooms[x, y]));
                        }
                    }
                    break;
            }
        }
        return neighbours;
    }

    private bool GenerateStep()
    {
        if (stack.Count == 0) return true;

        Room r = stack.Peek();
        var neighbours = GetNeighboursNotVisited(r.Index.x, r.Index.y);

        if (neighbours.Count != 0)
        {
            var index = 0;
            if (neighbours.Count > 1)
            {
                index = UnityEngine.Random.Range(0, neighbours.Count);
            }

            var item = neighbours[index];
            Room neighbour = item.Item2;
            neighbour.visited = true;
            RemoveRoomWall(r.Index.x, r.Index.y, item.Item1);

            stack.Push(neighbour);
        }
        else
        {
            stack.Pop();
        }

        return false;
    }

    public void CreateMaze()
    {
        Debug.Log("Maze generating...");
        if (generating) return;

        Reset();

        RemoveRoomWall(0, 0, Room.Directions.BOTTOM);
        RemoveRoomWall(numX - 1, numY - 1, Room.Directions.RIGHT);

        stack.Push(rooms[0, 0]);

        generating = true;

        // Genera el laberinto sin esperar.
        while (!GenerateStep())
        {
            // Continúa generando hasta completar el laberinto
        }

        generating = false;
        Debug.Log("Maze generated.");
    }

    private void Reset()
    {
        for (int i = 0; i < numX; ++i)
        {
            for (int j = 0; j < numY; ++j)
            {
                rooms[i, j].SetDirFlag(Room.Directions.TOP, true);
                rooms[i, j].SetDirFlag(Room.Directions.RIGHT, true);
                rooms[i, j].SetDirFlag(Room.Directions.BOTTOM, true);
                rooms[i, j].SetDirFlag(Room.Directions.LEFT, true);
                rooms[i, j].visited = false;
            }
        }
    }
}
