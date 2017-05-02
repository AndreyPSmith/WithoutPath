using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WithoutPath.Global
{
    public class Helpers
    {
        /// <summary>
        /// The regex strip html.
        /// </summary>
        private static readonly Regex RegexStripHtml = new Regex("<[^>]*>", RegexOptions.Compiled);
        public static string StripHtml(string html)
        {
            return string.IsNullOrWhiteSpace(html) ? string.Empty :
                RegexStripHtml.Replace(html, string.Empty).Trim();
        }

        /// <summary>
        /// The regex strip script html.
        /// </summary>
        private static readonly Regex RegexStripScript = new Regex(@"<script\b[^>]*>([\s\S]*?)<\/script>", RegexOptions.Compiled);
        public static string StripScript(string html)
        {
            return string.IsNullOrWhiteSpace(html) ? string.Empty :
                 RegexStripScript.Replace(html, string.Empty).Trim();
        }

        public static string CleanContent(string content, bool removeHtml)
        {
            if (removeHtml)
            {
                content = StripHtml(content);
            }

            content =
                content.Replace("\\", string.Empty).
                Replace("|", string.Empty).
                Replace("(", string.Empty).
                Replace(")", string.Empty).
                Replace("[", string.Empty).
                Replace("]", string.Empty).
                Replace("*", string.Empty).
                Replace("?", string.Empty).
                Replace("}", string.Empty).
                Replace("{", string.Empty).
                Replace("^", string.Empty).
                Replace("+", string.Empty);

            var words = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            foreach (var word in
                words.Select(t => t.ToLowerInvariant().Trim()).Where(word => word.Length > 1))
            {
                sb.AppendFormat("{0} ", word);
            }

            return sb.ToString();
        }
    }
}
