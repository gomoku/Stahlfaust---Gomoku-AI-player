Stahlfaust - Gomoku AI player
by Marco Kunze and Sebastian Nowozin (makunze@cs.tu-berlin.de, nowozin@cs.tu-berlin.de)

Introduction
Gomoku, sometimes known as "five wins" is a game played on a 15x15 board by two players. The goal is simple: to be the first player to have a line of five or more stones. While the rules are as simple as that, the game can become very complex and challenging.

Stahlfaust is an AI player for the standard Gomoku game. It uses alpha-beta search, threat tree dbsearch and defensive search to provide a challenge to you. However, its not very fast... :-(

The game was developed as term project for the Artificial Intelligence course of Professor Zhang Liqing at the Shanghai Jiaotong University by Marco Kunze and Sebastian Nowozin.
Screenshots
Everybody likes screenshots, so here is one. debugging output windows you can view:

![Tags: Five in a Row, Tic Tac Toe, TicTacToe, 5 in a Row, Go-Moku, Connect, Connect5, Connect6, Caro, Noughts and Crosses, Gomoku, Renju, Pente, Piskvork, Amoba, Kółko i Krzyżyk, Gomocup, AI, Engine, Artificial Intelligence, Brain, Pbrain, Gra, Game, Source Code Files, Program, Programming, Github, Board, Coding.](stahlfaust.png "Tags: Five in a Row, Tic Tac Toe, TicTacToe, 5 in a Row, Go-Moku, Connect, Connect5, Connect6, Caro, Noughts and Crosses, Gomoku, Renju, Pente, Piskvork, Amoba, Kółko i Krzyżyk, Gomocup, AI, Engine, Artificial Intelligence, Brain, Pbrain, Gra, Game, Source Code Files, Program, Programming, Github, Board, Coding.")

Technology
The guts of the AI is an intelligent definition of threats, used in both the alpha-beta search and the dependency based search. This definition has been given by L. Victor Allis in his PhD thesis titled " Searching for Solutions in Games and Artificial Intelligence". (By the way, he invented db-search).

We implemented some extra stuff though and we also try to find the threat trees of the opponent player in order to establish a defense.
Download
The whole program is opensource and free software. It is released under the conditions of the GNU General Public License. The license is included in the distribution in the 'LICENSE' file. Please read and acknowledge this license before using this program. On Linux you will need Mono, a free implementation of the Microsoft .NET framework and compilers. The source is written almost completely in C#.

    Stahlfaust 1.0, stahlfaust-1.0.tar.gz (410kb) 

Thanks
We would like to thank Professor Zhang Liqing for his excellent AI course and the challenge posed to us to write a competitive Gomoku player. We easily defeated the previous' years champion program and landed a respectable second place in this years competition.

We would also like to thank Victor Allis for sharing insights into the game of Gomoku.

Feedback
We are curious about user-, developer- and mathematicans feedback, so please mail us your thoughts to makunze@cs.tu-berlin.de and nowozin@cs.tu-berlin.de.
last update: Friday, 18 Mar 2005

Original author: by Marco Kunze and Sebastian Nowozin. 
Original website: http://www.nowozin.net/sebastian/tu-berlin-2006/stahlfaust/
Email:   makunze@cs.tu-berlin.de and nowozin@cs.tu-berlin.de
Country: Deutsche
Programming language: C#
IDE: Microsoft .NET framework 
Year: 2005
Notes: First time on any source code repository.

Tags: Five in a Row, Tic Tac Toe, TicTacToe, 5 in a Row, Go-Moku, Connect, Connect5, Connect6, Caro, Noughts and Crosses, Gomoku, Renju, Pente, Piskvork, Amoba, Kółko i Krzyżyk, Gomocup, AI, Engine, Artificial Intelligence, Brain, Pbrain, Gra, Game, Source Code Files, Program, Programming, Github, Board, Coding.
