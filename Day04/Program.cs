using System.Text.RegularExpressions;

bool useExampleInput = false;
string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

IList<int> matchesPerCard = File
	.ReadAllLines(inputFilename)
	.Select(line => line.Split(':', '|'))
	.Select(indexWinnersAndActualParts =>
		Regex
			.Matches(indexWinnersAndActualParts[1], @"\d+")
			.Select(item => item.Value)
			.Intersect(
				Regex
					.Matches(indexWinnersAndActualParts[2], @"\d+")
					.Select(item => item.Value))
			.Count())
	.ToList();
int sumOfScratchCardPoints = matchesPerCard
	.Where(item => item != 0)
	.Sum(item => 1 << (item - 1));

IList<int> numberOfCards = Enumerable.Repeat(1, matchesPerCard.Count).ToList();
for (int currentCardIndex = 0; currentCardIndex < matchesPerCard.Count; ++currentCardIndex)
{
	for (int wonCardIndex = currentCardIndex + 1;
		wonCardIndex < currentCardIndex + 1 + matchesPerCard[currentCardIndex];
		++wonCardIndex)
	{
		numberOfCards[wonCardIndex] += numberOfCards[currentCardIndex];
	}
}

Console.WriteLine("Day 4A");
Console.WriteLine($"Sum of scratch card points: {sumOfScratchCardPoints}");

Console.WriteLine("Day 4B");
Console.WriteLine($"Number of cards: {numberOfCards.Sum()}");
