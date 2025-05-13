using System;

public static class RandomNameGenerator 
{
    public static string GetRandomName() {
        string[] syllablesStart = { "Ka", "Lo", "Ma", "Re", "Za", "Vi", "Do", "El", "Sa" };
        string[] syllablesEnd = { "rin", "ra", "tor", "mia", "lan", "vek", "dra", "nel", "lar" };

        Random rand = new();
        string name = syllablesStart[rand.Next(syllablesStart.Length)] + syllablesEnd[rand.Next(syllablesEnd.Length)];
        return name;
    }
}
