/* Five-Wins Gtk# GUI
 *
 * Marco Kunze, Sebastian Nowozin
 */

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using GLib;
using Gtk;
using GtkSharp;
using Glade;

public class FiveGUI
{
	// Glade-bound widgets
	[Glade.Widget]
	Gtk.Window mainWindow;

	[Glade.Widget]
	Gtk.Button connectButton;

	[Glade.Widget]
	Gtk.Button quitButton;

	[Glade.Widget]
	Gtk.Entry connectAddress;

	[Glade.Widget]
	Gtk.CheckButton logCheckBox;

	[Glade.Widget]
	Gtk.CheckButton clientDBSearchEnabled;

	[Glade.Widget]
	Gtk.Entry logFilename;

	// Self defined widgets
	[Glade.Widget]
	Gtk.Image boardImage;	// The board

	[Glade.Widget]
	Gtk.Label boardLabel;

	// Global game variables
	int xyDim = -1;	// The number of rows and columns on the board. Must be symmetric.
	int pixStep;
	int ownPlayer = -1;
	int currentPlayer = -1;
	Gdk.Pixbuf pBoard;
	Gdk.Pixbuf stone0;
	Gdk.Pixbuf stone1;
	int[,] board;

	// Network related data
	bool connected = false;
	System.Net.Sockets.Socket clientSocket;
	StreamReader netRead;
	StreamWriter netWrite;

	// Initialization code
	public static void Main (string[] args)
	{
		Application.Init ();

		FiveGUI fiveg = new FiveGUI ();
		fiveg.Init ();

		fiveg.ownPlayer = 0;
		fiveg.currentPlayer = 0;

		Application.Run ();
	}

	private void Init ()
	{
		//Glade.XML gxml = new Glade.XML ("autopanog.glade", "autopanogWin", null);
		Glade.XML gxml = new Glade.XML (null, "fivegui.glade", null, null);
		gxml.Autoconnect (this);

		InitializeBoard (10);
		RedrawBoard (450);
		// TODO
	}

	// Helper Initialization
	public void InitializeBoard (int dim)
	{
		xyDim = dim;
		board = new int[dim, dim];

		for (int y = 0 ; y < dim ; ++y)
			for (int x = 0 ; x < dim ; ++x)
				board[x, y] = -1;
	}

	int boardSizeChanged = 0;
	Gdk.Pixbuf blackV, blackH;

	public void RedrawLines ()
	{
		if (boardSizeChanged > 0) {
			blackV = new Gdk.Pixbuf (Gdk.Colorspace.Rgb, false, 8, 2, boardSizeChanged);
			blackH = new Gdk.Pixbuf (Gdk.Colorspace.Rgb, false, 8, boardSizeChanged, 2);
			blackV.Fill (0x00000000);
			blackH.Fill (0x00000000);
			boardSizeChanged = 0;
		}

		for (int l = 0 ; l <= xyDim ; ++l) {
			blackV.CopyArea (0, 0, blackV.Width, blackV.Height, pBoard, l * pixStep, 0);
			blackH.CopyArea (0, 0, blackH.Width, blackH.Height, pBoard, 0, l * pixStep);
		}
	}

	public void RedrawBoard (int pixelDim)
	{
		pixStep = pixelDim / xyDim;
		int min = xyDim * pixStep + 2;
		pBoard = new Gdk.Pixbuf (Gdk.Colorspace.Rgb, true,
			8, min, min);

		// SVG does not work well under windows yet
		/*string goodSmiley = "smiley100.svg";
		string badSmiley = "smiley111.svg";
		stone0 = Rsvg.Pixbuf.FromFileAtSize (ownPlayer == 0 ? goodSmiley : badSmiley,
			pixStep, pixStep);
		stone1 = Rsvg.Pixbuf.FromFileAtSize (ownPlayer == 1 ? goodSmiley : badSmiley,
			pixStep, pixStep);*/
		string goodSmiley = "smiley100.png";
		string badSmiley = "smiley111.png";

		stone0 = new Gdk.Pixbuf (ownPlayer == 0 ? goodSmiley : badSmiley);
		stone0 = stone0.ScaleSimple (pixStep, pixStep, Gdk.InterpType.Bilinear);

		stone1 = new Gdk.Pixbuf (ownPlayer == 1 ? goodSmiley : badSmiley);
		stone1 = stone1.ScaleSimple (pixStep, pixStep, Gdk.InterpType.Bilinear);

		// Draw board full with stones (DEBUG)
		for (int y = 0 ; y < xyDim ; ++y) {
			for (int x = 0 ; x < xyDim ; ++x) {
				if (board[x, y] == 0)
					DrawStone (0, x, y);
				else if (board[x, y] == 1)
					DrawStone (1, x, y);
			}
		}
		boardSizeChanged = min;
		RedrawLines ();

		boardImage.FromPixbuf = pBoard;
	}

	public bool DrawStone (int player, int x, int y)
	{
		Gdk.Pixbuf stone = null;
		if (player == 0) {
			stone = stone0;
		} else if (player == 1) {
			stone = stone1;
		}
		if (stone == null)
			throw (new ArgumentException ("player not 0 or 1"));

		// Check if field is empty
		if (board[x, y] != -1)
			return (false);
		board[x, y] = player;

		stone.CopyArea (0, 0, stone.Width, stone.Height,
			pBoard, x * pixStep, y * pixStep);
		RedrawLines ();
		boardImage.FromPixbuf = pBoard;
		return (true);
	}


	// Default events
	public void OnFiveGUIDelete (object obj, DeleteEventArgs args)
	{
		Application.Quit ();

		args.RetVal = true;
	}

