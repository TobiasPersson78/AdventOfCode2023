using System.Text.RegularExpressions;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";
decimal minXY = useExampleInput
	? 7
	: 200_000_000_000_000m;
decimal maxXY = useExampleInput
	? 27
	: 400_000_000_000_000m;

List<Ray3D> hails =
	File
		.ReadAllLines(inputFilename)
		.Select(GetNumbersFromLine)
		.Select(numbers => new Ray3D(numbers[0], numbers[1], numbers[2], numbers[3], numbers[4], numbers[5]))
		.ToList();

int numberOfIntersections =
	hails
		.SelectMany((firstHail, index) =>
			hails
				.Skip(index + 1)
				.Select(secondHail => (First: firstHail, Second: secondHail)))
		.Select(pair => (First: FlattenHorizontal(pair.First), Second: FlattenHorizontal(pair.Second)))
		.Select(pair => TryGetIntersection(pair.First, pair.Second))
		.Where(point => point is not null)
		.Select(point => point!.Value)
		.Count(point =>
			point.X >= minXY && point.X <= maxXY &&
			point.Y >= minXY && point.Y <= maxXY);

(decimal X, decimal Y) rockPositionXY = GetRockPosition(hails.Select(FlattenHorizontal).ToList());
(decimal X, decimal Y) rockPositionXZ = GetRockPosition(hails.Select(FlattenVertical).ToList());
long sumOfRockCoordinates = (long)Math.Round(rockPositionXY.X + rockPositionXY.Y + rockPositionXZ.Y);

Console.WriteLine("Day 24A");
Console.WriteLine($"Number of intersections in the test area: {numberOfIntersections}");
Console.WriteLine("Day 24B");
Console.WriteLine($"Sum of rock coordinates: {sumOfRockCoordinates}");

(decimal X, decimal Y) GetRockPosition(List<Ray2D> rays) =>
	SpiralEnumeration()
		.Select(rockVelocity =>
		{
			(decimal X, decimal Y)? rockPosition = null;

			for (int i = 0; rockPosition is null; ++i)
			{
				rockPosition = TryGetIntersection(
					ChangeVelocity(rays[i], rockVelocity),
					ChangeVelocity(rays[i + 1], rockVelocity));
			}

			bool allRaysHitTheRock =
				rays
					.Select(ray => ChangeVelocity(ray, rockVelocity))
					.All(ray => RayPassesPoint(ray, rockPosition.Value));
			return allRaysHitTheRock
					? rockPosition
					: null;
		})
		.First(item => item is not null)
		!.Value;

Ray2D ChangeVelocity(Ray2D ray, (int DeltaX, int DeltaY) velocityChange) =>
	ray with { DeltaX = ray.DeltaX + velocityChange.DeltaX, DeltaY = ray.DeltaY + velocityChange.DeltaY };

bool RayPassesPoint(Ray2D ray, (decimal X, decimal Y) point) =>
	Math.Abs((point.X - ray.X) * ray.DeltaY - (point.Y - ray.Y) * ray.DeltaX) < 0.00001m;

(decimal X, decimal Y)? TryGetIntersection(Ray2D ray1, Ray2D ray2)
{
	decimal deltaX = ray2.X - ray1.X;
	decimal deltaY = ray2.Y - ray1.Y;
	decimal determinant = ray2.DeltaX * ray1.DeltaY - ray2.DeltaY * ray1.DeltaX;

	if (determinant == 0)
	{
		return null;
	}

	decimal u = (deltaY * ray2.DeltaX - deltaX * ray2.DeltaY) / determinant;
	decimal v = (deltaY * ray1.DeltaX - deltaX * ray1.DeltaY) / determinant;

	if (u < 0 || v < 0)
	{
		return null;
	}

	return (ray1.X + ray1.DeltaX * u, ray1.Y + ray1.DeltaY * u);
}

IEnumerable<(int X, int Y)> SpiralEnumeration()
{
	yield return (0, 0);
	for (int distance = 1; ; ++distance)
	{
		foreach (int index in Enumerable.Range(-distance, 2*distance))
		{
			yield return (index, distance);
			yield return (distance, -index);
			yield return (-index, -distance);
			yield return (-distance, index);
		}
	}
}

IList<decimal> GetNumbersFromLine(string line) => Regex.Matches(line, @"(-?\d+)").Select(match => decimal.Parse(match.Value)).ToList();

Ray2D FlattenHorizontal(Ray3D ray) => new(ray.X, ray.Y, ray.DeltaX, ray.DeltaY);

Ray2D FlattenVertical(Ray3D ray) => new(ray.X, ray.Z, ray.DeltaX, ray.DeltaZ);

record Ray2D(decimal X, decimal Y, decimal DeltaX, decimal DeltaY);
record Ray3D(decimal X, decimal Y, decimal Z, decimal DeltaX, decimal DeltaY, decimal DeltaZ);
