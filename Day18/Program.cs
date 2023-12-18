using System.Globalization;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

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

List<(long X, long Y)> GetPointsFromInstructions(IEnumerable<(char BindingDirection, int Steps)> directionAndSteps)
{
	(long x, long y) = (0, 0);
	List<(long X, long Y)> points = new() { (0, 0) }; // Start from (0, 0).

	foreach ((char direction, int steps) in directionAndSteps)
	{
		x += direction switch
		{
			'R' or '0' => steps,
			'L' or '2' => -steps,
			_ => 0
		};
		y += direction switch
		{
			'D' or '1' => steps,
			'U' or '3' => -steps,
			_ => 0
		};
		points.Add((x, y));
	}

	return points;
}
