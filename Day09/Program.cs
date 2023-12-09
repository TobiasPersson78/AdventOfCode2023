bool useExampleInput = false;
string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<List<int>> listOfSequences =
	File
		.ReadAllLines(inputFilename)
		.Select(line =>
			line
				.Split(' ')
				.Select(item => int.Parse(item))
				.ToList())
		.ToList();

int sumOfAllExtrapolatedValuesPartA = listOfSequences.Sum(ExtrapolateSequence);
int sumOfAllExtrapolatedValuesPartB = listOfSequences.Sum(item =>
	ExtrapolateSequence(
		item
			.AsEnumerable()
			.Reverse()
			.ToList()));

Console.WriteLine("Day 9A");
Console.WriteLine($"Sum of all extrapolated end values: {sumOfAllExtrapolatedValuesPartA}");

Console.WriteLine("Day 9B");
Console.WriteLine($"Sum of all extrapolated start values: {sumOfAllExtrapolatedValuesPartB}");

IList<int> Difference(IList<int> sequence) =>
	Enumerable
		.Range(0, sequence.Count - 1)
		.Select(index => sequence[index + 1] - sequence[index])
		.ToList();

int ExtrapolateSequence(IList<int> sequence) =>
	sequence.All(item => item == 0)
		? 0
		: sequence.Last() + ExtrapolateSequence(Difference(sequence));
