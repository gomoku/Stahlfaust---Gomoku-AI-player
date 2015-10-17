public interface Player
{
	void RegOppMove(Coordinate move);
	Coordinate GetMove();
	int AskSize();
	string AskColor();
	void SetSize(int size);
	void SetColor(string color);
}
