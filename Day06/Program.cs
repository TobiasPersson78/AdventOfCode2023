using System.Text.RegularExpressions;

bool useExampleInput = false;
string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

IList<string> lines = File.ReadAllLines(inputFilename);
List<int> timesPartA = Regex.Matches(lines[0], @"\d+").Select(match => int.Parse(match.Value)).ToList();
List<int> distancesPartA = Regex.Matches(lines[1], @"\d+").Select(match => int.Parse(match.Value)).ToList();

int productPartA =
	timesPartA
		.Zip(distancesPartA)
		.Select(timeAndDistance =>
			Enumerable
				.Range(1, timeAndDistance.First - 1)
				.Where(holdTime => holdTime * (timeAndDistance.First - holdTime) > timeAndDistance.Second)
				.Count())
		.Aggregate(
			1,
			(int acc, int curr) => acc * curr);

long timePartB = long.Parse(Regex.Replace(lines[0], @"[^\d]", ""));
long distancePartB = long.Parse(Regex.Replace(lines[1], @"[^\d]", ""));
int countPartB =
	Enumerable
		.Range(1, Convert.ToInt32(timePartB) - 1)
		.Where(holdTime => holdTime * (timePartB - holdTime) > distancePartB)
		.Count();

Console.WriteLine("Day 6A");
Console.WriteLine($"Product of winning possibilities: {productPartA}");

Console.WriteLine("Day 6B");
Console.WriteLine($"Number of winning possibilities: {countPartB}");
