Write-Host "Day 1"

#$inputFilename = "exampleInputPart1.txt"
#$inputFilename = "exampleInputPart2.txt"
$inputFilename = "input.txt"

[string[]]$lines = Get-Content $inputFilename

$digits = $lines | ForEach-Object {$_ -replace '[^\d]+'}
$numbers = $digits | ForEach-Object { [int]"$($_[0])$($_[-1])" }
$sum = ($numbers | Measure-Object -sum).Sum

Write-Host "Part 1: The sum of the calibration values is $sum."

$textToDigits = @{
    '0' = 0
    '1' = 1
    '2' = 2
    '3' = 3
    '4' = 4
    '5' = 5
    '6' = 6
    '7' = 7
    '8' = 8
    '9' = 9

    'one' = 1
    'two' = 2
    'three' = 3
    'four' = 4
    'five' = 5
    'six' = 6
    'seven' = 7
    'eight' = 8
    'nine' = 9
}

$numbers = @()

foreach ($currentLine in $lines) {
    # Find first digit.
    $matchFound = $false
    for ($i = 0; $i -lt $currentLine.Length; $i++) {
        $currentSubstring = $currentLine.Substring($i)
        foreach ($currentTextToDigit in $textToDigits.GetEnumerator()) {
            if ($currentSubstring.StartsWith($currentTextToDigit.Key)) {
                $firstDigit = $currentTextToDigit.Value
                $matchFound = $true
                break
            }
        }
        if ($matchFound) {
            break
        }
    }

    # Find last digit.
    $matchFound = $false
    for ($i = 0; $i -lt $currentLine.Length; $i++) {
        $currentSubstring = $currentLine.Substring(0, $currentLine.Length - $i)
        foreach ($currentTextToDigit in $textToDigits.GetEnumerator()) {
            if ($currentSubstring.EndsWith($currentTextToDigit.Key)) {
                $lastDigit = $currentTextToDigit.Value
                $matchFound = $true
                break
            }
        }
        if ($matchFound) {
            break
        }
    }

    $numbers += $firstDigit * 10 + $lastDigit
}

$sum = ($numbers | Measure-Object -sum).Sum

Write-Host "Part 2: The sum of the calibration values is $sum."
