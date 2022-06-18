using Assets.Scripts.Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class FieldController : MonoBehaviour
{

    public GroundChessBoard bedrockVoxel;
    public GameObject wallVoxel;
    public GameObject eagleVoxel;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject mainCamera;
    public static Dictionary<(int, int), GameObject> grid = new Dictionary<(int, int), GameObject>();

    public float playerSpeed = 3;
    public float enemySpeed = 2;

    private Cell[,] cells;
    private int width;
    private int height;
    private Player player;

    private int instantiatingCount = 0;
    private int MaxEnemy = 5;
    private int TimerEnemy = 30;


    //Карта
    private string[] map = new[] {
        ".............",
        ".#.#.#E#.#.#.",
        ".#.#.#B#.#.#.",
        ".#...#.#...#.",
        ".....#.#.....",
        "B.##.....##.B",
        ".....#.#.....",
        ".#...#.#...#.",
        ".#.#.....#.#.",
        ".#.#.###.#.#.",
        ".....#A#P...."
    };

    void SpawnPlayer(int i, int j, Cell[,] cells)
    {
        var playerGO = Instantiate(playerPrefab, new Vector3(j + 1, 1, i + 1), Quaternion.identity, transform);
        player = playerGO.GetComponent<Player>();
        player.Initialize(playerSpeed, cells, i, j, true);
        cells[j + 1, i + 1].Occupy(player);
    }

    void SpawnEnemy(int i, int j, Cell[,] cells)
    {
        var enemyGO = Instantiate(enemyPrefab, new Vector3(j + 1, 1, i + 1), Quaternion.identity, transform);
        var e = enemyGO.GetComponent<EnemyAI>();
        e.Initialize(enemySpeed, cells, i, j, false);
        cells[j + 1, i + 1].Occupy(e);
        ++instantiatingCount;
    }

    IEnumerator ExecuteAfterTime(float timeInSec, int i, int j, Cell[,] cells)
    {
        SpawnEnemy(i, j, cells);
        while (instantiatingCount < MaxEnemy)
        {
            yield return new WaitForSeconds(timeInSec);
            SpawnEnemy(i, j, cells);
        }
    }



    void Start()
    {
        height = map.Length;
        width = map[0].Length;

        cells = new Cell[width + 2, height + 2];

        for (int i = 0; i < height; i++)
        {
            if (map[i].Length != width)
            {
                throw new System.Exception("Invalid map: " + i + " - " + map[i].Length + " != " + width);
            }
            for (int j = 0; j < width; j++)
            {
                if (map[i][j] == '#')
                {
                    cells[j + 1, i + 1] = new Cell(CellSpace.Wall);
                }
                if (map[i][j] == 'B')
                {
                    cells[j + 1, i + 1] = new Cell(CellSpace.Bedrock);
                }
                if (map[i][j] == '.')
                {
                    cells[j + 1, i + 1] = new Cell(CellSpace.Empty);
                }
                //cells[j + 1, i + 1] = new Cell(map[i][j] == '#' ? CellSpace.Wall : CellSpace.Empty);
                if (map[i][j] == 'A')
                {
                    cells[j + 1, i + 1] = new Cell(CellSpace.Eagle);
                }
                if (map[i][j] == 'P')
                {
                    cells[j + 1, i + 1] = new Cell(CellSpace.Empty);
                    SpawnPlayer(i, j, cells);
                }
                if (map[i][j] == 'E')
                {
                    cells[j + 1, i + 1] = new Cell(CellSpace.Empty);
                    StartCoroutine(ExecuteAfterTime(TimerEnemy, i, j, cells));
                }
            }
        }

        //Внешние стены
        for (int i = 0; i < width + 2; i++)
        {
            cells[i, 0] = new Cell(CellSpace.Bedrock);
            cells[i, height + 1] = new Cell(CellSpace.Bedrock);
        }

        for (int i = 0; i < height + 2; i++)
        {
            cells[0, i] = new Cell(CellSpace.Bedrock);
            cells[width + 1, i] = new Cell(CellSpace.Bedrock);
        }

        for (var x = 0; x < width + 2; x++)
        {
            for (var y = 0; y < height + 2; y++)
            {
                var c = Instantiate(bedrockVoxel, new Vector3(x, 0, y), Quaternion.identity, transform);
                c.SetColor((x + y) % 2 == 0);

                if (cells[x, y].Space == CellSpace.Bedrock)
				{
                    Instantiate(bedrockVoxel, new Vector3(x, 1, y), Quaternion.identity, transform);
                }

                if (cells[x, y].Space == CellSpace.Wall)
                {
                    GameObject obj = Instantiate(wallVoxel, new Vector3(x, 1, y), Quaternion.identity, transform);
                    grid[(x, y)] = obj;
                }

                if (cells[x, y].Space == CellSpace.Eagle)
                {
                    GameObject obj = Instantiate(eagleVoxel, new Vector3(x, 1, y), Quaternion.identity, transform);
                    grid[(x, y)] = obj;
                }
            }
        }

        mainCamera.transform.position = new Vector3((width + 2) / 2, 10,(height + 2) / 2);
        mainCamera.transform.eulerAngles = new Vector3(90, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			StartCoroutine(player.TryMove(Vector2Int.right));
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			StartCoroutine(player.TryMove(Vector2Int.left));
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			StartCoroutine(player.TryMove(Vector2Int.up));
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			StartCoroutine(player.TryMove(Vector2Int.down));
		}
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.Fire();
        }

        for (var x = 0; x < cells.GetLength(0); x++)
        {
            for (var y = 0; y < cells.GetLength(1); y++)
            {
                if (cells[x, y].Occupant is EnemyAI enemy)
                {
                    enemy.StartCoroutine(enemy.Think());
                }
            }
        }
    }
}
