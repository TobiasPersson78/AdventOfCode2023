bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

string[] matrix = File.ReadAllLines(inputFilename);

IEnumerable<((int X, int Y) Point, (int DeltaX, int DeltaY) Direction)> startPointAndDirections =
	(from yIndex in Enumerable.Range(0, matrix.Length)
	 select new[] { ((0, yIndex), (1, 0)), ((matrix[0].Length - 1, yIndex), (-1, 0)) }) // Left side going east, right side going west
		.Concat(
			from xIndex in Enumerable.Range(0, matrix[0].Length)
			select new[] { ((xIndex, 0), (0, 1)), ((xIndex, matrix.Length - 1), (0, -1)) }) // Top side going south, bottom side going north
		.SelectMany(item => item);
List<int> numberOfEnergizedCells =
	startPointAndDirections
		.Select(item => GetEnergizedCellsCount(matrix, item.Point, item.Direction))
		.ToList();

Console.WriteLine("Day 16A");
Console.WriteLine($"Number of energized cells: {numberOfEnergizedCells.First()}");

Console.WriteLine("Day 16A");
Console.WriteLine($"Maximum number of energized cells: {numberOfEnergizedCells.Max()}");

int GetEnergizedCellsCount(string[] matrix, (int X, int Y) startPoint, (int DeltaX, int DeltaY) startDirection)
{
	Stack<((int X, int Y) Point, (int DeltaX, int DeltaY) Direction)> pointsAndDirectionsToTraverse = new([(startPoint, startDirection)]);
	Dictionary<(int X, int Y), HashSet<(int DeltaX, int DeltaY)>> visitations = new();

	while (pointsAndDirectionsToTraverse.Any())
	{
		((int X, int Y) point, (int DeltaX, int DeltaY) direction) = pointsAndDirectionsToTraverse.Pop();
		HashSet<(int DeltaX, int DeltaY)> pointVisitations =
			visitations.TryGetValue(point, out HashSet<(int DeltaX, int DeltaY)>? existingSet)
				? existingSet
				: new HashSet<(int DeltaX, int DeltaY)>();

		if ((point.X >= 0 && point.X < matrix[0].Length) &&
			(point.Y >= 0 && point.Y < matrix.Length) &&
			!pointVisitations.Contains(direction))
		{
			pointVisitations.Add(direction);
			visitations[point] = pointVisitations;

			foreach (((int X, int Y) Point, (int DeltaX, int DeltaY) Direction) next in GetNextPositions(matrix, point, direction))
			{
				pointsAndDirectionsToTraverse.Push(next);
			}
		}
	}

	return visitations.Count;
}

IEnumerable<((int X, int Y) Point, (int DeltaX, int DeltaY) Direction)> GetNextPositions(
	string[] matrix,
	(int X, int Y) point,
	(int DeltaX, int DeltaY) direction) =>
	(matrix[point.Y][point.X] switch
	{
		'/' => [(-direction.DeltaY, -direction.DeltaX)],
		'\\' => [(direction.DeltaY, direction.DeltaX)],
		'|' when direction.DeltaX == -1 || direction.DeltaX == 1 => [(0, -1), (0, 1)], // North, South
		'-' when direction.DeltaY == -1 || direction.DeltaY == 1 => [(-1, 0), (1, 0)], // West, East
		_ => new[] { direction } // Empty space, or a splitter from the non-splitting direction
	})
		.Select(direction => ((point.X + direction.DeltaX, point.Y + direction.DeltaY), direction));
