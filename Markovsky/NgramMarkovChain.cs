using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Markovsky
{
    public class NgramMarkovChain
    {
        public void Run(int ngramSize, string file)
        {
            String trainingFile = File.ReadAllText(file);
            string[] words = Tokenize(trainingFile);
            Dictionary<Ngram, Dictionary<string, int>> occurrences = CreateMarkovTable(words, ngramSize);

            int wordsToProduce = 1000;
            var random = new Random();
            var candidatesForKickoff = occurrences.Keys.Where(x => char.IsUpper(x.Words[0][0])).ToList();
            Ngram currentNgram = SelectWordToKickoffGeneration(random, candidatesForKickoff);
            var produced = new List<string>(currentNgram.Words);

            for (int i = 0; i < wordsToProduce; i++)
            {
                var nextOccurrences = occurrences[currentNgram];
                string selectedWord = ChooseWord(random, nextOccurrences);
                produced.Add(selectedWord);
                currentNgram = new Ngram { Words = currentNgram.Words.Skip(1).Concat(new[]{ selectedWord }).ToArray() };
            }

            for (int i = 0; i < produced.Count - 1; i++)
            {
                String thisWord = produced[i];
                String nextWord = produced[i + 1];

                if (nextWord == "." || nextWord == ",")
                {
                    Console.Write(thisWord);
                }
                else
                {
                    Console.Write(thisWord + " ");
                }
            }

            Console.WriteLine("\nDone!");
        }

        private static Dictionary<Ngram, Dictionary<string, int>> CreateMarkovTable(string[] words, int ngramSize)
        {
            Dictionary<Ngram, Dictionary<String, int>> occurrences = new Dictionary<Ngram, Dictionary<String, int>>();

            for (int i = 0; i < words.Length - ngramSize; i++)
            {
                String nextWord = words[i + ngramSize];

                Ngram ngram = new Ngram { Words = words.Skip(i).Take(ngramSize).ToArray() };

                if (occurrences.TryGetValue(ngram, out var nextWords))
                {
                    if (nextWords.ContainsKey(nextWord))
                    {
                        nextWords[nextWord]++;
                    }
                    else
                    {
                        nextWords.Add(nextWord, 1);
                    }
                }
                else
                {
                    occurrences.Add(ngram, new Dictionary<string, int> { { nextWord, 1 } });
                }
            }

            return occurrences;
        }

        private static string[] Tokenize(String content)
        {
            Regex regex = new Regex("([\\w'`’áéíóúãõâêôç]+|!|\\?|\\,|\\.|\\(|\\))", RegexOptions.IgnorePatternWhitespace);
            var words = regex.Matches(content).ToArray().Select(x => x.Captures[0].Value).ToArray();
            return words;
        }

        private static Ngram SelectWordToKickoffGeneration(Random random, List<Ngram> candidatesForKickoff)
        {
            return candidatesForKickoff[random.Next(0, candidatesForKickoff.Count - 1)];
        }

        private static string ChooseWord(Random random, IEnumerable<KeyValuePair<string, int>> nextOccurrences)
        {
            int sum = nextOccurrences.Sum(x => x.Value);

            //Each word has N chances in 100 to be selected.
            int selected = random.Next(0, sum + 1);

            String selectedWord = SelectWord(nextOccurrences, selected);
            return selectedWord;
        }

        public static String SelectWord(IEnumerable<KeyValuePair<String, int>> nextOccurences, int selected)
        {
            int sum = 0;
            foreach (var item in nextOccurences.OrderByDescending(x => x.Value))
            {
                sum += item.Value;
                if (sum >= selected)
                {
                    return item.Key;
                }
            }
            throw new Exception("Dead end");
        }

    }
}