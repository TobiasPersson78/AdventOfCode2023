using System.Collections.Immutable;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

string[] matrix = File.ReadAllLines(inputFilename);
Position startPosition = new(matrix.First().IndexOf('.'), 0);
Position endPosition = new(matrix.Last().IndexOf('.'), matrix.Length - 1);

IEnumerable<Func<Position, IEnumerable<Position>>> getNeighborFunctions = [GetNeighborsPartA, GetNeighborsPartB];
List<int> maxStepsUsingNeighborFunction =
	getNeighborFunctions
		.Select(FindMaxStepsUsingNeighborFunction)
		.ToList();

Console.WriteLine("Day 23A");
Console.WriteLine($"Steps for the longest hike: {maxStepsUsingNeighborFunction[0]}");
Console.WriteLine("Day 23B");
Console.WriteLine($"Steps for the longest hike: {maxStepsUsingNeighborFunction[1]}");

int FindMaxStepsUsingNeighborFunction(Func<Position, IEnumerable<Position>> getNeighborFunction)
{
	Dictionary<Position, List<(Position Destination, int Steps)>> nodes = new() { [endPosition] = [] }; // No need to check ways back from the end.
	Queue<Position> queue = new([startPosition]);

	while (queue.TryDequeue(out Position? position))
	{
		if (!nodes.ContainsKey(position))
		{
			List<(Position Destination, int Steps)> destinationsAndCosts = new();
			foreach (Position neighbor in getNeighborFunction(position))
			{
				destinationsAndCosts.Add((neighbor, 1));
				queue.Enqueue(neighbor);
			}
			nodes[position] = destinationsAndCosts;
		}
	}

	foreach (KeyValuePair<Position, List<(Position Destination, int Steps)>> middleNode
		in nodes.Where(keyAndValue => keyAndValue.Value.Count == 2).ToList())
	{
		(Position Destination, int Steps) middleToFirstNeighborAndSteps = middleNode.Value[0];
		(Position Destination, int Steps) middleToSecondNeighborAndSteps = middleNode.Value[1];

		List<(Position Destination, int Steps)> firstNeighborDestinationsAndSteps =
			nodes[middleToFirstNeighborAndSteps.Destination];
		(Position Destination, int Steps) firstNeighborToMiddleEntry =
			firstNeighborDestinationsAndSteps.FirstOrDefault(item => item.Destination == middleNode.Key);

		if (firstNeighborToMiddleEntry != default)
		{
			firstNeighborDestinationsAndSteps.Remove(firstNeighborToMiddleEntry);
			firstNeighborDestinationsAndSteps.Add(
				(middleToSecondNeighborAndSteps.Destination,
					firstNeighborToMiddleEntry.Steps + middleToSecondNeighborAndSteps.Steps));
		}

		List<(Position Destination, int Steps)> secondNeighborDestinationsAndSteps =
			nodes[middleToSecondNeighborAndSteps.Destination];
		(Position Destination, int Steps) secondNeighborToMiddleEntry =
			secondNeighborDestinationsAndSteps.FirstOrDefault(item => item.Destination == middleNode.Key);

		if (secondNeighborToMiddleEntry != default)
		{
			secondNeighborDestinationsAndSteps.Remove(secondNeighborToMiddleEntry);
			secondNeighborDestinationsAndSteps.Add(
				(middleToFirstNeighborAndSteps.Destination,
					secondNeighborToMiddleEntry.Steps + middleToFirstNeighborAndSteps.Steps));
		}
	}

	return GetMaxStepsToEnd(nodes, startPosition, []);
}

int GetMaxStepsToEnd(
	Dictionary<Position, List<(Position Destination, int Steps)>> nodes,
	Position position,
	ImmutableHashSet<Position> visited)
{
	List<(Position Destination, int Steps)> notVisitedNeighborsWithSteps =
		nodes[position]
			.Where(item => !visited.Contains(item.Destination))
			.ToList();
	return
		notVisitedNeighborsWithSteps.Any()
			? notVisitedNeighborsWithSteps
				.Select(neighborWithSteps =>
					(StepsToNeighBor: neighborWithSteps.Steps,
						StepsFromNeighborToEnd: neighborWithSteps.Destination == endPosition
							? 0
							: GetMaxStepsToEnd(nodes, neighborWithSteps.Destination, visited.Add(position))))
				.Max(pairOfSteps =>
					pairOfSteps.StepsFromNeighborToEnd == int.MinValue
						? int.MinValue
						: pairOfSteps.StepsToNeighBor + pairOfSteps.StepsFromNeighborToEnd)
			: int.MinValue;
}

IEnumerable<Position> GetNeighborsPartA(Position position) =>
	(matrix[position.Y][position.X] switch
	{
		'^' => new[] { new Position(position.X, position.Y - 1) },
		'>' => new[] { new Position(position.X + 1, position.Y) },
		'v' => new[] { new Position(position.X, position.Y + 1) },
		'<' => new[] { new Position(position.X - 1, position.Y) },
		_ => new[]
		{
			new Position(position.X, position.Y - 1),
			new Position(position.X + 1, position.Y),
			new Position(position.X, position.Y + 1),
			new Position(position.X - 1, position.Y)
		}
	})
		.Where(item =>
			item.X >= 0 && item.X <= matrix[0].Length &&
			item.Y >= 0 && item.Y <= matrix.Length &&
			matrix[item.Y][item.X] != '#');

IEnumerable<Position> GetNeighborsPartB(Position position) =>
	(new[]
	{
		new Position(position.X, position.Y - 1),
		new Position(position.X + 1, position.Y),
		new Position(position.X, position.Y + 1),
		new Position(position.X - 1, position.Y)
	})
		.Where(item =>
			item.X >= 0 && item.X <= matrix[0].Length &&
			item.Y >= 0 && item.Y <= matrix.Length &&
			matrix[item.Y][item.X] != '#');

record Position(int X, int Y);
