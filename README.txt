

Stahlfaust - a Gomoku AI
========================

Copyright (C) 2004-2005
Marco Kunze <makunze@cs.tu-berlin.de>,
Sebastian Nowozin <nowozin@cs.tu-berlin.de>.


This package contains binaries and source code for a small two week project of
ours, an artificial intelligence player for the standard Gomoku game ("five
wins").

  An in-depth documentation is available in the doc/ folder.  All files in
this package are licensed under the GNU General Public License, as included in
the "LICENSE" file.  Read and accept this license before running or modifying
this program.

  If you have any question relating to this program, please write us at
makunze@cs.tu-berlin.de and nowozin@cs.tu-berlin.de.

  Maybe we will remove updated version of the game at
http://cs.tu-berlin.de/~nowozin/stahlfaust/


FAQ
===

Q: Why is this game named "Stahlfaust"?
A: Its german and means "iron fist".  We thought this AI would squash all
   human and AI players in a competition, but actually we turned out to be
   second and we both easily win against the AI now.

Q: Can you give any playing advice?
A: Read the chapter about Gomoku in Victor Allis thesis and understand the
   idea of building threat trees.  If you practise well after some days you
   will have no problems building threat trees of maybe 10-15 moves in your
   mind while playing.  This is good enough to beat Stahlfaust and for most
   human players, too.

Q: Who should start?
A: Your choice, however Victor Allis proved the standard Gomoku to be a sure
   win for the starting player.  Hence, you can be more proud to win against
   this program in case you played the second move.

Q: Why is it so slow?
A: It just is.  No, really, we coded this in a hurry and the dbsearch - hah -
   we were happy it actually worked at all ;-)  So, don't complain, we know
   the dbsearch can be speed up by a factor of 50-100 in a week's work.  The
   alpha-beta search is quite fast though, using optimized lookup.

Q: How to compile it?
A: We developed it using both Mono (http://www.mono-project.com/) and Visual
   Studio 2003 .NET.  You can compile the VS.NET solution.  If you are
   familiar with Mono, you can get the gtksharp-gui to work, I am sure.

Q: Where can I read up on Gomoku?
A: http://en.wikipedia.org/wiki/Gomoku

Q: I want to become a better Gomoku player, how to?
A: Except playing it a lot (obvious) with many different programs, you should
   program a Gomoku AI yourself.  It teaches you how to value different
   situations on the board and an algorithmic approach to evaluating moves.


Contact
=======

Marco Kunze <makunze@cs.tu-berlin.de>,
Sebastian Nowozin <nowozin@cs.tu-berlin.de>.


