using System.Collections.Immutable;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

ImmutableList<Brick> fallingBricks =
	File
		.ReadAllLines(inputFilename)
		.Select(line => line.Split('~', ',').Select(int.Parse).ToList())
		.Select(numbersForLine =>
			new Brick(
				new Position(numbersForLine[0], numbersForLine[1], numbersForLine[2]),
				new Position(numbersForLine[3], numbersForLine[4], numbersForLine[5])))
		.OrderBy(brick => Math.Min(brick.From.Z, brick.To.Z))
		.ToImmutableList();
(_, ImmutableList<Brick> fixedBricks, ImmutableHashSet<Position> fixedPositions) =
	CascadeDown(fallingBricks);
int numberOfDisintegratableBricks =
	fixedBricks
		.Select(brickToRemove => (BrickToRemove: brickToRemove, FixedBricks: fixedBricks, FixedPositions: fixedPositions))
		.AsParallel()
		.Select(item =>
			(item.BrickToRemove,
				FixedBricks: item.FixedBricks.Remove(item.BrickToRemove),
				FixedPositions: item.FixedPositions.Except(GetPositions(item.BrickToRemove))))
		.Count(item => item.FixedBricks.All(brickToTest => !CanMove(brickToTest, item.FixedPositions.Except(GetPositions(brickToTest)))));
int sumOfChainReactionBrickMovement =
	fixedBricks
		.Select(brickToRemove => (BrickToRemove: brickToRemove, FixedBricks: fixedBricks))
		.AsParallel()
		.Select(item => item.FixedBricks.Remove(item.BrickToRemove))
		.Sum(item => CascadeDown(item).NumberOfMovedBricks);

Console.WriteLine("Day 22A");
Console.WriteLine($"Number bricks that can be disintegraded: {numberOfDisintegratableBricks}");
Console.WriteLine("Day 22B");
Console.WriteLine($"Number of bricks moved after chain reaction: {sumOfChainReactionBrickMovement}");

(int NumberOfMovedBricks, ImmutableList<Brick> FixedBricks, ImmutableHashSet<Position> FixedPositions)
	CascadeDown(ImmutableList<Brick> fallingBricks)
{
	int numberOfMovedBricks = 0;
	ImmutableList<Brick> fixedBricks = [];
	ImmutableHashSet<Position> fixedPositions = [];

	foreach (Brick brick in fallingBricks)
	{
		int movedBricksIncrement = 0;
		Brick currentBrick = brick;
		while (CanMove(currentBrick, fixedPositions))
		{
			movedBricksIncrement = 1;
			currentBrick = currentBrick with
			{
				From = currentBrick.From with { Z = currentBrick.From.Z - 1 },
				To = currentBrick.To with { Z = currentBrick.To.Z - 1 }
			};
		}
		numberOfMovedBricks += movedBricksIncrement;
		fixedBricks = fixedBricks.Add(currentBrick);
		fixedPositions = fixedPositions.Union(GetPositions(currentBrick));
	}

	return (numberOfMovedBricks, fixedBricks, fixedPositions);
}

bool CanMove(Brick brick, IEnumerable<Position> potentialSupporters)
{
	List<Position> belowPositions = GetBelowBrick(brick).ToList();

	return belowPositions.All(position => position.Z != 0) && // Implicitly consider the floor as support.
		!potentialSupporters.Intersect(belowPositions).Any();
}

IEnumerable<Position> GetBelowBrick(Brick brick) =>
	brick.From.Z == brick.To.Z
		? from x in Enumerable.Range(Math.Min(brick.From.X, brick.To.X), Math.Abs(brick.From.X - brick.To.X) + 1)
		  from y in Enumerable.Range(Math.Min(brick.From.Y, brick.To.Y), Math.Abs(brick.From.Y - brick.To.Y) + 1)
		  select new Position(x, y, brick.From.Z - 1)
		: [new Position(brick.From.X, brick.From.Y, Math.Min(brick.From.Z, brick.To.Z) - 1)];

IEnumerable<Position> GetPositions(Brick brick) =>
	from x in Enumerable.Range(Math.Min(brick.From.X, brick.To.X), Math.Abs(brick.From.X - brick.To.X) + 1)
	from y in Enumerable.Range(Math.Min(brick.From.Y, brick.To.Y), Math.Abs(brick.From.Y - brick.To.Y) + 1)
	from z in Enumerable.Range(Math.Min(brick.From.Z, brick.To.Z), Math.Abs(brick.From.Z - brick.To.Z) + 1)
	select new Position(x, y, z);

record Position(int X, int Y, int Z);
record Brick(Position From, Position To);
