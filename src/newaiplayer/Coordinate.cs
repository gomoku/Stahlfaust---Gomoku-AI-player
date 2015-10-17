using System;
public struct Coordinate : IComparable
{
	public readonly int X;
	public readonly int Y;
	public int Val;

	public Coordinate(int x, int y)
	{
		X = x;
		Y = y;
		Val = StatValEvaluator.UNDEF;
	}

	public override string ToString() 
	{
		return "abcdefghijklmnopqrstuvwxyz"[X] + "/" + Y;
	}

	// Only coordinate based
	public override bool Equals (object o2)
	{
		if (o2 == null)
			return (false);

		Coordinate c2 = (Coordinate) o2;

		if (X == c2.X && Y == c2.Y)
			return (true);

		return (false);
	}

	public int CompareTo(object obj)
	{
		Coordinate c = (Coordinate)obj;
		return this.Val < c.Val?-1: this.Val==c.Val?0:1;
	}

}
