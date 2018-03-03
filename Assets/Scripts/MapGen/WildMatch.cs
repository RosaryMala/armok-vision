
public static class WildMatch {

    /// <summary>
    /// Tests whether specified string can be matched agains provided pattern string. Pattern may contain single- and multiple-replacing
    /// wildcard characters.
    /// </summary>
    /// <param name="input">String which is matched against the pattern.</param>
    /// <param name="pattern">Pattern against which string is matched.</param>
    /// <param name="singleWildcard">Character which can be used to replace any single character in input string.</param>
    /// <param name="multipleWildcard">Character which can be used to replace zero or more characters in input string.</param>
    /// <returns>true if <paramref name="pat"/> matches the string <paramref name="str"/>; otherwise false.</returns>
    public static bool BoolMatch(string input, string pattern, char singleWildcard = '?', char multipleWildcard = '*')
    {
        int[] inputPosStack = new int[(input.Length + 1) * (pattern.Length + 1)];   // Stack containing input positions that should be tested for further matching
        int[] patternPosStack = new int[inputPosStack.Length];                      // Stack containing pattern positions that should be tested for further matching
        int stackPos = -1;                                                          // Points to last occupied entry in stack; -1 indicates that stack is empty
        bool[,] pointTested = new bool[input.Length + 1, pattern.Length + 1];       // Each true value indicates that input position vs. pattern position has been tested
        int inputPos = 0;   // Position in input matched up to the first multiple wildcard in pattern
        int patternPos = 0; // Position in pattern matched up to the first multiple wildcard in pattern
        // Match beginning of the string until first multiple wildcard in pattern
        while (inputPos < input.Length && patternPos < pattern.Length && pattern[patternPos] != multipleWildcard && (input[inputPos] == pattern[patternPos] || pattern[patternPos] == singleWildcard))
        {
            inputPos++;
            patternPos++;
        }
        // Push this position to stack if it points to end of pattern or to a general wildcard
        if (patternPos == pattern.Length || pattern[patternPos] == multipleWildcard)
        {
            pointTested[inputPos, patternPos] = true;
            inputPosStack[++stackPos] = inputPos;
            patternPosStack[stackPos] = patternPos;
        }
        bool matched = false;
        // Repeat matching until either string is matched against the pattern or no more parts remain on stack to test
        while (stackPos >= 0 && !matched)
        {
            inputPos = inputPosStack[stackPos];         // Pop input and pattern positions from stack
            patternPos = patternPosStack[stackPos--];   // Matching will succeed if rest of the input string matches rest of the pattern
            if (inputPos == input.Length && patternPos == pattern.Length)
                matched = true;     // Reached end of both pattern and input string, hence matching is successful
            else
            {
                // First character in next pattern block is guaranteed to be multiple wildcard
                // So skip it and search for all matches in value string until next multiple wildcard character is reached in pattern
                for (int curInputStart = inputPos; curInputStart < input.Length; curInputStart++)
                {
                    int curInputPos = curInputStart;
                    int curPatternPos = patternPos + 1;
                    if (curPatternPos == pattern.Length)
                    {   // Pattern ends with multiple wildcard, hence rest of the input string is matched with that character
                        curInputPos = input.Length;
                    }
                    else
                    {
                        while (curInputPos < input.Length && curPatternPos < pattern.Length && pattern[curPatternPos] != multipleWildcard &&
                            (input[curInputPos] == pattern[curPatternPos] || pattern[curPatternPos] == singleWildcard))
                        {
                            curInputPos++;
                            curPatternPos++;
                        }
                    }
                    // If we have reached next multiple wildcard character in pattern without breaking the matching sequence, then we have another candidate for full match
                    // This candidate should be pushed to stack for further processing
                    // At the same time, pair (input position, pattern position) will be marked as tested, so that it will not be pushed to stack later again
                    if (((curPatternPos == pattern.Length && curInputPos == input.Length) || (curPatternPos < pattern.Length && pattern[curPatternPos] == multipleWildcard))
                        && !pointTested[curInputPos, curPatternPos])
                    {
                        pointTested[curInputPos, curPatternPos] = true;
                        inputPosStack[++stackPos] = curInputPos;
                        patternPosStack[stackPos] = curPatternPos;
                    }
                }
            }
        }
        return matched;
    }

    /// <summary>
    /// Tests how closely a specified string matches a given pattern string. 
    /// </summary>
    /// <param name="input">String which is matched against the pattern.</param>
    /// <param name="pattern">Pattern against which string is matched.</param>
    /// <returns>-1 if no match can be made, 0 of the strings are an exact match, and a higher number the more difference there is.</returns>
    public static int ScoreMatch(string input, string pattern)
    {
        if (!BoolMatch(input, pattern))
            return -1;
        int similarity = pattern.Length - input.Length;
        foreach (char letter in pattern)
        {
            switch (letter)
            {
                case '*':
                    similarity += 2;
                    break;
                case '?':
                    similarity += 1;
                    break;
                default:
                    break;
            }
        }
        return similarity;
    }
}
