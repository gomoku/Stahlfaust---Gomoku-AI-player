
using System;
using System.IO;
using System.Text;
using System.Collections;


namespace DBSearchAlgorithm
{
	/** The central search module interface every db-search module has to
	 * implement.
	 */
	public interface DBSearchModule {
		/** Return new operators depending on the key operator to this node.
		 *
		 * Basically this means, we applied an operator f_1 leading to this node.
		 * Now we check if there is a new operator f_2 valid on this node which
		 * only works because we applied f_1.  Precisely, this means:
		 *
		 * f_1^{add} \cap f_2^{pre} \neq \emptyset.
		 *
		 * Where f_2 is the new operator.  There may be many operators possible,
		 * and we have to return all of them.
		 *
		 * @param node The node dependent operators should be created for.
		 *
		 * @returns The array of operators that are both valid (formally correct)
		 * and dependent on the node (dependent on the node's last operator).
		 */
		IDBOperator[] LegalOperators (DBNode node);

		/** Check the operator for validity on the given node.
		 *
		 * @param oper The operator to be checked.
		 * @param node The node the operator should be tested on.
		 *
		 * @returns True in case the operator is valid, false otherwise.
		 */
		bool Applicable (IDBOperator oper, DBNode node);

		/** Check node1 and node2 are independent paths, that is, they do not
		 * conflict.
		 *
		 * As we call it iteratively, we only need to check the last operator.
		 * So "Do not conflict" means: we can apply the last operator of node2
		 * to node1.
		 *
		 * @param partner The node the operator will be tested on.
		 * @param node The node that provides the last operator.
		 *
		 * @returns True, if the nodes can be combined, false otherwise.
		 */
		bool NotInConflict (DBNode partner, DBNode node);

		/** Combine two nodes if there is a new dependent operator in the
		 * combination node.
		 *
		 * Precisely that is, the combined node has operators that are neither
		 * available for node1 or node2.  Iff this is the case, then the new
		 * combined spacestate should be provided.
		 *
		 * @param node1 The first node that is to be combined.
		 * @param node1Path Path up to and excluding node1.
		 * @param node2 The second node that is to be combined.
		 * @param node2Path Path up to and excluding node2.
		 *
		 * @returns In case the nodes cannot be combined under the above
		 * condition, null is returned.  Otherwise the new, combined space
		 * state is returned.
		 */
		IDBSpaceState CombineIfResultIsNewOperators (DBNode node1, Stack node1Path,
			DBNode node2, Stack node2Path);

		void CombinationStage (int level, DBNode root, DBSearch dbs);

		/** A convenience function that can be implemented for internal use or
		 * caching purposes.  It is called whenever a new node is created in
		 * the db-graph.  It is called _after_ the node has been added to the
		 * graph.
		 *
		 * @param node The new created node.
		 * @param root The root node of the search graph.
		 */
		void RegisterNewNode (DBNode node, DBNode root);

		void DoExpirationCheck ();

		/** Predicate: is the given state a goal state.
		 *
		 * @param state The space state to be checked.
		 *
		 * @returns True in case the state is a goal state, false otherwise.
		 */
		bool IsGoal (IDBSpaceState state);

		/** After each stage (or search level?) we check if the number of
		 * goals are equal to or exceed this threshold.  The recommended value
		 * is 10.  Zero disables any threshold and the search will only stop
		 * after either resources or the search space is exhausted.
		 */
		int GoalCountThresh {
			get;
		}

		/** A global "one goal is enough" flag for the entire search.  If this
		 * is set and the first goal is found, the search is terminated
		 * instantly.
		 */
		bool OneGoalStopsSearch {
			get;
		}
	}

	/** The operator that is applied to transfer from state to state in the
	 * statespace.
	 */
	public interface
	IDBOperator
	{
		/** Apply the operator to the given state, returning a new state.
		 *
		 * @param state The source state.
		 *
		 * @returns The newly created destination state.
		 */
		IDBSpaceState Apply (DBSearchModule mod, IDBSpaceState state);
	}

	/** A single state in the state space.
	 */
	public interface
	IDBSpaceState
	{
		/** Recalculate the is-goal value of this state.
		 */
		bool UpdateIsGoal (DBSearchModule dbS);

		bool IsGoal {
			get;
		}

		/** A short textual description of the state, for debug purposes.
		 */
		string DescShort {
			get;
		}
	}


