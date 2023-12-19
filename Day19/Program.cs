using System.Text.RegularExpressions;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<string[]> rulesAndMachineParts =
	File
		.ReadAllText(inputFilename)
		.Replace("\r\n", "\n")
		.Trim()
		.Split("\n\n")
		.Select(groupOfLines => groupOfLines.Split('\n'))
		.ToList();
Dictionary<string, List<Rule>> workflows =
	rulesAndMachineParts
		.First()
		.Select(ParseWorkflow)
		.ToDictionary(item => item.Name, item => item.Rules);
List<Part> parts = rulesAndMachineParts.Last().Select(ParsePart).ToList();
List<Part> acceptedParts = new();
List<Part> rejectedParts = new();

foreach (Part part in parts)
{
	string currentWorkflow = "in";
	while (currentWorkflow != "A" && currentWorkflow != "R")
	{
		currentWorkflow = workflows[currentWorkflow].First(rule => rule.Predicate(part)).Target;
	}

	(currentWorkflow == "A" ? acceptedParts : rejectedParts).Add(part);
}

long sumOfRatingsNumbersPartA = acceptedParts.Sum(part => part.X + part.M + part.A + part.S);

Console.WriteLine("Day 19A");
Console.WriteLine($"Sum of rating numbers: {sumOfRatingsNumbersPartA}");

(string Name, List<Rule> Rules) ParseWorkflow(string line) =>
	(Regex.Match(line, @"^[^{]+").Value,
		Regex.Match(line, @"{([^}]+)}").Groups[1].Value.Split(',').Select(ParseRule).ToList());

Rule ParseRule(string linePart)
{
	if (Regex.IsMatch(linePart, "^([a-z]+|A|R)$"))
	{
		return new Rule(_ => true, linePart);
	}

	Match match = Regex.Match(linePart, @"^([xmas])([<>])(\d+):([a-z]+|A|R)$");

	Func<Part, int> propertySelector = match.Groups[1].Value switch
	{
		"x" => (Part item) => item.X,
		"m" => (Part item) => item.M,
		"a" => (Part item) => item.A,
		_ => (Part item) => item.S,
	};
	int comparisonValue = int.Parse(match.Groups[3].Value);
	Func<Part, bool> predicate = match.Groups[2].Value switch
	{
		"<" => (Part item) => propertySelector(item) < comparisonValue,
		_ => (Part item) => propertySelector(item) > comparisonValue,
	};

	return new Rule(predicate, match.Groups[4].Value);
}

Part ParsePart(string line)
{
	List<int> numbers =
		Regex
			.Matches(line, @"\d+")
			.Select(match => int.Parse(match.Value))
			.ToList();
	return new Part(numbers[0], numbers[1], numbers[2], numbers[3]);
}

record Part(int X, int M, int A, int S);
record Rule(Func<Part, bool> Predicate, string Target);
