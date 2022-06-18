using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Logic
{
	internal enum CellSpace
	{
		Empty,
		Bedrock,
		Wall,
		Eagle
	}

	internal class Cell
	{
		public CellSpace Space { get; private set; }
		public Tank Occupant;

		public Cell(CellSpace space)
		{
			Space = space;
		}

		public void Occupy(Tank occupant)
		{
			Occupant = occupant;
		}
	}
}
