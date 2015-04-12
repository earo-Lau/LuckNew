using LuckyDraw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckyDraw.Helper
{
    public class RouletteHelper
    {
        public static Random rand = new Random((int)DateTime.Now.Ticks);

        /// <summary>
        /// 生成轮盘
        /// </summary>
        /// <returns>返回轮盘</returns>
        public static List<Prize> BuildRoulette()
        {
            //生成的轮盘
            var roulette = new List<Prize>();

            using (LuckyDrawEntities _db = new LuckyDrawEntities())
            {
                var prizeList = _db.Prizes.Where(x => x.State && x.Count > 0);
                if (!prizeList.Any())
                    throw new Exception("没有奖品");

                //抽奖基数
                int total = 10000;

                //奖品列表
                var pList = new List<Prize>();
                foreach (var p in prizeList)
                {
                    var pCount = total * p.Percent / 100;
                    for (int i = 0; i < pCount; i++)
                    {
                        pList.Add(p);
                    }
                }

                //把奖品随机的放到轮盘中
                while (pList.Count > 0)
                {
                    var pos = rand.Next(pList.Count);
                    roulette.Add(pList[pos]);
                    pList.RemoveAt(pos);
                }
            }

            return roulette;
        }
    }
}