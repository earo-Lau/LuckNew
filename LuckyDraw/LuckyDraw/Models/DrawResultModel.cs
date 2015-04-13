using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckyDraw.Models
{
    public class DrawResultModel
    {
        public long Id { get; set; }
        public DateTime AddTime { get; set; }
        public Member Member { get; set; }
        public Prize Prize { get; set; }
    }
}