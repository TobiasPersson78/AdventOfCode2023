bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

Dictionary<string, BaseModule> moduleLookup =
	File
		.ReadAllLines(inputFilename)
		.Select(line => line.Split(" -> "))
		.Select(lineParts => 
			(BaseModule)(lineParts[0][0] switch
			{
				'%' => new FlipFlop(lineParts[0][1..], lineParts[1].Split(", ")),
				'&' => new Conjunction(lineParts[0][1..], lineParts[1].Split(", ")),
				_ => new Broadcaster(lineParts[0], lineParts[1].Split(", "))
			}))
		.ToDictionary(item => item.Name, item => item);

foreach (BaseModule module in moduleLookup.Values)
{
	foreach (string output in module.Outputs.Where(moduleLookup.ContainsKey))
	{
		moduleLookup[output].Inputs.Add(module.Name);
	}
}

(long LowPulses, long HighPulses) pulseCount = (0, 0);

for (int i = 0; i < 1000; ++i)
{
	(long lowPulses, long highPulses, IList<string> _) = ExecuteButtonPress();
	pulseCount = (pulseCount.LowPulses + lowPulses, pulseCount.HighPulses + highPulses);
}

foreach (BaseModule module in moduleLookup.Values)
{
	module.Reset();
}

Dictionary<string, long> minimumHighPulseInputToRxOutput = new();
for (int i = 1;  minimumHighPulseInputToRxOutput.Count != 4; ++i)
{
	(long _, long _, IList<string> highPulseInputs) = ExecuteButtonPress();
	foreach (string highPulseInput
		in highPulseInputs.Where(item => !minimumHighPulseInputToRxOutput.ContainsKey(item)))
	{
		minimumHighPulseInputToRxOutput[highPulseInput] = i;
	}
}

long pressesUntilHighRx = LeastCommonMultiple(minimumHighPulseInputToRxOutput.Values.ToList());

Console.WriteLine("Day 20A");
Console.WriteLine($"Low times high pulses: {pulseCount.LowPulses * pulseCount.HighPulses}");

Console.WriteLine("Day 20B");
Console.WriteLine($"Minimum presses until high rx: {pressesUntilHighRx}");

(long LowPulses, long HighPulses, IList<string> HighPulseInputToRxOutput) ExecuteButtonPress()
{
	long lowPulses = 0;
	long highPulses = 0;
	Queue<(string Source, string Target, bool Pulse)> pulseQueue = new([("button", "broadcaster", false)]);
	List<string> highPulseInputToRxOutput = new();

	while (pulseQueue.TryDequeue(out (string Source, string Target, bool Pulse) pulseMessage))
	{
		if (pulseMessage.Pulse)
		{
			++highPulses;
		}
		else
		{
			++lowPulses;
		}

		if (moduleLookup.TryGetValue(pulseMessage.Target, out BaseModule? targetModule))
		{
			if (pulseMessage.Pulse && targetModule.Outputs.Contains("rx"))
			{
				highPulseInputToRxOutput.Add(pulseMessage.Source);
			}

			foreach ((string Source, string Target, bool Pulse) newPulseMessage
				in moduleLookup[pulseMessage.Target].HandlePulse(pulseMessage.Source, pulseMessage.Pulse))
			{
				pulseQueue.Enqueue(newPulseMessage);
			}
		}
	}

	return (lowPulses, highPulses, highPulseInputToRxOutput);
}

long GreatestComonDivisor(long firstNumber, long secondNumber) =>
	secondNumber == 0
		? firstNumber
		: GreatestComonDivisor(secondNumber, firstNumber % secondNumber);

long LeastCommonMultiple(IList<long> numbers) =>
	numbers.Aggregate((acc, curr) => acc * curr / GreatestComonDivisor(acc, curr));

abstract class BaseModule(string name, IList<string> outputs)
{
	public string Name => name;
	public IList<string> Inputs { get; } = new List<string>();
	public IList<string> Outputs => outputs;

	public abstract IEnumerable<(string Source, string Target, bool Pulse)> HandlePulse(string source, bool pulse);

	public virtual void Reset()	{ }
}

class Broadcaster(string name, IList<string> outputs) : BaseModule(name, outputs)
{
	public override IEnumerable<(string Source, string Target, bool Pulse)> HandlePulse(string source, bool pulse) =>
		Outputs.Select(output => (Name, output, pulse));
}

class FlipFlop(string name, IList<string> outputs) : BaseModule(name, outputs)
{
	bool isOn = false;

	public override IEnumerable<(string Source, string Target, bool Pulse)> HandlePulse(string source, bool pulse)
	{
		if (!pulse && !isOn)
		{
			isOn = true;
			return Outputs.Select(output => (Name, output, true));
		}
		else if (!pulse && isOn)
		{
			isOn = false;
			return Outputs.Select(output => (Name, output, false));
		}

		return Enumerable.Empty<(string Source, string Target, bool Pulse)>();
	}

	public override void Reset() => isOn = false;
}

class Conjunction(string name, IList<string> outputs) : BaseModule(name, outputs)
{
	Dictionary<string, bool> lastPulseFromSource = new();

	public override IEnumerable<(string Source, string Target, bool Pulse)> HandlePulse(string source, bool pulse)
	{
		lastPulseFromSource[source] = pulse;
		bool outputSignal = !Inputs.All(input => lastPulseFromSource.GetValueOrDefault(input));

		return Outputs.Select(output => (Name, output, outputSignal));
	}

	public override void Reset() => lastPulseFromSource = new();
}
