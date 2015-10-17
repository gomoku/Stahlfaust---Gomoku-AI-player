using System;
using System.Net.Sockets;

class Communicator : Player {
	
	int port;
	string color;
	int size;

	System.IO.StreamReader inStream;
	System.IO.StreamWriter outStream;

	public int AskSize() 
	{
		return size;
	}
	public void SetSize(int size)
	{
	}
	public string AskColor()
	{
		return color;
	}
	public void SetColor(string color)
	{
	}
		

	public Communicator(string host, int port) {
		Console.WriteLine("Creating Communicator");
		this.port = port;

		TcpClient socket = null;
		
		try {
			socket = new TcpClient(host, port);
		} catch {
			Console.WriteLine("Failed to connect");
		}

		inStream = new System.IO.StreamReader(socket.GetStream());
		outStream = new System.IO.StreamWriter(socket.GetStream());
		
		Console.WriteLine("Hello: {0}", inStream.ReadLine());
		size = Int32.Parse(inStream.ReadLine());
		Console.WriteLine("Board size: {0}", size);
		color = inStream.ReadLine();
		Console.WriteLine("Our color: {0}", color);
	}

	public void RegOppMove(Coordinate move) {
		Console.WriteLine("Sending move: " + move.X + "/" + move.Y);
		outStream.WriteLine("" + move.X + "/" + move.Y);
		outStream.Flush();
	}

	public Coordinate GetMove() {
		try
		{
			string input = inStream.ReadLine();
			if (input == "You lost!") 
			{
				Console.WriteLine(input);
				return new Coordinate(-1, -1);
			}
			if (input == "You win!") 
			{
				Console.WriteLine(input);
				return new Coordinate(-1, -1);
			}
			int x = int.Parse(input.Substring(0, input.IndexOf("/")));
			int y = int.Parse(input.Substring(input.IndexOf("/")+1));
		
			Coordinate returnMove = new Coordinate(x, y);
			return returnMove;
		}
		catch (Exception) {return new Coordinate(-1, -1);}
	}
	
}
