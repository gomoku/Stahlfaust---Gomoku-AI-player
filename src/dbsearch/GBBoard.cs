
using System;
using System.Text;
using System.Collections;


public class
GoBangBoard
{
	public static int boardDim = 15;

	public object Clone ()
	{
		GoBangBoard gbNew = new GoBangBoard ((int[,]) board.Clone ());

		return (gbNew);
	}

	/** Flip all stones in color so the player situations are reversed.
	 */
	public void Flip ()
	{
		for (int y = 0 ; y < boardDim ; ++y)
			for (int x = 0 ; x < boardDim ; ++x)
				board[y,x] = -board[y,x];
	}

	// [y, x]
	internal int[,] board;

	public int CompareTo (object o2)
	{
		GoBangBoard gb2 = (GoBangBoard) o2;

		for (int y = 0 ; y < boardDim ; ++y) {
			for (int x = 0 ; x < boardDim ; ++x) {
				if (board[y,x] < gb2.board[y,x])
					return (-1);
				else if (board[y,x] > gb2.board[y,x])
					return (1);
			}
		}

		return (0);
	}

	public void AddExtraStones (GoBangBoard b2)
	{
		for (int y = 0 ; y < boardDim ; ++y) {
			for (int x = 0 ; x < boardDim ; ++x) {
				if (board[y,x] == 0 && b2.board[y,x] != 0)
					board[y,x] = b2.board[y,x];
			}
		}
	}

	// Check if there are no conflicting stones.
	public bool CompatibleWith (GoBangBoard b2)
	{
		for (int y = 0 ; y < boardDim ; ++y) {
			for (int x = 0 ; x < boardDim ; ++x) {
				if (board[y,x] == 0 || b2.board[y,x] == 0 ||
					board[y,x] == b2.board[y,x])
					continue;

				return (false);
			}
		}

		return (true);
	}

	public static void Main (string[] args)
	{
		GoBangBoard gb = new GoBangBoard ();
		Random rnd = new Random ();

		// Initialize board randomly
		for (int n = 0 ; n < 130 ; ++n)
			gb.board[rnd.Next (0, boardDim), rnd.Next (0, boardDim)] =
				rnd.Next (0, 3) - 1;

		int count = 0;
		foreach (StoneSet ss in gb.G5) {
			Console.Write ("ss at ({0},{1}) to ({2},{3}), len {4}: ",
				ss.x, ss.y, ss.ax, ss.ay, ss.stones.Length);
			foreach (int stone in ss.stones) {
				Console.Write ("{0}", (stone == 0) ? "." :
					((stone == 1) ? "O" : "X"));
			}
			Console.WriteLine ();

			count += 1;
		}
		Console.WriteLine ("|G5| = {0}", count);

		count = 0;
		foreach (StoneSet ss in gb.G6)
			count += 1;
		Console.WriteLine ("|G6| = {0}", count);

		count = 0;
		foreach (StoneSet ss in gb.G7)
			count += 1;
		Console.WriteLine ("|G7| = {0}", count);

		// Test operators a little
		gb.DumpBoard ();

		GBSpaceState state = new GBSpaceState (gb);
		GBOperator[] legalOpers = GBOperator.LegalOperators (state, 2);
		foreach (GBOperator gop in legalOpers)
			Console.WriteLine ("oper: {0}", gop);
	}

	public GoBangBoard ()
		: this (new int[boardDim, boardDim])
	{
	}

	public GoBangBoard (int[,] board)
	{
		this.board = board;

		g5 = new GCl (this, 5);
		g6 = new GCl (this, 6);
		g7 = new GCl (this, 7);
	}

	public void DumpBoard ()
	{
		Console.WriteLine (ToString ());
	}

	public override string ToString ()
	{
		StringBuilder sb = new StringBuilder ();

		sb.Append ("\n");
		sb.Append ("   a b c d e f g h i j k l m n o\n");

		for (int y = 0 ; y < boardDim ; ++y) {
			sb.AppendFormat ("{0:D2} ", y);

			for (int x = 0 ; x < boardDim ; ++x) {
				sb.AppendFormat ("{0} ", (board[y,x] == 0 ? "." :
					(board[y,x] == 1 ? "O" : "X")));
			}
			sb.Append ("\n");
		}
		sb.Append ("\n");

		return (sb.ToString ());
	}

	private GCl g5;
	public GCl G5 {
		get {
			return (g5);
		}
	}

	private GCl g6;
	public GCl G6 {
		get {
			return (g6);
		}
	}

	private GCl g7;
	public GCl G7 {
		get {
			return (g7);
		}
	}

	public struct StoneSet {
		internal int[] stones;

		// The x/y position on the board where the stone has been found
		// This is x_0, y_0
		internal int x;
		internal int y;

		// The x/y addition to deduce the position of the n'th stone:
		// x_n = x_0 + n*ax
		// y_n = y_0 + n*ay
		internal int ax;
		internal int ay;

		internal StoneSet (int x, int y, int ax, int ay, int size)
		{
			stones = new int[size];
			this.x = x;
			this.y = y;
			this.ax = ax;
			this.ay = ay;
		}
	}

	public class GCl : IEnumerable
	{
		private GCl () {
		}

		public GCl (GoBangBoard gb, int gSize)
		{
			this.gb = gb;
			this.gSize = gSize;
		}

		private GoBangBoard gb;
		private int gSize;

		// Make it foreach'able
		public IEnumerator GetEnumerator () {
			return (new GClEnumerator (gb, gSize));
		}

		public class GClEnumerator : IEnumerator
		{
			private GClEnumerator ()
			{
			}

			GoBangBoard gb;
			int dir = 0;
			int y = 0;
			int x = -1;
			int didx = 0;
			int ax = 1, ay = 0;

			int gSize;

			public GClEnumerator (GoBangBoard gb, int gSize)
			{
				this.gb = gb;
				this.gSize = gSize;
			}

			public object Current {
				get {
					StoneSet ss = new StoneSet (x, y, ax, ay, gSize);

					//Console.WriteLine ("dir {0} from ({1},{2})", dir, x, y);
					int px = x, py = y;
					for (int n = 0 ; n < gSize ; ++n) {
						ss.stones[n] = gb.board[py, px];
						py += ay;
						px += ax;
					}

					return (ss);
				}
			}

			public void Reset ()
			{
				x = -1;
				y = 0;
				dir = 0;
				ax = 1;
				ay = 0;
				didx = 0;
			}

			public bool MoveNext ()
			{
				//x += ax;
				//y += ay;

				if (dir == 0) {
					x += 1;
					if (x <= (GoBangBoard.boardDim - gSize))
						return (true);

					y += 1;
					x = 0;
					if (y < GoBangBoard.boardDim)
						return (true);

					dir = 1;
					x = y = 0;

					ax = 0;
					ay = 1;

					return (true);
				} else if (dir == 1) {
					y += 1;
					if (y <= (GoBangBoard.boardDim - gSize))
						return (true);

					x += 1;
					y = 0;
					if (x < GoBangBoard.boardDim)
						return (true);

					dir = 2;
					x = 0;
					y = GoBangBoard.boardDim - gSize;
					ax = ay = 1;
					didx = 0;

					return (true);
				} else if (dir == 2) {
					x += 1;
					y += 1;
					if (x <= (GoBangBoard.boardDim - gSize) &&
						y <= (GoBangBoard.boardDim - gSize))
					{
						return (true);
					}

					// Increase diagonal index and get new start coordinates
					didx += 1;
					if (didx <= ((GoBangBoard.boardDim - gSize) << 1)) {
						x = y = 0;
						if (didx < (GoBangBoard.boardDim - gSize)) {
							y = (GoBangBoard.boardDim - gSize) - didx;
						}
						if (didx > (GoBangBoard.boardDim - gSize)) {
							x = didx - (GoBangBoard.boardDim - gSize);
						}

						return (true);
					}

					// All diagonals done.
					dir = 3;
					didx = 0;
					x = 0;
					y = gSize - 1;

					ax = 1;
					ay = -1;

					return (true);
				} else if (dir == 3) {
					x += 1;
					y -= 1;
					if (x <= (GoBangBoard.boardDim - gSize) && y >= (gSize - 1))
						return (true);

					didx += 1;
					if (didx <= ((GoBangBoard.boardDim - gSize) << 1)) {
						if (didx <= (GoBangBoard.boardDim - gSize)) {
							x = 0;
							y = didx + (gSize - 1);
						} else if (didx > (GoBangBoard.boardDim - gSize)) {
							x = didx - (GoBangBoard.boardDim - gSize);
							y = GoBangBoard.boardDim - 1;
						}
						return (true);
					}

					return (false);
				}

				return (false);
			}
		}
	}
}

