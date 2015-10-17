
// You need graphviz 1.11 or higher (Debian is at 1.13 and ok)
//
// dot -Tps -o test.ps test.dot

using System;
using System.IO;
using System.Text;


public class
PrettyPrint
{
	// TODO: maybe you have to flip this colors
	public static string OneColor = "#ffffff";
	public static string MOneColor = "#000000";
	public static string FreeColor = "#909090";

	// TODO: maybe print the moves with a number so you can see the order they
	// were selected.
	public static void PrintBoard (TextWriter wr, string id, int[,] board, string infostring)
	{
		wr.WriteLine ("{0} [shape=plaintext", id);
		wr.WriteLine ("    label = <<table><tr><td>{1}</td></tr><tr><td>{0}</td></tr></table>>];", PrintBoardContent (board), infostring);
	}
	
	public static void PrintBoard (TextWriter wr, string id, int[,] board)
	{
		wr.WriteLine ("{0} [shape=plaintext", id);
		wr.WriteLine ("    label = <{0}>];", PrintBoardContent (board));
	}

	public static string PrintBoardContent (int[,] board)
	{
		return (PrintBoardContent (board, null));
	}

	public static string PrintBoardContent (int[,] board, string bgcolor)
	{
		return (PrintBoardContent (board, bgcolor, true));
	}

	public static string PrintBoardContent (int[,] board, string bgcolor,
		bool printTableEnd)
	{
		StringBuilder wr = new StringBuilder ();

		if (bgcolor == null) {
			wr.Append ("<table>");
		} else {
			wr.AppendFormat ("<table bgcolor=\"{0}\">", bgcolor);
		}

		wr.Append ("        <tr><td></td>");
		for (int x = 0 ; x < board.GetLength(1) ; ++x)
			wr.AppendFormat ("<td>{0}</td>", "abcdefghijklmnopqrstuvwxyz"[x]);
		wr.Append ("</tr>\n");

		for (int y = 0 ; y < board.GetLength(0) ; ++y) {
			wr.Append ("        <tr>");

			wr.AppendFormat ("<td>{0}</td>", y);
			for (int x = 0 ; x < board.GetLength(1) ; ++x) {
				wr.AppendFormat ("<td bgcolor=\"{0}\">+</td>",
					board[y,x] == 1 ? OneColor :
						(board[y,x] == -1 ? MOneColor : FreeColor));
			}
			wr.Append ("\n");
			wr.Append ("        </tr>\n");
		}
		if (printTableEnd)
			wr.Append ("        </table>\n");

		return (wr.ToString ());
	}

	public static void Main (string[] args)
	{
		int[,] board = new int[15, 15];

		board[7,8] = 1;
		board[8,8] = -1;
		board[6,8] = 1;
		board[6,7] = -1;

		StreamWriter wr = new StreamWriter ("test.dot");
		wr.WriteLine ("digraph board {");
		wr.WriteLine ("    graph [rankdir=TB];");
		PrintBoard (wr, "test1", board);
		PrintBoard (wr, "test2", board);
		wr.WriteLine ("test1 -> test2");
		wr.WriteLine ("}");
		wr.Close ();
	}
}


