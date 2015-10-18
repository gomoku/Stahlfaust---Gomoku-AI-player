using System;
using System.Collections;

public class InterestingFieldAgent : ICloneable
{
	bool[,] interestingfields;
	int size;

	/** Contains the threats we built.
	 * ArrayList<Treat>
	 */
	public ThreatList ownthreatlist;

	/** Contains the threats the opponent built.
	 * ThreatList<Threat>
	 */
	public ThreatList oppthreatlist;

	/** Contains the removed threats removed from our list.
	 * ThreatList<Threat>
	 */
	public ThreatList ownremovedthreatlist;
	
	/** Contains the removed threats removed from opponent's list.
	 * ThreatList<Threat>
	 */
	public ThreatList oppremovedthreatlist;

	/** Contains the added threats added from our list.
	 * ThreatList<Threat>
	 */
	public ThreatList ownaddedthreatlist;
	
	/** Contains the added threats added from opponent's list..
	 * ThreatList<Threat>
	 */
	public ThreatList oppaddedthreatlist;

	ThreatSearcher searcher;
	
	private InterestingFieldAgent()
	{
	}

	public InterestingFieldAgent(ThreatSearcher searcher, int size)
	{
		interestingfields = new bool[size, size];
		this.size = size;
		ownthreatlist = new ThreatList();
		oppthreatlist = new ThreatList();
		ownremovedthreatlist = new ThreatList();
		oppremovedthreatlist = new ThreatList();
		ownaddedthreatlist = new ThreatList();
		oppaddedthreatlist = new ThreatList();
		this.searcher = searcher;
	}
	
	/** Generates a list of interesting moves from the matrix.
	 *
	 * @returns ArrayList<Coordinate>
	 */
	public ArrayList InterestingFields()
	{
		ArrayList res = new ArrayList();

		for (int y = 0; y < interestingfields.GetLength(0); ++y) {
			for (int x = 0; x < interestingfields.GetLength(1); ++x) {
				if (interestingfields[x, y]) res.Add(new Coordinate(x, y));
			}
		}
		return res;
	}
	
	public void PrintArray()
	{
		for (int y = 0; y < interestingfields.GetLength(0); ++y) {
			for (int x = 0; x < interestingfields.GetLength(1); ++x) {
				Console.Write("{0} ", interestingfields[x, y]?"X":".");
			}
			Console.WriteLine();
		}
	}

	/** Updates the internal matrix of interestingfields with a new move.
	 *
	 * @param board The actual board, needed to make no occupied fields interesting.
	 */
	public void UpdateInterestingFieldArray(int[,] board, Coordinate field)
	{
		/*
		bool[,] opMat = new bool[,]{{true , false, false, false, true , false, false, false, true },
									{false, true , false, false, true , false, false, true , false},
									{false, false, true , false, true , false, true , false, false},
									{false, false, false, true , true , true , false, false, false},
									{true , true , true , true , true , true , true , true , true },
									{false, false, false, true , true , true , false, false, false},
									{false, false, true , false, true , false, true , false, false},
									{false, true , false, false, true , false, false, true , false},
									{true , false, false, false, true , false, false, false, true }
								   };
								   */
		bool[,] opMat = new bool[,]{
									{true , false, true , false, true },
									{false, true , true , true , false},
									{true , true , true , true , true },
									{false, true , true , true , false},
									{true , false, true , false, true }
								   };
		int opMatdelta = opMat.GetLength(0) / 2;


		int x = field.X;
		int y = field.Y;
		for (int j = 0; j < opMat.GetLength(0); ++j) 
		{
			for (int k = 0; k < opMat.GetLength(1); ++k) 
			{
				if (x + (j - opMatdelta) < board.GetLength(0) && x + (j - opMatdelta) >= 0 && 
					y + (k - opMatdelta) < board.GetLength(0) && y + (k - opMatdelta) >= 0) 
				{
					interestingfields[x + (j - opMatdelta), y +(k - opMatdelta)] = 
						((interestingfields[x + (j - opMatdelta), y +(k - opMatdelta)] || opMat[j, k]) && 
						board[x + (j - opMatdelta), y +(k - opMatdelta)] == 0);
				}
			}
		}

	}

	/** Add threats from from into into if they are not contained yet.
	 *
	 * @returns ArrayList<Threat> List of the added threats.
	 */
	private ThreatList Merge(ThreatList into, ThreatList from)
	{
		ThreatList res = new ThreatList ();
		foreach (Threat t in from)
		{
			// check for every t if it is already in into-list
			bool alreadyin = false;
			foreach (Threat ct in into)
			{
				alreadyin = true;
				if (ct.category == t.category && ct.fields.Count == t.fields.Count)
				{
					for (int i = 0; i < ct.fields.Count; ++i) 
					{
						alreadyin = alreadyin && 
							( ( ((Coordinate)(ct.fields[i])).X == ((Coordinate)(t.fields[i])).X ) &&
							  ( ((Coordinate)(ct.fields[i])).Y == ((Coordinate)(t.fields[i])).Y ) );
					}
				}
				else alreadyin = false;
			}
			if (alreadyin == false) {
				into.Add(t);
				res.Add(t);
			}
		}
		return res;
	}

