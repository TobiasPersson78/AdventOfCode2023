bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

Dictionary<string, BaseModule> moduleLookup =
	File
		.ReadAllLines(inputFilename)
		.Select(line => line.Split(" -> "))
		.Select(lineParts => (PrefixAndName: lineParts[0], Outputs: lineParts[1].Split(", ")))
		.Select(item =>
			(BaseModule)(item.PrefixAndName[0] switch
			{
				'%' => new FlipFlop(item.PrefixAndName[1..], item.Outputs),
				'&' => new Conjunction(item.PrefixAndName[1..], item.Outputs),
				_ => new Broadcaster(item.PrefixAndName, item.Outputs)
			}))
		.ToDictionary(item => item.Name, item => item);

foreach (BaseModule module in moduleLookup.Values)
{
	foreach (string output in module.Outputs)
	{
		if (moduleLookup.TryGetValue(output, out BaseModule? outputModule))
		{
			moduleLookup[output].Inputs.Add(module.Name);
		}
	}
}

long lowPulses = 0;
long highPulses = 0;

for( int i = 0; i < 1000; ++i)
{
	var result = ExecuteButtonPress();
	lowPulses += result.LowPulses;
	highPulses += result.HighPulses;
}

Console.WriteLine("Day 20A");
Console.WriteLine($"Low times high pulses: {lowPulses * highPulses}");

(long LowPulses, long HighPulses) ExecuteButtonPress()
{
	long lowPulses = 0;
	long highPulses = 0;
	Queue<(string Source, string Target, bool Pulse)> pulseQueue = new([("button", "broadcaster", false)]);

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
			foreach ((string Source, string Target, bool Pulse) newPulseMessage
				in moduleLookup[pulseMessage.Target].HandlePulse(pulseMessage.Source, pulseMessage.Pulse))
			{
				pulseQueue.Enqueue(newPulseMessage);
			}
		}
	}

	return (lowPulses, highPulses);
}

abstract class BaseModule(string name, IList<string> outputs)
{
	public string Name => name;
	public IList<string> Inputs { get; } = new List<string>();
	public IList<string> Outputs => outputs;

	public abstract IEnumerable<(string Source, string Target, bool Pulse)> HandlePulse(string source, bool pulse);

	public abstract void Reset();
}

class Broadcaster(string name, IList<string> outputs) : BaseModule(name, outputs)
{
	public override IEnumerable<(string Source, string Target, bool Pulse)> HandlePulse(string source, bool pulse) =>
		Outputs.Select(output => (Name, output, pulse));

	public override void Reset() { }
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
