
using System;
using System.Collections;

using DBSearchAlgorithm;


#if false
public class
GBOperatorCombineFastCheck
{
	private DBSearch dbS;
	private GBSearchModule gbS;
	private ArrayList thisStageDependencies;
	private int level;

	private GBOperatorCombineFastCheck ()
	{
	}

	public GBOperatorCombineFastCheck (DBSearch dbS, GBSearchModule gbS,
		ArrayList thisStageDependencies, int level)
	{
		this.dbS = dbS;
		this.gbS = gbS;
		this.thisStageDependencies = thisStageDependencies;
		this.level = level;
	}

	public void CombineCheck (ArrayList[,] affect)
	{
		int patternG5 = 0;
		GoBangBoard board = new GoBangBoard ();

		ArrayList[] affecting = new ArrayList[5];

		foreach (GoBangBoard.StoneSet ss in board.G5) {
			for (int n = 0 ; n < ss.stones.Length ; ++n)
				affecting[n] = affect[ss.y + n*ss.ay, ss.x + n*ss.ax];

			// Now in positions we have 'oneBits' number of positions.
			CheckC2 (affecting);
#if false
			// Do a pre-filtering of sensible patterns in this G_5
			bool[] fivePatterns = new bool[1 << 5];

			for (int p = 0 ; p < fivePatterns.Length ; ++p) {
				fivePatterns[p] = false;

				for (int n = 0 ; n < 5 ; ++n) {
					ss.stones[n] = (p & (1 << n)) == 0 ? 0 : 1;

					if (ss.stones[n] == 1 && affecting[n].Count == 0)
						goto nextPattern;
				}

				/*
				Console.Write ("A pattern {0}: ", p);
				for (int n = 0 ; n < 5 ; ++n)
					Console.Write ("{0}", ss.stones[n] == 1 ? "O" : ".");
				Console.WriteLine ();
				*/

				GBOperatorFive[] fiveP =
					GBOperatorFive.GetOperatorsIfValid (ss);
				GBOperatorFour[] fourP =
					GBOperatorFour.GetOperatorsIfValid (ss);
				/*
				Console.WriteLine ("  pattern {0}: {1} fives, {2} fours", p,
					(fiveP == null) ? "-" : String.Format ("{0}", fiveP.Length),
					(fourP == null) ? "-" : String.Format ("{0}", fourP.Length));
					*/

				// Only if we trigger a pattern, we mark it for later action.
				if ((fiveP != null && fiveP.Length > 0) ||
					(fourP != null && fourP.Length > 0))
				{
					fivePatterns[p] = true;
					//Console.WriteLine ("  pattern {0} OK", p);
				}
nextPattern:
				;
			}

			for (int p = 0 ; p < fivePatterns.Length ; ++p) {
				if (fivePatterns[p] == false)
					continue;

				// Now examine the pattern really.
				Console.WriteLine ("pattern {0} success", p);
				Console.WriteLine ("    a[0 to 4]: {0}, {1}, {2}, {3}, {4}",
					affecting[0].Count,
					affecting[1].Count,
					affecting[2].Count,
					affecting[3].Count,
					affecting[4].Count);

				patternG5 += 1;

				// TODO
				// Now we have an array of list of affecting moves plus a
				// pattern they may trigger.  We know that those moves in the
				// same array element are conflicting, so we do not need to
				// check those.  Also, we know the new operators are
				// independent of exchange of operators (commutative), so we
				// must only check each combination once, regardless of the
				// order.
				int oneBits = 0;
				for (int n = 0 ; n < 5 ; ++n) {
					if ((p & (1 << n)) != 0)
						oneBits += 1;
				}
				int[] positions = new int[oneBits];

				int oneBitPos = 0;
				for (int n = 0 ; n < 5 ; ++n) {
					if ((p & (1 << n)) == 0)
						continue;

					positions[oneBitPos] = n;
					oneBitPos += 1;
				}
			}
#endif
		}
		//Console.WriteLine ("G5 patterns: {0}", patternG5);
	}

	private void CheckC2 (ArrayList[] affecting)
	{
		int[] n = new int[2];

		for (n[0] = 0 ; n[0] < affecting.Length ; ++n[0]) {
			for (n[1] = (n[0] + 1) ; n[1] < affecting.Length ; ++n[1]) {
				int[] count = new int[2];
				int countN;
				if (affecting[n[0]].Count == 0 || affecting[n[1]].Count == 0)
					continue;

				do {
					// Enumerate all possible combinations at position (n0, n1)
					DBNode node1 = (DBNode) affecting[n[0]][count[0]];
					DBNode node2 = (DBNode) affecting[n[1]][count[1]];
					GBSpaceState state1 = (GBSpaceState) node1.State;
					GBSpaceState state2 = (GBSpaceState) node2.State;

					// Check that there is a appropiate dependency node in it and
					// then for combinability for state1/state2
					if ((thisStageDependencies.Contains (state1) == true ||
						thisStageDependencies.Contains (state2) == true) &&
						state2.GB.CompatibleWith (state1.GB) &&
						gbS.NotInConflict (state1, state2))
					{
						GBSpaceState gComb =
							(GBSpaceState) state2.LastOperator.Apply (gbS, state1);
						gComb.GB.AddExtraStones (state2.GB);
						gComb.UpdateLegalOperators (gbS.maximumCategory);

						// Now check if the new state results in new operators
						GBOperator[] n1o = state1.LegalOperators;
						GBOperator[] n2o = state2.LegalOperators;

						GBOperator[] nCo = gComb.LegalOperators;
						if (nCo == null)
							goto nextComb;
						//Console.WriteLine ("nCo.Length = {0}", nCo.Length);

						// Check: nCo \setminus (n1o \cup n2o) \neq \emptyset
						bool seenUnique = false;
						foreach (GBOperator gbo in nCo) {
							if (n1o != null && Array.IndexOf (n1o, gbo) >= 0)
								continue;
							if (n2o != null && Array.IndexOf (n2o, gbo) >= 0)
								continue;

							seenUnique = true;
							//Console.WriteLine ("unique: {0}", gbo);
						}
						if (seenUnique == false)
							goto nextComb;

						// We have found a new combination
						/*
						Console.WriteLine ("TODO: found found found:");
						Console.WriteLine ("From 1: {0}", state1.GB);
						Console.WriteLine ("From 2: {0}", state2.GB);
						Console.WriteLine ("To: {0}", gComb.GB);
						throw (new ArgumentException ("DEBUG"));
						*/

						DBNode combNode = new DBNode (DBNode.NodeType.Combination,
							gComb, level);
						combNode.IsGoal = gbS.IsGoal (gComb);

						/* TODO: have to get this back to db-search somehow,
						 * or throw exception here.
						if (combNode.IsGoal)
							goalCount += 1;
						*/

						dbS.AddCombinationNode (node1, combNode);
						node2.CombinedChildren.Add (combNode);
						gbS.RegisterNewNode (combNode);
					}

nextComb:
					for (countN = 0 ; countN < count.Length ; ++countN) {
						count[countN] += 1;

						if (count[countN] < affecting[n[countN]].Count)
							break;

						count[countN] = 0;
					}
				} while (countN < count.Length);
			}
		}
	}
}
#endif


