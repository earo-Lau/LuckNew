using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckyDraw.Models
{
    public class HomeViewModel
    {
        public string HeadImg { get; set; }

        public string VideoSrc { get; set; }

        public List<string> DrawRules { get; set; }

        public List<Manu> DrawRulesBack { get; set; }

        public List<Prize> Prizes { get; set; }

        public string Roulette { get; set; }
    }
}