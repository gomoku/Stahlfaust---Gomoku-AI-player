using System;
using System.Collections;

public class ThreatSearcher	
{
	private class Item 
	{
		public int category;
		public int offset;
		public ArrayList fields; // contains integers
		public bool create = false;

		public Item(int cat, int off, ArrayList ff) 
		{
			category = cat;
			offset = off;
			fields = ff;
		}
		public Item(int cat, int off, ArrayList ff, bool create) 
		{
			category = cat;
			offset = off;
			fields = ff;
			this.create = create;
		}

		public static Item Five(int offset)
		{
			ArrayList list = new ArrayList();
			return new Item(0, offset, list);
		}

		public static Item Four(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(4);
			return new Item(1, offset, list);
		}

		public static Item ShiftOneFour(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(3);
			return new Item(1, offset, list);
		}

		public static Item ShiftTwoFour(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(2);
			return new Item(1, offset, list);
		}

		public static Item ShiftThreeFour(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(1);
			return new Item(1, offset, list);
		}

		public static Item ShiftFourFour(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(0);
			return new Item(1, offset, list);
		}

		public static Item Three(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(1);
			list.Add(5);
			return new Item(2, offset, list);
		}

		public static Item OccupiedThree(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(1);
			list.Add(5);
			list.Add(6);
			return new Item(2, offset, list);
		}
		public static Item ReversedOccupiedThree(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(0);
			list.Add(1);
			list.Add(5);
			return new Item(2, offset, list);
		}
		public static Item StraightFour(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(0);
			list.Add(5);
			return new Item(1, offset, list);
		}
		public static Item BrokenThree(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(0);
			list.Add(2);
			list.Add(5);
			return new Item(2, offset, list);
		}
		public static Item ReversedBrokenThree(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(0);
			list.Add(3);
			list.Add(5);
			return new Item(2, offset, list);
		}
		public static Item CBrokenThreeOne(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(3);
			list.Add(4);
			return new Item(2, offset, list, true);
		}
		public static Item CBrokenThreeTwo(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(2);
			list.Add(4);
			return new Item(2, offset, list, true);
		}
		public static Item CBrokenThreeThree(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(2);
			list.Add(3);
			return new Item(2, offset, list, true);
		}
		public static Item CBrokenThreeFour(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(1);
			list.Add(4);
			return new Item(2, offset, list, true);
		}
		public static Item CBrokenThreeFive(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(1);
			list.Add(3);
			return new Item(2, offset, list, true);
		}
		public static Item CBrokenThreeSix(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(1);
			list.Add(2);
			return new Item(2, offset, list, true);
		}
		public static Item CBrokenThreeSeven(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(0);
			list.Add(4);
			return new Item(2, offset, list, true);
		}
		public static Item CBrokenThreeEight(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(0);
			list.Add(3);
			return new Item(2, offset, list, true);
		}
		public static Item CBrokenThreeNine(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(0);
			list.Add(2);
			return new Item(2, offset, list, true);
		}
		public static Item CBrokenThreeTen(int offset)
		{
			ArrayList list = new ArrayList();
			list.Add(0);
			list.Add(1);
			return new Item(2, offset, list, true);
		}

	}

	private Item[] table = new Item[262144];
	private int[,] board;
	private int attacker;
	bool create = false;


