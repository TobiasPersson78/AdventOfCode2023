using System.Collections.Immutable;

bool useExampleInput = false;
string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<(string Conditions, ImmutableArray<int> Groups)> conditionsAndGroups =
	File
		.ReadAllLines(inputFilename)
		.Select(line => line.Split(' '))
		.Select(splitLine =>
			(Conditions: splitLine[0],
			Groups: splitLine[1].Split(',').Select(int.Parse).ToImmutableArray()))
		.ToList();

long possibleCombinationsPartA =
	conditionsAndGroups
		.Sum(item =>
			GetPossibleCount(
				new Dictionary<(int ConditionsIndex, int CurrentGroupLength, int NumberOfCompletedGroup), long>(),
				item.Conditions + ".", // Ensure the conditions end with a dot
				item.Groups,
				0,
				0,
				0));
var test = string.Join('?', Enumerable.Repeat("abc", 5));
long possibleCombinationsPartB =
	conditionsAndGroups
		.Sum(item =>
			GetPossibleCount(
				new Dictionary<(int ConditionsIndex, int CurrentGroupLength, int NumberOfCompletedGroup), long>(),
				string.Join('?', Enumerable.Repeat(item.Conditions, 5)) + ".", // Ensure the conditions end with a dot
				Repeat(item.Groups, 5).ToImmutableArray(),
				0,
				0,
				0));

Console.WriteLine("Day 12A");
Console.WriteLine($"Sum of possible combinations: {possibleCombinationsPartA}");

Console.WriteLine("Day 12B");
Console.WriteLine($"Sum of possible combinations: {possibleCombinationsPartB}");

IEnumerable<T> Repeat<T>(IEnumerable<T> sequenceToRepeat, int numberOfRepetitions) =>
	numberOfRepetitions > 1
		? sequenceToRepeat.Concat(Repeat(sequenceToRepeat, numberOfRepetitions - 1))
		: sequenceToRepeat;

long GetPossibleCount(
	Dictionary<(int ConditionsIndex, int CurrentGroupLength, int NumberOfCompletedGroup), long> memoization,
	string conditions,
	ImmutableArray<int> groups,
	int conditionsIndex,
	int currentGroupLength,
	int numberOfCompletedGroups)
{
	if (memoization.TryGetValue((conditionsIndex, currentGroupLength, numberOfCompletedGroups), out long memoizedResult))
	{
		return memoizedResult;
	}

	long count = 0;

	if (conditionsIndex == conditions.Length)
	{
		// The end of the line has been reached. Are the number of groups matching?
		count = numberOfCompletedGroups == groups.Length
			? 1
			: 0;
	}
	else if (conditions[conditionsIndex] == '#')
	{
		// We have not yet reached past the end of a # sequence (but we may have started one).
		count = GetPossibleCount(memoization, conditions, groups, conditionsIndex + 1, currentGroupLength + 1, numberOfCompletedGroups);
	}
	else if (conditions[conditionsIndex] == '.' || groups.Length == numberOfCompletedGroups)
	{
		// We have encountered either a dot, or a question mark that must be a dot.

		if (numberOfCompletedGroups < groups.Length && currentGroupLength == groups[numberOfCompletedGroups])
		{
			// The current group has been completed.
			count = GetPossibleCount(memoization, conditions, groups, conditionsIndex + 1, 0, numberOfCompletedGroups + 1);
		}
		else if (currentGroupLength == 0)
		{
			// Continue to find the next group of #.
			count = GetPossibleCount(memoization, conditions, groups, conditionsIndex + 1, 0, numberOfCompletedGroups);
		}
		else
		{
			// Not a valid combination.
			count = 0;
		}
	}
	else
	{
		// We have encountered a question mark. Count the number of possibilities with it being
		// either a dot or a hash sign.

		long hashCount = GetPossibleCount(memoization, conditions, groups, conditionsIndex + 1, currentGroupLength + 1, numberOfCompletedGroups);

		long dotCount;
		if (currentGroupLength == groups[numberOfCompletedGroups])
		{
			// The question mark becoming a dot ends the current sequence at the correct length.
			dotCount = GetPossibleCount(memoization, conditions, groups, conditionsIndex + 1, 0, numberOfCompletedGroups + 1);
		}
		else if (currentGroupLength == 0)
		{
			// The question mark becoming a dot continues the current sequence of dots.
			dotCount = GetPossibleCount(memoization, conditions, groups, conditionsIndex + 1, 0, numberOfCompletedGroups);
		}
		else
		{
			// The question mark becoming a dot would cause the current sequence of # to have an invalid
			// length.
			dotCount = 0;
		}

		count = hashCount + dotCount;
	}

	memoization[(conditionsIndex, currentGroupLength, numberOfCompletedGroups)] = count;
	return count;
}
