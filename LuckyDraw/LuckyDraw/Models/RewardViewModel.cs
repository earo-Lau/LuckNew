using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckyDraw.Models
{
    public class RewardViewModel
    {
        public RewardFilter Filter { get; set; }

        public RewardPagerModel RewardPageModel { get; set; }
    }

    public class RewardPagerModel
    {
        public int PageNum { get; set; }
        public List<DrawResultModel> ResultModel { get; set; }
    }

    public class RewardFilter
    {
        public string MemberMobile { get; set; }
        public string MemberName { get; set; }
        public string PrizeName { get; set; }
    }
}