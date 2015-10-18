using System;
using System.IO;

class GoBang {
	private static bool winning (int[,] board, int i)
	{
		int maxx = board.GetLength(0);
		int maxy = board.GetLength(1);

		int[] pd1 = new int[maxx + maxy];
		int[] pd2 = new int[maxx + maxy];
		int[] pdy = new int[maxx];

		for (int y = 0 ; y < maxy ; ++y) 
		{
			int pdx = 0;

			for (int x = 0 ; x < maxx ; ++x) 
			{
				int pd1_idx = x - y + (maxy - 1);
				int pd2_idx = x + y;
				// pdy_idx is x

				if (board[x, y] == i) 
				{
					if (++pd1[pd1_idx] == 5 || ++pd2[pd2_idx] == 5 || ++pdy[x] == 5 || ++pdx == 5)
						return (true);
				} 
				else 
				{
					// reset numbers
					pd1[pd1_idx] = 0;
					pd2[pd2_idx] = 0;
					pdy[x] = 0;
					pdx = 0;
				}
			}
		}

		return (false);


	}

	private static bool valid(int[,] board, Coordinate move)
	{
		if (board[move.X, move.Y] != 0) return false;
		return true;
	}

	private static void printBoard(int[,] board)
	{
		Console.WriteLine("Board:");
		for (int y = 0; y < board.GetLength(0); ++y) {
			for (int x = 0; x < board.GetLength(1); ++x) {
				Console.Write("{0} ", board[x, y] == -1?"O":board[x, y] == 0?".":"X");
			}
			Console.WriteLine();
		}
	}

	public static void Main(string[] args) {
		Console.WriteLine("GoBang 0.001");
		string host;
		int port;
		if (args.Length < 2) 
		{
			host = "localhost";
			port = 7777;

		}
		else 
		{
			host = args[0];
			port = int.Parse(args[1]);
		}
		Player one = new NewAiPlayer();
		//Player two = new NewAiPlayer();
		Player two = new Communicator(host, port);

		string color = two.AskColor();
		int size = two.AskSize();

		if (color == "white")
		{
			one.SetColor("white");
			two.SetColor("black");
		}
		else
		{
			Player tmp = one;
			one = two;
			two = tmp;
			one.SetColor("white");
			two.SetColor("black");
		}
		one.SetSize(size);
		two.SetSize(size);
		

		int[,] board = new int[size, size];
		Coordinate move;
		while (true) {
			move = one.GetMove();
			Console.WriteLine("Got move from player 1: {0}", move);
			if (!valid(board,move)) 
				throw new Exception("Invalid move " + move);
			board[move.X, move.Y] = 1;
			printBoard(board);
			if (winning(board, 1)) throw new Exception("white has won");
			two.RegOppMove(move);
			move = two.GetMove();
			Console.WriteLine("Got move from player 2: {0}", move);
			if (!valid(board,move)) 
				throw new Exception("Invalid move " + move);
			board[move.X, move.Y] = -1;
			printBoard(board);
			if (winning(board, -1)) throw new Exception("black has won");
			one.RegOppMove(move);
		}
	}
}

