using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Logic
{
	internal class EnemyAI : Tank
	{
		private Vector2Int mydirection = Vector2Int.up;
		private Transform playerTank;
		private Transform eagleBase;

		private void Start()
		{
			StartCoroutine("TryFire");
			eagleBase = GameObject.Find("EagleVoxel(Clone)").transform;
		}

		void Update()
		{

		}

		public IEnumerator Think()
		{
			while (Timer.timeStart < 40)
			{
				var howToMove = UnityEngine.Random.Range(0, 2);
				if (howToMove == 0)
				{
					var whereToMove = new List<Vector2Int>()
				{
					Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down
				};
					mydirection = whereToMove[UnityEngine.Random.Range(0, whereToMove.Count)];
				}
				yield return TryMove(mydirection);
			}
			while (true)
			{
				var howToMove = UnityEngine.Random.Range(0, 2);
				if (howToMove == 0)
				{
					var whereToMove = new List<Vector2Int>()
				{
					Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down
				};
					mydirection = whereToMove[UnityEngine.Random.Range(0, whereToMove.Count)];
				}
				else
				{
					if (howToMove == 1)
					{ playerTank = GameObject.Find("Player(Clone)").transform; mydirection = MoveToPosition(playerTank.position); }
					else
					{mydirection = MoveToPosition(eagleBase.position); }
				}
				yield return TryMove(mydirection);
			}
			/*while (true)
			{ 
				mydirection = MoveToPosition(eagleBase.position);
				yield return TryMove(mydirection);
			}*/
		}

		public IEnumerator TryFire()
		{
			while (true)
			{
				yield return new WaitForSeconds(5);
				Fire(0);
				yield return new WaitForSeconds(15);
			}
		}

		public Vector2Int MoveToPosition(Vector3 Objectposition)
		{
			float dist0 = Vector3.Distance(transform.position, Objectposition);
			float dist1 = Vector3.Distance(new Vector3(transform.position.x - 1, 1, transform.position.z), Objectposition);
			float dist2 = Vector3.Distance(new Vector3(transform.position.x + 1, 1, transform.position.z), Objectposition);
			float dist3 = Vector3.Distance(new Vector3(transform.position.x, 1, transform.position.z - 1), Objectposition);
			float dist4 = Vector3.Distance(new Vector3(transform.position.x, 1, transform.position.z + 1), Objectposition);

			if (dist1 <= dist2 && dist1 <= dist3 && dist1 <= dist4)
			{ mydirection = Vector2Int.left; }
			if (dist2 <= dist1 && dist2 <= dist3 && dist2 <= dist4)
			{ mydirection = Vector2Int.right; }
			if (dist3 <= dist2 && dist3 <= dist1 && dist3 <= dist4)
			{ mydirection = Vector2Int.down; }
			if (dist4 <= dist1 && dist4 <= dist3 && dist4 <= dist2)
			{ mydirection = Vector2Int.up; }

			return mydirection;
		}
	}
}
