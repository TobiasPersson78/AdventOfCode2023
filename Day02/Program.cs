using System.Text.RegularExpressions;

bool useExampleInput = false;

string inputFilename = useExampleInput
    ? "exampleInput.txt"
    : "input.txt";

IList<Game> games = File
    .ReadAllLines(inputFilename)
    .Select(ParseGame)
    .ToList();

const int MaxRed = 12;
const int MaxGreen = 13;
const int MaxBlue = 14;

int sumOfGameIds =
    games
        .Where(game =>
            game.Subsets.All(subset =>
				subset.Red <= MaxRed &&
				subset.Green <= MaxGreen &&
				subset.Blue <= MaxBlue))
        .Sum(game => game.Id);

Console.WriteLine("Day 2A");
Console.WriteLine($"Sum of game ids: {sumOfGameIds}");

int sumOfGamePowers =
    games
        .Select(game =>
            game.Subsets.Max(subset => subset.Red) *
            game.Subsets.Max(subset => subset.Green) *
            game.Subsets.Max(subset => subset.Blue))
        .Sum();

Console.WriteLine("Day 2B");
Console.WriteLine($"Sum of game powers: {sumOfGamePowers}");

Game ParseGame(string game)
{
    string[] idAndSubsets = game.Split(':');
    return new Game(
        ParseNumber(idAndSubsets[0]),
        idAndSubsets[1].Split(';').Select(ParseSubset).ToList());
}

Subset ParseSubset(string subset)
{
	string[] colorCount = subset.Split(',');
	return new Subset(
		ParseNumber(colorCount.FirstOrDefault(item => item.Contains("red"), "0")),
		ParseNumber(colorCount.FirstOrDefault(item => item.Contains("green"), "0")),
		ParseNumber(colorCount.FirstOrDefault(item => item.Contains("blue"), "0")));
}

int ParseNumber(string input) => int.Parse(Regex.Match(input, @"\d+").Value);

public record Game(int Id, IList<Subset> Subsets);
public record Subset(int Red, int Green, int Blue);
