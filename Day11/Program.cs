bool useExampleInput = false;
string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<string> matrix =
	File
		.ReadAllLines(inputFilename)
		.ToList();
List<(int X, int Y)> galaxyPositions =
	(from x in Enumerable.Range(0, matrix[0].Length)
	 from y in Enumerable.Range(0, matrix.Count)
	 where matrix[y][x] == '#'
	 select (X: x, Y: y))
		.ToList();
List<int> emptyColumnIndices =
	Enumerable
		.Range(0, matrix[0].Length)
		.Where(xIndex => !galaxyPositions.Any(position => position.X == xIndex))
		.ToList();
List<int> emptyRowIndices =
	Enumerable
		.Range(0, matrix.Count)
		.Where(yIndex => !galaxyPositions.Any(position => position.Y == yIndex))
		.ToList();

long GetSumOfShortestPaths(long emptyCellFactor) =>
	galaxyPositions
		.SelectMany((firstGalaxy, index) =>
			galaxyPositions
				.Skip(index + 1)
				.Select(secondGalaxy => (First: firstGalaxy, Second: secondGalaxy)))
		.Sum(pairOfGalaxies =>
		{
			int minX = Math.Min(pairOfGalaxies.First.X, pairOfGalaxies.Second.X);
			int minY = Math.Min(pairOfGalaxies.First.Y, pairOfGalaxies.Second.Y);
			int maxX = Math.Max(pairOfGalaxies.First.X, pairOfGalaxies.Second.X);
			int maxY = Math.Max(pairOfGalaxies.First.Y, pairOfGalaxies.Second.Y);

			long pathBetweenTwoGalaxies = maxX - minX + maxY - minY;
			int numberOfEmptyColumnsBetweenTwoGalaxies = emptyColumnIndices.Count(index => index > minX && index < maxX);
			int numberOfEmptyRowsBetweenTwoGalaxies = emptyRowIndices.Count(index => index > minY && index < maxY);
			pathBetweenTwoGalaxies += numberOfEmptyColumnsBetweenTwoGalaxies * (emptyCellFactor - 1);
			pathBetweenTwoGalaxies += numberOfEmptyRowsBetweenTwoGalaxies * (emptyCellFactor - 1);

			return pathBetweenTwoGalaxies;
		});

long sumOfShortestPathsPartA = GetSumOfShortestPaths(2);
long sumOfShortestPathsPartB = GetSumOfShortestPaths(1_000_000);

Console.WriteLine("Day 11A");
Console.WriteLine($"Sum of shortest paths: {sumOfShortestPathsPartA}");

Console.WriteLine("Day 11B");
Console.WriteLine($"Sum of shortest paths: {sumOfShortestPathsPartB}");
