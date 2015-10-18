using System;
using System.Collections;

public class TestFirstSearcher
{
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

	private static void printBoard(int[,] board, Coordinate move)
	{
		Console.WriteLine("Board:");
		for (int y = 0; y < board.GetLength(0); ++y) {
			for (int x = 0; x < board.GetLength(1); ++x) {
				if (move.X == x && move.Y == y) Console.Write("x ");
				else Console.Write("{0} ", board[x, y] == -1?"O":board[x, y] == 0?".":"X");
			}
			Console.WriteLine();
		}
	}

	public static int[,] Flip(int[,] board)
	{
		int[,] res = new int[board.GetLength(1), board.GetLength(0)];
		for (int y = 0; y < board.GetLength(0); ++y) {
			for (int x = 0; x < board.GetLength(1); ++x) {
				res[y, x] = board[x, y];
			}
		}
		return res;
	}
	public static void Main()
	{
		int[,] board = new int[,]{
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  1,  0,  1,  0,  1,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{1,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{1,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}
		};

		board = Flip(board);

		ThreatSearcher searcher = new ThreatSearcher();

		ThreatList threats = searcher.investigate(board, new Coordinate(0, 13), 1);

		foreach (Threat t in threats)
			Console.WriteLine(t.ToString());
	}
}
