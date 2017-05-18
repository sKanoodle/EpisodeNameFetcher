using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EpisodeNameFetcher
{
    static class RegexOperations
    {
        public static string CleanWikipediaInput(this string input)
        {
            // delete all newlines that arent followd by a number or "Season"
            string pattern = @"\r\n(?!\d|Season)";
            string replacementPattern = String.Empty;
            input = Regex.Replace(input, pattern, replacementPattern);

            // clear lines starting with "Season"
            pattern = @"^Season.*$";
            return Regex.Replace(input, pattern, replacementPattern, RegexOptions.Multiline);
        }

        public static string MakeWikipediaDictionary(this string input, string seriesShortHandle)
        {
            string pattern = @"^\d+\t(\d+)\t""([^""]+).*$";
            string replacementPattern = $@"[""S0E$1""] = ""{seriesShortHandle} S0E$1 - $2"",";
            return Regex.Replace(input, pattern, replacementPattern, RegexOptions.Multiline);
        }

        public static string MakeTheTVDBDictionary(this string input, string seriesShortHandle)
        {
            string pattern = @"^(\d{1,2}) x (\d{1,2})\t([^\t]+).*$";
            string replacementPattern = $@"[""S$1E$2""] = ""{seriesShortHandle} S$1E$2 - $3"",";
            return Regex.Replace(input, pattern, replacementPattern, RegexOptions.Multiline);
        }

        public static string PadSeasonOrEpisodeNumber(this string input)
        {
            // pad season number
            string pattern = @"(S)([1-9])(?=E\d{1,2})";
            string replacementPattern = @"${1}0$2";
            input = Regex.Replace(input, pattern, replacementPattern);

            // pad episode number
            pattern = @"(?<=S\d{1,2})(E)(\d)(?!\d)";
            return Regex.Replace(input, pattern, replacementPattern);
        }

        public static string DeleteInvalidFilenameCharacters(this string input)
        {
            string pattern = @"[:?<>*|…]";
            string replacementPattern = String.Empty;
            input = Regex.Replace(input, pattern, replacementPattern);

            pattern = "[\\/]";
            replacementPattern = " ";
            return Regex.Replace(input, pattern, replacementPattern);
        }

        public static string InsertSeasonNumbers(this string input)
        {
            StringBuilder result = new StringBuilder();
            int season = 1;
            string line;
            using (TextReader reader = new StringReader(input))
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == String.Empty)
                        season += 1;
                    else
                        result.AppendLine(insert(line, season));
                }
            return result.ToString();

            string insert(string l, int s)
            {
                if (s > 99) throw new ArgumentException();
                string pattern = @"S0(E\d{2})";
                string replacementPattern = $@"S{season:00}$1";
                return Regex.Replace(l, pattern, replacementPattern);
            }
        }
    }
}
