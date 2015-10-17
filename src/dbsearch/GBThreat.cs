
using System;
using System.Text;
using System.Collections;

using DBSearchAlgorithm;



public class
GBThreatSearch
{
	public static void Main (string[] args)
	{
		GoBangBoard gb = new GoBangBoard ();
		Random rnd = new Random ();

		/*
		for (int n = 0 ; n < 23 ; ++n)
			gb.board[rnd.Next (0, gb.boardDim), rnd.Next (0, gb.boardDim)] =
				rnd.Next (0, 3) - 1;
		*/

		/*
		gb.board[5,5] = gb.board[5,8] = -1;
		gb.board[7,4] = gb.board[7,9] = -1;

		gb.board[5,4] = gb.board[5,6] = gb.board[5,7] = gb.board[5,9] = 1;
		gb.board[7,6] = gb.board[7,7] = 1;
		*/

		/*
		// Testcase A
		gb.board[4,9] = gb.board[5,6] = gb.board[6,8] = 1;
		gb.board[7,7] = gb.board[8,6] = gb.board[9,5] = gb.board[9,6] = 1;
		gb.board[9,9] = gb.board[10,11] = 1;

		gb.board[5,9] = gb.board[5,10] = -1;
		gb.board[6,7] = gb.board[6,9] = -1;
		gb.board[7,8] = gb.board[7,9] = gb.board[8,9] = -1;
		gb.board[9,10] = gb.board[10,4] = -1;
		*/

		/*
		// Testcase B
		gb.board[5,5] = gb.board[4,6] = gb.board[7,5] = gb.board[7,6] = 1;
		gb.board[5,6] = gb.board[6,6] = -1;
		*/

		/*
		gb.board[5,6] = gb.board[8,6] = gb.board[9,6] = 1;
		*/

		/*
		// FIXME: this sequence is not found as un-refutable, while it is in
		// fact (magic case, we know exactly what it is caused by and this is
		// unlikely to appear in a real game and difficult to fix (though
		// possible))
		// 138, figure 5.5
		gb.board[0,0] = gb.board[1,1] = gb.board[2,2] = gb.board[5,13] = -1;
		gb.board[6,9] = gb.board[7,7] = gb.board[7,8] = -1;
		gb.board[8,6] = gb.board[8,12] = -1;
		gb.board[10,7] = gb.board[10,9] = -1;
		gb.board[10,2] = gb.board[11,1] = gb.board[12,0] = -1;

		gb.board[2,12] = gb.board[3,12] = 1;
		gb.board[6,6] = gb.board[6,7] = gb.board[7,6] = gb.board[7,10] = 1;
		gb.board[8,8] = gb.board[8,11] = gb.board[9,8] = 1;
		*/

		// 127b
		gb.board[5,6] = gb.board[6,9] = gb.board[7,7] = gb.board[8,8] = 1;
		gb.board[8,9] = 1;

		gb.board[6,6] = gb.board[6,8] = gb.board[7,6] = gb.board[7,10] = -1;
		gb.board[8,6] = -1;

		/*
		// 127a
		gb.board[6,6] = gb.board[6,7] = gb.board[6,8] = 1;
		gb.board[7,7] = gb.board[8,6] = gb.board[8,7] = gb.board[8,8] = 1;
		gb.board[9,6] = gb.board[10,5] = gb.board[10,6] = gb.board[10,7] = 1;

		gb.board[6,5] = gb.board[7,5] = gb.board[7,6] = gb.board[7,8] = -1;
		gb.board[8,5] = gb.board[8,9] = gb.board[9,5] = gb.board[9,7] = gb.board[9,9] = -1;
		gb.board[10,4] = gb.board[11,6] = -1;

		// Move 1/2
		gb.board[5,5] = 1;
		gb.board[4,4] = -1;
		*/

		/*
		// Move 3/4
		gb.board[5,7] = 1;
		gb.board[4,7] = -1;

		// Move 5/6
		gb.board[5,9] = 1;
		gb.board[4,10] = -1;

		// Move 7/8
		gb.board[5,8] = 1;
		gb.board[5,6] = -1;

		// Move 9/10
		gb.board[5,11] = 1;
		gb.board[5,10] = -1;

		// Move 11/12
		gb.board[6,10] = 1;
		gb.board[6,9] = -1;

		// Move 13/14
		gb.board[7,9] = 1;
		gb.board[4,12] = -1;

		// Move 15/16
		gb.board[4,6] = 1;
		gb.board[3,5] = -1;
		*/

		Console.WriteLine ("Starting winning threat sequence search...");

		GBThreatSearch gts = new GBThreatSearch (gb, false);
		GBThreatSequence seq = gts.FindWinningThreatSeqOTF ();

		if (seq != null) {
			Console.WriteLine ("Board:");
			Console.WriteLine (gb);
			Console.WriteLine ();

			Console.WriteLine ("winning threat sequence found:");
			Console.WriteLine (seq);
		}

		GBThreatSearch gts2 = new GBThreatSearch (gb, true);
		GBThreatSequence seq2 = gts2.FindWinningThreatSeqOTF ();

		if (seq2 != null) {
			Console.WriteLine ("Board:");
			Console.WriteLine (gb);
			Console.WriteLine ();

			Console.WriteLine ("winning threat sequence found:");
			Console.WriteLine (seq2);
		}
	}

	// timeoutMS: zero or timeout limit in ms
	public static GBThreatSequence FindWinning (int[,] board, int timeoutMS,
		bool defOpponentSearch)
	{
		GBThreatSearch gts = new GBThreatSearch (new GoBangBoard (board),
			defOpponentSearch);
		GBThreatSequence seq = gts.FindWinningThreatSeqOTF (timeoutMS);

		if (seq != null) {
			Console.WriteLine ("Board:");
			Console.WriteLine (new GoBangBoard (board));
			Console.WriteLine ();

			Console.WriteLine ("Winning threat sequence found:");
			Console.WriteLine (seq);
		} else {
			Console.WriteLine ("No winning threat sequence found.");
		}

		return (seq);
	}

	private GBThreatSearch ()
	{
	}

	private GoBangBoard gb;
	private bool breadthFirst;

	public GBThreatSearch (GoBangBoard gb, bool breadthFirst)
	{
		this.gb = gb;
		this.breadthFirst = breadthFirst;
	}

	public static int[,] BuildOperatorMap (GoBangBoard board, out int maxCount)
	{
		// Get legal operators for the state represented by the board
		GBSpaceState state = new GBSpaceState (board);
		GBOperator[] opers = GBOperator.LegalOperators (state, 2);

		// Now build an attack count map and keep track of the maximum number
		// of operators a single attack stone creates.
		maxCount = 0;
		int[,] attackMap = new int[GoBangBoard.boardDim, GoBangBoard.boardDim];

		foreach (GBOperator op in opers) {
			if (op is GBOperatorThree3)
				continue;

			for (int n = 0 ; n < op.fAdd.GetLength (0) ; ++n) {
				// We are not (yet) interested in the defending stones.
				if (op.fAdd[n,2] == -1)
					continue;

				int x = op.fAdd[n,0];
				int y = op.fAdd[n,1];
				attackMap[y, x] += 1;
				Console.WriteLine ("at ({0}, {1}) oper: {2}", x, y, op);

				if (attackMap[y, x] > maxCount)
					maxCount = attackMap[y, x];
			}
		}

		return (attackMap);
	}

	/** Test a potential winning threat sequence for refutability.
	 *
	 * @param curBoard The original board the original attacker db-search was
	 * started with (root.State.GB).
	 * @param root The root node of the search tree.
	 * @param goalnode The new goal node identified by db-search.
	 *
	 * @returns null if the sequence is refutable, otherwise the sequence
	 * itself is returned.
	 */
	public static GBThreatSequence DefenseRefutable (GoBangBoard curBoard,
		DBNode root, DBNode goalnode)
	{
		// First create the goal path, that is, all nodes that lie on the
		// direct child path to the node.
		ArrayList gp = GBThreatSequence.GoalPath (root, goalnode);

		// Check if there is a clear refutable path node in the path and
		// return early if this is the case.
		//Console.WriteLine ("Path:");
		foreach (DBNode gpN in gp) {
			//Console.WriteLine ("   {0}", gpN);
			if (gpN.IsRefutePathRoot) {
				//Console.WriteLine ("DEBUG: node in refutepath");
				return (null);
			}
		}

		// Now combine the operators and build a one-by-one threat sequence.
		GBThreatSequence seq = GBThreatSequence.BuildThreatSequence (gp);

		// Clone and flip board
		GoBangBoard gbFlipped = (GoBangBoard) curBoard.Clone ();
		gbFlipped.Flip ();

		Console.Write ("  checking potential {0} pair-move sequence: ", gp.Count);
		Console.Out.Flush ();

		int refutes = DefenseRefutes (seq, gbFlipped);
		if (refutes < 0) {
			Console.WriteLine ("un-refutable");
			return (seq);
		}

		Console.WriteLine ("refutable at pair-move {0}", refutes);

		// Mark root of refutation
		if (refutes < (gp.Count-1)) {
			DBNode refutePathRoot = (DBNode) gp[refutes+1];
			refutePathRoot.IsRefutePathRoot = true;
		}

		return (null);
	}

	public GBThreatSequence FindWinningThreatSeqOTF ()
	{
		return (FindWinningThreatSeqOTF (0));
	}

	/** Try to find a winning threat sequence by dependency based search and
	 * on the fly refutation for goal nodes.
	 *
	 * Return early, as soon as a sure candidate has been found.
	 *
	 * @param timeoutMS If non-zero, a maximum time spend in db-search is
	 * given in milliseconds.  At least 1000 (1s) is meaningful to do
	 * something, though.
	 *
	 * @returns The first winning threat sequence on success, null otherwise.
	 */
	public GBThreatSequence FindWinningThreatSeqOTF (int timeoutMS)
	{
		// First find a number of possibly winning threat trees.
		GBSearchModule gbSearch = new GBSearchModule (GoBangBoard.boardDim);
		GBSpaceState rootState = new GBSpaceState ((GoBangBoard) gb.Clone ());
		rootState.UpdateIsGoal (gbSearch);

		// HEURISTIC: use category reduction (page 140-141)
		// FIXME: re-enable as soon as three3 bug is fixed.
		// FIXME: test if this is good in the real ai, otherwise disable again.
		//gbSearch.categoryReductionHeuristicOn = true;

		// Do on-the-fly refutation checking.
		gbSearch.doDefenseRefutationCheck = true;

		if (timeoutMS != 0) {
			gbSearch.doExpirationCheck = true;
			gbSearch.expireTime = DateTime.Now.AddMilliseconds (timeoutMS);
		}

		DBSearch db = new DBSearch (gbSearch, breadthFirst);

		try {
			Console.WriteLine ("Board:\n{0}\n", gb);
			db.Search (rootState);
			//db.DumpDOT ();
		} catch (GBSearchModule.GBSearchTimeoutException) {
			// We timed out...
			Console.WriteLine ("FindWinningThreatSeqOTF: timeouted...");
		} catch (GBWinningThreatSequenceFoundException gex) {
			//db.DumpDOT ();
			return (gex.seq);
		}

		return (null);
	}

	public class
	GBWinningThreatSequenceFoundException : ApplicationException
	{
		public GBThreatSequence seq;

		private GBWinningThreatSequenceFoundException ()
		{
		}

		public GBWinningThreatSequenceFoundException (GBThreatSequence seq)
		{
			this.seq = seq;
		}
	}

	// Global function
	public GBThreatSequence FindWinningThreatSeq ()
	{
		// First find a number of possibly winning threat trees.
		GBSearchModule gbSearch = new GBSearchModule (GoBangBoard.boardDim);
		GBSpaceState rootState = new GBSpaceState ((GoBangBoard) gb.Clone ());
		rootState.UpdateIsGoal (gbSearch);

		// HEURISTIC: use category reduction (page 140-141)
		gbSearch.categoryReductionHeuristicOn = true;

		DBSearch db = new DBSearch (gbSearch, breadthFirst);
		db.Search (rootState);
		//db.DumpDOTGoalsOnly ();

		// Now, enumerate all the possibly winning threat trees found
		GBThreatSequence[] potentialWinningSeqs =
			GBThreatSequence.BuildAllGoalPathes (gbSearch, db.Root);
		Console.WriteLine ("{0} potential winning threat sequences.",
			potentialWinningSeqs.Length);

		// Check them one by one until a surely winning threat tree is found
		GoBangBoard gbFlipped = (GoBangBoard) gb.Clone ();
		gbFlipped.Flip ();

		int DEBUGwinningFound = 0;
		GBThreatSequence DEBUGwinning = null;

		foreach (GBThreatSequence threatSeq in potentialWinningSeqs) {
			if (DefenseRefutes (threatSeq,
				(GoBangBoard) gbFlipped.Clone ()) < 0)
			{
				// Found a sure win, return early
				// FIXME: for debugging we count all winning sequences found,
				// but we should return as early as possible.
				DEBUGwinningFound += 1;
				DEBUGwinning = threatSeq;
				//Console.WriteLine ("WINNING:\n{0}", threatSeq);
				// FIXME
				//return (threatSeq);
			}
		}

		Console.WriteLine ("{0} winning of {1} potential winning threat sequences identified",
			DEBUGwinningFound, potentialWinningSeqs.Length);

		// Found no unrefuted threat sequence
		return (DEBUGwinning);
	}

	/** Check for refutability.
	 *
	 * @param seq The potential winning threat sequence to be checked.
	 * @param curBoard The current board against which the sequence shall be
	 * checked.
	 *
	 * @returns Negative value if the sequence is not refutable, otherwise the
	 * value returned is the refutation depth.
	 */
	private static int DefenseRefutes (GBThreatSequence seq, GoBangBoard curBoard)
	{
		return (DefenseRefutes (seq, curBoard, 0));
	}

	private static int DefenseRefutes (GBThreatSequence seq, GoBangBoard curBoard,
		int depth)
	{
		// If either we reached the end of the sequence (seq is null) or we
		// have a class zero threat, we consider the sequence to be
		// un-refutable and return a negative number.
		if (seq == null || seq.attackerThreatClass == 0)
			return (-1);

		// Make the attackers move (with -1 now, as the board is flipped)
		curBoard.board[seq.attacker.Y, seq.attacker.X] = -1;
		/*Console.WriteLine ("move at ({0},{1})",
			"abcdefghijklmnopqrstuvwxyz"[seq.attacker.X], seq.attacker.Y);

		Console.WriteLine ("DEFENSE board is:\n{0}", curBoard);
		Console.WriteLine ("   attacker threats with {0}", seq.attackerThreatClass);*/

		// Now search for possibly winning threat sequences that cover the
		// goals.  To do this, first build the goalsquares
		int[,] extraGoalSquares = ExtraGoalSquares (seq);

		// TODO
		GBSpaceState rootState =
			new GBSpaceState ((GoBangBoard) curBoard.Clone (),
				seq.attackerThreatClass);

		GBSearchModule gbSearch = new GBSearchModule (GoBangBoard.boardDim);

		// Extra constraints (page 137)
		//
		// 1. "The goal set U_g for player B should be extended with singleton
		// goals for occupying any square in threat a_j or reply d_j with j
		// \geq i."
		//
		// 2. "If B find a potential winning threat sequence, ... this threat
		// sequence is not investigated for counter play of player A.  Instead
		// in such a case we always assume that A's potential winning threat
		// sequence has been refuted."
		//
		// 3. "Thus in a db-search for player B, only threats having replies
		// consisting of a single move are applied."
		gbSearch.goalIfOccupied = extraGoalSquares;
		gbSearch.OneGoalStopsSearch = true;
		gbSearch.maximumCategory = seq.attackerThreatClass - 1;
		//Console.WriteLine ("       maxCat = {0}", gbSearch.maximumCategory);

		// Limit the root node's legal operators to only the one with the
		// appropiate category below the maximum one.  Disable the category
		// reduction heuristics, as we do the exhaustive defense search.
		rootState.UpdateLegalOperators (gbSearch.maximumCategory, false);

		// Do the db-search for the defender
		DBSearch db = new DBSearch (gbSearch, false);
		db.Search (rootState);
		if (db.GoalCount > 0) {
			/*
			Console.WriteLine ("Threat below class {0} or goal square occupied.",
				gbSearch.maximumCategory);
			db.DumpDOT ();
			Console.ReadLine ();*/

			return (depth);
		}

		/*Console.WriteLine ("No class {0} or below threats for the defender found yet",
			gbSearch.maximumCategory);*/

		// Make defenders move (now attacker 1, with advantage)
		foreach (GBMove defMove in seq.defender)
			curBoard.board[defMove.Y, defMove.X] = 1;
		//Console.WriteLine ("BOARD:\n{0}", curBoard);

		return (DefenseRefutes (seq.next, curBoard, depth+1));
	}

	private static int[,] ExtraGoalSquares (GBThreatSequence seq)
	{
		ArrayList squares = new ArrayList ();

		// Add only the defender moves for the moves just made
		foreach (GBMove defMove in seq.defender)
			squares.Add (new int[] { defMove.X, defMove.Y });

		// Add everything below.
		seq = seq.next;
		while (seq != null) {
			squares.Add (new int[] { seq.attacker.X, seq.attacker.Y });

			foreach (GBMove defMove in seq.defender)
				squares.Add (new int[] { defMove.X, defMove.Y });
			seq = seq.next;
		}

		int[,] sqA = new int[squares.Count, 2];
		for (int n = 0 ; n < squares.Count ; ++n) {
			sqA[n,0] = ((int[]) squares[n])[0];
			sqA[n,1] = ((int[]) squares[n])[1];
		}

		return (sqA);
	}
}


