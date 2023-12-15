bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

string[] instructions =
	File
		.ReadAllText(inputFilename)
		.Trim()
		.Split(',');

int sumOfHashingResults = instructions.Sum(Hash);

List<List<(string Label, string FocalLength)>> boxes =
	Enumerable
		.Range(0, 256)
		.Select(item => new List<(string Label, string FocalLength)>())
		.ToList();

foreach (string[] instructionParts in instructions.Select(item => item.Split('=', '-')))
{
	List<(string Label, string FocalLength)> box = boxes[Hash(instructionParts[0])];
	int labelIndex = box.FindIndex(item => item.Label == instructionParts[0]);

	if (instructionParts[1].Length == 0) // I.e., contained '-'
	{
		if (labelIndex != -1)
		{
			box.RemoveAt(labelIndex);
		}
	}
	else if (labelIndex == -1) // I.e., contained = but there's no matching label in the box
	{
		box.Add((instructionParts[0], instructionParts[1]));
	}
	else // I.e., contained = and there's already a matching label in the box
	{
		box[labelIndex] = (instructionParts[0], instructionParts[1]);
	}
}

int focusingPower =
	boxes
		.Select((box, boxIndex) =>
			box
				.Select((item, listIndex) => (boxIndex + 1) * int.Parse(item.FocalLength) * (listIndex + 1))
				.Sum())
		.Sum();

Console.WriteLine("Day 14A");
Console.WriteLine($"Sum of hashing results: {sumOfHashingResults}");

Console.WriteLine("Day 14B");
Console.WriteLine($"Focusing power: {focusingPower}");

int Hash(string input) =>
	input.Aggregate(0, (acc, curr) => (acc + curr) * 17 % 256);
