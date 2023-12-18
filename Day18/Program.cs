using System.Collections.Immutable;
using System.Globalization;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

Dictionary<char, int> horizontalLookup = new() { ['R'] = 1, ['L'] = -1, ['0'] = 1, ['2'] = -1 };
Dictionary<char, int> verticalLookup = new() { ['D'] = 1, ['U'] = -1, ['1'] = 1, ['3'] = -1 };
List<((char Direction, int Steps) PartA, (char Direction, int Steps) PartB)> instructions =
	File
		.ReadAllLines(inputFilename)
		.Select(line => line.Split(' '))
		.Select(lineParts =>
			(PartA: (lineParts[0][0], int.Parse(lineParts[1])),
				PartB: (lineParts[2][^2], int.Parse(lineParts[2][2..^2], NumberStyles.HexNumber))))
		.ToList();
long areaPartA =
	CalculateArea(
		GetPointsFromInstructions(
			instructions.Select(item => item.PartA)));
long areaPartB =
	CalculateArea(
		GetPointsFromInstructions(
			instructions.Select(item => item.PartB)));

Console.WriteLine("Day 18A");
Console.WriteLine($"Cubic meters of lava: {areaPartA}");

Console.WriteLine("Day 18B");
Console.WriteLine($"Cubic meters of lava: {areaPartB}");

long CalculateArea(IList<(long X, long Y)> polygon)
{
	IList<((long X, long Y) First, (long X, long Y) Second)> pointPairs =
		polygon
			.Zip(polygon.Skip(1))
			.ToList();

	// Shoelace algorithm & Pick's theorem
	long area = Math.Abs(pointPairs.Sum(pair =>
		(pair.Second.Y + pair.First.Y) * (pair.Second.X - pair.First.X)) / 2);
	long boundaryLength = pointPairs.Sum(pair =>
		Math.Abs(pair.Second.X - pair.First.X + pair.Second.Y - pair.First.Y));
	long interiorPoints = area - boundaryLength / 2 + 1;

	// Area with perimeter
	return interiorPoints + boundaryLength;
}

IList<(long X, long Y)> GetPointsFromInstructions(IEnumerable<(char Direction, int Steps)> directionAndSteps) =>
	directionAndSteps
		.Aggregate(
			ImmutableList.Create<(long X, long Y)>((0L, 0L)),
			(acc, curr) =>
				acc.Add(
					(X: acc[^1].X + horizontalLookup.GetValueOrDefault(curr.Direction, 0) * curr.Steps,
					Y: acc[^1].Y + verticalLookup.GetValueOrDefault(curr.Direction, 0) * curr.Steps)));
