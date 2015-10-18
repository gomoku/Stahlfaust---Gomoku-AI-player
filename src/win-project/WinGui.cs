using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace win_project
{


	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;

		private NewAiPlayer ai;
		private int[,] board = new int[15, 15];
		private MyPicBox[,] picboard;
		private DateTime lasttime = DateTime.Now;
		private History history = new History();
		
		private System.Windows.Forms.PictureBox empty;
		private System.Windows.Forms.PictureBox opp;
		private System.Windows.Forms.PictureBox own;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.PictureBox my_color;
		private System.Windows.Forms.PictureBox your_color;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.PictureBox next_turn;
		private System.Windows.Forms.PictureBox faust;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
#if MONO == false
		private System.Windows.Forms.RichTextBox richTextBox1;
#endif
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown numericUpDown2;
		private System.Windows.Forms.Button histback;
		private System.Windows.Forms.Button histfor;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown numericUpDown3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			InitBoard();

			// Let the player start by default
			ai = new NewAiPlayer();
			ai.SetColor("black");
			ai.SetSize(15);
			my_color.Image = opp.Image;
			your_color.Image = own.Image;
			next_turn.Image = own.Image;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.empty = new System.Windows.Forms.PictureBox();
			this.opp = new System.Windows.Forms.PictureBox();
			this.own = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.my_color = new System.Windows.Forms.PictureBox();
			this.your_color = new System.Windows.Forms.PictureBox();
			this.label3 = new System.Windows.Forms.Label();
			this.next_turn = new System.Windows.Forms.PictureBox();
			this.faust = new System.Windows.Forms.PictureBox();
			this.label4 = new System.Windows.Forms.Label();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
			this.histback = new System.Windows.Forms.Button();
			this.histfor = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1,
																					  this.menuItem6});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem2,
																					  this.menuItem5});
			this.menuItem1.Text = "Game";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem3,
																					  this.menuItem4});
			this.menuItem2.Text = "New";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 0;
			this.menuItem3.Text = "Computer starts";
			this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 1;
			this.menuItem4.Text = "You start";
			this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 1;
			this.menuItem5.Text = "Exit";
			this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 1;
			this.menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem7});
			this.menuItem6.Text = "About";
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 0;
			this.menuItem7.Text = "About";
			this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
			// 
			// empty
			// 
			this.empty.Enabled = false;
			this.empty.Image = ((System.Drawing.Image)(resources.GetObject("empty.Image")));
			this.empty.Location = new System.Drawing.Point(712, 8);
			this.empty.Name = "empty";
			this.empty.Size = new System.Drawing.Size(32, 32);
			this.empty.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.empty.TabIndex = 0;
			this.empty.TabStop = false;
			this.empty.Visible = false;
			// 
			// opp
			// 
			this.opp.Enabled = false;
			this.opp.Image = ((System.Drawing.Image)(resources.GetObject("opp.Image")));
			this.opp.Location = new System.Drawing.Point(720, 56);
			this.opp.Name = "opp";
			this.opp.Size = new System.Drawing.Size(32, 32);
			this.opp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.opp.TabIndex = 1;
			this.opp.TabStop = false;
			this.opp.Visible = false;
			// 
			// own
			// 
			this.own.Enabled = false;
			this.own.Image = ((System.Drawing.Image)(resources.GetObject("own.Image")));
			this.own.Location = new System.Drawing.Point(712, 104);
			this.own.Name = "own";
			this.own.Size = new System.Drawing.Size(32, 32);
			this.own.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.own.TabIndex = 2;
			this.own.TabStop = false;
			this.own.Visible = false;
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Location = new System.Drawing.Point(8, 8);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(482, 482);
			this.panel1.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(520, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "My Color:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(520, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "Your Color:";
			// 
			// my_color
			// 
			this.my_color.Enabled = false;
			this.my_color.Image = ((System.Drawing.Image)(resources.GetObject("my_color.Image")));
			this.my_color.Location = new System.Drawing.Point(624, 8);
			this.my_color.Name = "my_color";
			this.my_color.Size = new System.Drawing.Size(32, 32);
			this.my_color.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.my_color.TabIndex = 6;
			this.my_color.TabStop = false;
			// 
			// your_color
			// 
			this.your_color.Enabled = false;
			this.your_color.Image = ((System.Drawing.Image)(resources.GetObject("your_color.Image")));
			this.your_color.Location = new System.Drawing.Point(624, 48);
			this.your_color.Name = "your_color";
			this.your_color.Size = new System.Drawing.Size(32, 32);
			this.your_color.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.your_color.TabIndex = 7;
			this.your_color.TabStop = false;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(520, 96);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 16);
			this.label3.TabIndex = 8;
			this.label3.Text = "Next Turn:";
			// 
			// next_turn
			// 
			this.next_turn.Enabled = false;
			this.next_turn.Image = ((System.Drawing.Image)(resources.GetObject("next_turn.Image")));
			this.next_turn.Location = new System.Drawing.Point(624, 88);
			this.next_turn.Name = "next_turn";
			this.next_turn.Size = new System.Drawing.Size(32, 32);
			this.next_turn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.next_turn.TabIndex = 9;
			this.next_turn.TabStop = false;
			// 
			// faust
			// 
			this.faust.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.faust.Image = ((System.Drawing.Image)(resources.GetObject("faust.Image")));
			this.faust.Location = new System.Drawing.Point(712, 232);
			this.faust.Name = "faust";
			this.faust.Size = new System.Drawing.Size(110, 107);
			this.faust.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.faust.TabIndex = 10;
			this.faust.TabStop = false;
			this.faust.Visible = false;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(520, 168);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(88, 24);
			this.label4.TabIndex = 11;
			this.label4.Text = "DB Time Limit:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(624, 168);
			this.numericUpDown1.Maximum = new System.Decimal(new int[] {
																		   99,
																		   0,
																		   0,
																		   0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(32, 20);
			this.numericUpDown1.TabIndex = 12;
			this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDown1.Value = new System.Decimal(new int[] {
																		 8,
																		 0,
																		 0,
																		 0});
			this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
			this.numericUpDown1.Leave += new System.EventHandler(this.numericUpDown1_ValueChanged);
			// 
			// richTextBox1
			// 
			this.richTextBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.richTextBox1.Location = new System.Drawing.Point(512, 272);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(160, 216);
			this.richTextBox1.TabIndex = 13;
			this.richTextBox1.Text = "";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(520, 240);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 24);
			this.label5.TabIndex = 14;
			this.label5.Text = "History:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(520, 136);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(88, 24);
			this.label6.TabIndex = 15;
			this.label6.Text = "AB Time Limit:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// numericUpDown2
			// 
			this.numericUpDown2.Location = new System.Drawing.Point(624, 136);
			this.numericUpDown2.Maximum = new System.Decimal(new int[] {
																		   99,
																		   0,
																		   0,
																		   0});
			this.numericUpDown2.Name = "numericUpDown2";
			this.numericUpDown2.Size = new System.Drawing.Size(32, 20);
			this.numericUpDown2.TabIndex = 16;
			this.numericUpDown2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDown2.Value = new System.Decimal(new int[] {
																		 8,
																		 0,
																		 0,
																		 0});
			this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
			this.numericUpDown2.Leave += new System.EventHandler(this.numericUpDown2_ValueChanged);
			// 
			// histback
			// 
			this.histback.Location = new System.Drawing.Point(624, 240);
			this.histback.Name = "histback";
			this.histback.Size = new System.Drawing.Size(16, 24);
			this.histback.TabIndex = 17;
			this.histback.Text = "<";
			this.histback.Click += new System.EventHandler(this.histback_Click);
			// 
			// histfor
			// 
			this.histfor.Enabled = false;
			this.histfor.Location = new System.Drawing.Point(640, 240);
			this.histfor.Name = "histfor";
			this.histfor.Size = new System.Drawing.Size(16, 24);
			this.histfor.TabIndex = 18;
			this.histfor.Text = ">";
			this.histfor.Click += new System.EventHandler(this.histfor_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(520, 200);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(88, 24);
			this.label7.TabIndex = 19;
			this.label7.Text = "DDB Time Limit:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// numericUpDown3
			// 
			this.numericUpDown3.Location = new System.Drawing.Point(624, 200);
			this.numericUpDown3.Maximum = new System.Decimal(new int[] {
																		   99,
																		   0,
																		   0,
																		   0});
			this.numericUpDown3.Name = "numericUpDown3";
			this.numericUpDown3.Size = new System.Drawing.Size(32, 20);
			this.numericUpDown3.TabIndex = 20;
			this.numericUpDown3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDown3.Value = new System.Decimal(new int[] {
																		 8,
																		 0,
																		 0,
																		 0});
			this.numericUpDown3.ValueChanged += new System.EventHandler(this.numericUpDown3_ValueChanged);
			this.numericUpDown3.Leave += new System.EventHandler(this.numericUpDown3_ValueChanged);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(686, 495);
			this.Controls.Add(this.numericUpDown3);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.histfor);
			this.Controls.Add(this.histback);
			this.Controls.Add(this.numericUpDown2);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.faust);
			this.Controls.Add(this.next_turn);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.your_color);
			this.Controls.Add(this.my_color);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.own);
			this.Controls.Add(this.opp);
			this.Controls.Add(this.empty);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.Text = "Stahlfaust";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]


		private bool winning (int i)
		{
			int maxx = board.GetLength(0);
			int maxy = board.GetLength(1);

			int[] pd1 = new int[maxx + maxy];
			int[] pd2 = new int[maxx + maxy];
			int[] pdy = new int[maxx];

			for (int y = 0 ; y < maxy ; ++y) 
			{
				int pdx = 0;

				for (int x = 0 ; x < maxx ; ++x) 
				{
					int pd1_idx = x - y + (maxy - 1);
					int pd2_idx = x + y;
					// pdy_idx is x

					if (board[x, y] == i) 
					{
						if (++pd1[pd1_idx] == 5 || ++pd2[pd2_idx] == 5 || ++pdy[x] == 5 || ++pdx == 5)
							return (true);
					} 
					else 
					{
						// reset numbers
						pd1[pd1_idx] = 0;
						pd2[pd2_idx] = 0;
						pdy[x] = 0;
						pdx = 0;
					}
				}
			}

			return (false);


		}

		private bool valid(Coordinate move)
		{
			if (board[move.X, move.Y] != 0) return false;
			return true;
		}

		private void InitBoard()
		{
			picboard = new MyPicBox[15, 15];
			for (int x = 0; x < picboard.GetLength(0); ++x) 
			{
				for (int y = 0; y < picboard.GetLength(1); ++y)
				{
					board[x, y] = 0;
					picboard[x, y] = new MyPicBox();
					picboard[x, y].Image = (Image)(empty.Image).Clone();
					picboard[x, y].Location = new System.Drawing.Point(x*32, y*32);
					this.picboard[x, y].Name = "picboard_" + x + "_" + y;
					this.picboard[x, y].Size = new System.Drawing.Size(32, 32);
					this.picboard[x, y].SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
					this.picboard[x, y].TabIndex = 0;
					picboard[x, y].x = x;
					picboard[x, y].y = y;
					panel1.Controls.Add(picboard[x, y]);
					this.picboard[x, y].Click += new System.EventHandler(this.field_Click);
				}
			}
		}

		private void ResetGame()
		{
			ai.dbtimelimit = (int)numericUpDown1.Value;
			ai.abtimelimit = (int)numericUpDown2.Value;
			ai.ddbtimelimit = (int)numericUpDown3.Value;
			this.Text = "Stahlfaust";
			for (int x = 0; x < picboard.GetLength(0); ++x) 
			{
				for (int y = 0; y < picboard.GetLength(1); ++y)
				{
					board[x, y] = 0;
					picboard[x, y].Image = (Image)(empty.Image).Clone();
				}
			}
			history.Reset();
#if MONO == false
			richTextBox1.Clear();
#endif
			lasttime = DateTime.Now;
		}

		private void AnnounceSequence()
		{
			if (ai.sequence_used == true) 
			{
				faust.Visible = true;
				this.Text = "Stahlfaust - wins again";

			}
			else
			{ 
				faust.Visible = false;
				this.Text = "Stahlfaust";
			}
		}

		private void RegAiMove(Coordinate move)
		{
			DateTime time = DateTime.Now;
			board[move.X, move.Y] = -1;
			picboard[move.X, move.Y].Image = opp.Image;
			next_turn.Image = own.Image;
			this.Text = "Stahlfaust - " + ai.statusStr.ToString();
			TimeSpan t = time.Subtract(lasttime);
			HistoryEntry e = new HistoryEntry(move, -1, t);
			history.AddEntry(e);
#if MONO == false
			richTextBox1.AppendText(e.ToString() + "\n");
#endif
			lasttime = DateTime.Now;
			Update();
		}

		private void RegHumanMove(Coordinate move)
		{
			DateTime time = DateTime.Now;
			board[move.X, move.Y] = 1;
			picboard[move.X, move.Y].Image = own.Image;
			next_turn.Image = opp.Image;
			TimeSpan t = time.Subtract(lasttime);
			HistoryEntry e = new HistoryEntry(move, 1, t);
			history.AddEntry(e);
#if MONO == false
			richTextBox1.AppendText(e.ToString() + "\n");
#endif
			lasttime= DateTime.Now;
			Update();
		}


		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void menuItem5_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		//Computer starts button
		private void menuItem3_Click(object sender, System.EventArgs e)
		{
			next_turn.Image = opp.Image;

			ai = new NewAiPlayer();
			ai.SetColor("white");
			ai.SetSize(15);
			ResetGame();
			
			Coordinate move;
			move = ai.GetMove();
			Console.WriteLine("Got move from player 1: {0}", move);
			if (!valid(move)) 
				throw new Exception("Invalid move " + move);
			RegAiMove(move);
			next_turn.Image = opp.Image;

		}

		//Player starts
		private void menuItem4_Click(object sender, System.EventArgs e)
		{

			next_turn.Image = own.Image;
			ai = new NewAiPlayer();
			ai.SetColor("black");
			ai.SetSize(15);
			ResetGame();

		}

		private void field_Click(object sender, System.EventArgs e)
		{
			MyPicBox box = sender as MyPicBox;
			Coordinate move = new Coordinate(box.x, box.y);
			Console.WriteLine("Got click on field: {0}", move);
			if (!valid(move)) 
			{
				MessageBox.Show("Invalid move " + move);
			}
			else
			{
				RegHumanMove(move);
				if (winning(1))
				{
					MessageBox.Show(this, "You win");
					DumpHistory();
					ai = new NewAiPlayer();
				}
				else
				{
					ai.RegOppMove(move);
					move = ai.GetMove();
					RegAiMove(move);
					if (winning(-1)) 
					{
						MessageBox.Show("You lost");
						DumpHistory();
						ai = new NewAiPlayer();
					}
				}
			}
		}

		private void menuItem7_Click(object sender, System.EventArgs e)
		{
			MessageBox.Show(this, "Stahlfaust v1.0\n\nKunze, Nowozin\n(c) 2004"); 
		}

		private void numericUpDown1_ValueChanged(object sender, System.EventArgs e)
		{
			ai.dbtimelimit = (int)numericUpDown1.Value;
		}

		private void DumpHistory()
		{
			foreach (HistoryEntry e in history)
			{
				Console.WriteLine(e);
			}
		}

		private void numericUpDown2_ValueChanged(object sender, System.EventArgs e)
		{
			ai.abtimelimit = (int)numericUpDown2.Value;		
		}

		private void numericUpDown3_ValueChanged(object sender, System.EventArgs e)
		{
			ai.ddbtimelimit = (int)numericUpDown3.Value;		
		}

		private void histback_Click(object sender, System.EventArgs e)
		{
			HistoryEntry current = (HistoryEntry)history[history.Active];

			history.Active--;

			picboard[current.field.X, current.field.Y].Image = (Image)empty.Image.Clone();

			if (history.Active == 0) histback.Enabled = false;
			histfor.Enabled = true;
		}

		private void histfor_Click(object sender, System.EventArgs e)
		{
			HistoryEntry current = (HistoryEntry)history[++history.Active];

			if (current.owner == 1)
				picboard[current.field.X, current.field.Y].Image = (Image)own.Image.Clone();
			else 
				picboard[current.field.X, current.field.Y].Image = (Image)opp.Image.Clone();

			if (history.Active + 1 == history.Count) histfor.Enabled = false;
			histback.Enabled = true;

		}


	}
}
