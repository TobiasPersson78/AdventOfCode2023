bool useExampleInput = false;
string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

IList<string> lines = File.ReadAllLines(inputFilename);
List<List<HandAndBid>> listOfHandsAndBidsWithoutAndWithJokers =
	new[] { false, true }
		.Select(jacksAreJokers =>
			lines
				.Select(line => line.Split(' '))
				.Select(handAndBidStrings =>
					new HandAndBid(
						new Hand(
							GetTypeOfHand(handAndBidStrings[0], jacksAreJokers),
							GetCardValues(handAndBidStrings[0], jacksAreJokers)),
						int.Parse(handAndBidStrings[1])))
				.Order(new HandAndBidComparer())
				.ToList())
		.ToList();

List<int> winnings =
	listOfHandsAndBidsWithoutAndWithJokers
		.Select(listOfHandAndBids =>
			listOfHandAndBids
				.Select((handAndBid, index) => handAndBid.Bid * (index + 1))
				.Sum())
		.ToList();

Console.WriteLine("Day 7A");
Console.WriteLine($"Total winnings: {winnings[0]}");

Console.WriteLine("Day 7B");
Console.WriteLine($"Total winnings: {winnings[1]}");

IList<int> GetCardValues(string cards, bool jacksAreJokers) =>
	cards
		.Select(card =>
			card switch
			{
				'A' => 14,
				'K' => 13,
				'Q' => 12,
				'J' => jacksAreJokers ? 1 : 11,
				'T' => 10,
				_ => card - '0'
			})
		.ToList();

TypeOfHand GetTypeOfHand(string cards, bool jacksAreJokers)
{
	List<IGrouping<char, char>> groupsOfCards = cards.GroupBy(card => card).ToList();
	IGrouping<char, char>? groupOfFive = groupsOfCards.FirstOrDefault(group => group.Count() == 5);
	IGrouping<char, char>? groupOfFour = groupsOfCards.FirstOrDefault(group => group.Count() == 4);
	IGrouping<char, char>? groupOfThree = groupsOfCards.FirstOrDefault(group => group.Count() == 3);
	List<IGrouping<char, char>> groupOfPairs = groupsOfCards.Where(group => group.Count() == 2).ToList();
	int numberOfJokers =
		jacksAreJokers
			? cards.Count(card => card == 'J')
			: 0;

	if (groupOfFive != null ||
		(groupOfFour != null && numberOfJokers == 1) ||
		(groupOfThree != null && numberOfJokers == 2) ||
		(groupOfPairs.Any() && numberOfJokers == 3) ||
		numberOfJokers == 4)
	{
		return TypeOfHand.FiveOfAKind;
	}

	if (groupOfFour != null ||
		(groupOfThree != null && numberOfJokers == 1) ||
		(groupOfPairs.Any(pair => pair.Key != 'J') && numberOfJokers == 2) ||
		numberOfJokers == 3)
	{
		return TypeOfHand.FourOfAKind;
	}

	if ((groupOfThree != null && groupOfPairs.Any()) ||
		(groupOfPairs.Count() == 2 && numberOfJokers == 1))
	{
		return TypeOfHand.FullHouse;
	}

	if (groupOfThree != null ||
		(groupOfPairs.Any() && numberOfJokers == 1) ||
		numberOfJokers == 2)
	{
		return TypeOfHand.ThreeOfAKind;
	}

	if (groupOfPairs.Count() == 2 ||
		(groupOfPairs.Any() && numberOfJokers == 1))
	{
		return TypeOfHand.TwoPair;
	}

	if (groupOfPairs.Count() == 1 ||
		numberOfJokers == 1)
	{
		return TypeOfHand.OnePair;
	}

	return TypeOfHand.HighCard;
}

class HandAndBidComparer : IComparer<HandAndBid>
{
	public int Compare(HandAndBid? x, HandAndBid? y) =>
		x!.Hand.TypeOfHand != y!.Hand.TypeOfHand
			? (int)x.Hand.TypeOfHand - (int)y.Hand.TypeOfHand
			: x.Hand.Cards
				.Zip(y.Hand.Cards)
				.Select(pair => pair.First - pair.Second)
				.FirstOrDefault(item => item != 0);
}

enum TypeOfHand
{
	FiveOfAKind = 6,
	FourOfAKind = 5,
	FullHouse = 4,
	ThreeOfAKind = 3,
	TwoPair = 2,
	OnePair = 1,
	HighCard = 0,
}

record Hand(TypeOfHand TypeOfHand, IList<int> Cards);
record HandAndBid(Hand Hand, int Bid);
