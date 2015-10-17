using System;
using System.Collections;

public class ComparerTest
{
	public static void Main()
	{
		int[,] board = new int[,]{
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  1,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  1,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  1,  0,  0,  1,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  1,  0,  0,  1,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  1,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0, -1,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, 
			{0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}
		};
		TunnelSearcher searcher = new FirstSearcher();
		InterestingFieldAgent ifa = new InterestingFieldAgent(searcher, 15);
		FirstEvaluator eval = new FirstEvaluator(ifa);
		StatValComparer comp = new StatValComparer(eval);

		Coordinate c1 = new Coordinate(7, 7);
		Coordinate c2 = new	Coordinate(10, 10);
		comp.Board = board;
		comp.Attacker = 1;

		ArrayList a1 = new ArrayList();
		a1.Add(c1);
		a1.Add(c2);
		a1.Sort(comp);

		foreach (Coordinate c in a1)
			Console.WriteLine("Coordinate: {0}, Val: {1}", c, c.Val);
		
		
		
	}
}
