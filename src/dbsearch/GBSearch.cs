
using System;
using System.Text;
using System.Collections;

using DBSearchAlgorithm;


public class
GBSearchModule : DBSearchModule
{
	public ArrayList[,] nodesThatAffectSquares;

	private GBSearchModule ()
	{
	}

	public GBSearchModule (int boardDim)
	{
		// Initialize the affecting node array for fast combination checking.
		nodesThatAffectSquares = new ArrayList[boardDim, boardDim];

		for (int y = 0 ; y < nodesThatAffectSquares.GetLength (0) ; ++y)
			for (int x = 0 ; x < nodesThatAffectSquares.GetLength (1) ; ++x)
				nodesThatAffectSquares[y, x] = new ArrayList ();
	}

	public void CombinationStage (int level, DBNode root, DBSearch dbs)
	{
#if false
		ArrayList thisDepsStates = new ArrayList ();
		CombinationStageCompileDeps (level, root, thisDepsStates);

		GBOperatorCombineFastCheck cfc =
			new GBOperatorCombineFastCheck (dbs, this, thisDepsStates, level);

		cfc.CombineCheck (nodesThatAffectSquares);
#endif
	}
	public void CombinationStageCompileDeps (int level, DBNode node, ArrayList deps)
	{
		if (node == null)
			return;

		if (node.Type == DBNode.NodeType.Dependency &&
			node.Level == level &&
			node.IsGoal == false)
		{
			if (deps.Contains (node.State))
				return;
			/*if (deps.Contains (node.State))
				throw (new ArgumentException ("Invalid tree supplied"));*/

			deps.Add (node.State);

			// We do not need to look below.
			return;
		}

		foreach (DBNode child in node.Children)
			CombinationStageCompileDeps (level, child, deps);
	}


	public void DEBUGnodeArray ()
	{
		Console.WriteLine ("Node affection array count:");

		for (int y = 0 ; y < nodesThatAffectSquares.GetLength (0) ; ++y) {
			for (int x = 0 ; x < nodesThatAffectSquares.GetLength (1) ; ++x) {
				Console.Write ("{0:D2} ", nodesThatAffectSquares[y, x].Count);
			}
			Console.WriteLine ();
		}
	}

	public class
	GBSearchTimeoutException : ApplicationException
	{
	}

	public void DoExpirationCheck ()
	{
		if (doExpirationCheck) {
			if (DateTime.Now > expireTime)
				throw (new GBSearchTimeoutException ());
		}
	}

	public void RegisterNewNode (DBNode node, DBNode root)
	{
		DoExpirationCheck ();

		if (node.IsGoal == false)
			return;

		// For goal nodes, we check if on-the-fly refutation is wanted, and if
		// so, we try to refute the goal node.
		if (doDefenseRefutationCheck) {
			GBSpaceState gRootState = (GBSpaceState) root.State;
			GBThreatSequence seq = GBThreatSearch.DefenseRefutable
				(gRootState.GB, root, node);

			if (seq != null) {
				throw (new GBThreatSearch.GBWinningThreatSequenceFoundException (seq));
			}
		}

		// This is old code
#if false
		// Now the last node did modify something, but we only want to cache
		// dependency nodes, as combination nodes will not be combined
		// themselves.
		if (node.Type != DBNode.NodeType.Dependency)
			return;

		GBSpaceState state = (GBSpaceState) node.State;
		GBOperator last = state.LastOperator;
		if (last == null)
			return;

		// Now, we add this node to the array at all its influencing places,
		// but only where it sets a stone of the attacker (which is the only
		// position that can create new possibilities for attack).
		for (int n = 0 ; n < last.fAdd.GetLength (0) ; ++n) {
			// Ignore everything except the attacker stones.
			if (last.fAdd[n,2] != 1)
				continue;

			// Add coordinate
			nodesThatAffectSquares[last.fAdd[n, 1], last.fAdd[n, 0]].Add (node);

			return;
		}
#endif
	}

	/** Test the GBSearchModule
	 */
	public static void Main (string[] args)
	{
		// Initialize a board randomly
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

		gb.board[6,6] = gb.board[6,7] = gb.board[6,8] = 1;
		gb.board[7,7] = gb.board[8,6] = gb.board[8,7] = gb.board[8,8] = 1;
		gb.board[9,6] = gb.board[10,5] = gb.board[10,6] = gb.board[10,7] = 1;

		gb.board[6,5] = gb.board[7,5] = gb.board[7,6] = gb.board[7,8] = -1;
		gb.board[8,5] = gb.board[8,9] = gb.board[9,5] = gb.board[9,7] = gb.board[9,9] = -1;
		gb.board[10,4] = gb.board[11,6] = -1;

		gb.board[5,5] = 1;
		gb.board[4,4] = -1;

		gb.board[6,9] = 1;
		gb.board[6,10] = -1;
		gb.board[4,7] = 1;
		gb.board[5,7] = -1;

		/*
		gb.board[6,10] = 1;
		gb.board[6,9] = -1;
		*/

		/*
		gb.board[6,6] = gb.board[6,7] = gb.board[6,8] = 1;
		gb.board[7,7] = gb.board[8,6] = gb.board[8,7] = gb.board[8,8] = 1;
		gb.board[9,6] = gb.board[10,5] = gb.board[10,6] = gb.board[10,7] = 1;

		gb.board[6,5] = gb.board[7,5] = gb.board[7,6] = gb.board[7,8] = -1;
		gb.board[8,5] = gb.board[8,9] = gb.board[9,5] = gb.board[9,7] = gb.board[9,9] = -1;
		gb.board[10,4] = gb.board[11,6] = -1;

		// Move 1/2
		gb.board[5,5] = 1;
		gb.board[4,4] = -1;

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

		/* TODO: check this, ask marco
		gb.board[4,4] = gb.board[6,6] = gb.board[7,7] = 1;
		*/

		GBSearchModule gbSearch = new GBSearchModule (GoBangBoard.boardDim);
		GBSpaceState rootState = new GBSpaceState (gb);
		rootState.UpdateIsGoal (gbSearch);

		DBSearch db = new DBSearch (gbSearch, false);
		db.Search (rootState);

		db.DumpDOT ();
		//db.DumpDOTGoalsOnly ();
		gbSearch.DEBUGnodeArray ();
		/*
		foreach (DLPSpaceState state in db.GoalStates ()) {
			DumpOperatorChain (state);
		}
		*/
	}

	public IDBOperator[] LegalOperators (DBNode node)
	{
		GBSpaceState gstate = (GBSpaceState) node.State;

		return (gstate.LegalOperators);
	}

	public bool Applicable (IDBOperator oper, DBNode node)
	{
		GBOperator goper = (GBOperator) oper;
		GBSpaceState gstate = (GBSpaceState) node.State;

		return (goper.Applicable (gstate));
	}

	public bool Applicable (IDBOperator oper, GBSpaceState state)
	{
		GBOperator goper = (GBOperator) oper;

		return (goper.Applicable (state));
	}

	// TODO: create the appropiate documentation at this crucial point, see
	// DLPSearch.cs
	public bool NotInConflict (DBNode node1, DBNode node2)
	{
		return (NotInConflict ((GBSpaceState) node1.State,
			(GBSpaceState) node2.State));
	}

	public bool NotInConflict (GBSpaceState gstate1, GBSpaceState gstate2)
	{
		// We only need to check the last operator chain, as all previous have
		// been checked in the previous stages/levels
		while (gstate2 != null && gstate2.LastOperator != null) {
			if (Applicable (gstate2.LastOperator, gstate1) == false)
				return (false);

			gstate2 = gstate2.LastState;
		}
		return (true);
	}

	// TODO: this needs to be changed fundamentally, including changes in
	// DBSearch.cs.
	//
	// GoMoku will need up to four nodes combined in one combination step, for
	// a situation like this (artificial, but illustrates the point):
	//
	//    O  O  O
	//    O  O  O
	//    O3 O2 O1
	//
	// Lets assume the two rows at the top already existed before, and the
	// states O1, O2 and O3 have been independently explored (as they create a
	// new application of an operator, this will be the case).  However, then
	// the row O1, O2 and O3 create a new threat.  This can only be "seen" by
	// db-search if they are allowed to be combined in one step.  No single
	// combination will create a new dependent operator.
	//
	// We might do up-to-four combination efficiently by exploiting the board
	// geometry.  To do that, we create an array the same dimensions as the
	// board of linked list, storing states.  For each dependency node state,
	// store the state in a number of linked lists.  Store them in that linked
	// lists that are behind the coordinates in the f_{add} set of the last
	// operator of the state.
	//
	// Then, for each coordinate, we do the following: extract all linked
	// lists in a G_7 environment centered at the coordinate into one big
	// linked list.  Only check all the states refered in this big list for
	// 2-, 3- and 4-combinability.  For each coordinate we have to do this
	// four times for the different G_7 environment.
	//
	// The pseudo code for the whole combination stage would look like:
	//
	// foreach (Coordinate coord in boardCoordinates) {
	//   foreach (G_7 g7 centeredIn coord) {
	//     ArrayList states = new ArrayList ();
	//     foreach (Square sq in g7)
	//       states.AddRange (sq.States);
	//
	//     foreach (2-Combination (c1, c2) in states)
	//       CheckCombinability (c1, c2);
	//     foreach (3-Combination (c1, c2, c3) in states)
	//       CheckCombinability (c1, c2, c3);
	//     foreach (4-Combination (c1, c2, c3, c4) in states)
	//       CheckCombinability (c1, c2, c3, c4);
	//   }
	// }
	//
	// The numerical complexity is (n \over k) = \frac{n!}{k! (n - k)!}
	// where n = number of states collected in "states". k = number of
	// elements in the combination (2, 3, 4).
	//
	// So, for n states on the combined G_7 square list this would give
	// (n \over k) k-Combinations.
	//
	// States |  2-C |    3-C |     4-C
	// -------+------+--------+--------
	//     10 |   45 |    120 |     210
	//     20 |  190 |   1140 |    4845
	//    100 | 4950 | 161700 | 3921225
	//
	// So we should keep the number of states influencing a board square well
	// below 100, while <= 20 is certainly ok.
	//
	// XXX: for now, we only test with two-combinability
	public IDBSpaceState CombineIfResultIsNewOperators (DBNode node1,
		Stack node1Path, DBNode node2, Stack node2Path)
	{
		GBSpaceState gstate1 = (GBSpaceState) node1.State;
		GBSpaceState gstate2 = (GBSpaceState) node2.State;
		GBOperator last2 = gstate2.LastOperator;

		// First check the boards are not incompatible:
		// TODO: check if this is right (or necessary, does not seem to change
		// any results).
		if (gstate2.GB.CompatibleWith (((GBSpaceState) node1.State).GB) == false)
			return (null);

		// Build combined state by applying operator chain.
		ArrayList operatorChain = new ArrayList ();
		foreach (DBNode pN in node2Path) {
			if (node1Path.Contains (pN))
				break;

			GBSpaceState nstate = (GBSpaceState) pN.State;
			operatorChain.Add (nstate.LastOperator);

			if (nstate.CombinedOperPath != null) {
				ArrayList combRev = (ArrayList) nstate.CombinedOperPath.Clone ();
				combRev.Reverse ();

				operatorChain.AddRange (combRev);
			}
		}
		operatorChain.Reverse ();
		operatorChain.Add (last2);

		GBSpaceState gComb = (GBSpaceState) node1.State;
		foreach (GBOperator oper in operatorChain)
			gComb = (GBSpaceState) oper.Apply (this, gComb);

		//GBSpaceState gComb = (GBSpaceState) last2.Apply (this, node1.State);
		//gComb.GB.AddExtraStones (gstate2.GB);
		gComb.UpdateLegalOperators (maximumCategory, categoryReductionHeuristicOn);

		// Now check if the new state results in new operators
		GBOperator[] n1o = gstate1.LegalOperators;
		GBOperator[] n2o = gstate2.LegalOperators;

		GBOperator[] nCo = gComb.LegalOperators;
		if (nCo == null)
			return (null);

		// Check: nCo \setminus (n1o \cup n2o) \neq \emptyset
		foreach (GBOperator gbo in nCo) {
			if (n1o != null && Array.IndexOf (n1o, gbo) >= 0)
				return (null);
			if (n2o != null && Array.IndexOf (n2o, gbo) >= 0)
				return (null);
		}

		gComb.UpdateIsGoal (this);

		// Now that the combination succeeded, we still need to copy over all
		// the operators we applied 'at once' when copying the field content.
		// We need to do this for the threat sequence search later, which
		// needs operator-level granularity for the defense search.
		gComb.BuildCombinedOperatorPath (node1Path, node2Path);

		return (gComb);
	}

	/** Check the goal conditions on page 135.
	 *
	 * @param state The space state to be checked for goal'iness.
	 *
	 * @returns True in case it is a goal state, false otherwise.
	 */
	public bool IsGoal (IDBSpaceState stateDB)
	{
		GBSpaceState state = (GBSpaceState) stateDB;

		return (state.IsGoal);
	}

	public int GoalCountThresh {
		get {
			return (0);
		}
	}

	internal int maximumCategory = 2;

	/** The category reduction heuristics (page 140-141).  For a global
	 * defense search this must be disabled, but for the potential winning
	 * threat sequence search it can be enabled for node reduction.
	 */
	internal bool categoryReductionHeuristicOn = false;

	/** This should be set to true for the attacker so on-the-fly refutation
	 * is attempted.
	 */
	internal bool doDefenseRefutationCheck = false;

	internal bool doExpirationCheck = false;
	internal DateTime expireTime;

	/** The additional goal fields which, when occupied, will mark a goal
	 * state.  This is only important for the global defense search.
	 *
	 * The format is [,0] is the x coordinate, [,1] the y coordinate.
	 */
	internal int[,] goalIfOccupied = null;

	internal bool oneGoalStopsSearch = false;
	public bool OneGoalStopsSearch {
		get {
			return (oneGoalStopsSearch);
		}
		set {
			oneGoalStopsSearch = value;
		}
	}
}


