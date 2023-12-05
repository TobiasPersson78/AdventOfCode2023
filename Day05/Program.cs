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

IList<NumberAndRange> inputNumbersPartA = seedNumbers.Select(item => new NumberAndRange(item, 1)).ToList();
IList<NumberAndRange> convertedNumbersPartA = ConvertNumbers(inputNumbersPartA);
long lowestLocationNumberPartA = convertedNumbersPartA.Min(item => item.Number);

List<NumberAndRange> inputNumbersPartB =
	Enumerable.Range(0, seedNumbers.Count / 2)
		.Select(index => new NumberAndRange(seedNumbers[2 * index], seedNumbers[2 * index + 1]))
		.ToList();
IList<NumberAndRange> convertedNumbersPartB = ConvertNumbers(inputNumbersPartB);
long lowestLocationNumberPartB = convertedNumbersPartB.Min(item => item.Number);

Console.WriteLine("Day 5A");
Console.WriteLine($"Lowest location number part A: {lowestLocationNumberPartA}");

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
			Func<NumberAndRange, Converter, bool> isBetweenCheckerStart = (NumberAndRange numberToSplit, Converter converter) =>
				IsBetween(
					numberToSplit.Number,
					numberToSplit.Number + numberToSplit.Range - 1,
					converter.SourceStart + 1);
			Func<Converter, long> splitPointProviderStart = (Converter converter) => converter.SourceStart + 1;
			Func<NumberAndRange, Converter, bool> isBetweenCheckerEnd = (NumberAndRange numberToSplit, Converter converter) =>
				IsBetween(
					numberToSplit.Number,
					numberToSplit.Number + numberToSplit.Range - 1,
					converter.SourceStart + converter.Range - 1);
			Func<Converter, long> splitPointProviderEnd = (Converter converter) => converter.SourceStart + converter.Range;

			List<NumberAndRange> splitNumbers = new(numbersToConvert);
			foreach ((Func<NumberAndRange, Converter, bool>? isBetweenChecker, Func<Converter, long>? splitPointProvider) in new[]
					 {
						 (IsBetweenChecker: isBetweenCheckerStart, SplitPointProvider: splitPointProviderStart),
						 (IsBetweenChecker: isBetweenCheckerEnd, SplitPointProvider: splitPointProviderEnd)
					 })
			{
				Stack<NumberAndRange> numbersToSplit = new(splitNumbers.AsEnumerable().Reverse());
				splitNumbers.Clear();
				while (numbersToSplit.TryPop(out NumberAndRange? numberToSplit))
				{
					NumberAndRange localNumberToSplit = numberToSplit;
					Converter? matchingConverter = converters.FirstOrDefault(converter => isBetweenChecker(localNumberToSplit, converter));
					IList<NumberAndRange> splits =
						matchingConverter is not null
							? SplitRange(numberToSplit, splitPointProvider(matchingConverter))
								.ToList()
							: new[] { numberToSplit };
					if (splits.Count <= 1)
					{
						splitNumbers.Add(numberToSplit);
					}
					else
					{
						foreach (NumberAndRange split in splits)
						{
							numbersToSplit.Push(split);
						}
					}
				}
			}

			List<NumberAndRange> convertedNumbers = splitNumbers
				.Select(item =>
				{
					Converter? matchingConverter = converters.FirstOrDefault(converter =>
						IsBetween(
							converter.SourceStart,
							converter.SourceStart + converter.Range - 1,
							item.Number));
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
	long splitPointOffset = splitPoint - numberAndRange.Number;

	if (splitPointOffset > 0 &&
		splitPointOffset <= numberAndRange.Range - 1)
	{
		yield return new NumberAndRange(numberAndRange.Number, splitPointOffset);
		yield return new NumberAndRange(numberAndRange.Number + splitPointOffset, numberAndRange.Range - splitPointOffset);
	}
	else
	{
		yield return numberAndRange;
	}
}

bool IsBetween(long start, long end, long valueToTest) =>
	start <= valueToTest && valueToTest <= end;

record Converter(long DestinationStart, long SourceStart, long Range);
record NumberAndRange(long Number, long Range);
