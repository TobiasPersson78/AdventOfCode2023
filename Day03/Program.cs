using System.Text.RegularExpressions;

bool useExampleInput = false;
string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

IList<string> lines = File.ReadAllLines(inputFilename);
List<NumberPosition> numberPositions = lines
	.SelectMany((line, index) => Regex
		.Matches(line, @"\d+")
		.Select(match => new NumberPosition(
			index,
			match.Index,
			match.Index + match.Length - 1,
			int.Parse(match.Value))))
	.ToList();
List<PartAndNumbers> partsNumbers = lines
	.SelectMany((line, index) => Regex
		.Matches(line, @"[^\d.]")
		.Select(match => new PartAndNumbers(
			match.Value[0],
			numberPositions
				.Where(item =>
					item.Row >= index - 1 &&
					item.Row <= index + 1 &&
					item.ColumnStart <= match.Index + 1 &&
					item.ColumnEnd >= match.Index - 1)
				.ToList())))
	.ToList();
int sumOfPartNumbers = partsNumbers
	.Sum(partAndNumbers =>
		partAndNumbers.Values.Sum(item => item.Value));
int sumOfGearRatios = partsNumbers
	.Where(item => item.PartId == '*' && item.Values.Count == 2)
	.Sum(item => item.Values[0].Value * item.Values[1].Value);

Console.WriteLine("Day 3A");
Console.WriteLine($"Sum of matching part numbers: {sumOfPartNumbers}");

Console.WriteLine("Day 3B");
Console.WriteLine($"Sum of gear ratios: {sumOfGearRatios}");

record NumberPosition(int Row, int ColumnStart, int ColumnEnd, int Value);
record PartAndNumbers(char PartId, List<NumberPosition> Values);
