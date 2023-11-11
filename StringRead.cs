public class StringRead
{
	private char[] Stream;

	public int Position;

	public StringRead(string String)
	{
		Stream = String.ToCharArray();
		Position = 0;
	}

	public char Read()
	{
		char result = Stream[Position];
		Position++;
		return result;
	}

	public string ReadUntil(char Limit)
	{
		string text = "";
		while (Peek() != Limit)
		{
			text += Read();
		}
		return text;
	}

	public char Peek()
	{
		return Stream[Position];
	}

	public void Skip(int Count)
	{
		Position += Count;
	}
}
