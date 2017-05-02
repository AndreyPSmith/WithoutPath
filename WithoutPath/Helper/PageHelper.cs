using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WithoutPath.Helper
{
    public static class PageHelper
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html, int currentPage, int totalPages, Func<int, string> pageUrl)
        {
            var builder = new StringBuilder();

            //Prev
            var prevBuilder = new TagBuilder("a");
            prevBuilder.MergeAttribute("class", "fa fa-chevron-left nav-btn");
            if (currentPage == 1)
            {
                prevBuilder.MergeAttribute("href", "#");
                builder.AppendLine("<li class=\"active\">" + prevBuilder + "</li>");
            }
            else
            {
                prevBuilder.MergeAttribute("href", pageUrl.Invoke(currentPage - 1));
                builder.AppendLine("<li>" + prevBuilder + "</li>");
            }
            //По порядку
            for (int i = 1; i <= totalPages; i++)
            {
                //Условие что выводим только необходимые номера
                if (((i <= 3) || (i > (totalPages - 3))) || ((i > (currentPage - 2)) && (i < (currentPage + 2))))
                {
                    var subBuilder = new TagBuilder("a");
                    subBuilder.InnerHtml = i.ToString(CultureInfo.InvariantCulture);
                    if (i == currentPage)
                    {
                        subBuilder.MergeAttribute("href", "#");
                        builder.AppendLine("<li class=\"active\">" + subBuilder + "</li>");
                    }
                    else
                    {
                        subBuilder.MergeAttribute("href", pageUrl.Invoke(i));
                        builder.AppendLine("<li>" + subBuilder + "</li>");
                    }
                }
                else if ((i == 4) && (currentPage > 5))
                {
                    //Троеточие первое
                    builder.AppendLine("<li class=\"disabled\"> <a href=\"#\">...</a> </li>");
                }
                else if ((i == (totalPages - 3)) && (currentPage < (totalPages - 4)))
                {
                    //Троеточие второе
                    builder.AppendLine("<li class=\"disabled\"> <a href=\"#\">...</a> </li>");
                }
            }
            //Next
            var nextBuilder = new TagBuilder("a");
            nextBuilder.MergeAttribute("class", "fa fa-chevron-right nav-btn");
            if (currentPage == totalPages || totalPages == 0)
            {
                nextBuilder.MergeAttribute("href", "#");
                builder.AppendLine("<li class=\"active\">" + nextBuilder + "</li>");
            }
            else
            {
                nextBuilder.MergeAttribute("href", pageUrl.Invoke(currentPage + 1));
                builder.AppendLine("<li>" + nextBuilder + "</li>");
            }
            return new MvcHtmlString("<ul>" + builder + "</ul>");
        }
    }
}