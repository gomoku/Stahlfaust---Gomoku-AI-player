

public class SortedLimitedList : ArrayList
{
	private SortedLimitedList ()
	{
	}

	int max;

	public SortedLimitedList (int maxElements)
		: base (maxElements)
	{
		max = maxElements;
	}

	/** Add a new object to the list.
	 *
	 * The object to be added must implement the IComparable element and must
	 * be of the same type as all the other elements in the list.
	 *
	 * @param obj The object to be added.
	 *
	 * @returns The position the object was inserted.
	 */
	public override int Add (object obj)
	{
		for (int pos = Count ; pos > 0 &&
			((IComparable) base[pos-1]).CompareTo (obj) >= 0 ; --pos)
		{
			if (pos < max)
				Set (pos, base[pos-1]);

			pos -= 1;
		}

		if (pos < max) {
			Set (pos, obj);
		} else {
			pos = -1;
		}

		return pos;
	}

	/** Set an element position to an object reference.
	 *
	 * @param idx The index of the element to be set.
	 * @param obj The object reference to be stored at the element.
	 */
	private void Set (int idx, object obj)
	{
		if (idx < Count) {
			base[idx] = obj;
		} else if (idx == Count) {
			base.Add (obj);
		}
	}
}

