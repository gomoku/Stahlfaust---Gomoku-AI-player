import java.net.*;
import java.io.*;
import java.util.*;
class Server {

	int size;

	int port;

	int turn;
	int noMoves;

	BufferedWriter[] out;
	BufferedReader[] in;

	char[][] board;

	Server(int size, int port) {
		this.size = size;
		this.port = port;
		turn = 0;
		noMoves = 0;

		try {

			System.out.println("Server coming up...");
			out = new BufferedWriter[2];
			in = new BufferedReader[2];
			

			board = new char[size][size];

			for (int x = 0; x < size; ++x)
				for (int y = 0; y < size; ++y)
					board[x][y] = ' ';

			ServerSocket ss1 = new ServerSocket(port);
			Random r = new Random();
			int x = r.nextInt(11);
			int pl;
			for (int i = 0; i < 2; ++i) {
				pl = (x+i)%2;
				System.out.println("Waiting for player " + (pl + 1));
				Socket s1 = ss1.accept();
				out[pl] = new BufferedWriter(new OutputStreamWriter(s1.getOutputStream()));
				in[pl] = new BufferedReader(new InputStreamReader(s1.getInputStream()));

				System.out.println("Connection to " + (pl+1) + " player established");
				out[pl].write("Hello, you are player " + (pl+1) +"\n");
				out[pl].flush();
			}
		} catch (IOException e) {System.out.println("error");};
		System.out.println("Server running");
	}

	void printBoard() {
		System.out.println("Current Board:");
		for (int y = 0; y < size; ++y) {
			for (int x = 0; x < size; ++x)
				System.out.print(board[x][y]);
			System.out.println("");
		}
		System.out.println("Next turn: player " + turn);
		System.out.println("Moves played: " + noMoves);
	}
	void sendBoard() {
		try {
			out[turn].write("Current Board:\n  ");
			out[turn].flush();
			for (int x = 0; x < size; ++x) {
				out[turn].write(x + " ");
			}
			out[turn].write("\n");
			out[turn].flush();
			for (int y = 0; y < size; ++y) {
				out[turn].write(y + " ");
				for (int x = 0; x < size; ++x) {
					out[turn].write(board[x][y] + " ");
					out[turn].flush();
				}
				out[turn].write("\n");
				out[turn].flush();
			}
			out[turn].write("Next turn: player " + turn + "\n");
			out[turn].flush();
			out[turn].write("Moves played: " + noMoves + "\n");
			out[turn].flush();
		} catch(IOException e) {System.out.println("sendBoard() : error");};
	}

	void sendInitialData() {
		System.out.println("Sending initial data");
		try {
			out[0].write("" + size + "\n");
			out[0].flush();
			out[1].write("" + size + "\n");
			out[1].flush();
			out[1].write("black\n");
			out[0].write("white\n");
			out[0].flush();
			out[1].flush();
			turn = 0;
		} catch(IOException e) {System.out.println("sendInitialData() : error");};
	}

	boolean validMove(String move) {
		try {
			System.out.println("Received string from Player: " + move);
			int x = Integer.parseInt(move.substring(0, move.indexOf("/")));
			int y = Integer.parseInt(move.substring(move.indexOf("/")+1));
			System.out.println("Got move: " + x + "/" + y);
			if (x > size || y > size || x < 0 || y < 0) {
				System.out.println("Move out of the board.");
				return false;
			}
			if (board[x][y] != ' ') {
				System.out.println("Field not empty");
				return false;
			}
			return true;
		} catch(Exception e) {
			System.out.println("Invalid move (Syntax)");
			return false;
		}

	}

	void makeMove(String move) {
		int x = Integer.parseInt(move.substring(0, move.indexOf("/")));
		int y = Integer.parseInt(move.substring(move.indexOf("/")+1));

		if (turn == 0) board[x][y] = 'X'; else board[x][y] = 'O';
		noMoves++;
		turn = 1 - turn;

		
	}

