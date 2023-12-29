const fs = require('fs');

function WriteLine(value) { console.log(value); }
function match(pattern, other) { return (pattern & other) === pattern; }

function bitCount(pattern) {
    let result = 0;
    while (pattern > 0) {
        if (pattern & 1) {
            result++;
        }
        pattern >>= 1;
    }
    return result;
}

function pattern(draw) {
    let result = 0;
    for (const number of draw) {
        result |= (1 << number);
    }
    return result;
}

function draw(pattern) {
    const result = [];
    for (let i = 0; i < 64; i++) {
        if (pattern & (1 << i)) {
            result.push(i);
        }
    }
    return result;
}

// Load draws from JSON file
const dataFilePath = './data.json';
if (!fs.existsSync(dataFilePath)) throw new Error(`File ${dataFilePath} not found.`);
const draws = JSON.parse(fs.readFileSync(dataFilePath));
const validDraws = draws.filter(draw => draw.length > 0);

// Generate patterns from draws
const drawPatterns = validDraws.map(draw => pattern(draw));

// Combine patterns to identify common patterns
const patterns = [];
for (let i = 0; i < drawPatterns.length - 1; i++) {
    for (let j = i + 1; j < drawPatterns.length; j++) {
        patterns.push(drawPatterns[i] & drawPatterns[j]);
    }
}
patterns = patterns.distinct().sort();

// Group patterns by bit count
const patternGroups = patterns.map(pattern => ({ count: bitCount(pattern), pattern })).groupBy(pattern => pattern.count);
patternGroups = patternGroups.sort((a, b) => a.key - b.key);

// Process each pattern group
for (const group of patternGroups) {
    WriteLine(`occurrence of ${group.key} bits has ${group.value.length} patterns.`);

    // Find matches for each pattern in the group
    const matches = group.value.map(pattern => {
        const matchingDraws = drawPatterns.filter(draw => match(draw, pattern));
        const drawMatches = matchingDraws.map(draw => draw(draw));
        return { pattern, matches: drawMatches };
    });

    // Determine the highest number of matches
    const maxMatches = matches.reduce((max, match) => Math.max(max, match.matches.length), 0);
    WriteLine(`Highest number of matches is ${maxMatches} patterns.`);

    // Print top matches
    const topMatches = matches.filter(match => match.matches.length === maxMatches);
    for (const match of topMatches.sort(match => match.pattern)) {
        WriteLine(`* Pattern ${draw(match.pattern).join(', ')} occurs ${match.matches.length} times:`);

        for (const draw of match.matches) {
            WriteLine(`* * Draw ${draw.join(', ')}.`);
        }
    }
    // Conclude for the pattern group
    WriteLine(`Finished for {group.key} bits.`);
}
