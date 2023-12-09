bool useExampleInput = false;
string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<List<int>> listOfSequences =
	File
		.ReadAllLines(inputFilename)
		.Select(line => line.Split(' '))
		.Select(numberStrings =>
			numberStrings
				.Select(int.Parse)
				.ToList())
		.ToList();

int sumOfAllExtrapolatedValuesPartA = listOfSequences.Sum(sequence => ExtrapolateSequence(sequence).AfterEnd);
int sumOfAllExtrapolatedValuesPartB = listOfSequences.Sum(sequence => ExtrapolateSequence(sequence).BeforeStart);

Console.WriteLine("Day 9A");
Console.WriteLine($"Sum of all extrapolated end values: {sumOfAllExtrapolatedValuesPartA}");

Console.WriteLine("Day 9B");
Console.WriteLine($"Sum of all extrapolated start values: {sumOfAllExtrapolatedValuesPartB}");

IEnumerable<int> Difference(ICollection<int> sequence) =>
	sequence
		.Zip(sequence.Skip(1))
		.Select(pair => pair.Second - pair.First);

(int BeforeStart, int AfterEnd) ExtrapolateSequence(ICollection<int> sequence) =>
	sequence.All(item => item == 0)
		? (0, 0)
		: new[] { ExtrapolateSequence(Difference(sequence).ToList()) }
			.Select(nextLine => (sequence.First() - nextLine.BeforeStart, sequence.Last() + nextLine.AfterEnd))
			.First();
