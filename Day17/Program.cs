bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

string[] matrix = File.ReadAllLines(inputFilename);

const int MaxForwardStepsPartA = 3;
const int MinForwardStepsPartB = 4;
const int MaxForwardStepsPartB = 10;
int minimumHeatLossPartA = GetMinimumHeatLoss(matrix, false);
int minimumHeatLossPartB = GetMinimumHeatLoss(matrix, true);

Console.WriteLine("Day 17A");
Console.WriteLine($"Minimum heat loss: {minimumHeatLossPartA}");

Console.WriteLine("Day 17B");
Console.WriteLine($"Minimum heat loss: {minimumHeatLossPartB}");

int GetMinimumHeatLoss(string[] matrix, bool isPartB)
{
	PriorityQueue<NodeState, int> statesToTraverse = new([
		(new(0, 0, 1, 0, 0), 0), // Start with upper left position, going right
		(new(0, 0, 0, 1, 0), 0)]); // Start with upper left position, going down
	(int X, int Y) endPoint = (matrix[0].Length - 1, matrix.Length - 1); // Lower right position is the end target.
	HashSet<NodeState> visitedStates = new();
	int maxHeatLossToEndPoint = int.MaxValue;

	while (statesToTraverse.TryDequeue(out NodeState? current, out int totalHeatLoss))
	{
		if (!visitedStates.Contains(current))
		{
			visitedStates.Add(current);

			if ((current.X, current.Y) == endPoint &&
				(!isPartB || (current.ForwardSteps >= MinForwardStepsPartB)))
			{
				maxHeatLossToEndPoint = totalHeatLoss;
				break;
			}

			foreach (NodeState next in
				GetNextStates(current, isPartB)
					.Where(item => item.X >= 0 && item.X < matrix[0].Length)
					.Where(item => item.Y >= 0 && item.Y < matrix.Length))
			{
				statesToTraverse.Enqueue(next, totalHeatLoss + matrix[next.Y][next.X] - '0');
			}
		}
	}

	return maxHeatLossToEndPoint;
}

IEnumerable<NodeState> GetNextStates(NodeState state, bool isPartB)
{
	if (state.ForwardSteps < (isPartB ? MaxForwardStepsPartB : MaxForwardStepsPartA))
	{
		yield return new(state.X + state.DeltaX, state.Y + state.DeltaY, state.DeltaX, state.DeltaY, state.ForwardSteps + 1);
	}

	if (!isPartB || state.ForwardSteps >= MinForwardStepsPartB)
	{
		yield return new(state.X + state.DeltaY, state.Y + state.DeltaX, state.DeltaY, state.DeltaX, 1); // Left turn
		yield return new(state.X - state.DeltaY, state.Y - state.DeltaX, -state.DeltaY, -state.DeltaX, 1); // Right turn
	}
}

record NodeState(int X, int Y, int DeltaX, int DeltaY, int ForwardSteps);