public class
GBOperatorFive : GBOperator
{
	public override int Class {
		get {
			return (0);
		}
	}

	public override string Name {
		get {
			return ("Five");
		}
	}

	public static bool Present (GoBangBoard.StoneSet ss)
	{
		int countWhite = 0;
		int countHole = 0;

		CountStones (ss, out countWhite, out countHole);
		if (countWhite != 4 || countHole != 1)
			return (false);

		return (true);
	}

	public static GBOperatorFive[] GetOperatorsIfValid
		(GoBangBoard.StoneSet ss)
	{
		if (Present (ss) == false)
			return (null);

		// Operator is valid, lets apply it.
		GBOperatorFive five = new GBOperatorFive ();

		five.fAdd = new int[1,3];
		for (int n = 0 ; n < ss.stones.Length ; ++n) {
			if (ss.stones[n] != 0)
				continue;

			five.fAdd[0,0] = ss.x + n*ss.ax;
			five.fAdd[0,1] = ss.y + n*ss.ay;
			five.fAdd[0,2] = 1;
		}

		return (new GBOperatorFive[] { five });
	}
}


public class
GBOperatorFour : GBOperator
{
	public override int Class {
		get {
			return (1);
		}
	}

	public override string Name {
		get {
			return ("Four");
		}
	}

	public static bool Present (GoBangBoard.StoneSet ss)
	{
		int countWhite = 0;
		int countHole = 0;

		CountStones (ss, out countWhite, out countHole);
		if (countWhite != 3 || countHole != 2)
			return (false);

		return (true);
	}

	public static GBOperatorFour[] GetOperatorsIfValid
		(GoBangBoard.StoneSet ss)
	{
		if (Present (ss) == false)
			return (null);

		// Operator is valid, lets apply two variations
		GBOperatorFour four1 = new GBOperatorFour ();
		GBOperatorFour four2 = new GBOperatorFour ();

		four1.fAdd = new int[2,3];
		four2.fAdd = new int[2,3];
		int hole = 0;
		for (int n = 0 ; n < ss.stones.Length ; ++n) {
			if (ss.stones[n] != 0)
				continue;

			four2.fAdd[hole,0] = four1.fAdd[hole,0] = ss.x + n*ss.ax;
			four2.fAdd[hole,1] = four1.fAdd[hole,1] = ss.y + n*ss.ay;

			if (hole == 0) {
				four1.fAdd[0,2] = 1;
				four2.fAdd[0,2] = -1;

				hole += 1;
			} else {
				four1.fAdd[1,2] = -1;
				four2.fAdd[1,2] = 1;
			}
		}

		return (new GBOperatorFour[] { four1, four2 });
	}
}


