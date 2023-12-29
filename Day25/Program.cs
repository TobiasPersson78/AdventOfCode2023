using System.Text.RegularExpressions;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

Dictionary<string, List<string>> graphConnections =
	File
		.ReadAllLines(inputFilename)
		.Select(line => Regex.Matches(line, @"[^\s:]+").Select(match => match.Value).ToList())
		.Aggregate(
			new Dictionary<string, List<string>>(),
			(acc, curr) =>
			{
				acc.TryAdd(curr[0], []);
				foreach (string target in curr[1..])
				{
					acc.TryAdd(target, []);
					acc[curr[0]].Add(target);
					acc[target].Add(curr[0]);
				}

				return acc;
			});

Dictionary<string, (HashSet<string> InternalNodes, List<string> Edges)> superNodeGraphConnections = new();
do
{
	superNodeGraphConnections = KargersAlgorithm(graphConnections);
} while (superNodeGraphConnections.First().Value.Edges.Count != 3);

int groupSizeProduct = superNodeGraphConnections.Aggregate(1, (acc, curr) => curr.Value.InternalNodes.Count * acc);
Console.WriteLine("Day 25A");
Console.WriteLine($"Product of group sizes: {groupSizeProduct}");

Dictionary<string, (HashSet<string> InternalNodes, List<string> Edges)> KargersAlgorithm(
	Dictionary<string, List<string>> graph)
{
	Dictionary<string, (HashSet<string> InternalNodes, List<string> Edges)> graphToCut =
		graph.ToDictionary(
			item => item.Key,
			item => (new HashSet<string>([item.Key]), item.Value.ToList()));
	Random random = new();

	while (graphToCut.Count > 2)
	{
		string firstNode = graphToCut.Keys.ElementAt(random.Next(graphToCut.Count));
		List<string> firstNodeEdges = graphToCut[firstNode].Edges;

		string secondNode = firstNodeEdges[random.Next(firstNodeEdges.Count)];
		foreach (var secondNodeEdge in graphToCut[secondNode].Edges)
		{
			graphToCut[secondNodeEdge].Edges.Remove(secondNode);
			if (secondNodeEdge != firstNode)
			{
				firstNodeEdges.Add(secondNodeEdge);
				graphToCut[secondNodeEdge].Edges.Add(firstNode);
			}
		}
		graphToCut[firstNode].InternalNodes.UnionWith(graphToCut[secondNode].InternalNodes);
		firstNodeEdges.Remove(secondNode);
		graphToCut.Remove(secondNode);
	}

	return graphToCut;
}
