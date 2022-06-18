using Assets.Scripts.Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

internal class BulletController : MonoBehaviour
{
    public GameObject wallVoxel;
    private Vector2Int direction;
    public float speed = 10;
    private Cell[,] cells;
    public AudioSource shoot;
    public GameObject explosion;
    private int who;
    public static int killedEnemy = 0;

    public void Initialize(Cell[,] cells)
    {
        this.cells = cells;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (shoot != null)
        { shoot.Play(); }

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(direction.x, 0, direction.y) * speed * Time.deltaTime;

        var ourCell = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        if (cells[ourCell.x, ourCell.y].Space != CellSpace.Empty)
        {
            if (explosion != null)
            {
                var fx = Instantiate(explosion, new Vector3(ourCell.x, 1, ourCell.y), Quaternion.identity);
                Destroy(fx, 3);
            }
            Destroy(gameObject);
        }
        if (cells[ourCell.x, ourCell.y].Occupant != null & this.who == 1)
        {
            var enemy = cells[ourCell.x, ourCell.y].Occupant.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.Die();
                Destroy(gameObject);
                ++killedEnemy;
                if (killedEnemy == 5)
                {
                    killedEnemy = 0;
                    Time.timeScale = 0;
                    SceneManager.LoadScene("WinScreen");
                    Time.timeScale = 1;
                };
            }
        }
        if (cells[ourCell.x, ourCell.y].Occupant != null & this.who == 0)
        {
            var player = cells[ourCell.x, ourCell.y].Occupant.GetComponent<Player>();
            if (player != null)
            {
                player.Die();
                Destroy(gameObject);
            }
        }
        if (cells[ourCell.x, ourCell.y].Space == CellSpace.Wall)
        {
            var wall = FieldController.grid[(ourCell.x, ourCell.y)].GetComponent<Destructable>();
            wall.TakeDamage();
            cells[ourCell.x, ourCell.y] = new Cell(CellSpace.Empty);
            Destroy(gameObject);
        }
        if (cells[ourCell.x, ourCell.y].Space == CellSpace.Eagle)
        {
            var eagle = FieldController.grid[(ourCell.x, ourCell.y)].GetComponent<Destructable>();
            eagle.TakeDamage();
            cells[ourCell.x, ourCell.y] = new Cell(CellSpace.Empty);
            Destroy(gameObject);
            killedEnemy = 0;
            Time.timeScale = 0;
            SceneManager.LoadScene("LoseScreen");
            Time.timeScale = 1;
        }
    }

    public void Fire(Vector2Int direction, int who)
    {
        this.direction = direction;
        this.who = who;
    }
}