	public class
	DBNode
	{
		// FIXME
		public bool dumped = false;
		public int subLevel = 0;
		private static int DebugNodeNumber = 1;
		public int DebugNN;

		public enum NodeType {
			Root,
			Dependency,
			Combination,
		};

		NodeType type;
		public NodeType Type {
			get {
				return (type);
			}
		}

		private bool isGoal = false;
		public bool IsGoal {
			get {
				return (isGoal);
			}
			set {
				isGoal = value;
			}
		}

		private bool dependencyExpanded = false;
		public bool DependencyExpanded {
			get {
				return (dependencyExpanded);
			}
			set {
				dependencyExpanded = value;
			}
		}

		private bool isRefutePathRoot = false;
		public bool IsRefutePathRoot {
			get {
				return (isRefutePathRoot);
			}
			set {
				isRefutePathRoot = value;
			}
		}

		int level;
		public int Level {
			get {
				return (level);
			}
		}

		ArrayList combinedChildren = null;
		public ArrayList CombinedChildren {
			get {
				return (combinedChildren);
			}
		}

		ArrayList children = new ArrayList ();
		public ArrayList Children {
			get {
				return (children);
			}
		}
		/*
		DBNode child = null;
		public DBNode Child {
			get {
				return (child);
			}
			set {
				child = value;
			}
		}
		DBNode sibling = null;
		public DBNode Sibling {
			get {
				return (sibling);
			}
			set {
				sibling = value;
			}
		}
		*/

		// State represented by the node.
		IDBSpaceState state;
		public IDBSpaceState State {
			get {
				return (state);
			}
		}

		public DBNode (NodeType type, IDBSpaceState state, int level)
		{
			// FIXME
			this.DebugNN = DebugNodeNumber;
			DebugNodeNumber += 1;

			this.type = type;
			if (type == NodeType.Dependency)
				combinedChildren = new ArrayList ();

			this.state = state;
			this.isGoal = state.IsGoal;
			this.level = level;
		}

		// TODO:
		// if tracing back this nodes anypath to the root node, will we cross
		// 'node' ?
		public bool BacktraceLeadsOverNode (DBNode node)
		{
			// TODO: this is inefficient, as we check almost the whole graph
			// By reversing it (and storing backpointers in each node, we
			// could make this faster).
			if (this == node)
				return (true);

			if (node.Level >= Level)
				return (false);

			foreach (DBNode child in node.Children) {
				if (BacktraceLeadsOverNode (child))
					return (true);
			}

			return (false);
		}

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder ();

			sb.AppendFormat ("{0} level {1}, type {2} node: \"{3}\"",
				DebugNN, level, type, state);
			sb.Append (" children: ");
			foreach (DBNode child in Children)
				sb.AppendFormat ("{0}, ", child.DebugNN);

			return (sb.ToString ());
		}
	}


	public class
	DBSearch
	{
		DBNode root;
		public DBNode Root {
			get {
				return (root);
			}
		}

		private bool TreeSizeIncreased;
		int level;
		int goalCount = 0;

		/** The total number of goals found.
		 */
		public int GoalCount {
			get {
				return (goalCount);
			}
		}

		private DBSearch ()
		{
		}

		bool breadthFirst;
		public DBSearch (DBSearchModule module, bool breadthFirst)
		{
			this.module = module;
			this.breadthFirst = breadthFirst;
		}

		private DBSearchModule module;

		private int nodeCount = 0;
		private void DumpTree (DBNode node)
		{
			if (node == null)
				return;

			if (node.dumped)
				return;
			node.dumped = true;

			for (int n = 0 ; n < node.Level ; ++n)
				Console.Write ("    ");

			Console.WriteLine (node);

			foreach (DBNode child in node.Children)
				DumpTree (child);
		}

		public void DumpDOTGoalsOnly ()
		{
			// Build a list of all nodes lying on pathes leading to goal nodes
			// Element type: DBNode
			ArrayList goalPathNodes = GoalPathNodes ();
			Console.WriteLine ("goalPathNodes.Count = {0}",
				goalPathNodes.Count);
			Console.WriteLine ("Contains (root) = {0}", goalPathNodes.Contains (root));
			DumpDOT (goalPathNodes);
		}

		public void DumpDOT ()
		{
			Console.WriteLine ("DumpDOT ()");
			DumpDOT (null);
		}

		public void DumpDOT (ArrayList nodesFilter)
		{
			StreamWriter wr = new StreamWriter ("output.dot");
			wr.WriteLine ("digraph dbsearch {");
			//wr.WriteLine ("    graph [rankdir=TB, ranksep=\"2.5\"];");
			wr.WriteLine ("    graph [rankdir=TB];");

			wr.WriteLine ("    { node [shape=plaintext];");
			wr.Write ("        ");
			for (int levelPrint = 0 ; levelPrint < level ; ++levelPrint) {
				if (levelPrint == 0) {
					wr.Write ("root");
				} else {
					wr.Write (" -> dep{0} -> comb{0}", levelPrint, levelPrint);
				}
			}
			wr.WriteLine (";");
			wr.WriteLine ("}");

			DumpDOT (wr, root, nodesFilter);

			wr.WriteLine ("}");
			wr.Close ();
		}

		private void DumpDOT (TextWriter dotFile, DBNode node,
			ArrayList nodesFilter)
		{
			if (node == null)
				return;

			if (nodesFilter != null && nodesFilter.Contains (node) == false)
				return;

			if (node.dumped)
				return;
			node.dumped = true;

			for (int n = 0 ; n < node.Level ; ++n)
				dotFile.Write ("    ");

			// Order nodes on level
			string rankLevel;
			if (node.Level == 0) {
				rankLevel = "root";
			} else if (node.Type == DBNode.NodeType.Dependency) {
				rankLevel = String.Format ("dep{0}", node.Level);
			} else if (node.Type == DBNode.NodeType.Combination) {
				rankLevel = String.Format ("comb{0}", node.Level);
			} else {
				rankLevel = "invalid";
			}
			if (node.subLevel == 0)
				dotFile.WriteLine ("{{ rank=same; {0};", rankLevel);

			dotFile.WriteLine ("{0} [shape=box label = <{1}>];",
				node.DebugNN, node.State.DescShort);
			if (node.subLevel == 0)
				dotFile.WriteLine ("}");
			foreach (DBNode child in node.Children) {
				if (nodesFilter != null && nodesFilter.Contains (child) == false)
					continue;

				for (int n = 0 ; n < node.Level ; ++n)
					dotFile.Write ("    ");

				dotFile.WriteLine ("{0} -> {1};", node.DebugNN, child.DebugNN);
			}

			// Also combined children
			if (node.CombinedChildren != null) {
				foreach (DBNode child in node.CombinedChildren) {
					if (nodesFilter != null && nodesFilter.Contains (child) == false)
						continue;
					for (int n = 0 ; n < node.Level ; ++n)
						dotFile.Write ("    ");

					dotFile.WriteLine ("{0} -> {1} [color=\"#b0b0b0\"] ;", node.DebugNN, child.DebugNN);
				}
			}

			foreach (DBNode child in node.Children)
				DumpDOT (dotFile, child, nodesFilter);
		}

		public ArrayList GoalPathNodes ()
		{
			ArrayList goalpath = new ArrayList ();

			GoalPathNodesI (goalpath, root, new Stack ());

			return (goalpath);
		}

		private void GoalPathNodesI (ArrayList goalpath, DBNode node, Stack path)
		{
			if (node == null)
				return;

			if (module.IsGoal (node.State) && goalpath.Contains (node) == false) {
				/*
				Console.WriteLine ("goals:");
				foreach (IDBSpaceState state in goals)
					Console.WriteLine ("    {0}", state);

				Console.WriteLine ("    add: {0}", node.State);
				*/
				goalpath.Add (node);

				foreach (DBNode pNode in path) {
					if (goalpath.Contains (pNode) == false)
						goalpath.Add (pNode);
				}
			}

			foreach (DBNode child in node.Children) {
				path.Push (node);
				GoalPathNodesI (goalpath, child, path);
				path.Pop ();
			}
		}

		// Return the list of goal states reached from the root.
		public ArrayList GoalStates ()
		{
			ArrayList goals = new ArrayList ();

			GoalStatesI (goals, root);

			return (goals);
		}

		private void GoalStatesI (ArrayList goals, DBNode node)
		{
			if (node == null)
				return;

			if (module.IsGoal (node.State) && goals.Contains (node.State) == false) {
				/*
				Console.WriteLine ("goals:");
				foreach (IDBSpaceState state in goals)
					Console.WriteLine ("    {0}", state);

				Console.WriteLine ("    add: {0}", node.State);
				*/
				goals.Add (node.State);
			}

			foreach (DBNode child in node.Children)
				GoalStatesI (goals, child);
		}

		public void Search (IDBSpaceState rootState)
		{
			rootState.UpdateIsGoal (module);
			root = new DBNode (DBNode.NodeType.Root, rootState, 0);
			if (root.IsGoal)
				goalCount += 1;

			module.RegisterNewNode (root, root);

			level = 1;

			TreeSizeIncreased = true;
			try {
				while (TreeSizeIncreased) {
					//Console.WriteLine ("level {0}", level);

					TreeSizeIncreased = false;

					if (breadthFirst)
						dependencyNodeQueue.Clear ();
					AddDependencyStage (root);

					if (breadthFirst)
						SolveDependencyStage ();
					//DumpTree (root);
					/*Console.WriteLine ("level {0} dependency finished: {1} nodes, {2} goals",
						level, nodeCount, goalCount);*/

					if (TreeSizeIncreased == false)
						break;
					if (module.GoalCountThresh != 0 && goalCount >= module.GoalCountThresh)
						break;

					/*
					Console.WriteLine ("COMB TEST");
					module.CombinationStage (level, root, this);

					if (module.GoalCountThresh != 0 && goalCount >= module.GoalCountThresh)
						break;
						*/

					AddCombinationStage (root, new Stack ());
					/*Console.WriteLine ("level {0} combination finished: {1} nodes, {2} goals",
						level, nodeCount, goalCount);*/
					if (TreeSizeIncreased == false)
						break;
					if (module.GoalCountThresh != 0 && goalCount >= module.GoalCountThresh)
						break;
					//DumpTree (root);

					level += 1;
				}
			} catch (GoalCountExceededException) {
				/*
				Console.WriteLine ("GOAL count limit of {0} exceeded.",
					module.GoalCountThresh);*/
			}
		}

		// A
		private Queue dependencyNodeQueue = new Queue ();

		private void AddDependencyStage (DBNode node)
		{
			//Console.WriteLine ("AddDependencyStage");
			if (node == null)
				return;

			if (level == (node.Level + 1) &&
				(node.Type == DBNode.NodeType.Root ||
					node.Type == DBNode.NodeType.Combination))
			{
				// Do not expand goal nodes any further or nodes that have
				// already been expanded.
				if (node.IsGoal == false && node.DependencyExpanded == false) {
					if (breadthFirst) {
						dependencyNodeQueue.Enqueue (new DepQElem (node, 0));

						node.DependencyExpanded = true;
					} else
						AddDependentChildren (node, 0);
				}

				// FIXME: check if this is ok, seems to be and makes sense
				return;
			}

			foreach (DBNode child in node.Children)
				AddDependencyStage (child);
		}

		/** Solve the dependency queue by repeatly solving the first element
		 * until there are no elements left.
		 *
		 * This implements an efficient Breadth-First tree walk for the
		 * dependency stage and biases the threat sequences found towards the
		 * shortest ones.  However, it adds a little overhead, but the
		 * advantage of balancing out the search space exploration by far
		 * outweights this overhead.
		 */
		private void SolveDependencyStage ()
		{
			// As long as the dependency queue still has elements, do a
			// breadth-first traversal of the graph by treating the elements
			// as queue.
			while (dependencyNodeQueue.Count > 0) {
				DepQElem head = (DepQElem) dependencyNodeQueue.Dequeue ();
				/*Console.WriteLine ("head on level {0}: {1}",
					head.subLevel, head.node);*/

				AddDependentChildren (head.node, head.subLevel);
			}
		}

		internal class DepQElem {
			internal readonly DBNode node;
			internal readonly int subLevel;

			internal DepQElem (DBNode node, int subLevel) {
				this.node = node;
				this.subLevel = subLevel;
			}
		}

		private void AddDependentChildren (DBNode node, int subLevel)
		{
			// Console.WriteLine ("    AddDependentChildren");
			// TODO
			IDBOperator[] opers = module.LegalOperators (node);
			if (opers == null)
				return;
			//Console.WriteLine ("      opers.Count = {0}", opers.Length);

			foreach (IDBOperator op in opers) {
				if (module.Applicable (op, node)) {
					DBNode newChild = LinkNewChildToGraph (node, op);
					newChild.subLevel = subLevel;

					if (newChild.IsGoal == false) {
						if (breadthFirst) {
							dependencyNodeQueue.Enqueue
								(new DepQElem (newChild, subLevel + 1));
						} else {
							AddDependentChildren (newChild, subLevel + 1);
						}
					} else if (module.OneGoalStopsSearch ||
						(module.GoalCountThresh != 0 &&
							goalCount >= module.GoalCountThresh))
					{
						throw (new GoalCountExceededException ());
					}
				}
			}
		}

		private class
		GoalCountExceededException : ApplicationException
		{
		}


		// Returns new child
		private DBNode LinkNewChildToGraph (DBNode node, IDBOperator op)
		{
			IDBSpaceState newState = op.Apply (module, node.State);
			newState.UpdateIsGoal (module);

			DBNode newNode = new DBNode (DBNode.NodeType.Dependency,
				newState, level);
			if (newNode.IsGoal)
				goalCount += 1;

			TreeSizeIncreased = true;
			nodeCount += 1;

			// Graph bookkeeping
			node.Children.Add (newNode);

			module.RegisterNewNode (newNode, root);

			return (newNode);
		}

		// TODO
		private void AddCombinationStage (DBNode node, Stack path)
		{
			//Console.WriteLine ("AddCombinationStage");
			if (node == null || node.Level > level)
				return;

			if (node.Type == DBNode.NodeType.Dependency &&
				node.Level == level &&
				node.IsGoal == false)
			{
				/*for (int l = 0 ; l < node.Level ; ++l)
					Console.Write ("    ");
				Console.WriteLine ("({0}) down", node.DebugNN);*/
				module.DoExpirationCheck ();
				FindAllCombinationNodes (node, path, root, new Stack ());
			}

			path.Push (node);
			foreach (DBNode child in node.Children)
				AddCombinationStage (child, path);

			path.Pop ();
		}


		// partnerPath: path up to and excluding partner
		// nodePath: path up to and excluding node
		private void FindAllCombinationNodes (DBNode partner, Stack partnerPath,
			DBNode node, Stack nodePath)
		{
			//Console.WriteLine ("FindAllCombinationNodes");
			if (node == null || node == partner)
				return;

			if (node.IsGoal)
				return;

			if (node.Type == DBNode.NodeType.Root ||
				module.NotInConflict (partner, node))
			{
				// HaveCommonChild checks if we already combined in the
				// reverse direction, but we also need to check whether their
				// path's are independant.  That is, whether tracing node's
				// path back to root we will cross over partner.  Also whether
				// tracing partner's path back will lead us over node.
				if (node.Type == DBNode.NodeType.Dependency &&
					HaveCommonCombinedChild (partner, node) == false)
				{
					IDBSpaceState combination =
						module.CombineIfResultIsNewOperators (partner, partnerPath,
							node, nodePath);

					if (combination != null) {
						DBNode combNode = new DBNode (DBNode.NodeType.Combination,
							combination, level);
						combNode.IsGoal = module.IsGoal (combination);
						if (combNode.IsGoal)
							goalCount += 1;

						AddCombinationNode (partner, combNode);

						// Also make the new node a child ("combined child") of
						// node.  We do not add it it to the Children array
						// for two reasons:
						//   1. We want a strict tree structure in memory, no
						//      DAG
						//   2. The only point where this DAG-like
						//      relationship is used is in this method, when
						//      its checked we did not already combine them.
						node.CombinedChildren.Add (combNode);

						// Notify the implementation of a new node.
						module.RegisterNewNode (combNode, root);

						if (combNode.IsGoal) {
							if (module.OneGoalStopsSearch ||
								(module.GoalCountThresh != 0 &&
									goalCount >= module.GoalCountThresh))
							{
								throw (new GoalCountExceededException ());
							}
						}
					}
				}

				nodePath.Push (node);
				foreach (DBNode child in node.Children)
					FindAllCombinationNodes (partner, partnerPath, child, nodePath);

				nodePath.Pop ();
			}
		}

		private int DEBUGParentNN (DBNode node, DBNode search)
		{
			if (node.Children.IndexOf (search) >= 0)
				return (node.DebugNN);

			foreach (DBNode child in node.Children) {
				int cnn = DEBUGParentNN (child, search);
				if (cnn != -1)
					return (cnn);
			}

			return (-1);
		}

		public void AddCombinationNode (DBNode node, DBNode newNode)
		{
			TreeSizeIncreased = true;
			nodeCount += 1;

			// Graph bookkeeping
			node.Children.Add (newNode);
		}

		private bool HaveCommonCombinedChild (DBNode node1, DBNode node2)
		{
			foreach (DBNode child1 in node1.Children) {
				if (node2.CombinedChildren.IndexOf (child1) >= 0)
					return (true);
			}

			foreach (DBNode child2 in node2.Children) {
				if (node1.CombinedChildren.IndexOf (child2) >= 0)
					return (true);
			}

			return (false);
		}
	}
}


