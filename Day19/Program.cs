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

long sumOfRatingsNumbers =
	parts
		.Where(part =>
		{
			string currentWorkflow = "in";
			while (currentWorkflow != "A" && currentWorkflow != "R")
			{
				currentWorkflow = workflows[currentWorkflow].First(rule => CheckRule(rule, part)).Target;
			}

			return currentWorkflow == "A";
		})
		.Sum(part => part.X + part.M + part.A + part.S);

long possibleCombinations = EvaluateWorkflow(workflows["in"], new ValueSet(1, 4000, 1, 4000, 1, 4000, 1, 4000), workflows);

Console.WriteLine("Day 19A");
Console.WriteLine($"Sum of rating numbers: {sumOfRatingsNumbers}");

Console.WriteLine("Day 19B");
Console.WriteLine($"Number of allowed combinations: {possibleCombinations}");

(string Name, List<Rule> Rules) ParseWorkflow(string line) =>
	(Regex.Match(line, @"^[^{]+").Value,
		Regex.Match(line, @"{([^}]+)}").Groups[1].Value.Split(',').Select(ParseRule).ToList());

Rule ParseRule(string linePart)
{
	if (Regex.IsMatch(linePart, "^([a-z]+|A|R)$"))
	{
		return new Rule(true, linePart, string.Empty, string.Empty, 0);
	}

	Match match = Regex.Match(linePart, @"^([xmas])([<>])(\d+):([a-z]+|A|R)$");
	return new Rule(
		false,
		match.Groups[4].Value,
		match.Groups[1].Value,
		match.Groups[2].Value,
		int.Parse(match.Groups[3].Value));
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

bool CheckRule(Rule rule, Part part)
{
	if (rule.IsConstant)
	{
		return true;
	}

	Func<Part, int> propertySelector = rule.Property switch
	{
		"x" => (Part item) => item.X,
		"m" => (Part item) => item.M,
		"a" => (Part item) => item.A,
		_ => (Part item) => item.S,
	};

	return rule.Condition switch
	{
		"<" => propertySelector(part) < rule.Value,
		_ => propertySelector(part) > rule.Value,
	};
}

long EvaluateWorkflow(List<Rule> rules, ValueSet valueSet, Dictionary<string, List<Rule>> workflowsLookup) =>
	rules
		.Aggregate(
			(Combinations: 0L, ValueSet: valueSet),
			(acc, curr) =>
				SplitValueSetByRule(curr, acc.ValueSet) switch
				{
					(ValueSet currentValueSet, ValueSet nextValueSet) when IsEmptySet(currentValueSet) =>
						(acc.Combinations, nextValueSet),
					(ValueSet currentValueSet, ValueSet nextValueSet) when  curr.Target == "A" =>
						(acc.Combinations + GetCombinationsForValueSet(currentValueSet), nextValueSet),
					(ValueSet currentValueSet, ValueSet nextValueSet) when curr.Target != "R" =>
						(acc.Combinations + EvaluateWorkflow(workflows[curr.Target], currentValueSet, workflowsLookup), nextValueSet),
					(ValueSet currentValueSet, ValueSet nextValueSet) => (acc.Combinations, nextValueSet)
				})
		.Combinations;

(ValueSet Matched, ValueSet Unmatched) SplitValueSetByRule(Rule rule, ValueSet valueSet) =>
	(rule.Property, rule.Condition) switch
	{
		("x", "<") =>
			(valueSet with { XMax = Math.Min(rule.Value - 1, valueSet.XMax) },
				valueSet with { XMin = Math.Max(rule.Value, valueSet.XMin) }),
		("m", "<") =>
			(valueSet with { MMax = Math.Min(rule.Value - 1, valueSet.MMax) },
				valueSet with { MMin = Math.Max(rule.Value, valueSet.MMin) }),
		("a", "<") =>
			(valueSet with { AMax = Math.Min(rule.Value - 1, valueSet.AMax) },
				valueSet with { AMin = Math.Max(rule.Value, valueSet.AMin) }),
		("s", "<") =>
			(valueSet with { SMax = Math.Min(rule.Value - 1, valueSet.SMax) },
				valueSet with { SMin = Math.Max(rule.Value, valueSet.SMin) }),
		("x", ">") =>
			(valueSet with { XMin = Math.Max(rule.Value + 1, valueSet.XMin) },
				valueSet with { XMax = Math.Min(rule.Value, valueSet.XMax) }),
		("m", ">") =>
			(valueSet with { MMin = Math.Max(rule.Value + 1, valueSet.MMin) },
				valueSet with { MMax = Math.Min(rule.Value, valueSet.MMax) }),
		("a", ">") =>
			(valueSet with { AMin = Math.Max(rule.Value + 1, valueSet.AMin) },
				valueSet with { AMax = Math.Min(rule.Value, valueSet.AMax) }),
		("s", ">") =>
			(valueSet with { SMin = Math.Max(rule.Value + 1, valueSet.SMin) },
				valueSet with { SMax = Math.Min(rule.Value, valueSet.SMax) }),
		(_, _) => // A constant rule always matches fully. There is nothing unmatched.
			(valueSet, valueSet with { XMin = 0, XMax = -1 }),
	};

bool IsEmptySet(ValueSet valueSet) =>
	valueSet.XMin > valueSet.XMax ||
	valueSet.MMin > valueSet.MMax ||
	valueSet.AMin > valueSet.AMax ||
	valueSet.SMin > valueSet.SMax;

long GetCombinationsForValueSet(ValueSet valueSet) =>
	(valueSet.XMax - valueSet.XMin + 1L) *
	(valueSet.MMax - valueSet.MMin + 1L) *
	(valueSet.AMax - valueSet.AMin + 1L) *
	(valueSet.SMax - valueSet.SMin + 1L);

record Part(int X, int M, int A, int S);
record Rule(bool IsConstant, string Target, string Property, string Condition, int Value);
record ValueSet(int XMin, int XMax, int MMin, int MMax, int AMin, int AMax, int SMin, int SMax);
