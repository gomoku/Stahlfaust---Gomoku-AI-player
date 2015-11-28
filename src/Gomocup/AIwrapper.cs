using System;
using System.IO;

class GomocupEngine : GomocupInterface
{
	NewAiPlayer ai;
	TextWriter output;

	public override string brain_about
	{
		get
		{
			return "name=\"Stahlfaust\", author=\"Marco Kunze, Sebastian Nowozin\", version=\"1.0\", country=\"Germany\"";
		}
	}

	public override void brain_init()
	{
		if (width < 5 || width > 30 || height < 5 || width != height)
		{
			Console.WriteLine("ERROR size of the board");
			return;
		}
		brain_restart();
	}

	public override void brain_restart()
	{
		output = Console.Out;
		Console.SetOut(TextWriter.Null);
		ai = new NewAiPlayer();
		ai.SetSize(width);
		Console.SetOut(output);
		Console.WriteLine("OK");
	}

	public override void brain_my(int x, int y)
	{
		Console.SetOut(TextWriter.Null);
		ai.RegOwnMove(new Coordinate(x, y));
		Console.SetOut(output);
	}

	public override void brain_opponents(int x, int y)
	{
		Console.SetOut(TextWriter.Null);
		ai.RegOppMove(new Coordinate(x, y));
		Console.SetOut(output);
	}

	public override void brain_block(int x, int y)
	{
		Console.WriteLine("ERROR brain_block not implemented");
	}

	public override int brain_takeback(int x, int y)
	{
		return 1;
	}

	public override void brain_turn()
	{
		int limit = Math.Min(info_timeout_turn, info_time_left / 10) / 1000;
		ai.dbtimelimit = ai.ddbtimelimit = limit / 4;
		ai.abtimelimit = Math.Max(1, limit - ai.dbtimelimit - ai.ddbtimelimit - 1);

		Console.SetOut(TextWriter.Null);
		Coordinate coord = ai.GetMove();
		Console.SetOut(output);
		do_mymove(coord.X, coord.Y);
	}

	public override void brain_end()
	{
	}

	public override void brain_eval(int x, int y)
	{
	}
}
