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

List<int> sumOfNotes =
	new[] { 0, 1 }
		.Select(allowedMistakes =>
			patterns
				.Sum(item =>
					FindHorizontalMirrorIndex(item, allowedMistakes) + 1 +
					100 * (FindHorizontalMirrorIndex(Transpose(item), allowedMistakes) + 1)))
		.ToList();

Console.WriteLine("Day 13A");
Console.WriteLine($"Sum of notes without mistakes: {sumOfNotes[0]}");

Console.WriteLine("Day 13B");
Console.WriteLine($"Sum of notes with one mistake: {sumOfNotes[1]}");

int FindHorizontalMirrorIndex(string[] pattern, int allowedMismatches) =>
	Enumerable
		.Range(0, pattern[0].Length - 1)
		.FirstOrDefault(xIndex =>
			pattern
				.Select(line =>
					Enumerable
						.Range(0, line.Length / 2)
						.Select(offset => (LeftIndex: xIndex - offset, RightIndex: xIndex + 1 + offset))
						.Where(item => item.LeftIndex >= 0 && item.RightIndex < line.Length)
						.Count(item => line[item.LeftIndex] != line[item.RightIndex]))
				.Sum() == allowedMismatches,
			-1);

string[] Transpose(string[] pattern) =>
	Enumerable
		.Range(0, pattern[0].Length)
		.Select(xIndex => string.Join("", pattern.Select(line => line[xIndex])))
		.ToArray();
