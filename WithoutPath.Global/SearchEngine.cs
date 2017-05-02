using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using WithoutPath.DAL;

namespace WithoutPath.Global
{
    public class SearchEngine
    {
        public static IEnumerable<Post> Search(string searchString, IQueryable<Post> source)
        {
            var term =  Helpers.CleanContent(searchString.ToLowerInvariant().Trim(), false);
            var terms = term.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var regex = string.Format(CultureInfo.InvariantCulture, "({0})", string.Join("|", terms));

            foreach (var entry in source)
            {
                var rank = 0;

                var dateRank = 0;

                var nameRank = 0;

                int id;

                int.TryParse(searchString, out id);

                string date = entry.AddedDate.Day + "." + entry.AddedDate.Month + "." + entry.AddedDate.Year;

                if (!string.IsNullOrWhiteSpace(entry.Header) && !string.IsNullOrWhiteSpace(searchString))
                {
                    rank += Regex.Matches(entry.Header.ToLowerInvariant(), searchString.ToLowerInvariant()).Count;
                    dateRank += Regex.Matches(date, searchString).Count;
                    nameRank += Regex.Matches(entry.Character.Name.ToLowerInvariant(), searchString.ToLowerInvariant()).Count;

                }
                if (rank > 0 || id == entry.Id || dateRank > 0 || nameRank > 0)
                {
                    yield return entry;
                }
            }
        }
    }
}