public class
GBSpaceState : IDBSpaceState
{
	public override bool Equals (object o2)
	{
		if (o2 == null)
			return (false);

		return (CompareTo (o2) == 0);
	}

	public int CompareTo (object o2)
	{
		GBSpaceState gbs2 = (GBSpaceState) o2;

		return (gb.CompareTo (gbs2.gb));
	}

	GBOperator lastOperator = null;
	public GBOperator LastOperator {
		get {
			return (lastOperator);
		}
	}

	GBSpaceState lastState = null;
	public GBSpaceState LastState {
		get {
			return (lastState);
		}
	}

	private GoBangBoard gb;
	public GoBangBoard GB {
		get {
			return (gb);
		}
	}

	public string DescShort {
		get {
			StringBuilder sb = new StringBuilder ();

			sb.Append (PrettyPrint.PrintBoardContent (gb.board,
				IsGoal ? "#ffff00" : null, false));

			sb.AppendFormat ("<tr><td>Is goal: {0}</td></tr>\n", IsGoal ? "YES" : "no");
			sb.AppendFormat ("<tr><td>Last operator: {0}</td></tr>\n",
				lastOperator == null ? "-" : lastOperator.ToString ());

			if (LegalOperators != null) {
				foreach (GBOperator goper in LegalOperators)
					sb.AppendFormat ("<tr><td>Operator: {0}</td></tr>\n", goper);
			}

			sb.Append ("</table>");

			return (sb.ToString ());
			/*
			string nodeContent = "?";
			if (lastOperator != null) {
				for (int n = 0 ; n < lastOperator.fAdd.GetLength (0) ; ++n) {
					if (lastOperator.fAdd[n,2] == 1)
						nodeContent = String.Format ("{0},{1}",
							"abcdefghijklmnopqrstuvwxyz"[lastOperator.fAdd[n,0]],
							lastOperator.fAdd[n,1]);
				}
			}
			return (String.Format ("<table bgcolor=\"{0}\"><tr><td>{1}</td></tr></table>",
				IsGoal ? "#ffff00" : "#ffffff", nodeContent));
				*/
		}
	}

	private bool isGoal;
	public bool IsGoal {
		get {
			return (isGoal);
		}
	}

	public bool UpdateIsGoal (DBSearchModule dbS)
	{
		GBSearchModule gbS = (GBSearchModule) dbS;
		int white, hole;

		// 1. Five
		foreach (GoBangBoard.StoneSet ss in GB.G5) {
			GBOperator.CountStones (ss, out white, out hole);

			if (white == 5 && hole == 0) {
				/*Console.WriteLine (GB);
				Console.WriteLine ("=> goal because of five");*/

				isGoal = true;
				return (true);
			}
		}

		// 2. Straight four
		// FIXME/TODO: check if this is right, i mean the check here
		if (gbS.maximumCategory >= 1) {
			foreach (GoBangBoard.StoneSet ss in GB.G6) {
				if (ss.stones[0] != 0 || ss.stones[5] != 0)
					continue;

				GBOperator.CountStones (ss, out white, out hole);

				if (white == 4 && hole == 2) {
					/*Console.WriteLine (GB);
					Console.WriteLine ("goal because of four");*/
					isGoal = true;
					return (true);
				}
			}
		}

		// Check if there are extra fixed goal squares.
		if (gbS.goalIfOccupied == null) {
			isGoal = false;

			return (false);
		}
// There extra squares, check if they are occupied by the defender
		// (-1, but in the flipped field its 1) in this state.
		for (int n = 0 ; n < gbS.goalIfOccupied.GetLength (0) ; ++n) {
			if (gb.board[gbS.goalIfOccupied[n,1], gbS.goalIfOccupied[n,0]] == 1) {
				/*Console.WriteLine (GB);
				Console.WriteLine ("goal because occupation of ({0},{1})",
					gbS.goalIfOccupied[n,0], gbS.goalIfOccupied[n,1]);*/
				isGoal = true;

				return (true);
			}
		}

		// Neither a necessary field has been occupied, nor a five/four been
		isGoal = false;
		return (false);
	}

	private GBOperator[] legalOperators;
	public GBOperator[] LegalOperators {
		get {
			return (legalOperators);
		}
		set {
			legalOperators = value;
		}
	}

	// element type: GBOperator
	ArrayList combinedOperPath = null;
	public ArrayList CombinedOperPath {
		get {
			return (combinedOperPath);
		}
	}

	public void BuildCombinedOperatorPath (Stack node1Path, Stack node2Path)
	{
		// Root is at the end of the path'es
		/*
		foreach (DBNode dn in node1Path)
			Console.WriteLine ("node1Path: {0}", dn);
		foreach (DBNode dn in node2Path)
			Console.WriteLine ("node2Path: {0}", dn);
			*/

		// We need to keep everything in node2Path until we meet a node of
		// node1
		foreach (DBNode pN in node2Path) {
			if (node1Path.Contains (pN))
				break;

			// Now the node pN was reached by applying an operator.
			// We have two kinds of operators to stack up: the ones in the
			// 'last' variable, which is the direct last operator applied.
			// And, because the node can have combinedOperPath operators, too,
			// we also need to add them.  The come chronologically first, but
			// we build this list in reverse, so we have to reverse them here,
			// too.
			GBSpaceState state = (GBSpaceState) pN.State;

			if (combinedOperPath == null)
				combinedOperPath = new ArrayList ();

			combinedOperPath.Add (state.LastOperator);

			// If there are combined operator in the path already, reverse
			// them.
			if (state.combinedOperPath != null) {
				ArrayList combRev = (ArrayList) state.combinedOperPath.Clone ();
				combRev.Reverse ();

				combinedOperPath.AddRange (combRev);
			}
		}

		// Make it a chronological chain that comes before 'LastOperator'
		if (combinedOperPath != null)
			combinedOperPath.Reverse ();
	}

	public void UpdateLegalOperators (int maxCat, bool doCatReduction)
	{
		// If we do category reduction, we shall proceed like this:
		// "... if in a node N of the db-search DAG, the defender has a threat
		// of category c_1, for each descendent of N the attacker is
		// restricted to threats of categories less than c_1".
		//
		// So we first find out the threats of the defender
		if (maxCat >= 1 && doCatReduction) {
			// Reverse attacker/defender
			GoBangBoard gbFlip = (GoBangBoard) gb.Clone ();
			gbFlip.Flip ();

			int lowestCat = GBOperator.LowestOperatorCategory (gbFlip);
			//Console.WriteLine ("lowestCat = {0}, maxCat = {1}", lowestCat, maxCat);

			// If the defender has a five already, there is no need to search.
			if (lowestCat == 0) {
				this.legalOperators = null;

				return;
			}

			// Now, we have to add one to the lowest category.  This is
			// because the operators judge what they can create in one move,
			// not what is already there on the board.
			lowestCat += 1;

			// Otherwise, if the defenders lowest category threat is below or
			// equal to the current maximum category, we decrease the maximum
			// category to allow only those attacker threats that can still
			// force.
			if (lowestCat <= maxCat) {
				Console.WriteLine ("doCatRed: from {0} to {1}", maxCat, lowestCat -1);
				maxCat = lowestCat - 1;
			} else {
				Console.WriteLine ("maxCat = {0}", maxCat);
			}
		}

		this.legalOperators = GBOperator.LegalOperators (this, maxCat);
	}

	private GBSpaceState ()
	{
	}

	public GBSpaceState (GoBangBoard gb)
		: this (gb, null, null, 2)
	{
	}

	public GBSpaceState (GoBangBoard gb, int maxCat)
		: this (gb, null, null, maxCat)
	{
	}

	public GBSpaceState (GoBangBoard gb, GBOperator lastOperator,
		GBSpaceState lastState, int maxCat)
		: this (gb, lastOperator, lastState, null)
	{
		this.legalOperators = GBOperator.LegalOperators (this, maxCat);
	}

	public GBSpaceState (GoBangBoard gb, GBOperator lastOperator,
		GBSpaceState lastState, GBOperator[] legalOperators)
	{
		this.gb = gb;
		this.lastOperator = lastOperator;
		this.lastState = lastState;

		this.legalOperators = legalOperators;
	}
}


