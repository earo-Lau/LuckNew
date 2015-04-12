using LuckyDraw.Helper;
using LuckyDraw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LuckyDraw.Controllers
{
    public delegate long LogPrizeInfoAsync(Member_Prize mp, Member member, string name, string address);

    public class HomeController : Controller
    {
        LuckyDraw.Models.LuckyDrawEntities _db = new Models.LuckyDrawEntities();
        protected static Random rand = new Random((int)DateTime.Now.Ticks);
        #region Views

        public ActionResult Index()
        {
            var viewModel = new HomeViewModel();
            //viewModel.HeadImg = _db.Manus.Single(x => x.Key.Equals("HeadImg")).Value;
            //viewModel.VideoSrc = _db.Manus.Single(x => x.Key.Equals("VideoSrc")).Value;
            //viewModel.DrawRules = _db.Manus.Where(x => x.Key.Equals("DrawRules"))
            //    .OrderBy(x => x.Order).Select(x => x.Value).ToList();

            return View(viewModel);
        }

        public PartialViewResult DrawResult(int num)
        {
            var drawList = (from mp in _db.Member_Prize
                            join m in _db.Members on mp.MemberID equals m.Id into Members
                            from members in Members.DefaultIfEmpty()
                            join p in _db.Prizes on mp.PrizeID equals p.Id into Prizes
                            from prize in Prizes.DefaultIfEmpty()
                            orderby mp.AddTime descending
                            select new DrawResultModel()
                            {
                                Member = members,
                                Prize = prize
                            }).Where(x => x.Member != null && x.Prize != null).Take(num);

            return PartialView(drawList.ToArray());
        }

        public PartialViewResult PrizeView(string ticket)
        {
            var _mp = _db.Member_Prize.SingleOrDefault(x => x.Ticket.Equals(ticket));
            if (_mp == null)
            {
                //未中奖时返回的视图
                return PartialView("~/Views/Home/DstBox/_Thank.cshtml");
            }

            var _prize = _db.Prizes.Find(_mp.PrizeID);
            if (_prize.Name == "未中奖")
            {
                //未中奖时返回的视图
                return PartialView("~/Views/Home/DstBox/_Thank.cshtml");
            }
            else
            {
                ViewBag.Ticket = ticket;
                return PartialView("~/Views/Home/DstBox/_Receive.cshtml", _prize);
            }

        }

        #endregion

        #region Actions

        #region Draw

        /// <summary>
        /// 抽奖
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public JsonResult Draw(Member member)
        {
            member.IP = Request.UserHostAddress;
            //if (CheckMember(member))
            //{
            //    return Json(new { result = false, msg = "您已经参加抽奖，请不要重复抽奖。" }, JsonRequestBehavior.AllowGet);
            //}

#if DEBUG
            return Json(new { result = true, prize = new Prize() { Id = 1, Name = "特等奖", Angle = 117 }, ticket = "000" }, JsonRequestBehavior.AllowGet);
#else
            var roulette = RouletteHelper.BuildRoulette();
            var prizeIndex = RouletteHelper.rand.Next(roulette.Count);
            var prize = roulette[prizeIndex];

            var ticket = LogPrize(member, prize);

            return Json(new { result = true, prize = roulette[prizeIndex], ticket = ticket }, JsonRequestBehavior.AllowGet);
#endif

        }

        /// <summary>
        /// 检查用户是否已经参与抽奖
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        protected bool CheckMember(Member member)
        {
            var result = _db.Members.Any(x => x.Mobile.Equals(member.Mobile)
                || x.IP.Equals(member.IP));

            return result;
        }

        /// <summary>
        /// 记录中奖信息
        /// </summary>
        /// <param name="member"></param>
        /// <param name="prize"></param>
        /// <returns></returns>
        public string LogPrize(Member member, Prize prize)
        {
            member = _db.Members.Add(member);
            _db.SaveChanges();
            var _MP = new Member_Prize()
            {
                MemberID = member.Id,
                PrizeID = prize.Id,
                AddTime = DateTime.Now,
                HasReceived = false,
                Ticket = Guid.NewGuid().ToString()
            };

            _db.Member_Prize.Add(_MP);
            //_db.Entry(_MP).State = System.Data.EntityState.Added;

            int result = _db.SaveChanges();
            if (result > 0)
            {
                return _MP.Ticket;
            }
            else
            {
                throw new Exception("抱歉，服务器出现错误。请稍后重试！");
            }
        }
        #endregion

        #region ReceivedPrize
        public JsonResult ReceivePrize(string Ticket, string Name, string Address)
        {
            var mp = _db.Member_Prize.SingleOrDefault(x => x.Ticket.Equals(Ticket));

            if (mp == null)
            {
                return Json(new { result = false, msg = "找不到您的中奖信息，请联系我们的客服。" });
            }

            var member = _db.Members.Find(mp.MemberID);
            if (member == null)
            {
                return Json(new { result = false, msg = "找不到您的用户信息，请联系我们的客服。" });
            }
            LogPrizeInfoAsync log = LogPrizeInfo;
            log.BeginInvoke(mp, member, Name, Address, LogPrizeInfoCallBack, log);

            return Json(new { result = true });
        }

        public long LogPrizeInfo(Member_Prize _mp, Member _member, string name, string address)
        {
            _member.Name = name;
            _member.Address = address;
            _mp.HasReceived = true;

            _db.Entry(_member).State = System.Data.EntityState.Modified;
            _db.Entry(_mp).State = System.Data.EntityState.Modified;
            var result = _db.SaveChanges();
            if (result > 0)
            {
                return _mp.PrizeID;
            }
            else
            {
                throw new Exception("抱歉，服务器出现错误。请稍后重试！");
            }
        }

        public void LogPrizeInfoCallBack(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                return;

            var log = (LogPrizeInfoAsync)asyncResult.AsyncState;
            var result = log.EndInvoke(asyncResult);

            var prize = _db.Prizes.Find(result);
            if (prize.Count > 0)
            {
                prize.Count--;
                _db.SaveChanges();
            }
            else
            {
                throw new Exception("抱歉,您收慢了一步，该奖品已经抽完。");
            }
        }
        #endregion

        #endregion
    }
}