using LuckyDraw.Helper;
using LuckyDraw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;

namespace LuckyDraw.API
{
    public class LuckyController : ApiController
    {
        /// <summary>
        /// 抽奖API
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Draw()
        {
            var roulette = RouletteHelper.BuildRoulette();
            var prizeIndex = RouletteHelper.rand.Next(roulette.Count);

            return Ok(roulette[prizeIndex]);
        }
    }
}