	String readMove() {
		String move = "";
		try {
			move = in[turn].readLine();
		} catch(IOException e) {};
		return move;
	}

	boolean existMove() {
		return (noMoves < size * size);
	}

	int winner() {
		for (int x = 0; x < size; ++x) {
			for (int y = 0; y < size; ++y) {
				try {
					if (board[x][y] == 'X' &&
							board[x+1][y+1] == 'X' &&
							board[x+2][y+2] == 'X' &&
							board[x+3][y+3] == 'X' &&
							board[x+4][y+4] == 'X')
						return 0;
				} catch (Exception e) {};
				try {
					if (board[x][y] == 'X' &&
							board[x+1][y] == 'X' &&
							board[x+2][y] == 'X' &&
							board[x+3][y] == 'X' &&
							board[x+4][y] == 'X') 
						return 0;
				} catch (Exception e) {};
				try {
					if (board[x][y] == 'X' &&
							board[x][y+1] == 'X' &&
							board[x][y+2] == 'X' &&
							board[x][y+3] == 'X' &&
							board[x][y+4] == 'X') 
						return 0;
				} catch (Exception e) {};
				try {
					if (board[x+4][y] == 'X' &&
							board[x+3][y+1] == 'X' &&
							board[x+2][y+2] == 'X' &&
							board[x+1][y+3] == 'X' &&
							board[x][y+4] == 'X')
						return 0;
				} catch (Exception e) {};
				try {
					if (board[x][y] == 'O' &&
							board[x+1][y+1] == 'O' &&
							board[x+2][y+2] == 'O' &&
							board[x+3][y+3] == 'O' &&
							board[x+4][y+4] == 'O') 
						return 1;
				} catch (Exception e) {};
				try {
					if (board[x][y] == 'O' &&
							board[x+1][y] == 'O' &&
							board[x+2][y] == 'O' &&
							board[x+3][y] == 'O' &&
							board[x+4][y] == 'O') 
						return 1;
				} catch (Exception e) {};
				try {
					if (board[x][y] == 'O' &&
							board[x][y+1] == 'O' &&
							board[x][y+2] == 'O' &&
							board[x][y+3] == 'O' &&
							board[x][y+4] == 'O')
						return 1;
				} catch (Exception e) {};
				try {
					if (board[x+4][y] == 'O' &&
							board[x+3][y+1] == 'O' &&
							board[x+2][y+2] == 'O' &&
							board[x+1][y+3] == 'O' &&
							board[x][y+4] == 'O')
						return 1;
				} catch (Exception e) {};
			}
		}
		return -1;
	}
	
	public void run() {
		sendInitialData();
		String move = "";
		int winner = -1;
		while (existMove()) {
			move = readMove();
			if (validMove(move) == false) {
				if (move.equals("board")) {
					sendBoard();
				} else {
					System.out.println("Illegal Move/Statement");
					try {
						out[1-turn].write("Illegal Move/Statement");
						out[1-turn].flush();
					} catch(IOException e) {System.out.println("run() : error");};
					System.exit(1);
				}
			} else {
				makeMove(move);
				if ((winner = winner()) != -1) {
					printBoard();
					System.out.println("Player " + (2-winner) + " has won!");
					try {
						out[turn].write("You lost!");
						out[turn].flush();
						out[1-turn].write("You win!");
						out[1-turn].flush();
					} catch (IOException e) {System.out.println("run() : error");}
					System.exit(1);
				}
				printBoard();
				try {
					out[turn].write(move + "\n");
					out[turn].flush();
				} catch (IOException e) {System.out.println("makemove() : error");}
				move = "";
			}
		}
		
		if (move != "") System.out.println("No valid move: " + move);
	}

	public static void main (String[] args) {
		Server server = new Server(Integer.parseInt(args[0]), Integer.parseInt(args[1]));
		server.run();
	}
}

				

		