public class
GBOperatorThree2 : GBOperator
{
	public override int Class {
		get {
			return (2);
		}
	}

	public override string Name {
		get {
			return ("Three (2 replies)");
		}
	}

	public static bool Present (GoBangBoard.StoneSet ss)
	{
		/*Console.Write ("three ({0},{1}): ", ss.x, ss.y);
		foreach (int a in ss.stones)
			Console.Write ("{0}", a == 1 ? "O" : (a == 0 ? "." : "X"));
		Console.WriteLine ();*/

		// The pattern is ..???.., with "." position fixed, so we can
		// early-check on this now.
		if (ss.stones[0] != 0 || ss.stones[1] != 0 ||
			ss.stones[5] != 0 || ss.stones[6] != 0)
			return (false);

		// Now the middle three (index 2, 3, 4) have to be tested
		int countWhite = 0;
		int countHole = 0;

		CountStones (ss, out countWhite, out countHole);
		if (countWhite != 2 || countHole != 5) {
			return (false);
		}

		return (true);
	}

	public static GBOperatorThree2[] GetOperatorsIfValid
		(GoBangBoard.StoneSet ss)
	{
		if (Present (ss) == false)
			return (null);

		// Operator is valid, lets produce it (only one variant)
		GBOperatorThree2 three2 = new GBOperatorThree2 ();

		three2.fAdd = new int[3,3];
		for (int n = 2 ; n < (ss.stones.Length - 2) ; ++n) {
			if (ss.stones[n] != 0)
				continue;

			three2.fAdd[0,0] = ss.x + n*ss.ax;
			three2.fAdd[0,1] = ss.y + n*ss.ay;
			three2.fAdd[0,2] = 1;
		}

		// Two defending moves (s_2 and s_6, index 1 and 5)
		three2.fAdd[1,0] = ss.x + 1*ss.ax;
		three2.fAdd[1,1] = ss.y + 1*ss.ay;
		three2.fAdd[1,2] = -1;

		three2.fAdd[2,0] = ss.x + 5*ss.ax;
		three2.fAdd[2,1] = ss.y + 5*ss.ay;
		three2.fAdd[2,2] = -1;

		return (new GBOperatorThree2[] { three2 });
	}
}


