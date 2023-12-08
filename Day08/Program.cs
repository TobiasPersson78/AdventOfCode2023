bool useExampleInput = false;
string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

IList<string> lines = File.ReadAllLines(inputFilename);

string leftRightDirections = lines[0];
Dictionary<string, (string Left, string Right)> nodes =
	lines
		.Skip(2)
		.ToDictionary(line => line[..3], line => (Left: line[7..10], Right: line[12..15]));

int numberOfStepsPartA = GetStepsUntilStopCondition("AAA", item => item == "ZZZ");
long numberOfStepsPartB = LeastCommonMultiple(
	nodes
		.Keys
		.Where(item => item[2] == 'A')
		.Select(item => (long)GetStepsUntilStopCondition(item, item => item[2] == 'Z'))
		.ToList());

Console.WriteLine("Day 8A");
Console.WriteLine($"Number of steps: {numberOfStepsPartA}");

Console.WriteLine("Day 8B");
Console.WriteLine($"Number of steps: {numberOfStepsPartB}");

int GetStepsUntilStopCondition(string key, Func<string, bool> stopConditionCheck)
{
	int numberOfSteps;
	for (numberOfSteps = 0; !stopConditionCheck(key); ++numberOfSteps)
	{
		key = leftRightDirections[numberOfSteps % leftRightDirections.Length] == 'L'
			? nodes[key].Left
			: nodes[key].Right;
	}
	return numberOfSteps;
}

long GreatestComonDivisor(long firstNumber, long secondNumber) =>
	secondNumber == 0
		? firstNumber
		: GreatestComonDivisor(secondNumber, firstNumber % secondNumber);

long LeastCommonMultiple(IList<long> numbers) =>
	numbers.Aggregate((acc, curr) => acc * curr / GreatestComonDivisor(acc, curr));