	public void UpdateThreatLists(int[,] board, Coordinate move, int attacker)
	{
		// 0. Initialized the removelists
		ownremovedthreatlist = new ThreatList();
		oppremovedthreatlist = new ThreatList();
		ownaddedthreatlist = new ThreatList();
		oppaddedthreatlist = new ThreatList();

		// 1. Remove blocked threats from both lists.
		ThreatList lookup = new ThreatList();
		foreach (Threat t in oppthreatlist) {
			foreach (Coordinate c in t.fields) {
				if (c.X == move.X && c.Y == move.Y) {
					oppremovedthreatlist.Add(t);
					lookup.Add(t.cause);
				}
			}
		}
		foreach (Threat t in oppremovedthreatlist) oppthreatlist.Remove(t);
		foreach (Threat t in ownthreatlist) {
			foreach (Coordinate c in t.fields) {
				if (c.X == move.X && c.Y == move.Y) {
					ownremovedthreatlist.Add(t);
					lookup.Add(t.cause);
				}
			}
		}
		foreach (Threat t in ownremovedthreatlist) ownthreatlist.Remove(t);

		//Lookup if "causes" still cause threats
		foreach (Coordinate c in lookup)
		{
			if (board[c.X, c.Y] == 1)
			{
				ThreatList curthreats = searcher.investigate(board, c, 1);

				ownaddedthreatlist = Merge(ownthreatlist, curthreats);

			}
			if (board[c.X, c.Y] == -1)
			{
				ThreatList curthreats = searcher.investigate(board, c, -1);

				oppaddedthreatlist = Merge(oppthreatlist, curthreats);
			}
		}
		
		if (attacker == 1)
		{
			// We are attacking
			// 2. Add our threats we build to our list

			ThreatList curthreats = searcher.investigate(board, move, attacker, true);

			Merge(ownaddedthreatlist, Merge(ownthreatlist, curthreats));

		}
		else
		{
			// The opponent is attacking
			// 2. Add his threats to his list
			ThreatList curthreats = searcher.investigate(board, move, attacker, true);

			Merge(oppaddedthreatlist, Merge(oppthreatlist, curthreats));
		}

	}

	/** Generates a list of "really" interesting moves, using the following 
	 * method:
	 *
	 * 1. Read all the interestingfields from the array.
	 * 2. Look if we are forced to move, if yes, add them to "really" interesting 
	 * moves.
	 * 2a. If we are not forced, add all the interesting fields to the really 
	 * interesting moves.
	 * 3. If we didn't just add all the moves look if we can force to move, if 
	 * category is less than lowest category in (2), add them to "really" 
	 * interesting moves.
	 *
	 */
	public ArrayList ReallyInterestingFields(int[,] board, int attacker)
	{
		int minoppcat = 3;
		ArrayList allfields = InterestingFields();
		ArrayList fields = new ArrayList();

		// Guaranteed to be set below.
		ArrayList opponentlist = null;
		ArrayList ownlist = null;

		if (attacker == 1) {
			opponentlist = oppthreatlist;
			ownlist = ownthreatlist;
		}
		else 
		{
			opponentlist = ownthreatlist;
			ownlist = oppthreatlist;
		}
		// Step 2
		bool fieldadded = false;
		foreach(Coordinate field in allfields)
		{
			fieldadded = false;
			foreach (Threat t in opponentlist) {
				if (t.create != false)
					continue;

				if (t.fields.Contains (field)) {
					if (fieldadded == false) {
						fields.Add(field);
						fieldadded = true;
					}

					if (t.category < minoppcat)
						minoppcat = t.category;
				}
			}
		}

		// Step 2a
		if (fields.Count == 0) {
			fields = allfields;
		} else {
			// Step 3
			foreach (Coordinate field in allfields) {
				foreach (Threat t in ownlist) {
					if (t.category > minoppcat)
						continue;

					if (t.fields.Contains (field)) {
						fields.Add(field);

						break;
					}
				}
			}
			/*
			foreach(Coordinate field in allfields)
			{
				ArrayList forced = searcher.investigate(board, field, attacker, null, false, minoppcat-1);
				if (forced != null && forced.Count > 0) {
					//Console.WriteLine("We force Enemy: {0}", field);
					foreach (Threat t in forced)
					{
						fields.Add(field);
					}
				}
			}
			*/
		}
		return fields;
	}

	public object Clone ()
	{
		InterestingFieldAgent ic = new InterestingFieldAgent ();

		ic.searcher = searcher;
		ic.interestingfields = (bool[,]) interestingfields.Clone ();

		ic.ownthreatlist = (ThreatList) ownthreatlist.Clone ();
		ic.oppthreatlist = (ThreatList) oppthreatlist.Clone ();

		ic.ownremovedthreatlist = (ThreatList) ownremovedthreatlist.Clone ();
		ic.oppremovedthreatlist = (ThreatList) oppremovedthreatlist.Clone ();

		ic.ownaddedthreatlist = (ThreatList) ownaddedthreatlist.Clone ();
		ic.oppaddedthreatlist = (ThreatList) oppaddedthreatlist.Clone ();

		return (ic);
	}

	public void PrintThreats()
	{
		Console.WriteLine("Current Threats: ");
		Console.WriteLine("Own Threats :");
		foreach (Threat t in ownthreatlist) {
			Console.WriteLine("Category : {0}\t CreateFlag: {1}", t.category, t.create);
			foreach (Coordinate c in t.fields) 
			{
				Console.WriteLine("Coordinate : {0}", c);
			}
		}
		Console.WriteLine("Opp Threats :");
		foreach (Threat t in oppthreatlist) {
			Console.WriteLine("Category : {0}\t CreateFlag: {1}", t.category, t.create);
			foreach (Coordinate c in t.fields) 
			{
				Console.WriteLine("Coordinate : {0}", c);
			}
		}
	}
}


