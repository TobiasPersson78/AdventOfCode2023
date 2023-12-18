bool useExampleInput = true;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<(char Direction, int Steps, int Color)> instructions =
	File
		.ReadAllLines(inputFilename)
		.Select(line => line.Split(' '))
		.Select(lineParts =>
			(Direction: lineParts[0][0],
			Steps: int.Parse(lineParts[1]),
			Color: int.Parse(lineParts[2][2..^2], System.Globalization.NumberStyles.HexNumber)))
		.ToList();

const int NotDugColor = 0;
const int NotInteriorColor = -1;

List<List<int>> matrix = new() { new() { 0xFFFFFF } };

TraverseInstructions(matrix, instructions);
FloodFillBorders(matrix);
PrintMatrix(matrix);

int cubicMetersOfLava = matrix.Sum(row => row.Count(cell => cell != NotInteriorColor));

Console.WriteLine("Day 17A");
Console.WriteLine($"Cubic meters of lava: {cubicMetersOfLava}");

void TraverseInstructions(List<List<int>> matrix, List<(char Direction, int Steps, int Color)> instructions)
{
	(int x, int y) = (0, 0);
	foreach ((char direction, int steps, int color) in instructions)
	{
		int remainingSteps = steps;
		while (remainingSteps-- > 0)
		{
			if (direction == 'R' && x == matrix[0].Count - 1)
			{
				ExtendRight(matrix);
			}
			if (direction == 'L' && x == 0)
			{
				ExtendLeft(matrix);
				++x;
			}
			if (direction == 'D' && y == matrix.Count - 1)
			{
				ExtendDown(matrix);
			}
			if (direction == 'U' && y == 0)
			{
				ExtendUp(matrix);
				++y;
			}

			switch (direction)
			{
				case 'R':
					matrix[y][++x] = color;
					break;
				case 'L':
					matrix[y][--x] = color;
					break;
				case 'D':
					matrix[++y][x] = color;
					break;
				case 'U':
					matrix[--y][x] = color;
					break;
			}
		}
	}
}

void FloodFillBorders(List<List<int>> matrix)
{
	IEnumerable<(int X, int Y)> borderPoints =
		(from x in Enumerable.Range(0, matrix[0].Count)
		 select new[] { (X: x, Y: 0), (X: x, Y: matrix.Count - 1) })
		 .Concat(
			from y in Enumerable.Range(0, matrix.Count)
			select new[] { (X: 0, Y: y), (X: matrix[0].Count - 1, Y: y) })
		 .SelectMany(item => item);
	Stack<(int X, int Y)> pointsToTraverse = new(borderPoints);

	while (pointsToTraverse.TryPop(out (int X, int Y) currentPoint))
	{
		if (matrix[currentPoint.Y][currentPoint.X] == NotDugColor)
		{
			matrix[currentPoint.Y][currentPoint.X] = NotInteriorColor;

			foreach ((int X, int Y) neighbor in
				GetNeighbors(currentPoint)
					.Where(item => item.X >= 0 && item.X <= matrix[0].Count-1)
					.Where(item => item.Y >= 0 && item.Y <= matrix.Count - 1))
			{
				pointsToTraverse.Push(neighbor);
			}
		}
	}
}

void ExtendRight(List<List<int>> matrix)
{
	foreach (List<int> row in matrix)
	{
		row.Add(NotDugColor);
	}
}

void ExtendLeft(List<List<int>> matrix)
{
	foreach (List<int> row in matrix)
	{
		row.Insert(0, NotDugColor);
	}
}

void ExtendDown(List<List<int>> matrix) =>
	matrix.Add(Enumerable.Repeat(NotDugColor, matrix[0].Count).ToList());

void ExtendUp(List<List<int>> matrix) =>
	matrix.Insert(0, Enumerable.Repeat(NotDugColor, matrix[0].Count).ToList());

IEnumerable<(int X, int Y)> GetNeighbors((int X, int Y) point) =>
	from deltaX in Enumerable.Range(-1, 3)
	from deltaY in Enumerable.Range(-1, 3)
	where (point.X + deltaX, point.Y + deltaY) != point
	select (point.X + deltaX, point.Y + deltaY);

void PrintMatrix(List<List<int>> matrix)
{
	foreach (List<int> row in matrix)
	{
		Console.WriteLine(
			string.Join(
				"",
				row.Select(item =>
					item != NotDugColor
						? item == NotInteriorColor
							? '#'
							: '.'
						: ' ')));
	}
}