// G_6: Straight Four

public class
GBOperatorStraightFour : GBOperator
{
	public override int Class {
		get {
			return (1);
		}
	}

	public override string Name {
		get {
			return ("Straight Four");
		}
	}

	public static bool Present (GoBangBoard.StoneSet ss)
	{
		// The pattern is .????. with "." position fixed.
		if (ss.stones[0] != 0 || ss.stones[5] != 0)
			return (false);

		// Now the middle three (index 2, 3, 4) have to be tested
		int countWhite = 0;
		int countHole = 0;

		CountStones (ss, out countWhite, out countHole);
		if (countWhite != 3 || countHole != 3)
			return (false);

		return (true);
	}

	public static GBOperatorStraightFour[] GetOperatorsIfValid
		(GoBangBoard.StoneSet ss)
	{
		if (Present (ss) == false)
			return (null);

		// Operator is valid, lets produce it (only one variant)
		GBOperatorStraightFour sfour = new GBOperatorStraightFour ();

		sfour.fAdd = new int[1,3];
		for (int n = 1 ; n < (ss.stones.Length - 1) ; ++n) {
			if (ss.stones[n] != 0)
				continue;

			sfour.fAdd[0,0] = ss.x + n*ss.ax;
			sfour.fAdd[0,1] = ss.y + n*ss.ay;
			sfour.fAdd[0,2] = 1;
		}

		return (new GBOperatorStraightFour[] { sfour });
	}
}

// G_6: Broken Three

