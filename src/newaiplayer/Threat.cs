using System;
using System.Collections;
using System.Text;

public class ThreatList : ArrayList, ICloneable
{
	public override object Clone ()
	{
		ThreatList tl = new ThreatList ();

		for (int n = 0 ; n < Count ; ++n)
			tl.Add (((Threat) this[n]).Clone ());

		return (tl);
	}
}

public class Threat : ICloneable
{
	/** Field that caused the threat
	 */
	public readonly Coordinate cause;

	/** List of the fields which can block the threat.
	 * ArrayList<Coordinate>
	 */
	public readonly ArrayList fields;

	/** Category of the threat
	 */
	public readonly int category;

	/** Shows special creator situation, where a cat1-threat can be built out of 
	 * nothing.
	 */
	public readonly bool create;

	public Threat(Coordinate cause, int cc, ArrayList ff, bool create)
	{
		this.cause = cause;
		category = cc;
		fields = ff;
		this.create = create;
	}
	public Threat(Coordinate cause, int cc, ArrayList ff) :
		this(cause, cc, ff, false) {}

	public object Clone()
	{
		Threat res = new Threat(cause, category, (ArrayList)fields.Clone(), create);
		return res;
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("cause: {0}\n", cause);
		sb.AppendFormat("cat: {0}\n", category);
		foreach (Coordinate c in fields)
			sb.AppendFormat("Defense Field: {0}\n", c);
		return sb.ToString();
	}

}
