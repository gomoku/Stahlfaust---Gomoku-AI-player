using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.IO;

public class NewAiPlayer : Player 
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

	int[,] board;
	public const int BOARDSIZE = 15;

	// These are linked to the GUI
	public bool sequence_used = false;
	// default: 8 seconds
	public int dbtimelimit = 8; //timelimit for dbsearch
	public int ddbtimelimit = 8; //timelimit for ddbsearch
	public int abtimelimit = 8; //timelimit for alpha-beta-search
	// not anymore linked to the GUI

	public TimeSpan dbextratime; // resttime from the ab-search available for db-search.
	private DateTime expiretime; // point of time when alphabetasearch has to be stopped.
	private int expirenodecnt; // node counter for expiring.

	string color;

	int boardvalue;
	const int maximumsearchdepth = 16;

	Coordinate lastoppmove = new Coordinate(-1, -1);
	Coordinate bestfield;
	Coordinate tmpbestfield;
	GBThreatSequence threatsequence = null;
	GBThreatSequence lastAppliedThreatSequence = null;

	StatValEvaluator eval;
	InterestingFieldAgent fieldagent;
	ThreatSearcher searcher;

	public StringBuilder statusStr = new StringBuilder ();

#if STATS
	int nodesvisited;
	int overallnodes;
	int donemoves;
	int maxdepth;
	int[] depthnodes = new int[maximumsearchdepth+2+1];
#endif
#if PRETTY
	StreamWriter wr;