/** The base class for all GoBang operators.
 */
public class
GBOperator : IDBOperator
{
	// First dimension: the number of changed squares on the board,
	// Second dimension, 0/1: x/y coordinates, 2: value required (Pre) or
	//   set (Add)
	internal int[,] fAdd;

	// The global threat class (0, 1, 2).
	public virtual int Class {
		get {
			return (-1);
		}
	}

	public virtual string Name {
		get {
			return ("???");
		}
	}

	/** Test for operator equality.  This is important as to identify common
	 * operators in combination nodes.
	 *
	 * @param o2 The other operator to be compared.
	 *
	 * @returns True in case they are equal, false otherwise.
	 */
	public override bool Equals (object o2)
	{
		if (o2 == null)
			return (false);

		GBOperator op2 = (GBOperator) o2;

		if (Name != op2.Name)
			return (false);

		if (fAdd.GetLength (0) != op2.fAdd.GetLength (0))
			return (false);

		// XXX: Tricky, as we have not specified an ordering for the f_{add}
		// set (which we maybe should do, as we could get rid of the inner
		// loop then).  So, operator equality should be on board level, hence
		// we need to compare the stone coordinates, and not the relative
		// settings in the fAdd array.
		for (int n1 = 0 ; n1 < fAdd.GetLength (0) ; ++n1) {
			for (int n2 = 0 ; n2 < fAdd.GetLength (0) ; ++n2) {
				if (fAdd[n1,0] == op2.fAdd[n2,0] &&
					fAdd[n1,1] == op2.fAdd[n2,1] &&
					fAdd[n1,2] == op2.fAdd[n2,2])
					goto foundStone;
			}

			// Stone was not found, operators are not equal
			return (false);

foundStone:
			;
		}

		// All stones have been found.
		return (true);
	}

	public override string ToString ()
	{
		StringBuilder str = new StringBuilder ();
		str.AppendFormat ("{0} {{ ", Name);
		for (int n = 0 ; n < fAdd.GetLength (0) ; ++n) {
			str.AppendFormat ("({0},{1}: {2}), ",
				"abcdefghijklmno"[fAdd[n,0]], fAdd[n,1],
				fAdd[n,2] == 1 ? "O" : (fAdd[n,2] == -1 ? "X" : "?"));
		}
		str.Append ("}");

		return (str.ToString ());
	}

	// TODO: maybe remove this, its useless now
	public bool Applicable (GBSpaceState state)
	{
		if (Valid (state) == false)
			return (false);

		return (true);
	}

	/** Check if the operator is formally valid.
	 *
	 * Note: we only check if the squares the operator will occupy are
	 * currently free.  The logical check if the operator is applicable must
	 * have preceeded this one already.
	 *
	 * @param state The state the operator will be tested for validity on.
	 *
	 * @returns True if the operator is valid, false otherwise.
	 */
	public bool Valid (GBSpaceState state)
	{
		for (int n = 0 ; n < fAdd.GetLength (0) ; ++n) {
			if (state.GB.board[fAdd[n,1], fAdd[n,0]] != 0)
				return (false);
		}

		return (true);
	}

	/** Apply the operator to the given state, returning a new state.
	 *
	 * @param state The source state.
	 *
	 * @returns The newly created destination state.
	 */
	public IDBSpaceState Apply (DBSearchModule module, IDBSpaceState stateDB)
	{
		GBSearchModule gmod = (GBSearchModule) module;
		GBSpaceState state = (GBSpaceState) stateDB;

		// TODO: remove later, we already checked this previously
		if (Valid (state) == false)
			throw (new ArgumentException ("Operator not applicable!"));

		GoBangBoard newBoard = (GoBangBoard) state.GB.Clone ();

		// Apply all the f_{add} stones
		for (int n = 0 ; n < fAdd.GetLength (0) ; ++n)
			newBoard.board[fAdd[n,1], fAdd[n,0]] = fAdd[n,2];

		GBSpaceState newState = new GBSpaceState (newBoard, this, state,
			gmod.maximumCategory);
		newState.UpdateIsGoal (gmod);

		return (newState);
	}


	public static GBOperator[] LegalOperators (DBNode node, int maxCat)
	{
		GBSpaceState dst = (GBSpaceState) node.State;

		return (LegalOperators (dst, maxCat));
	}

	// Return the operator's category that is the lowest among the applicable
	// operators for state.
	public static int LowestOperatorCategory (GoBangBoard board)
	{
		// A maximum, not important, just must be larger than any real
		// category
		int max = 99;

		// Search category 0 and 1 in G_5
		foreach (GoBangBoard.StoneSet ss in board.G5) {
			// There is no category below zero, so we can return early.
			if (GBOperatorFive.Present (ss))
				return (0);

			if (GBOperatorFour.Present (ss))
				max = 1;
		}

		if (max < 99)
			return (max);

		// Search category 1 and 2 in G_6
		foreach (GoBangBoard.StoneSet ss in board.G6) {
			// If there was no 0/1 before, this is certainly the minimum.
			if (GBOperatorStraightFour.Present (ss))
				return (1);

			if (max > 2 && GBOperatorBrokenThree.Present (ss)) {
				max = 2;
			} else if (max > 2 && GBOperatorThree3.Present (ss))
				max = 2;
		}

		if (max < 99)
			return (max);

		foreach (GoBangBoard.StoneSet ss in board.G7) {
			if (GBOperatorThree2.Present (ss))
				return (2);
		}

		return (99);
	}

	public static GBOperator[] LegalOperators (GBSpaceState state, int maxCat)
	{
		ArrayList opersGoal = new ArrayList ();
		ArrayList opersFours = new ArrayList ();
		ArrayList opersThrees = new ArrayList ();

		// Check G_5 for operators
		foreach (GoBangBoard.StoneSet ss in state.GB.G5) {
			if (IsLastOperatorDependent (ss, state.LastOperator) == false)
				continue;

			GBOperatorFive[] fives = null;
			if (maxCat >= 0)
				fives = GBOperatorFive.GetOperatorsIfValid (ss);

			GBOperatorFour[] fours = null;
			if (maxCat >= 1)
				fours = GBOperatorFour.GetOperatorsIfValid (ss);

			if (fives != null)
				opersGoal.AddRange (fives);
			if (fours != null)
				opersFours.AddRange (fours);
		}

		bool Three2Applicable = false;
		if (maxCat >= 2) {
			// Check G_7 for operators
			foreach (GoBangBoard.StoneSet ss in state.GB.G7) {
				if (IsLastOperatorDependent (ss, state.LastOperator) == false)
					continue;

				/*
				Console.Write ("7ss: ");
				for (int n = 0 ; n < ss.stones.Length ; ++n)
					Console.Write ("{0} ", ss.stones[n] == 1 ? "O" :
						(ss.stones[n] == -1 ? "X" : "."));
				Console.WriteLine ();
				*/

				GBOperatorThree2[] three2 =
					GBOperatorThree2.GetOperatorsIfValid (ss);

				if (three2 != null) {
					Three2Applicable = true;
					opersThrees.AddRange (three2);
				}
			}
		}

		// Check G_6 for operators
		if (maxCat >= 1) {
			foreach (GoBangBoard.StoneSet ss in state.GB.G6) {
				if (IsLastOperatorDependent (ss, state.LastOperator) == false)
					continue;

				GBOperatorStraightFour[] sfours = null;
				if (maxCat >= 1)
					sfours = GBOperatorStraightFour.GetOperatorsIfValid (ss);

				GBOperatorBrokenThree[] bthrees = null;
				GBOperatorThree3[] three3s = null;
				if (maxCat >= 2) {
					bthrees = GBOperatorBrokenThree.GetOperatorsIfValid (ss);
				}

				// Heuristic "restricted trees", page 141.
				// HEURISTIC
				// FIXME: re-enable this after testing.
				if (maxCat >= 2 /*&& Three2Applicable == false*/) {
					three3s = GBOperatorThree3.GetOperatorsIfValid (ss);
				}

				if (sfours != null)
					opersGoal.AddRange (sfours);
				if (bthrees != null)
					opersThrees.AddRange (bthrees);
				if (three3s != null)
					opersThrees.AddRange (three3s);
			}
		}

		// Order: goal, fours, threes
		opersGoal.AddRange (opersFours);
		opersGoal.AddRange (opersThrees);

		if (opersGoal.Count > 0)
			return ((GBOperator[]) opersGoal.ToArray (typeof (GBOperator)));

		return (null);
	}

	public static void CountStones (GoBangBoard.StoneSet ss,
		out int white, out int hole)
	{
		white = hole = 0;

		for (int n = 0 ; n < ss.stones.Length ; ++n) {
			if (ss.stones[n] == 1)
				white += 1;
			else if (ss.stones[n] == 0)
				hole += 1;
		}
	}

	private static bool IsLastOperatorDependent (GoBangBoard.StoneSet ss,
		GBOperator last)
	{
		// Special case for root node
		if (last == null)
			return (true);

		int px = ss.x, py = ss.y;

		for (int n = 0 ; n < ss.stones.Length ; ++n) {
			// Now check if it could theoretically depend.  Then the
			// last operator must have touched (fAdd) stones available
			// in this stoneset.
			for (int k = 0 ; k < last.fAdd.GetLength (0) ; ++k) {
				if (last.fAdd[k,0] == px && last.fAdd[k,1] == py)
					return (true);
			}
			py += ss.ay;
			px += ss.ax;
		}

		return (false);
	}
}



