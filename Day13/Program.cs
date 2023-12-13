using System.IO.Pipes;

bool useExampleInput = false;
string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<string[]> patterns =
	File
		.ReadAllText(inputFilename)
		.Replace("\r\n", "\n")
		.Trim()
		.Split("\n\n")
		.Select(groupOfLines => groupOfLines.Split('\n'))
		.ToList();

var horizontalMiddle = patterns.Select(pattern => FindHorizontalMiddle(pattern)).ToList();
var verticalMiddle = patterns.Select(pattern => FindVerticalMiddle(pattern)).ToList();

int sumOfNotesPartA = horizontalMiddle.Zip(verticalMiddle).Sum(pair => pair.First + 1 + 100 * (pair.Second + 1));

Console.WriteLine("Day 13A");
Console.WriteLine($"Sum of notes: {sumOfNotesPartA}");

int FindHorizontalMiddle(string[] pattern)
{
	for (int doubleMiddleIndex = 1; doubleMiddleIndex < pattern[0].Length * 2 - 2; ++doubleMiddleIndex)
	{
		int middleIndex = doubleMiddleIndex / 2;
		int offsetToRight = doubleMiddleIndex & 1;

		bool isMirrorLine =
			pattern
				.All(line =>
					Enumerable
						.Range(0, line.Length / 2)
						.All(offset =>
							middleIndex - offset < 0 ||
							middleIndex + offsetToRight + offset >= line.Length ||
							line[middleIndex - offset] == line[middleIndex + offsetToRight + offset]));

		if (isMirrorLine)
		{
			return middleIndex;
		}
	}

	return -1;
}

int FindVerticalMiddle(string[] pattern)
{
	for (int doubleMiddleIndex = 1; doubleMiddleIndex < pattern.Length * 2 - 2; ++doubleMiddleIndex)
	{
		int middleIndex = doubleMiddleIndex / 2;
		int offsetToBottom = (doubleMiddleIndex & 1) & 1;

		bool isMirrorColumn =
			Enumerable
				.Range(0, pattern[0].Length)
				.All(xIndex =>
					Enumerable
						.Range(0, pattern.Length / 2)
						.All(offset =>
							middleIndex - offset < 0 ||
							middleIndex + offsetToBottom + offset >= pattern.Length ||
							pattern[middleIndex - offset][xIndex] == pattern[middleIndex + offsetToBottom + offset][xIndex]));

		if (isMirrorColumn)
		{
			return middleIndex;
		}
	}

	return -1;
}
