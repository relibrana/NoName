using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum Directions
    {
        TOP,
        RIGHT,
        BOTTOM,
        LEFT,
        NONE,
    }

    [SerializeField]
    GameObject topWall;
    [SerializeField]
    GameObject rightWall;
    [SerializeField]
    GameObject bottomWall;
    [SerializeField]
    GameObject leftWall;

    Dictionary<Directions, GameObject> walls = new Dictionary<Directions, GameObject>();

    public Vector2Int Index { get; set; }
    public bool visited { get; set; } = false;

    Dictionary<Directions, bool> dirflags = new Dictionary<Directions, bool>();

    private void Awake()
    {
        // Inicializa las paredes en el diccionario walls
        walls[Directions.TOP] = topWall;
        walls[Directions.RIGHT] = rightWall;
        walls[Directions.BOTTOM] = bottomWall;
        walls[Directions.LEFT] = leftWall;

        // Inicializa dirflags con todas las direcciones como activas
        dirflags[Directions.TOP] = true;
        dirflags[Directions.RIGHT] = true;
        dirflags[Directions.BOTTOM] = true;
        dirflags[Directions.LEFT] = true;
    }

    private void SetActive(Directions dir, bool flag)
    {
        if (walls.ContainsKey(dir))
        {
            walls[dir].SetActive(flag);
        }
    }

    public void SetDirFlag(Directions dir, bool flag)
    {
        dirflags[dir] = flag;
        SetActive(dir, flag);
    }
}