	private void generateTable() 
	{
		Console.WriteLine("Generating searcher table: start");
		int digit = 0;
		for (int i = 0; i < 262144; ++i) 
		{

			digit = i;
			bool assigned = false;
			
			int[] line = new int[9];
			for (int y = 0; y < 9; ++y) 
			{
				line[y] = digit % 4;
				digit >>= 2;
			}
			// free Field =  1
			// our Field =   2
			// opp Field =   0
			// nonexist Field = 3

			for (int k = 0; k < 5; ++k) 
			{
				if (line[k+0] == 2 
					&& line[k+1] == 2 
					&& line[k+2] == 2 
					&& line[k+3] == 2
					&& line[k+4] == 2)
				{
					table[i] = Item.Five(k-4);
					assigned = true;
					//					Console.WriteLine("Found a five");
				}
			}
			if (assigned == false) 
			{
				for (int k = 0; k < 3; ++k) 
				{
					if (line[k+0] == 1 
						&& line[k+1] == 1 
						&& line[k+2] == 2 
						&& line[k+3] == 2
						&& line[k+4] == 2
						&& line[k+5] == 1
						&& line[k+6] == 1)
					{
						table[i] = Item.Three(k-4);
						assigned = true;
						//					Console.WriteLine("Found a three");
					}
				}
			}
			if (assigned == false)
			{
				for (int k = 0; k < 3; ++k) 
				{
					if ( ((line[k+0] == 0) || (line[k+0] == 3))
						&& line[k+1] == 1 
						&& line[k+2] == 2 
						&& line[k+3] == 2
						&& line[k+4] == 2
						&& line[k+5] == 1
						&& line[k+6] == 1)
					{
						table[i] = Item.OccupiedThree(k-4);
						assigned = true;
						//					Console.WriteLine("Found a three");
					}
					if (line[k+0] == 1 
						&& line[k+1] == 1 
						&& line[k+2] == 2 
						&& line[k+3] == 2
						&& line[k+4] == 2
						&& line[k+5] == 1
						&& ((line[k+6] == 0) || (line[k+6] == 3)) )
					{
						table[i] = Item.ReversedOccupiedThree(k-4);
						assigned = true;
						//					Console.WriteLine("Found a three");
					}

				}
			}

			if (assigned == false)
			{
				for (int k = 0; k < 4; ++k) 
				{
					if (line[k+0] == 1 
						&& line[k+1] == 2 
						&& line[k+2] == 2 
						&& line[k+3] == 2
						&& line[k+4] == 2
						&& line[k+5] == 1)
					{
						table[i] = Item.StraightFour(k-4);
						assigned = true;
						//					Console.WriteLine("Found a straight four");
					}
				}
			}

			if (assigned == false)
			{
				for (int k = 0; k < 5; ++k) 
				{
					if (line[k+0] == 2 
						&& line[k+1] == 2 
						&& line[k+2] == 2 
						&& line[k+3] == 2
						&& line[k+4] == 1)
					{
						table[i] = Item.Four(k-4);
						assigned = true;
						//					Console.WriteLine("Found a four");
					}
					if (line[k+0] == 2 
						&& line[k+1] == 2 
						&& line[k+2] == 2 
						&& line[k+3] == 1
						&& line[k+4] == 2)
					{
						table[i] = Item.ShiftOneFour(k-4);
						assigned = true;
						//					Console.WriteLine("Found a four");
					}
					if (line[k+0] == 2 
						&& line[k+1] == 2 
						&& line[k+2] == 1 
						&& line[k+3] == 2
						&& line[k+4] == 2)
					{
						table[i] = Item.ShiftTwoFour(k-4);
						assigned = true;
						//					Console.WriteLine("Found a four");
					}
					if (line[k+0] == 2 
						&& line[k+1] == 1 
						&& line[k+2] == 2 
						&& line[k+3] == 2
						&& line[k+4] == 2)
					{
						table[i] = Item.ShiftThreeFour(k-4);
						assigned = true;
						//					Console.WriteLine("Found a four");
					}
					if (line[k+0] == 1 
						&& line[k+1] == 2 
						&& line[k+2] == 2 
						&& line[k+3] == 2
						&& line[k+4] == 2)
					{
						table[i] = Item.ShiftFourFour(k-4);
						assigned = true;
						//					Console.WriteLine("Found a four");
					}
				}
			}
			if (assigned == false)
			{
				for (int k = 0; k < 4; ++k) 
				{
					if (line[k+0] == 1 
						&& line[k+1] == 2 
						&& line[k+2] == 1 
						&& line[k+3] == 2
						&& line[k+4] == 2
						&& line[k+5] == 1)
					{
						table[i] = Item.BrokenThree(k-4);
						assigned = true;
						//Console.WriteLine("Found a broken three");
					}
					if (line[k+0] == 1 
						&& line[k+1] == 2 
						&& line[k+2] == 2 
						&& line[k+3] == 1
						&& line[k+4] == 2
						&& line[k+5] == 1)
					{
						table[i] = Item.ReversedBrokenThree(k-4);
						assigned = true;
						//Console.WriteLine("Found a reversed broken three");
					}
				}
			}
			if (assigned == false)
			{
				for (int k = 0; k < 5; ++k) 
				{
					if (line[k+0] == 2 
						&& line[k+1] == 2 
						&& line[k+2] == 2 
						&& line[k+3] == 1
						&& line[k+4] == 1)
					{
						table[i] = Item.CBrokenThreeOne(k-4);
						assigned = true;
						//Console.WriteLine("Found a broken three");
					}
				}
			}
			if (assigned == false)
			{
				for (int k = 0; k < 5; ++k) 
				{
					if (line[k+0] == 2 
						&& line[k+1] == 2 
						&& line[k+2] == 1 
						&& line[k+3] == 2
						&& line[k+4] == 1)
					{
						table[i] = Item.CBrokenThreeTwo(k-4);
						assigned = true;
						//Console.WriteLine("Found a broken three");
					}
				}
			}
			if (assigned == false)
			{
				for (int k = 0; k < 5; ++k) 
				{
					if (line[k+0] == 2 
						&& line[k+1] == 2 
						&& line[k+2] == 1 
						&& line[k+3] == 1
						&& line[k+4] == 2)
					{
						table[i] = Item.CBrokenThreeThree(k-4);
						assigned = true;
						//Console.WriteLine("Found a broken three");
					}
				}
			}
			if (assigned == false)
			{
				for (int k = 0; k < 5; ++k) 
				{
					if (line[k+0] == 2 
						&& line[k+1] == 1 
						&& line[k+2] == 2 
						&& line[k+3] == 2
						&& line[k+4] == 1)
					{
						table[i] = Item.CBrokenThreeFour(k-4);
						assigned = true;
						//Console.WriteLine("Found a broken three");
					}
				}
			}
			if (assigned == false)
			{
				for (int k = 0; k < 5; ++k) 
				{
					if (line[k+0] == 2 
						&& line[k+1] == 1 
						&& line[k+2] == 2 
						&& line[k+3] == 1
						&& line[k+4] == 2)
					{
						table[i] = Item.CBrokenThreeFive(k-4);
						assigned = true;
						//Console.WriteLine("Found a broken three");
					}
				}
			}
			if (assigned == false)
			{
				for (int k = 0; k < 5; ++k) 
				{
					if (line[k+0] == 2 
						&& line[k+1] == 1 
						&& line[k+2] == 1 
						&& line[k+3] == 2
						&& line[k+4] == 2)
					{
						table[i] = Item.CBrokenThreeSix(k-4);
						assigned = true;
						//Console.WriteLine("Found a broken three");
					}
				}
			}
			if (assigned == false)
			{
				for (int k = 0; k < 5; ++k) 
				{
					if (line[k+0] == 1 
						&& line[k+1] == 2 
						&& line[k+2] == 2 
						&& line[k+3] == 2
						&& line[k+4] == 1)
					{
						table[i] = Item.CBrokenThreeSeven(k-4);
						assigned = true;
						//Console.WriteLine("Found a broken three");
					}
				}
			}
			if (assigned == false)
			{
				for (int k = 0; k < 5; ++k) 
				{
					if (line[k+0] == 1 
						&& line[k+1] == 2 
						&& line[k+2] == 2 
						&& line[k+3] == 1
						&& line[k+4] == 2)
					{
						table[i] = Item.CBrokenThreeEight(k-4);
						assigned = true;
						//Console.WriteLine("Found a broken three");
					}
				}
			}
			if (assigned == false)
			{
				for (int k = 0; k < 5; ++k) 
				{
					if (line[k+0] == 1 
						&& line[k+1] == 2 
						&& line[k+2] == 1 
						&& line[k+3] == 2
						&& line[k+4] == 2)
					{
						table[i] = Item.CBrokenThreeNine(k-4);
						assigned = true;
						//Console.WriteLine("Found a broken three");
					}
				}
			}
			if (assigned == false)
			{
				for (int k = 0; k < 5; ++k) 
				{
					if (line[k+0] == 1 
						&& line[k+1] == 1 
						&& line[k+2] == 2 
						&& line[k+3] == 2
						&& line[k+4] == 2)
					{
						table[i] = Item.CBrokenThreeTen(k-4);
						assigned = true;
						//Console.WriteLine("Found a broken three");
					}
				}
			}
			/*
			if (line[4] == 2 && table[i] != null) {
				Console.Write("Generating searcher table: key: {0}, string: ", i);
				for (int y = 0; y < 9; ++y) {
					Console.Write(line[y] == 1?".":line[y]==2?"X":line[y]==3?"#":line[y]==0?"O":"");
				}
				Console.WriteLine("   , cat: {0}", table[i].category);
			}
			*/
			
		}
		Console.WriteLine("Generating searcher table: finished");

	}

