using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Logic
{
	internal class Tank : MonoBehaviour
	{
	    public bool isMoving { get; private set; }

        public BulletController bulletPrefab;
        public AudioSource life;
        public AudioSource MusicDie;
        public GameObject explosion;
        public GameObject mana;
        public float playerSpeed = 3;

        private Player player;
        private float moveSpeed;
        private Cell[,] cells;
        private int i;
        private int j;
        private bool rebirth;

        private Vector2Int direction = Vector2Int.up;

		public void Initialize(float moveSpeed, Cell[,] cells, int i, int j, bool rebirth)
		{
            this.moveSpeed = moveSpeed;
            this.cells = cells;
            this.i = i;
            this.j = j;
            this.rebirth = rebirth;
        }

        public IEnumerator TryMove(Vector2Int delta)
        {
            if (isMoving)
            {
                yield break;
            }

            isMoving = true;
            direction = delta;

            var rotationY = Vector2.SignedAngle(Vector2.up, delta * new Vector2Int(-1, 1));
            if (rotationY < 0)
            {
                rotationY = 270;
            }
            var from = GetCoords();
            var tc = from + delta;

            if ((tc.x >= cells.GetLength(0)) ^ (tc.y >= cells.GetLength(1)) ^ (tc.x < 0) ^ (tc.y < 0))
            {
                isMoving = false;
                yield break;
            }

            var targetCell = cells[tc.x, tc.y];

            //Повернуться
            var currentRotation = gameObject.transform.eulerAngles;
            var targetRotation = new Vector3(0, rotationY, 0);
            gameObject.transform.eulerAngles = targetRotation;

            //Если есть место, переместиться
            if (targetCell.Occupant == null && targetCell.Space == CellSpace.Empty)
            {
                cells[tc.x, tc.y].Occupy(this);
                cells[from.x, from.y].Occupy(null);
                var currentPosition = new Vector3(from.x, 1, from.y);
                var targetPosition = new Vector3(tc.x, 1, tc.y);

                //var currentRotation = gameObject.transform.eulerAngles;
                //var targetRotation = new Vector3(0, rotationY, 0);

                var moveTime = 1f / moveSpeed;
                float t = 0;
                while (t < moveTime)
                {
                    t += Time.deltaTime;
                    gameObject.transform.position = currentPosition + (t / moveTime) * (targetPosition - currentPosition);

                    var f = Mathf.Min(1, 2 * t / moveTime);
                    gameObject.transform.eulerAngles = currentRotation + f * (targetRotation - currentRotation);
                    yield return null;
                }
                gameObject.transform.position = targetPosition;
                ///gameObject.transform.eulerAngles = targetRotation;
            }

            isMoving = false;
        }

        private Vector2Int GetCoords()
        {
            Vector2Int p = default;
            for (var x = 0; x < cells.GetLength(0); x++)
            {
                for (var y = 0; y < cells.GetLength(1); y++)
                {
                    if (cells[x, y].Occupant == this)
                    {
                        p = new Vector2Int(x, y);
                    }
                }
            }
            return p;
        }

        //Стрелять по направлению танка
        public void Fire(int who = 1)
        {
            var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.Initialize(cells);
            bullet.Fire(direction, who);
        }

        private void Rebirth()
        {
            while (cells[j + 1, i + 1].Occupant != null )
            {
                var enemy = cells[j + 1, i + 1].Occupant.GetComponent<EnemyAI>();
                if (cells[j + 2, i + 1].Occupant != null)
                { enemy.TryMove(Vector2Int.right); }
                else { enemy.TryMove(Vector2Int.down); }
                    
            };
            var fx = Instantiate(mana, new Vector3(j + 1, 1, i + 1), Quaternion.identity);
            Destroy(fx, 3);
            cells[j+1, i+1].Occupy(this);
            gameObject.transform.position = new Vector3(j+1, 1, i+1);
            if (life != null)
            { life.Play(); }
        }

        IEnumerator ExecuteAfterTime(float timeInSec)
        {
            yield return new WaitForSeconds(timeInSec);
            this.Rebirth();
        }

        //Смерть
        public void Die()
        {
            
            var p = GetCoords();
            if (MusicDie != null)
            { MusicDie.Play(); }
            var fx = Instantiate(explosion, new Vector3(p.x, 1, p.y), Quaternion.identity);
            cells[p.x, p.y].Occupy(null);
            
            if (rebirth)
            {
                StartCoroutine(ExecuteAfterTime(2));
            }
            else 
            {
                StopAllCoroutines();
                Destroy(gameObject, 2);
            }
            Destroy(fx, 3);
        }
    }
}