#endif
	
	public class ABSearchTimeoutException : ApplicationException
	{
	}

	public NewAiPlayer()
	{
		lastoppmove = new Coordinate(-1, -1);
		
		searcher = new ThreatSearcher();

#if STATS
		nodesvisited = 0;
		overallnodes = 0;
		maxdepth = 0;
#endif
	}
	
	public void SetSize(int size)
	{
		board = new int[size, size];
		fieldagent = new InterestingFieldAgent(searcher, size);
		eval = new StatValEvaluator(fieldagent);
	}

	public void SetColor(string color)
	{
		this.color = color;
	}

	public int AskSize()
	{
		return 15;
	}

	public string AskColor()
	{
		return "white";
	}

	public void SetBoard(int[,] bb)
	{
		SetSize(bb.GetLength(0));
		for (int y = 0; y < bb.GetLength(1); ++y)
		{
			for (int x = 0; x < bb.GetLength(0); ++x)
			{
				if (bb[x, y] == 1) RegOwnMove(new Coordinate(x, y));
				if (bb[x, y] == -1) RegOppMove(new Coordinate(x, y));
			}
		}
	}

	public void RegOwnMove(Coordinate move)
	{
		board[move.X, move.Y] = 1;
		fieldagent.UpdateInterestingFieldArray(board, move);
		fieldagent.UpdateThreatLists(board, move, 1);
		boardvalue += eval.statVal(board, move, 1);
	}

	public void RegOppMove(Coordinate move)
	{
		board[move.X, move.Y] = -1;
		fieldagent.UpdateInterestingFieldArray(board, move);
		fieldagent.UpdateThreatLists(board, move, -1);
		move.Val = eval.statVal(board, move, -1);
		boardvalue += move.Val;
		lastoppmove = move;
	}

	/** Check if there is a visible winning threat sequence for the opponent
	 * on the given board.
	 *
	 * This can be used to quickly "no-false-positive" check a move to be
	 * made.  If this function returns true, and we really make this move,
	 * then the opponent has a sure win.  So, if this function returns true,
	 * you should not make this move if possible.
	 *
	 * @param board The board to be checked for opponent-attackability.
	 * @param dbDefenseTimeLimit The limit in milliseconds we allow for
	 * checking the move.  Once this time is exceeded, we return.  Zero means
	 * no limit.
	 *
	 * @returns true if the board allows a winning sequence by the opponent,
	 * false has no meaning.
	 */
	public bool CheckForOpponentSequence (int[,] board, int dbDefenseTimeLimit)
	{
		return (GetOpponentSequence (board, dbDefenseTimeLimit) != null);
	}

	// TODO: docs
	public ArrayList ThreatDefensiveInterestingFields (int[,] board,
		int dbDefenseTimeLimit)
	{
		GBThreatSequence seq = GetOpponentSequence (board, dbDefenseTimeLimit);
		if (seq == null)
			return (null);

		// Only sequences which have defending moves (that is, category 1 and
		// above) do count.  The special case STRAIGHT FOUR is a GOAL and does
		// not have any defending moves.
		if (seq.defender == null || seq.defender.Length == 0)
			return (null);

		ArrayList defFields = new ArrayList ();

		// Add the attack and the defending moves of the first sequence ply to
		// the "defending interesting fields".
		defFields.Add (new Coordinate (seq.attacker.X, seq.attacker.Y));
		
		if (seq.defender != null) {
			foreach (GBMove gbm in seq.defender)
				defFields.Add (new Coordinate (gbm.X, gbm.Y));
		}
		

		// Now only keep the most attacking ones, that is the ones that
		// creates the highest number of distinct operators.
		ArrayList fields = GetDefensePriorityFields (board, defFields);
		Console.WriteLine ("ThreatDefensiveInterestingFields:");
		Console.Write ("  defFields: ");
		foreach (Coordinate c in defFields)
			Console.Write ("{0}, ", c);
		Console.WriteLine ();
		Console.Write ("  to max filtered: ");
		foreach (Coordinate c in fields)
			Console.Write ("{0}, ", c);
		Console.WriteLine ();

		return (fields);
	}

	/** Try to find a winning threat sequence for the opponent.
	 *
	 * @param board The current game board to be checked.
	 * @param dbDefenseTimeLimit The limit in milliseconds to be allowed for
	 * the search.
	 *
	 * @returns The winning threat sequence found null if none has been found.
	 */
	private GBThreatSequence GetOpponentSequence (int[,] board, int dbDefenseTimeLimit)
	{
		int[,] boardRev = new int[board.GetLength(1), board.GetLength(0)];
		for (int y = 0 ; y < boardRev.GetLength(0) ; ++y) {
			for (int x = 0 ; x < boardRev.GetLength(1) ; ++x) {
				// Reverse, so we check for "us", where us is, in fact, the
				// opponent.
				boardRev[y,x] = -board[x,y];
			}
		}

		Console.WriteLine ("Searching for opponent attackability of future board:");
		Console.WriteLine (new GoBangBoard (boardRev));

		GBThreatSequence seq = GBThreatSearch.FindWinning (boardRev,
			dbDefenseTimeLimit * 1000, true);

		if (seq != null)
			Console.WriteLine ("Opponent sequence: {0}", seq);

		return (seq);
	}

	// return type: ArrayList<Coordinate>
	private ArrayList GetDefensePriorityFields (int[,] board, ArrayList consider)
	{
		int[,] boardRev = new int[board.GetLength(1), board.GetLength(0)];
		for (int y = 0 ; y < boardRev.GetLength(0) ; ++y) {
			for (int x = 0 ; x < boardRev.GetLength(1) ; ++x) {
				// Reverse, so we check for "us", where us is, in fact, the
				// opponent.
				boardRev[y,x] = -board[x,y];
			}
		}

		int maxCount;
		int[,] operatorMap = GBThreatSearch.BuildOperatorMap
			(new GoBangBoard (boardRev), out maxCount);
		Console.WriteLine ("operator map:");
		for (int y = 0 ; y < operatorMap.GetLength(0) ; ++y) {
			for (int x = 0 ; x < operatorMap.GetLength(1) ; ++x) {
				Console.Write ("{0} ", operatorMap[y, x]);
			}
			Console.WriteLine ();
		}

		// Only keep and determine the max fields of the consider list.
		int considerMaxCount = 0;

		foreach (Coordinate c in consider) {
			if (operatorMap[c.Y, c.X] > considerMaxCount)
				considerMaxCount = operatorMap[c.Y, c.X];
		}

		ArrayList fields = new ArrayList ();
		foreach (Coordinate c in consider) {
			if (operatorMap[c.Y, c.X] == considerMaxCount)
				fields.Add (new Coordinate (c.X, c.Y));
		}

		return (fields);
	}

	/** Find a winning threat sequence for us.
	 *
	 * @param board The board to be checked.
	 *
	 * @returns The threat sequence found on success, or null if no sequence
	 * has been found.
	 */
	public GBThreatSequence FindWinning(int[,] board)
	{
		int[,] boardRev = new int[board.GetLength(1), board.GetLength(0)];
		for (int y = 0 ; y < boardRev.GetLength(0) ; ++y) {
			for (int x = 0 ; x < boardRev.GetLength(1) ; ++x) {
				boardRev[y,x] = board[x,y];
			}
		}

		Console.WriteLine ("Searching for board");
		Console.WriteLine (new GoBangBoard (boardRev));

		// A timelimit of zero means no limit.
		return GBThreatSearch.FindWinning (boardRev, dbtimelimit*1000, false);
	}
	
	private enum MoveState {
		Normal,
		Attack,
	}

	private MoveState state = MoveState.Normal;

	public Coordinate GetMove ()
	{
		statusStr = new StringBuilder ();

		try {
			return (GetMoveS ());
		} catch (Exception ex) {
			statusStr.Append ("=> FAILSAFE");

			Console.WriteLine ("FAILSAFE catched the following exception:");
			Console.WriteLine (ex);
			Console.WriteLine ();

			Console.WriteLine ("Falling back to simple alpha-beta");

			try {
				state = MoveState.Normal;
				bestfield = DoABMove ();
			} catch (Exception ex2) {
				Console.WriteLine ("AGAIN got an exception, giving up:");
				Console.WriteLine (ex2);

				System.Environment.Exit (0);
			}

			RegOwnMove (bestfield);

			return (bestfield);
		}
	}

	public Coordinate GetMoveS ()
	{
#if PRETTY
		wr = new StreamWriter ("test.dot");
		wr.WriteLine ("digraph board {");
		wr.WriteLine ("    graph [rankdir=TB];");
#endif

		bool getState = true;

		while (getState) {
			switch (state) {
				case MoveState.Normal:
					statusStr.Append ("Normal, ");
					getState = GetMoveNormal ();
					break;
				case MoveState.Attack:
					statusStr.Append ("Attack, ");
					getState = GetMoveAttack ();
					break;
			}
		}
#if PRETTY
		wr.WriteLine ("}");
		wr.Close ();
#endif
		statusStr.AppendFormat ("Move ({0},{1})",
			"abcdefghijklmnopqrstuvwxyz"[bestfield.X], bestfield.Y);

		RegOwnMove (bestfield);

		return (bestfield);
	}

	private bool GetMoveNormal ()
	{
		Console.WriteLine ("GetMoveNormal");
		Debug.Assert (threatsequence == null);

		// If only one field, skip sequence search
		ArrayList intFields = fieldagent.ReallyInterestingFields (board, 1);
		if (intFields.Count > 1) {
			// Search for winning threat sequence
			threatsequence = FindWinning(board);
			lastAppliedThreatSequence = null;
			if (threatsequence != null && threatsequence.attacker == null)
				threatsequence = null;

			if (threatsequence != null) {
				state = MoveState.Attack;
				statusStr.Append ("ATTACK!, ");

				return (true);
			}
		}

		// No threat sequence for us found, so lets proceed
		bestfield = DoABMove ();

		return (false);
	}

	/** For the alpha-beta we keep a list of interesting fields for the
	 * _first_ depth run.
	 */
	private ArrayList firstMoveFields = null;

	private Coordinate DoABMove ()
	{
		Console.WriteLine ("DoABMove");
		ArrayList intFields = fieldagent.ReallyInterestingFields(board, 1);

		ArrayList defFields = null;

		// Only get the defensive fields if we have a choice
		if (intFields.Count > 1) {
			defFields = ThreatDefensiveInterestingFields (board, ddbtimelimit);

			if (defFields != null) {
				Console.WriteLine ("Found a defensive sequence, limiting interesting fields to them:");
				firstMoveFields = new ArrayList ();

				// Combine the fields by and
				// fields := intFields \cap defFields
				foreach (Coordinate c in defFields) {
					Console.WriteLine ("   {0}", c);

					if (intFields.Contains (c))
						firstMoveFields.Add (c);
				}
			} else
				firstMoveFields = intFields;
		} else {
			// If there are no defending interesting fields, just take the
			// normal ones.
			firstMoveFields = intFields;
		}

		// Now fields contains the list of interesting moves.

		// Set bestfield and tmpbestfield to dummy value
		if (intFields.Count > 0)
			bestfield = (Coordinate) firstMoveFields[0];
		if (intFields.Count > 0)
			tmpbestfield = (Coordinate) firstMoveFields[0];

		if (lastoppmove.X == -1 && lastoppmove.Y == -1) {
			bestfield = new Coordinate(board.GetLength(0)/2, board.GetLength(0)/2);
		} else {
			// bestfield is already set to the first element, so if there is
			// only one field interesting, we already selected the correct one
			// (the first, the only one)
			if (firstMoveFields.Count > 1)
				IterativeAlphaBeta (lastoppmove, maximumsearchdepth);
		}

#if STATS
		Console.WriteLine("getMove: \t Nodes visited: \t{0}", nodesvisited);
		Console.WriteLine("getMove: \t Overall visited: \t{0}", overallnodes += nodesvisited);
		Console.WriteLine("getMove: \t Moves done: \t{0}", ++donemoves);
		Console.WriteLine("getMove: \t Nodes/Moves: \t{0}", overallnodes/(double)donemoves);
		Console.WriteLine("getMove: \t maxdepth: \t{0}", maxdepth);
		for (int i = 0; i < depthnodes.Length; ++i) {
			Console.WriteLine("getMove: \t in depth {0}: \t{1}", i, depthnodes[i]);
		}

		nodesvisited = 0;
		maxdepth = 0;
		depthnodes = new int[maximumsearchdepth+1];
#endif
		/*
		{
			// TODO: fix this code
			board[bestfield.X, bestfield.Y] = 1;
			if (CheckForOpponentSequence (board, 60)) {
				Console.WriteLine ("MOVE IS A BAD IDEA");
			} else {
				Console.WriteLine ("move is ok");
			}
			board[bestfield.X, bestfield.Y] = 0;
		}
		*/

		return (bestfield);
	}

	private bool GetMoveAttack ()
	{
		Console.WriteLine ("GetMoveAttack");
		Debug.Assert (threatsequence != null);

		// Check the threat sequence is still valid on the current board.
		// This is the rare magic case that could defeat us (C-3, C-4)
		if (threatsequence.CheckValidRevBoard (board) == false) {
			statusStr.Append ("magic :-(, ");
			Console.WriteLine ("Winning threat sequence not valid anymore, magic case.");

			threatsequence = null;
			state = MoveState.Normal;

			return (true);
		}

		// Also check if there is still attacker moves left
		if (threatsequence.attacker == null) {
			threatsequence = null;
			statusStr.Append ("seqEnd reached");

			state = MoveState.Normal;
			return (true);
		}


		// Check the last move of the opponent is one of the expected defender
		// moves in our threat sequence.  If it is not, fall back on
		// alpha-beta search.

		if (lastAppliedThreatSequence != null &&
			lastAppliedThreatSequence.defender != null)
		{
			bool defendedThreat = false;

			foreach (GBMove gbm in lastAppliedThreatSequence.defender) {
				if (gbm.X == lastoppmove.X && gbm.Y == lastoppmove.Y) {
					defendedThreat = true;

					break;
				}
			}

			// If the threat was not defended against, fall back to alpha-beta
			if (defendedThreat == false) {
				statusStr.Append ("not defended, ");
				threatsequence = null;

				state = MoveState.Normal;
				return (true);
			}
			statusStr.Append ("defended, ");
		}

		// Check if the move is in the interesting fields, which basically
		// asserts us there is no opponent threat we have to respond to.
		ArrayList fields = fieldagent.ReallyInterestingFields(board, 1);
		Coordinate wantmove = new Coordinate (threatsequence.attacker.X,
			threatsequence.attacker.Y);

		if (fields.Contains (wantmove)) {
			Console.WriteLine("Move is chosen from sequence");
			bestfield = wantmove;
			statusStr.Append ("move ({0},{1}) from seq, ", wantmove.X, wantmove.Y);
			sequence_used = true;
			lastAppliedThreatSequence = threatsequence;

			// Advance to next threat
			threatsequence = threatsequence.next;

			// End of sequence -> fall back to alpha beta, for the _next_
			// move.
			if (threatsequence == null)
				state = MoveState.Normal;
		} else {
			Console.WriteLine("Move from sequence is not in interesting fields");
			sequence_used = false;
			statusStr.Append ("move not from seq, ");

			bestfield = DoABMove ();
		}

		// Move made
		return (false);
	}


	private int IterativeAlphaBeta(Coordinate field, int maxDepth)
	{

		dbextratime = TimeSpan.Zero;
		expiretime = DateTime.Now.AddSeconds((double)abtimelimit);
		int[,] boardsave = (int[,])board.Clone();
		InterestingFieldAgent fieldagentbackup = (InterestingFieldAgent)fieldagent.Clone();

		bool timeout = false;
		int bestalpha = 0;
		int curdepth = 0;
		int founddepth = 0;

		TimeSpan resttime = TimeSpan.Zero;
		TimeSpan usedtime = TimeSpan.Zero;

		statusStr.AppendFormat ("AB ({0}): ", maxDepth);
		while (curdepth < maxDepth &&
			timeout == false &&
			bestalpha < StatValEvaluator.WINBORDER && bestalpha > -StatValEvaluator.WINBORDER && 
			resttime >= usedtime)
		{
			DateTime starttime = DateTime.Now;
			try 
			{
				curdepth += 2;
				statusStr.AppendFormat ("{0},", curdepth);

				int alpha = -StatValEvaluator.WIN;
				int beta = StatValEvaluator.WIN;
				Console.WriteLine("Starting alphabeta, ply: {0}", curdepth / 2);
				board = (int[,]) boardsave.Clone();

				alpha = alphabeta(lastoppmove, 0, alpha, beta, curdepth, 0
#if PRETTY
							, "root"
#endif
					);
				bestfield = tmpbestfield;
				bestalpha = alpha;
				founddepth = curdepth;
				/*
				Console.WriteLine("alphabeta finished");
				Console.WriteLine("Found best alpha: \t{0}", bestalpha);
				Console.WriteLine("Found best move: \t{0}", bestfield);
				*/

				usedtime = DateTime.Now.Subtract(starttime);
				resttime = expiretime.Subtract(DateTime.Now);
				Console.WriteLine(usedtime);

			} catch (ABSearchTimeoutException) {
				resttime = TimeSpan.Zero;
				statusStr.Append ("timeout!, ");

				Console.WriteLine("Alpha-Beta stopped by timeout.");
				timeout = true;
			}
		}
		dbextratime = resttime < TimeSpan.Zero?TimeSpan.Zero:resttime;
		Console.WriteLine("We left {0} for the db-search, hooray :)", dbextratime);
		Console.WriteLine("Found best move in ply {0}", founddepth / 2);
		Console.WriteLine("Found overall best alpha: \t{0}", bestalpha);
		Console.WriteLine("Found overall best move: \t{0}", bestfield);

		board = boardsave;
		fieldagent = fieldagentbackup;

		return bestalpha;
	}

	private int alphabeta(Coordinate field, int boardvalue, int alpha, int beta, int depthbound, int depth
#if PRETTY
			, string parent
#endif
			)
	{
#if STATS
		++nodesvisited;
		depthnodes[depth]++;
		if (depth > maxdepth) maxdepth = depth;
#endif
		++expirenodecnt;
		if (expirenodecnt >= 3000)
		{
			expirenodecnt = 0;
			//Console.WriteLine("Checking timeout: {0}", DateTime.Now);
			if (DateTime.Now > expiretime)
			{
				//Console.WriteLine("Timeout: {0}", DateTime.Now);
				throw new ABSearchTimeoutException();
			}
		}

		int attacker = (depth % 2 == 0) ? 1 : -1;
		
		InterestingFieldAgent fieldagentbackup = (InterestingFieldAgent)fieldagent.Clone();
		
		board[field.X, field.Y] = -1 * attacker; // remember: field comes still from the upper level.
		fieldagent.UpdateInterestingFieldArray(board, field);
		fieldagent.UpdateThreatLists(board, field, -1 * attacker);

		boardvalue += field.Val;

#if PRETTY
		string thisnode = "" + nodesvisited;
		if (nodesvisited < 250) {
			string infostring = "";
			infostring += "<table>";
			infostring += "<tr><td>";
			infostring += "node: " + thisnode +"-" + depthnodes[depth] + ", boardval: " + boardvalue + ", moveval: " + eval.statVal(board, field, -1 * attacker);
			infostring += "</td></tr>";
			infostring += "<tr><td>compare val: " + field.Val + "</td></tr>";
			infostring += "<tr><td>last move from: " + attacker * -1 + "</td></tr>";
			infostring += "<tr><td>Own threats: " + fieldagent.ownthreatlist.Count + "</td></tr>";
			foreach (Threat t in fieldagent.ownthreatlist) {
				infostring += "<tr><td>cat : "+ t.category +"</td></tr>";
				foreach (Coordinate c in t.fields) infostring += "<tr><td>coord : "+ c + "</td></tr>";
			}
			infostring += "<tr><td>Own threats added: " + fieldagent.ownaddedthreatlist.Count + "</td></tr>";
			foreach (Threat t in fieldagent.ownaddedthreatlist) {
				infostring += "<tr><td>cat : "+ t.category +"</td></tr>";
				foreach (Coordinate c in t.fields) infostring += "<tr><td>coord : "+ c + "</td></tr>";
			}
			infostring += "<tr><td>Own threats removed: " + fieldagent.ownremovedthreatlist.Count + "</td></tr>";
			foreach (Threat t in fieldagent.ownremovedthreatlist) {
				infostring += "<tr><td>cat : "+ t.category +"</td></tr>";
				foreach (Coordinate c in t.fields) infostring += "<tr><td>coord : "+ c + "</td></tr>";
			}
			infostring += "<tr><td>Opp threats: " + fieldagent.oppthreatlist.Count + "</td></tr>";
			foreach (Threat t in fieldagent.oppthreatlist) {
				infostring += "<tr><td>cat : "+ t.category +"</td></tr>";
				foreach (Coordinate c in t.fields) infostring += "<tr><td>coord : "+ c + "</td></tr>";
			}
			infostring += "<tr><td>Opp threats added: " + fieldagent.oppaddedthreatlist.Count + "</td></tr>";
			foreach (Threat t in fieldagent.oppaddedthreatlist) {
				infostring += "<tr><td>cat : "+ t.category +"</td></tr>";
				foreach (Coordinate c in t.fields) infostring += "<tr><td>coord : "+ c + "</td></tr>";
			}
			infostring += "<tr><td>Opp threats removed: " + fieldagent.oppremovedthreatlist.Count + "</td></tr>";
			foreach (Threat t in fieldagent.oppremovedthreatlist) {
				infostring += "<tr><td>cat : "+ t.category +"</td></tr>";
				foreach (Coordinate c in t.fields) infostring += "<tr><td>coord : "+ c + "</td></tr>";
			}
			infostring += "</table>";
			PrettyPrint.PrintBoard(wr, thisnode, board, infostring);
			wr.WriteLine("{0} -> {1};", parent, nodesvisited);
		}
#endif

		if (boardvalue > StatValEvaluator.WINBORDER || boardvalue < -StatValEvaluator.WINBORDER) 
		{
			board[field.X, field.Y] = 0;
			if (attacker == 1) return boardvalue - depth * 10000;
			else return boardvalue + depth * 10000;
		}
		
		if (depth == depthbound) 
		{
			board[field.X, field.Y] = 0;
			return boardvalue;
		}

		ArrayList fields = null;
		if (depth == 0)
		{
			fields = firstMoveFields;
		}
		else
		{
			fields = fieldagent.ReallyInterestingFields(board, attacker);
		}
		
		// Calculate the static value for all fields
		for (int i = 0; i < fields.Count; ++i)
		{
			Coordinate c = (Coordinate)fields[i];
			board[c.X, c.Y] = attacker;
			InterestingFieldAgent valifabackup = (InterestingFieldAgent)fieldagent.Clone();
			fieldagent.UpdateThreatLists(board, c, attacker);
			c.Val = eval.statVal(board, c, attacker);
			fieldagent = valifabackup;
			board[c.X, c.Y] = 0;
			fields[i] = c;
		}

		fields.Sort();
		if (depth % 2 == 0) 
			{
			//MAX node
			fields.Reverse();
			foreach(Coordinate ifield in fields) {
				int max = alpha;
				if ((max = alphabeta(ifield, boardvalue, alpha, beta, depthbound, depth+1
#if PRETTY
								, thisnode
#endif
								)) > alpha) 
				{
					alpha = max;
					if (depth == 0) tmpbestfield = ifield;
				}
				if (depth == 0) Console.WriteLine("Got max from {0}: {1}", ifield, max);
				if (alpha >= beta) {
					fieldagent = fieldagentbackup;
					if (depth == 0) tmpbestfield = ifield;
					board[field.X, field.Y] = 0;
					return beta;
				}
			}
			fieldagent = fieldagentbackup;
			board[field.X, field.Y] = 0;
			return alpha;
		} 
		else {
			//MIN node
			foreach(Coordinate ifield in fields) {
				if (depth == 0) tmpbestfield = ifield;
				int min = beta;

				if ((min = alphabeta(ifield, boardvalue, alpha, beta, depthbound, depth+1
#if PRETTY
								, thisnode
#endif
								)) < beta) 
				{
					beta = min;
					if (depth == 0) tmpbestfield = ifield;
				}
				if (alpha >= beta) {
					fieldagent = fieldagentbackup;
					if (depth == 0) tmpbestfield = ifield;
					board[field.X, field.Y] = 0;
					return alpha;
				}
			}
			fieldagent = fieldagentbackup;
			board[field.X, field.Y] = 0;
			return beta;
		}
	}
}