public class
GBOperatorBrokenThree : GBOperator
{
	public override int Class {
		get {
			return (2);
		}
	}

	public override string Name {
		get {
			return ("Broken Three");
		}
	}

	public static bool Present (GoBangBoard.StoneSet ss)
	{
		// The pattern is .????. with "." position fixed.
		if (ss.stones[0] != 0 || ss.stones[5] != 0)
			return (false);

		// Now the middle three (index 2, 3, 4, 5) have to be tested, there
		// should only be two white stones.
		int countWhite = 0;
		int countHole = 0;

		CountStones (ss, out countWhite, out countHole);
		if (countWhite != 2 || countHole != 4)
			return (false);

		// Now, the broken three condition: "s_4 neither minimum nor maximum
		// in { s_2, s_3, s_4, s_5 }", where s_4 = s_5 = ., s_2 = s_3 = o.
		//
		// This is a bit ambiguous, because it does not say anything else
		// about the other values s_2, s_3, s_5.

		// Case 1: .o..o.
		if (ss.stones[2] == 0 && ss.stones[3] == 0)
			return (true);

		// Case 2: .o.o..
		if (ss.stones[2] == 0 && ss.stones[4] == 0)
			return (true);

		// Case 3: ..o.o.
		if (ss.stones[1] == 0 && ss.stones[3] == 0)
			return (true);

		// Case 4: .oo... and ...oo.
		if ((ss.stones[1] == 1 && ss.stones[2] == 1) ||
			(ss.stones[3] == 1 && ss.stones[4] == 1))
			return (true);

		return (false);
	}

	public static GBOperatorBrokenThree[] GetOperatorsIfValid
		(GoBangBoard.StoneSet ss)
	{
		if (Present (ss) == false)
			return (null);

		// Case 1
		if (ss.stones[2] == 0 && ss.stones[3] == 0) {
			// Operator is valid, lets produce it (two variants)
			GBOperatorBrokenThree bthree1 = new GBOperatorBrokenThree ();
			GBOperatorBrokenThree bthree2 = new GBOperatorBrokenThree ();

			bthree1.fAdd = new int[4,3];
			bthree2.fAdd = new int[4,3];

			int hole = 0;
			for (int n = 1 ; n < (ss.stones.Length-1) ; ++n) {
				if (ss.stones[n] != 0)
					continue;

				bthree1.fAdd[hole,0] = bthree2.fAdd[hole,0] = ss.x + n*ss.ax;
				bthree1.fAdd[hole,1] = bthree2.fAdd[hole,1] = ss.y + n*ss.ay;

				if (hole == 0) {
					bthree1.fAdd[0,2] = 1;
					bthree2.fAdd[0,2] = -1;

					hole += 1;
				} else {
					bthree1.fAdd[1,2] = -1;
					bthree2.fAdd[1,2] = 1;
				}
			}

			// Two defending moves (s_1 and s_6, index 0 and 5)
			bthree1.fAdd[2,0] = bthree2.fAdd[2,0] = ss.x;
			bthree1.fAdd[2,1] = bthree2.fAdd[2,1] = ss.y;
			bthree1.fAdd[2,2] = bthree2.fAdd[2,2] = -1;

			bthree1.fAdd[3,0] = bthree2.fAdd[3,0] = ss.x + 5*ss.ax;
			bthree1.fAdd[3,1] = bthree2.fAdd[3,1] = ss.y + 5*ss.ay;
			bthree1.fAdd[3,2] = bthree2.fAdd[3,2] = -1;

			return (new GBOperatorBrokenThree[] { bthree1, bthree2 });
		}

		// Case 4:
		// .oo...
		if (ss.stones[1] == 1 && ss.stones[2] == 1) {
			GBOperatorBrokenThree bthree = new GBOperatorBrokenThree ();

			bthree.fAdd = new int[4,3];

			bthree.fAdd[0,0] = ss.x + 0*ss.ax;
			bthree.fAdd[0,1] = ss.y + 0*ss.ay;
			bthree.fAdd[0,2] = -1;

			bthree.fAdd[1,0] = ss.x + 5*ss.ax;
			bthree.fAdd[1,1] = ss.y + 5*ss.ay;
			bthree.fAdd[1,2] = -1;

			bthree.fAdd[2,0] = ss.x + 3*ss.ax;
			bthree.fAdd[2,1] = ss.y + 3*ss.ay;
			bthree.fAdd[2,2] = -1;

			bthree.fAdd[3,0] = ss.x + 4*ss.ax;
			bthree.fAdd[3,1] = ss.y + 4*ss.ay;
			bthree.fAdd[3,2] = 1;

			return (new GBOperatorBrokenThree[] { bthree });
		} else if (ss.stones[3] == 1 && ss.stones[4] == 1) {
			// and ...oo.
			GBOperatorBrokenThree bthree = new GBOperatorBrokenThree ();

			bthree.fAdd = new int[4,3];

			bthree.fAdd[0,0] = ss.x + 0*ss.ax;
			bthree.fAdd[0,1] = ss.y + 0*ss.ay;
			bthree.fAdd[0,2] = -1;

			bthree.fAdd[1,0] = ss.x + 5*ss.ax;
			bthree.fAdd[1,1] = ss.y + 5*ss.ay;
			bthree.fAdd[1,2] = -1;

			bthree.fAdd[2,0] = ss.x + 2*ss.ax;
			bthree.fAdd[2,1] = ss.y + 2*ss.ay;
			bthree.fAdd[2,2] = -1;

			bthree.fAdd[3,0] = ss.x + 1*ss.ax;
			bthree.fAdd[3,1] = ss.y + 1*ss.ay;
			bthree.fAdd[3,2] = 1;

			return (new GBOperatorBrokenThree[] { bthree });
		}

		// Case 2 and 3
		//
		// Only one variation exist:
		// .o.o.. or ..o.o.
		// xoxoox    xooxox
		if ((ss.stones[1] == 1 && ss.stones[3] == 1) ||
			(ss.stones[2] == 1 && ss.stones[4] == 1))
		{
			GBOperatorBrokenThree bthree = new GBOperatorBrokenThree ();
			bthree.fAdd = new int[4,3];

			int posCount = 0;
			int pFillC = 0;
			for (int n = 1 ; n < (ss.stones.Length-1) ; ++n) {
				if (ss.stones[n] == 1) {
					posCount += 1;

					continue;
				}

				// Free stone
				// Case A: middle, fill with oponent
				if (posCount == 1) {
					bthree.fAdd[pFillC,0] = ss.x + n*ss.ax;
					bthree.fAdd[pFillC,1] = ss.y + n*ss.ay;
					bthree.fAdd[pFillC,2] = -1;

					pFillC += 1;
				} else {
					// Case B: not the middle, fill with our own
					bthree.fAdd[pFillC,0] = ss.x + n*ss.ax;
					bthree.fAdd[pFillC,1] = ss.y + n*ss.ay;
					bthree.fAdd[pFillC,2] = 1;

					pFillC += 1;
				}
			}

			// The two corner stones: x....x
			bthree.fAdd[2,0] = ss.x;
			bthree.fAdd[2,1] = ss.y;
			bthree.fAdd[2,2] = -1;

			bthree.fAdd[3,0] = ss.x + 5*ss.ax;
			bthree.fAdd[3,1] = ss.y + 5*ss.ay;
			bthree.fAdd[3,2] = -1;

			return (new GBOperatorBrokenThree[] { bthree, });
		}

		throw (new ApplicationException ("Broken three: impossible case"));
	}
}


