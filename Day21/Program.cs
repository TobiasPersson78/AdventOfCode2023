using System.Collections.Immutable;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

int maxSteps = useExampleInput
	? 6
	: 64;
string[] matrix = File.ReadAllLines(inputFilename);
(int X, int Y) startPosition =
	matrix
		.Select((line, lineIndex) => (X: lineIndex, Y: line.IndexOf('S')))
		.First(position => position.Y >= 0);

ImmutableList<IImmutableSet<(int X, int Y)>> positionsAfterStepsPartA =
	Enumerable
		.Range(0, maxSteps)
		.Aggregate(
			ImmutableList.Create<IImmutableSet<(int X, int Y)>>(new[] { startPosition }.ToImmutableHashSet()),
			(acc, curr) => acc.Add(Traverse(acc.Last(), false)));
ImmutableList<IImmutableSet<(int X, int Y)>> positionsAfterStepsPartB =
	Enumerable
		.Range(0, 3 * matrix.Length - matrix.Length/2)
		.Aggregate(
			ImmutableList.Create < IImmutableSet<(int X, int Y)>>(new[] { startPosition }.ToImmutableHashSet()),
			(acc, curr) => acc.Add(Traverse(acc.Last(), true)));
(int X, int Y) pointA = (matrix.Length / 2, positionsAfterStepsPartB[matrix.Length / 2].Count);
(int X, int Y) pointB = (matrix.Length + matrix.Length / 2, positionsAfterStepsPartB[matrix.Length + matrix.Length / 2].Count);
(int X, int Y) pointC = (2 * matrix.Length + matrix.Length / 2, positionsAfterStepsPartB[2 * matrix.Length + matrix.Length / 2].Count);
long interpolationResult = NewtonInterpolation(pointA, pointB, pointC, 26501365);

Console.WriteLine("Day 21A");
Console.WriteLine($"Number of reachable positions: {positionsAfterStepsPartA.Last().Count}");
Console.WriteLine("Day 21B");
Console.WriteLine($"Number of reachable positions: {interpolationResult}");

IEnumerable<(int X, int Y)> GetNeighbors((int X, int Y) position, bool expandInfinitely) =>
	new[] { (DeltaX: 0, DeltaY: -1), (DeltaX: 1, DeltaY: 0), (DeltaX: 0, DeltaY: 1), (DeltaX: -1, DeltaY: 0) }
		.Select(delta => (X: position.X + delta.DeltaX, Y: position.Y + delta.DeltaY))
		.Where(position =>
		{
			int x = position.X;
			int y = position.Y;

			if (expandInfinitely)
			{
				x = x % matrix[0].Length < 0
					? x % matrix[0].Length + matrix[0].Length
					: x % matrix[0].Length;
				y = y % matrix.Length < 0
					? y % matrix.Length + matrix.Length
					: y % matrix.Length;
			}

			return
				x >= 0 && x < matrix[0].Length &&
				y >= 0 && y < matrix.Length &&
				matrix[y][x] != '#';
		});

IImmutableSet<(int X, int Y)> Traverse(IImmutableSet<(int X, int Y)> positionsAfterPreviousSteps, bool expandInfinitely) =>
	positionsAfterPreviousSteps
		.SelectMany(position => GetNeighbors(position, expandInfinitely))
		.ToImmutableHashSet();

long NewtonInterpolation((int X, int Y) pointA, (int X, int Y) pointB, (int X, int Y) pointC, int n)
{
	(double x0, double y0) = pointA;
	(double x1, double y1) = pointB;
	(double x2, double y2) = pointC;

	double y01 = (y1 - y0) / (x1 - x0);
	double y12 = (y2 - y1) / (x2 - x1);
	double y012 = (y12 - y01) / (x2 - x0);

	return Convert.ToInt64(Math.Round(y0 + y01 * (n - x0) + y012 * (n - x0) * (n - x1)));
}
