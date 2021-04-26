using System;
using System.Collections.Generic;
using System.Text;

namespace Objects
{
    public static class Titles
    {

        public static List<string> TitlesToSkip = new List<string>()
        {
            "documentclass",
            "usepackage",
            "begin",
            "newcommand",
            "N",
            "textnumero",
            "year",
        };

        public static List<string> TitlesWithArgument = new List<string>()
        {
            "thanks",
            "begin",
            "end",
            "section",
        };
    }
}
