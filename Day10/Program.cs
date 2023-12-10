bool useExampleInput = false;
string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

string mazeString = File.ReadAllText(inputFilename).Replace("\r\n", "\n");
int mazeWidth = mazeString.IndexOf('\n');
int mazeHeight = mazeString.Length / (mazeWidth + 1);
int startPositionIndex = mazeString.IndexOf('S');
(int X, int Y) startPosition = (startPositionIndex % (mazeWidth + 1), startPositionIndex / (mazeWidth + 1));
(int X, int Y) position = startPosition;
(Direction direction, char sTile) = useExampleInput
	? (Direction.South, '7')
	: (Direction.North, '|');
string[] maze = mazeString.Replace('S', sTile).Split('\n');
bool[,] visitedCells = new bool[mazeWidth, mazeHeight];
Dictionary<(char Pipe, Direction InDirection), Direction> LookupForPipeAndDirection = new()
{
	{ ('|', Direction.North), Direction.North },
	{ ('|', Direction.South), Direction.South },
	{ ('-', Direction.West), Direction.West },
	{ ('-', Direction.East), Direction.East },
	{ ('L', Direction.South), Direction.East },
	{ ('L', Direction.West), Direction.North },
	{ ('J', Direction.East), Direction.North },
	{ ('J', Direction.South), Direction.West },
	{ ('7', Direction.East), Direction.South },
	{ ('7', Direction.North), Direction.West },
	{ ('F', Direction.North), Direction.East },
	{ ('F', Direction.West), Direction.South }
};

int numberOfStepsToLoop ;
for (numberOfStepsToLoop = 0; numberOfStepsToLoop == 0 || position != startPosition; ++numberOfStepsToLoop)
{
	// Move
	position = direction switch
	{
		Direction.North => (position.X, position.Y - 1),
		Direction.East => (position.X + 1, position.Y),
		Direction.South => (position.X, position.Y + 1),
		_ => (position.X - 1, position.Y)
	};
	visitedCells[position.X, position.Y] = true;

	// Check direction change
	direction = LookupForPipeAndDirection[(maze[position.Y][position.X], direction)];
}

int numberOfEnclosedCells = 0;
for(int row = 0; row < mazeHeight; ++row)
{
	int crossingsInRow = 0;
	char lastBend = '.'; // Any not 7 or J.

	// Count the number of empty cells after an odd number of crossings over the row. U shapes
	// does not cross but FJ and L7 shapes does, even if there are horizontal steps between.
	for (int column = 0; column < mazeWidth; ++column)
	{
		if (visitedCells[column, row])
		{
			switch (maze[row][column])
			{
				case 'F':
				case 'L':
					lastBend = maze[row][column];
					break;
				case '|':
				case '7' when lastBend == 'L':
				case 'J' when lastBend == 'F':					
					++crossingsInRow; // Not an U shape, so it crosses over the row.
					break;
			}
		}
		else if ((crossingsInRow & 1) == 1)
		{
			++numberOfEnclosedCells;
		}
	}
}

Console.WriteLine("Day 10A");
Console.WriteLine($"Steps to reach furthest point: {numberOfStepsToLoop/2}");

Console.WriteLine("Day 10B");
Console.WriteLine($"Number of enclosed cells: {numberOfEnclosedCells}");

enum Direction
{
	North,
	East,
	South,
	West
}
