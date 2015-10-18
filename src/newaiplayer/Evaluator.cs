/** Interface for Evaluators. Evaluators can rate boards and moves.
 */
public interface Evaluator
{
	/** Calculates the static value of a move.
	 *
	 * @param board The actual board.
	 * @param move The node containing the last move.
	 */
	int statVal(int[,] board, Coordinate move, int attacker);
}
