Write-Host "Day 1"

# $inputFilenamePart1 = "exampleInputPart1.txt"
# $inputFilenamePart2 = "exampleInputPart2.txt"
$inputFilenamePart1 = "input.txt"
$inputFilenamePart2 = "input.txt"

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

$matchPartOne = (
    $textToDigits.GetEnumerator() |
        ForEach-Object { $_.Key } |
        Where-Object { ($_ -as [int] -is [int]) } ) -join '|'

$numbers = Get-Content $inputFilenamePart1 |
    ForEach-Object {
        $currentLineMatches = Select-String $matchPartOne -InputObject $_ -AllMatches
        $textToDigits[$currentLineMatches.Matches[0].Value] * 10 + $textToDigits[$currentLineMatches.Matches[-1].Value]
    }
$sum = ($numbers | Measure-Object -sum).Sum
Write-Host "Part 1: The sum of the calibration values is $sum."

$matchPartTwo = (
    $textToDigits.GetEnumerator() |
        ForEach-Object { $_.Key } ) -join '|'
$regexFromRightPartTwo = [regex]::new($matchPartTwo, [System.Text.RegularExpressions.RegexOptions]::RightToLeft)

$numbers = Get-Content $inputFilenamePart2 |
    ForEach-Object {
        $currentLineMatches = Select-String $matchPartTwo -InputObject $_ -AllMatches | Sort-Object -Property Index
        $currentLineMatchesFromRight = $regexFromRightPartTwo.Matches($_) | Sort-Object -Property Index
        $textToDigits[$currentLineMatches.Matches[0].Value] * 10 + $textToDigits[$currentLineMatchesFromRight[-1].Value]
    }
$sum = ($numbers | Measure-Object -sum).Sum
Write-Host "Part 2: The sum of the calibration values is $sum."