// G_6: Three with 3 reply moves

public class
GBOperatorThree3 : GBOperator
{
	public override int Class {
		get {
			return (2);
		}
	}

	public override string Name {
		get {
			return ("Three (3 replies)");
		}
	}

	public static bool Present (GoBangBoard.StoneSet ss)
	{
		// The pattern is .????. with "." position fixed.
		if (ss.stones[0] != 0 || ss.stones[5] != 0)
			return (false);

		// Now the middle three (index 2, 3, 4, 5) have to be tested, there
		// should only be two white stones.
		int countWhite = 0;
		int countHole = 0;

		CountStones (ss, out countWhite, out countHole);
		if (countWhite != 2 || countHole != 4)
			return (false);

		// The "three with 3 reply moves" condition: "s_2 either minimum or
		// maximum in { s_2, s_3, s_4, s_5 }".
		//
		// This is a bit ambiguous, because it does not say anything else
		// about the other values s_3, s_4, s_5.
		//
		// This most likely means that there should not be two empty stones in
		// the middle: .o..o., but it must be either .oo..., ...oo., .o.o..,
		// ..o.o. or ..oo..
		if (ss.stones[2] == 0 && ss.stones[3] == 0)
			return (false);

		return (true);
	}

	public static GBOperatorThree3[] GetOperatorsIfValid
		(GoBangBoard.StoneSet ss)
	{
		if (Present (ss) == false)
			return (null);

		// Case A: ..oo..
		if (ss.stones[1] == 0 && ss.stones[4] == 0) {
			// Operator is valid, lets produce it (two variants)
			GBOperatorThree3 three31 = new GBOperatorThree3 ();
			GBOperatorThree3 three32 = new GBOperatorThree3 ();

			three31.fAdd = new int[4,3];
			three32.fAdd = new int[4,3];

			int hole = 0;
			for (int n = 1 ; n < (ss.stones.Length-1) ; ++n) {
				if (ss.stones[n] != 0)
					continue;

				three31.fAdd[hole,0] = three32.fAdd[hole,0] = ss.x + n*ss.ax;
				three31.fAdd[hole,1] = three32.fAdd[hole,1] = ss.y + n*ss.ay;

				if (hole == 0) {
					three31.fAdd[0,2] = 1;
					three32.fAdd[0,2] = -1;

					hole += 1;
				} else {
					three31.fAdd[1,2] = -1;
					three32.fAdd[1,2] = 1;
				}
			}

			// Two defending moves (s_1 and s_6, index 0 and 5)
			three31.fAdd[2,0] = three32.fAdd[2,0] = ss.x;
			three31.fAdd[2,1] = three32.fAdd[2,1] = ss.y;
			three31.fAdd[2,2] = three32.fAdd[2,2] = -1;

			three31.fAdd[3,0] = three32.fAdd[3,0] = ss.x + 5*ss.ax;
			three31.fAdd[3,1] = three32.fAdd[3,1] = ss.y + 5*ss.ay;
			three31.fAdd[3,2] = three32.fAdd[3,2] = -1;

			return (new GBOperatorThree3[] { three31, three32 });
		} else if ((ss.stones[2] == 0 && ss.stones[4] == 0) ||
			(ss.stones[1] == 0 && ss.stones[3] == 0))
		{
			// Case B & C: .o.o.. and ..o.o.
			// TODO
			GBOperatorThree3 three3 = new GBOperatorThree3 ();

			three3.fAdd = new int[4,3];

			int stoneCount = 0;
			int pCell = 0;
			for (int n = 1 ; n < (ss.stones.Length-1) ; ++n) {
				if (ss.stones[n] != 0) {
					stoneCount += 1;

					continue;
				}

				three3.fAdd[pCell,0] = ss.x + n*ss.ax;
				three3.fAdd[pCell,1] = ss.y + n*ss.ay;

				if (stoneCount == 1) {
					three3.fAdd[pCell,2] = 1;
				} else
					three3.fAdd[pCell,2] = -1;

				pCell += 1;
			}

			// Two defending moves (s_1 and s_6, index 0 and 5)
			three3.fAdd[2,0] = ss.x;
			three3.fAdd[2,1] = ss.y;
			three3.fAdd[2,2] = -1;

			three3.fAdd[3,0] = ss.x + 5*ss.ax;
			three3.fAdd[3,1] = ss.y + 5*ss.ay;
			three3.fAdd[3,2] = -1;

			return (new GBOperatorThree3[] { three3, });
		} else if (ss.stones[3] == 0 && ss.stones[4] == 0) {
			// Case D: .oo...
			GBOperatorThree3 three3 = new GBOperatorThree3 ();

			three3.fAdd = new int[4,3];

			three3.fAdd[0,0] = ss.x + 3*ss.ax;
			three3.fAdd[0,1] = ss.y + 3*ss.ay;
			three3.fAdd[0,2] = 1;

			three3.fAdd[1,0] = ss.x + 4*ss.ax;
			three3.fAdd[1,1] = ss.y + 4*ss.ay;
			three3.fAdd[1,2] = -1;

			// Two defending moves (s_1 and s_6, index 0 and 5)
			three3.fAdd[2,0] = ss.x;
			three3.fAdd[2,1] = ss.y;
			three3.fAdd[2,2] = -1;

			three3.fAdd[3,0] = ss.x + 5*ss.ax;
			three3.fAdd[3,1] = ss.y + 5*ss.ay;
			three3.fAdd[3,2] = -1;

			return (new GBOperatorThree3[] { three3, });
		} else if (ss.stones[1] == 0 && ss.stones[2] == 0) {
			// Case E: ...oo.
			GBOperatorThree3 three3 = new GBOperatorThree3 ();

			three3.fAdd = new int[4,3];

			three3.fAdd[0,0] = ss.x + 2*ss.ax;
			three3.fAdd[0,1] = ss.y + 2*ss.ay;
			three3.fAdd[0,2] = 1;

			three3.fAdd[1,0] = ss.x + 1*ss.ax;
			three3.fAdd[1,1] = ss.y + 1*ss.ay;
			three3.fAdd[1,2] = -1;

			// Two defending moves (s_1 and s_6, index 0 and 5)
			three3.fAdd[2,0] = ss.x;
			three3.fAdd[2,1] = ss.y;
			three3.fAdd[2,2] = -1;

			three3.fAdd[3,0] = ss.x + 5*ss.ax;
			three3.fAdd[3,1] = ss.y + 5*ss.ay;
			three3.fAdd[3,2] = -1;

			return (new GBOperatorThree3[] { three3, });
		} else {
			throw (new ApplicationException ("BUG in Three3, uncatched case"));
		}
	}
}