	private ThreatList SearchBoards(Coordinate node)
	{
		ThreatList res = new ThreatList ();
		int turn = attacker;

		Item tmpval;
		int digit1 = 0;
		int digit2 = 0;
		int digit3 = 0;
		int digit4 = 0;
		for (int j = 8; j >= 0; --j) 
		{
			if (j + node.X - 4 >= 0 && j + node.X - 4 < board.GetLength(0)) 
			{
				digit1 += board[node.X + j - 4, node.Y] * turn + 1;
				//			Console.WriteLine("Grabbing {0}/{1}", node.move.x + j - 4, node.move.y);
			}
			else digit1 += 3;
			if (j + node.Y - 4 >= 0 && j + node.Y - 4 < board.GetLength(1)) 
			{
				digit2 += board[node.X, node.Y + j - 4] * turn + 1;
				//			Console.WriteLine("Grabbing {0}/{1}", node.move.x , node.move.y + j -4);
			}
			else digit2 += 3;
			if (node.X + j - 4 >= 0 && node.X + j - 4 < board.GetLength(0) &&
				node.Y + j - 4 >= 0 && node.Y + j - 4 < board.GetLength(1) ) 
			{
				digit3 += board[node.X + j - 4, node.Y + j - 4] * turn + 1;
				//			Console.WriteLine("Grabbing {0}/{1}", node.move.x + j - 4, node.move.y + j - 4);
			}
			else digit3 += 3;
			if (node.X - j + 4 >= 0 && node.X - j + 4 < board.GetLength(0) &&
				node.Y + j - 4 >= 0 && node.Y + j - 4 < board.GetLength(1) ) 
			{
				digit4 += board[node.X - j + 4, node.Y + j - 4] * turn + 1;
				//			Console.WriteLine("Grabbing {0}/{1}", node.move.x - j + 4, node.move.y + j - 4);
			}
			else digit4 += 3;
			if (j != 0) 
			{
				digit1 <<= 2;
				digit2 <<= 2;
				digit3 <<= 2;
				digit4 <<= 2;
			}
		}
		
		tmpval = table[digit1];

		if (tmpval != null && (tmpval.create == false || create == true))
		{
			//Console.WriteLine("horz category: {0}", tmpval.category);
			ArrayList moves = new ArrayList();
			foreach(int field in tmpval.fields) 
			{
				moves.Add(new Coordinate(node.X + tmpval.offset + field, node.Y));
			}
			Threat t = new Threat(node, tmpval.category, moves, tmpval.create);
			res.Add(t);
		}
		tmpval = table[digit2];
		if (tmpval != null && (tmpval.create == false || create == true))
		{
			//Console.WriteLine("vert category: {0}", tmpval.category);
			//Console.WriteLine("Node: {0}", node);
			//Console.WriteLine("offset: {0}", tmpval.offset);
			ArrayList moves = new ArrayList();
			foreach(int field in tmpval.fields) 
			{
				//Coordinate tmp = new Coordinate(node.X , node.Y + tmpval.offset + field);
				//Console.WriteLine("adding move: {0}", tmp);
				moves.Add(new Coordinate(node.X, node.Y + tmpval.offset + field));
			}
			Threat t = new Threat(node, tmpval.category, moves, tmpval.create);
			res.Add(t);
		}
		tmpval = table[digit3];
		if (tmpval != null && (tmpval.create == false || create == true))
		{
			//Console.WriteLine("diag 1 category: {0}", tmpval.category);
			ArrayList moves = new ArrayList();
			foreach(int field in tmpval.fields) 
			{
				moves.Add(new Coordinate(node.X + tmpval.offset + field, node.Y + tmpval.offset + field));
			}
			Threat t = new Threat(node, tmpval.category, moves, tmpval.create);
			res.Add(t);
		}
		tmpval = table[digit4];
		if (tmpval != null && (tmpval.create == false || create == true))
		{
			//Console.WriteLine("diag 2 category: {0}", tmpval.category);
			//Console.WriteLine("Node: {0}", node);
			//Console.WriteLine("offset: {0}", tmpval.offset);
			ArrayList moves = new ArrayList();
			foreach(int field in tmpval.fields) 
			{
				//Coordinate tmp = new Coordinate(node.X -(tmpval.offset + field), node.Y + tmpval.offset + field);
				//Console.WriteLine("adding move: {0}", tmp);
				moves.Add(new Coordinate(node.X -(tmpval.offset + field), node.Y + tmpval.offset + field));
			}
			Threat t = new Threat(node, tmpval.category, moves, tmpval.create);
			res.Add(t);
		}

		return res;

	}

	public ThreatSearcher()
	{
		generateTable();
	}

	public ThreatList investigate(int[,] board, Coordinate move, int attacker)
	{
		return investigate(board, move, attacker, false);
	}

	public ThreatList investigate(int[,] board, 
		Coordinate move, 
		int attacker,
		bool create)
	{
		this.board = board;
		this.attacker = attacker;
		this.create = create;

		bool reset = false;
		if (board[move.X, move.Y] == attacker * -1) throw new Exception();
		if (board[move.X, move.Y] != attacker) 
		{
			board[move.X, move.Y] = attacker;
			reset = true;
		}

		ThreatList res = SearchBoards(move);

		if (reset == true) board[move.X, move.Y] = 0;
		return res;
	}

}

