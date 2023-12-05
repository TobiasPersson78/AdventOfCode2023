using System.Text.RegularExpressions;

bool useExampleInput = false;
string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

IList<string> lines = File
	.ReadAllLines(inputFilename)
	.Where(item => !string.IsNullOrEmpty(item))
	.Append(":")
	.ToList();

IList<long> seedNumbers = GetNumbersFromLine(lines[0]);

//IList<NumberAndRange> inputNumbersPartA = seedNumbers.Select(item => new NumberAndRange(item, 1)).ToList();
//IList<NumberAndRange> convertedNumbersPartA = ConvertNumbers(inputNumbersPartA);
//long lowestLocationNumberPartA = convertedNumbersPartA.Min(item => item.Number);

List<NumberAndRange> inputNumbersPartB = new();
for (int i = 0; i < seedNumbers.Count; i += 2)
{
	inputNumbersPartB.Add(new(seedNumbers[i], seedNumbers[i+1]));
}
IList<NumberAndRange> convertedNumbersPartB = ConvertNumbers(inputNumbersPartB);
long lowestLocationNumberPartB = convertedNumbersPartB.Min(item => item.Number);

//Console.WriteLine("Day 5A");
//Console.WriteLine($"Lowest location number part A: {lowestLocationNumberPartA}");

Console.WriteLine("Day 5B");
Console.WriteLine($"Lowest location number part B: {lowestLocationNumberPartB}");

IList<long> GetNumbersFromLine(string line) => Regex.Matches(line, @"\d+").Select(match => long.Parse(match.Value)).ToList();

IList<NumberAndRange> ConvertNumbers(IList<NumberAndRange> numbersToConvert)
{
	List<Converter> converters = new();

	foreach (string line in lines.Skip(2))
	{
		IList<long> numbersOnLine = GetNumbersFromLine(line);

		if (numbersOnLine.Any())
		{
			converters.Add(new(numbersOnLine[0], numbersOnLine[1], numbersOnLine[2]));
		}
		else
		{
			var splitNumbersStart = numbersToConvert
				.SelectMany(item =>
				{
					Converter? matchingConverter = converters.FirstOrDefault(converter =>
						IsBetween(converter.SourceStart, converter.Range, item.Number));

					return matchingConverter is not null
						? SplitRange(item, matchingConverter.SourceStart + matchingConverter.Range)
						: new[] { item };
				}).ToList();
			var splitNumbersEnd = splitNumbersStart
				.SelectMany(item =>
				{
					Converter? matchingConverter = converters.FirstOrDefault(converter =>
						IsBetween(converter.SourceStart, converter.Range, item.Number + item.Range));

					return matchingConverter is not null
						? SplitRange(item, matchingConverter.SourceStart + matchingConverter.Range)
						: new[] { item };
				}).ToList();
			var convertedNumbers = splitNumbersEnd
				.Select(item =>
				{
					Converter? matchingConverter = converters.FirstOrDefault(converter =>
						IsBetween(converter.SourceStart, converter.Range, item.Number));
					return matchingConverter is not null
						? new NumberAndRange(
							matchingConverter.DestinationStart + (item.Number - matchingConverter.SourceStart),
							item.Range)
						: item;
				})
				.ToList();
			numbersToConvert = convertedNumbers;
			converters.Clear();
		}
	}

	return numbersToConvert;
}

IEnumerable<NumberAndRange> SplitRange(NumberAndRange numberAndRange, long splitPoint)
{
	if (numberAndRange.Range <= 1)
	{
		yield return numberAndRange;
		yield break;
	}

	long splitPointOffset = splitPoint - numberAndRange.Number;

	if (splitPointOffset >= 0 &&
		splitPointOffset < numberAndRange.Range)
	{
		yield return new NumberAndRange(numberAndRange.Number, splitPointOffset);
		yield return new NumberAndRange(numberAndRange.Number + splitPointOffset, numberAndRange.Range - splitPointOffset);
	}
	else
	{
		yield return numberAndRange;
	}
}

bool IsBetween(long start, long range, long valueToTest) =>
	start <= valueToTest && valueToTest < start + range;

record Converter(long DestinationStart, long SourceStart, long Range);
record NumberAndRange(long Number, long Range);