	// Buttons
	public void OnConnectClicked (object obj, EventArgs ev)
	{
		Console.WriteLine ("connect");
		connected = false;

		string[] addrParts = connectAddress.Text.Split (':');
		if (addrParts.Length != 2)
			throw (new ArgumentException ("invalid address specified"));

		IPAddress ipAddress = Dns.Resolve (addrParts[0]).AddressList[0];
		IPEndPoint ipEndpoint = new IPEndPoint (ipAddress, Int32.Parse (addrParts[1]));

		clientSocket = new System.Net.Sockets.Socket (AddressFamily.InterNetwork,
			SocketType.Stream, ProtocolType.Tcp);
		try {
			clientSocket.Connect (ipEndpoint);
		} catch (Exception ex) {
			Console.WriteLine ("connection failed.");

			return;
		}

		if (clientSocket.Connected) {
			connected = true;
			Console.WriteLine ("connected.");

			NetworkStream ns = new NetworkStream (clientSocket);
			netRead = new StreamReader (ns);
			netWrite = new StreamWriter (ns);

			BootNetwork ();
			SetupGame ();
		}
	}

	public void SetupGame ()
	{
		InitializeBoard (xyDim);
		boardSizeChanged = 450;
		RedrawBoard (boardSizeChanged);
		currentPlayer = 0;

		UpdateMoveDisplay ();
		GetRemoteMove ();
	}

	public bool GetRemoteMove ()
	{
		if (currentPlayer == ownPlayer)
			return (false);

		string netline = ReadNetworkLine ();
		string[] win = netline.Split ('\n');
		if (String.Compare (win[0], "You win!") == 0) {
			MessageDialog md = new MessageDialog (mainWindow, 
				DialogFlags.DestroyWithParent, MessageType.Info,
				ButtonsType.Close,
				"You won the game!\n\nCongratulations!");

			md.Run ();
			md.Destroy();
			Application.Quit ();
			return (true);
		} else if (String.Compare (win[0], "You lost!") == 0) {
			MessageDialog md = new MessageDialog (mainWindow, 
				DialogFlags.DestroyWithParent, MessageType.Info,
				ButtonsType.Close,
				"You lost the game...\n\nMaybe you have better luck next time!");

			md.Run ();
			md.Destroy();
			Application.Quit ();
			return (true);
		}
		string[] pos = netline.Split ('/');
		int px, py;
		px = Int32.Parse (pos[0]);
		py = Int32.Parse (pos[1]);

		DrawStone (currentPlayer, px, py);

		// Test GBThreatSearch
		if (clientDBSearchEnabled.Active) {
			int[,] boardRev = new int[xyDim, xyDim];
			for (int y = 0 ; y < xyDim ; ++y) {
				for (int x = 0 ; x < xyDim ; ++x) {
					int stoneV = 0;

					if (board[x,y] == currentPlayer) {
						stoneV = -1;
					} else if (board[x,y] == -1) {
						stoneV = 0;
					} else
						stoneV = 1;
					boardRev[y,x] = stoneV;
				}
			}

			Console.WriteLine ("Searching for board");
			Console.WriteLine (new GoBangBoard (boardRev));
			GBThreatSearch.FindWinning (boardRev, 5000);
		}

		currentPlayer = (currentPlayer == 0) ? 1 : 0;

		return (true);
	}

	public void UpdateMoveDisplay ()
	{
		if (ownPlayer == currentPlayer)
			boardLabel.Text = "Board - your move";
		else
			boardLabel.Text = "Board - opponent's move";

		while (Application.EventsPending ())
			Application.RunIteration (false);
	}

	public void BootNetwork ()
	{
		// One line to waste
		ReadNetworkLine ();

		// Board dimension
		string boardSizeStr = ReadNetworkLine ();
		Console.WriteLine ("boardSizeStr: {0}", boardSizeStr);

		// Player color
		string playerBootStr = ReadNetworkLine ();
		Console.WriteLine ("playerBootStr: {0}", playerBootStr);

		xyDim = Int32.Parse (boardSizeStr);
		string[] colorL = playerBootStr.Split ('\n');
		if (String.Compare (colorL[0], "white") == 0) {
			ownPlayer = 0;
		} else if (String.Compare (colorL[0], "black") == 0) {
			ownPlayer = 1;
		} else {
			throw (new ArgumentException ("player color is neither white nor black"));
		}
	}

	public string ReadNetworkLine ()
	{
		if (connected == false)
			throw (new Exception ("not connected"));

		return (netRead.ReadLine ());
	}

	// line already has to be line-terminated
	public void WriteNetworkLine (string line)
	{
		netWrite.Write (line);
		netWrite.Flush ();
	}

	public void OnQuitClicked (object obj, EventArgs ev)
	{
		Application.Quit ();
	}

	// Board events
	public void OnBoardClicked (object obj, ButtonPressEventArgs bev)
	{
		Gdk.EventButton evb = bev.Event;
		Console.WriteLine ("boardclick at: {0}, {1}", evb.X, evb.Y);

		int px = ((int) evb.X) / pixStep;
		int py = ((int) evb.Y) / pixStep;
		Console.WriteLine (" --> {0}, {1}", px, py);

		/* TODO: uncomment
		if (ownPlayer != currentPlayer) {
			Console.WriteLine ("move move move, but its not your move...");
			return;
		}
		*/

		if (DrawStone (currentPlayer, px, py) == false) {
			// Move was not ok
			Console.WriteLine ("     invalid move");

			return;
		}

		// Move was ok
		DoPlayerMove (px, py);
		WriteNetworkLine (String.Format ("{0}/{1}\n", px, py));

		currentPlayer = (currentPlayer == 0) ? 1 : 0;
		UpdateMoveDisplay ();

		GetRemoteMove ();
		UpdateMoveDisplay ();
	}

	public void DoPlayerMove (int px, int py)
	{
		// TODO: send to network
	}
}