/** A potentially winning threat sequence.
 */
public class
GBThreatSequence
{
	/** A single attacker move.
	 */
	internal GBMove attacker;

	/** The threat class the attacker threatens with. (How many moves will it
	 * take him to win if the defender does not set any stone on the defending
	 * fields.)
	 */
	internal int attackerThreatClass;

	/** Zero or more defender moves.
	 */
	internal GBMove[] defender;

	/** The next attacker/defender move pair or null.
	 *
	 * If it is null, this is the end of the threat sequence and the last
	 * attacker move creates a double threat.
	 */
	internal GBThreatSequence next;

	private GBThreatSequence ()
	{
	}

	/** Check if the threat sequence is still usable in that all attacker and
	 * defender stones are still free on the given board.
	 *
	 * @param boardRev The reversed ([x,y]) board to be checked.
	 *
	 * @returns true if the threat sequence is still valid, false otherwise.
	 */
	public bool CheckValidRevBoard (int[,] boardRev)
	{
		if (boardRev[attacker.X, attacker.Y] != 0)
			return (false);

		foreach (GBMove gbm in defender) {
			if (boardRev[gbm.X, gbm.Y] != 0)
				return (false);
		}

		if (next == null)
			return (true);

		return (next.CheckValidRevBoard (boardRev));
	}

	public override string ToString ()
	{
		StringBuilder sb = new StringBuilder ();

		GBThreatSequence seq = this;
		sb.Append ("\n");
		sb.Append ("Sequence:\n");

		while (seq != null) {
			if (seq.attacker != null) {
				sb.AppendFormat ("  move: {0}, threat class {1}\n",
					seq.attacker, seq.attackerThreatClass);

				foreach (GBMove reply in seq.defender)
					sb.AppendFormat ("      def-reply: {0}\n", reply);
			} else {
				sb.AppendFormat ("  already GOAL");
			}

			seq = seq.next;
		}

		return (sb.ToString ());
	}

	public static GBThreatSequence[] BuildAllGoalPathes (GBSearchModule module,
		DBNode root)
	{
		ArrayList goalpathes = GoalPathes (module, root);
		Console.WriteLine ("I have {0} goal pathes", goalpathes.Count);

		// Create the threat sequences.
		ArrayList threatSeqs = new ArrayList ();

		// Build one individual threat sequence for each goalpath.
		foreach (ArrayList gp in goalpathes) {
			GBThreatSequence ts = BuildThreatSequence (gp);
			threatSeqs.Add (ts);
		}

		// DEBUG output
		Console.WriteLine ("{0} threat seqs lists", threatSeqs.Count);
		int seqNo = 1;
		foreach (GBThreatSequence seqW in threatSeqs) {
			GBThreatSequence seq = seqW;
			Console.WriteLine ();

			Console.WriteLine (((GBSpaceState) root.State).GB);
			Console.WriteLine ();
			Console.WriteLine ("Sequence {0}: ", seqNo);

			int m = 0;
			while (seq != null) {
				Console.WriteLine ("  move{0}: {1}, threat class {2}", m,
					seq.attacker, seq.attackerThreatClass);
				m += 1;

				foreach (GBMove reply in seq.defender) {
					Console.WriteLine ("      def-reply{0}: {1}", m, reply);
					m += 1;
				}

				seq = seq.next;
			}

			seqNo += 1;
		}

		return ((GBThreatSequence[])
			threatSeqs.ToArray (typeof (GBThreatSequence)));
	}

	// @param gp Goal path.
	public static GBThreatSequence BuildThreatSequence (ArrayList gp)
	{
		GBThreatSequence ts = new GBThreatSequence ();

		GBThreatSequence now = ts;
		for (int n = 0 ; n < gp.Count ; ++n) {
			DBNode pathnode = (DBNode) gp[n];
			GBSpaceState state = (GBSpaceState) pathnode.State;

			// First add the nodes of the path.
			if (state.CombinedOperPath != null) {
				/*Console.WriteLine ("DEBUG CombinedOperPath, {0} moves",
					state.CombinedOperPath.Count);*/

				foreach (GBOperator oper in state.CombinedOperPath) {
					//Console.WriteLine ("    oper: {0}", oper);
					FillInMoves (now, oper);

					now.next = new GBThreatSequence ();
					now = now.next;
				}
			}

			//Console.WriteLine ("DEBUG LastOperator");
			// Now the last operator
			GBOperator last = (GBOperator) state.LastOperator;
			if (last == null)
				continue;

			//Console.WriteLine ("    last: {0}", last);
			FillInMoves (now, last);

			// Chain it up, baby.
			now.next = null;
			if (n < (gp.Count - 1)) {
				now.next = new GBThreatSequence ();
				now = now.next;
			}
		}

		return (ts);
	}

	private static void FillInMoves (GBThreatSequence seq, GBOperator oper)
	{
		// Create attacker and defender moves.
		ArrayList defenderMoves = new ArrayList ();

		//Console.WriteLine ("DEBUG FillInMoves, {0} moves", oper.fAdd.GetLength (0));

		for (int move = 0 ; move < oper.fAdd.GetLength (0) ; ++move) {
			// Attacker move
			if (oper.fAdd[move, 2] == 1) {
				seq.attacker = new GBMove (oper.fAdd[move, 0],
					oper.fAdd[move, 1], oper.fAdd[move, 2]);
				seq.attackerThreatClass = oper.Class;
			} else if (oper.fAdd[move, 2] == -1) {
				defenderMoves.Add (new GBMove (oper.fAdd[move, 0],
					oper.fAdd[move, 1], oper.fAdd[move, 2]));
			}
		}
		seq.defender = (GBMove[]) defenderMoves.ToArray (typeof (GBMove));
	}


	/** Build a list of goal pathes.
	 */
	private static ArrayList GoalPathes (GBSearchModule module, DBNode root)
	{
		ArrayList goalpathes = new ArrayList ();

		GoalPathesI (module, goalpathes, root, new Stack ());

		return (goalpathes);
	}

	private static void GoalPathesI (GBSearchModule module,
		ArrayList goalpathes, DBNode node, Stack path)
	{
		if (node == null)
			return;

		if (module.IsGoal (node.State)) {
			ArrayList newGoalPath = new ArrayList ();

			foreach (DBNode pNode in path)
				newGoalPath.Add (pNode);

			newGoalPath.Reverse ();

			newGoalPath.Add (node);
			goalpathes.Add (newGoalPath);

			return;
		}

		foreach (DBNode child in node.Children) {
			path.Push (node);
			GoalPathesI (module, goalpathes, child, path);
			path.Pop ();
		}
	}

	/** Build a single goal path.
	 */
	public static ArrayList GoalPath (DBNode root, DBNode goalnode)
	{
		/*Console.WriteLine ("GoalPath (root = {0}, goalnode = {1})",
			root.DebugNN, goalnode.DebugNN);*/
		ArrayList goalpath = new ArrayList ();

		GoalPathI (goalpath, root, goalnode, new Stack ());

		return (goalpath);
	}

	private static void GoalPathI (ArrayList goalpath,
		DBNode node, DBNode goal, Stack path)
	{
		//Console.WriteLine ("GoalPathI");
		if (node == null)
			return;

		if (node == goal) {
			foreach (DBNode pNode in path)
				goalpath.Add (pNode);

			goalpath.Reverse ();
			goalpath.Add (node);

			return;
		}

		foreach (DBNode child in node.Children) {
			path.Push (node);
			GoalPathI (goalpath, child, goal, path);
			path.Pop ();
		}
	}
}


public class
GBMove
{
	int x, y;
	public int X {
		get {
			return (x);
		}
	}
	public int Y {
		get {
			return (y);
		}
	}
	int stone;
	public int Stone {
		get {
			return (stone);
		}
	}

	private GBMove ()
	{
	}

	public GBMove (int x, int y, int stone)
	{
		this.x = x;
		this.y = y;
		this.stone = stone;
	}

	public override string ToString ()
	{
		return (String.Format ("({0},{1}: {2})",
			"abcdefghijklmnopqrstuvwxyz"[x], y,
			stone == 1 ? "O" : "X"));
	}
}


