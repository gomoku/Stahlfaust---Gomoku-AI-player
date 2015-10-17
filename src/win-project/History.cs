using System;
using System.Collections;

namespace win_project
{

	public class History : ArrayList
	{
		private int active = -1;
		public int Active 
		{
			get 
			{
				return active;
			}
			set 
			{
				if (0 <= value && value < this.Count) active = value;
			}
		}
		public History()
		{
		}

		public void Reset()
		{
			this.Clear();
			active = -1;
		}

		public void AddEntry(HistoryEntry e)
		{
			this.Add(e);
			active++;
		}
	}

	public class HistoryEntry
	{
		/** Field where the move has been made
		 */
		public Coordinate field;

		/** 1 for human or -1 for pc
		 */
		public int owner;

		/** Time needed for the move.
		 */
		public TimeSpan time;

		public HistoryEntry(Coordinate ff, int oo, TimeSpan tt)
		{
			field = ff;
			owner = oo;
			time = tt;
		}

		public override string ToString()
		{
			return (owner == 1?"You: ":"Ai:  ") + field + ", " 
				+ time.Seconds + "." + time.Milliseconds + "s";
		}

	}
}
