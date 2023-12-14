using System.Collections.Immutable;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<List<char>> matrix =
	File
		.ReadAllLines (inputFilename)
		.Select(line => line.ToList())
		.ToList();

Tilt(matrix, (0, -1)); // North
long totalLoadPartA = LoadOnNorthSide(matrix);

(long offset, long cycleLength) = GetCycleLength(matrix);
long totalLoadPartB = -1;
for (long remainingCycles = (1_000_000_000 - offset) % cycleLength + 1; remainingCycles > 0; --remainingCycles)
{
	RotateCycle(matrix);
	totalLoadPartB = LoadOnNorthSide(matrix);
}

Console.WriteLine("Day 14A");
Console.WriteLine($"Total load on north side: {totalLoadPartA}");

Console.WriteLine("Day 14B");
Console.WriteLine($"Total load on north side after 1 billion rotations: {totalLoadPartB}");

(long Offset, long CycleLength) GetCycleLength(List<List<char>> matrix)
{
	int rotations = 1;
	Dictionary<string, long> lookupOfMovableRockPositionCycles = new()
	{
		{ string.Join("", GetMovableRocks(matrix).Select(item => item.ToString())), rotations }
	};

	while(true)
	{
		++rotations;
		RotateCycle(matrix);
		string key = string.Join("", GetMovableRocks(matrix).Select(item => item.ToString()));

		if (lookupOfMovableRockPositionCycles.TryGetValue(key, out long previousLocation))
		{
			return (previousLocation, rotations - previousLocation);
		}

		lookupOfMovableRockPositionCycles[key] = rotations;
	}
}

void RotateCycle(List<List<char>> matrix)
{
	Tilt(matrix, (0, -1)); // North
	Tilt(matrix, (-1, 0)); // West
	Tilt(matrix, (0, 1)); // South
	Tilt(matrix, (1, 0)); // East
}

void Tilt(List<List<char>> matrix, (int X, int Y) direction)
{
	ImmutableArray<(int X, int Y)> movableRockPositions = GetMovableRocks(matrix);

	IOrderedEnumerable<(int X, int Y)> orderedMovableRockPositions = direction switch
	{
		(0, -1) => movableRockPositions.OrderBy(item => item.Y),
		(0, 1) => movableRockPositions.OrderByDescending(item => item.Y),
		(-1, 0) => movableRockPositions.OrderBy(item => item.X),
		_ => movableRockPositions.OrderByDescending(item => item.X)
	};

	foreach ((int X, int Y) movableRockPosition in orderedMovableRockPositions)
	{
		(int X, int Y) newPosition = GetEmptyPositionInDirection(matrix, movableRockPosition, direction);
		ExchangeValues(matrix, movableRockPosition, newPosition);
	}
}

void ExchangeValues(List<List<char>> matrix, (int X, int Y) firstPosition, (int X, int Y) secondPosition)
{
	char temp = matrix[firstPosition.Y][firstPosition.X];
	matrix[firstPosition.Y][firstPosition.X] = matrix[secondPosition.Y][secondPosition.X];
	matrix[secondPosition.Y][secondPosition.X] = temp;
}

(int X, int Y) GetEmptyPositionInDirection(List<List<char>> matrix, (int X, int Y) position, (int X, int Y) direction)
{
	(int X, int Y) newPosition;
	for (newPosition = Add(position, direction);
		IsValidPosition(matrix, newPosition) && matrix[newPosition.Y][newPosition.X] == '.';
		newPosition = Add(newPosition, direction))
	{
	}
	return (newPosition.X - direction.X, newPosition.Y - direction.Y);
}

(int X, int Y) Add((int X, int Y) position, (int X, int Y) movement) =>
	(position.X + movement.X, position.Y + movement.Y);

bool IsValidPosition(List<List<char>> matrix, (int X, int Y) position) =>
	position.X >= 0 && position.X < matrix[0].Count &&
	position.Y >= 0 && position.Y < matrix.Count;

long LoadOnNorthSide(List<List<char>> matrix) =>
	GetMovableRocks(matrix).Sum(position => (long)matrix.Count - position.Y);

ImmutableArray<(int X, int Y)> GetMovableRocks(List<List<char>> matrix) =>
	(from xIndex in Enumerable.Range(0, matrix[0].Count)
	 from yIndex in Enumerable.Range(0, matrix.Count)
	 where matrix[yIndex][xIndex] == 'O'
	 select (X: xIndex, Y: yIndex)).ToImmutableArray();
