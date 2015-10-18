
/* MoveRecording.cs - Record and replay moves functionality
 *
 * Marco Kunze, Sebastian Nowozin
 */

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;

public class MoveRecording
{
	public static void Main (string[] args)
	{
		MoveList ml = new MoveList ();

		ml.Add (1, 2);
		ml.Add (3, 4);
		WriteMoveList ("test.moves", ml);

		MoveList ml2 = ReadMoveList ("test.moves");
		foreach (MoveList.Move move in ml2.moves)
			Console.WriteLine ("move: {0}, {1}", move.x, move.y);
	}

	public static MoveList ReadMoveList (string filename)
	{
		XmlSerializer xs = new XmlSerializer (typeof (MoveList));
		TextReader reader = new StreamReader (filename);

		MoveList ml = null;
		try {
			ml = (MoveList) xs.Deserialize (reader);
		} catch (Exception ex) {
			Console.Error.WriteLine ("Error deserializing \"{0}\"", filename);
		}
		reader.Close ();

		return (ml);
	}

	public static void WriteMoveList (string filename, MoveList ml)
	{
		XmlSerializer xs = new XmlSerializer (typeof (MoveList));
		TextWriter writer = new StreamWriter (filename);
		xs.Serialize (writer, ml);
		writer.Close ();
	}
}

[Serializable]
public class MoveList
{
	public Move[] moves = null;

	public Move this[int idx] {
		get {
			return (moves[idx]);
		}
	}

	public int Count {
		get {
			if (moves == null)
				return (0);

			return (moves.Length);
		}
	}

	public void Add (int x, int y)
	{
		Move m = new Move ();
		m.x = x;
		m.y = y;

		ArrayList mla;
		if (moves != null)
			mla = new ArrayList (moves);
		else
			mla = new ArrayList ();
		mla.Add (m);
		moves = (Move[]) mla.ToArray (typeof (Move));
	}

	[Serializable]
	public class Move {
		public int x, y;
	}
}


