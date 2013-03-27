using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/*
 * A concordance is an alphabetical list of the words present in a text
 * with a count of how often each word appears and citations of where each word appears in the text (e.g., page number).
 * Write a program -- in the programming language of your choice -- that generates a concordance of an arbitrary text document written in English:
 * the text can be read from stdin, and the program should output the concordance to stdout or a file.
 * For each word, it should print the count and the (sorted) list of citations,
 * in this case the zero-indexed sentence number in which that word occurs.
 * You may assume that the input contains only spaces, newlines, standard English letters, and standard English punctuation marks.
 */

namespace CerebroConcordance
{
	class Program
	{
		static void Main(string[] args)
		{
			//string input = "A concordance is an alphabetical list of the words present in a text with a count of how often each word appears and citations of where each word appears in the text (e.g., page number). Write a program -- in the programming language of your choice -- that generates a concordance of an arbitrary text document written in English: the text can be read from stdin, and the program should output the concordance to stdout or a file. For each word, it should print the count and the (sorted) list of citations, in this case the zero-indexed sentence number in which that word occurs. You may assume that the input contains only spaces, newlines, standard English letters, and standard English punctuation marks.";
			//string input = "James Bryant Conant (1893–1978) was a chemist, President of Harvard University, and the first U.S. Ambassador to West Germany. As a Harvard professor, he was one of the first to explore the relationship between chemical equilibrium and the reaction rate of chemical processes. He studied the biochemistry of oxyhemoglobin, helped to elucidate the structure of chlorophyll, and contributed insights that underlie modern theories of acid-base chemistry. It was during his presidency of Harvard (1933–53) that women were first admitted to Harvard Medical School and Harvard Law School. As chairman of the National Defense Research Committee during World War II, he oversaw the Manhattan Project, which developed the first atomic bombs. After the war, he served on the Joint Research and Development Board that coordinated defense research, and on the General Advisory Committee of the Atomic Energy Commission. In his later years at Harvard, he taught the history and philosophy of science, and wrote about the scientific method. In 1953 he became the U.S. High Commissioner for Germany, overseeing the restoration of German sovereignty, and then was U.S. Ambassador to West Germany until 1957. (Full article...)";
			string input = Console.ReadLine();
			Concordance concordance = new Concordance(input);
			concordance.GenerateWordLocations();
			concordance.WriteOnConsole();
			Console.ReadKey();
		}
	}

	class Concordance
	{
		private readonly string _text;
		private readonly SortedList<string, List<int>> _wordLocations;

		/// <summary>
		/// Creates a new Concordance object.
		/// </summary>
		public Concordance(string text)
		{
			_text = text;
			_wordLocations = new SortedList<string, List<int>>();
		}

		/// <summary>
		/// Generates the actual concordance.
		/// </summary>
		public void GenerateWordLocations()
		{
			string[] sentenceList = SplitSentences(_text);
			for (int sentenceIndex = 0; sentenceIndex < sentenceList.Length; ++sentenceIndex)
			{
				// Splits the current sentence in a collection of words.
				// The regex will match words, compounds and abbreviations (such as e.g.),
				// it is also a little complex to not match some cases ("--" and "...").
				// [\w]				Matches a simple word
				// ([-\.][\w]+)+	Matches compounds and abbreviations
				// [.]?				Matches the final dot for an abbreviation
				MatchCollection wordCollection = Regex.Matches(sentenceList[sentenceIndex], @"[\w]+(([-\.][\w]+)+[.]?)?");

				foreach (Match wordMatch in wordCollection)
				{
					string word = wordMatch.Value.ToLower(); // Lower case only to prevent duplicates like "a" and "A"
					if (_wordLocations.ContainsKey(word) == false)
					{
						_wordLocations.Add(word, new List<int>());
					}
					_wordLocations[word].Add(sentenceIndex);
				}
			}
		}

		/// <summary>
		/// Splits a text in an array of sentences.
		/// </summary>
		private string[] SplitSentences(string text)
		{
			// Split the text around the separators defined by the regex.
			// [\.!\?]+		Matches any sentence end mark
			// \s+			Matches one or more white-space characters
			// (?=[A-Z])	Look-ahead matching a capital letter
			// This regex is not perfect as it can match false positives (e.g.: "A. B. C." or "U.S. Ambassador").
			// The separator itself will be removed, except for the look-ahead.
			string[] sentences = Regex.Split(text, @"[\.!\?]+\s+(?=[A-Z])");
			return sentences;
		}

		/// <summary>
		/// Prints the result to the console.
		/// </summary>
		public void WriteOnConsole()
		{
			foreach (KeyValuePair<string, List<int>> pair in _wordLocations)
			{
				List<int> locations = pair.Value;
				Console.Write(pair.Key + " {" + locations.Count + ":" + locations[0]);
				for (int locationIndex = 1; locationIndex < locations.Count; ++locationIndex)
				{
					Console.Write("," + locations[locationIndex]);
				}
				Console.WriteLine("}");
			}
		}
	}
}
