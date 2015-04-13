using LuckyDraw.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LuckyDraw.Controllers
{
    public class DashBroadController : Controller
    {
        //
        // GET: /DashBroad/
        #region PrizeList
        #region View
        /// <summary>
        /// 奖项列表
        /// </summary>
        /// <returns></returns>
        public ActionResult PrizeList()
        {
            using (var _db=new LuckyDrawEntities())
            {
                var pList = _db.Prizes.Where(x => x.State);

                return View("PrizeList", pList.ToArray());
            }
        }
        #endregion

        #region Action
        /// <summary>
        /// 添加或者编辑奖项
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult EditPrize(Prize model)
        {
            using (var _db=new LuckyDrawEntities())
            {
                if (model.Id > 0)
                {
                    var prize = _db.Prizes.Find(model.Id);
                    if (prize == null)
                        return Json(new { success = false, msg = "找不到该奖项" });
                    
                        prize.Name = model.Name;
                        prize.Detail = model.Detail;
                        prize.Count = model.Count;
                        prize.Percent = model.Percent;
                        prize.Angle = model.Angle;
                }
                else {
                    var prize = new Prize()
                    {
                        Name = model.Name,
                        Angle = model.Angle,
                        Percent = model.Percent,
                        Count = model.Count,
                        Detail = model.Detail,
                        State = true
                    };

                    _db.Prizes.Add(prize);
                }
                var result= _db.SaveChanges();
                if(result>0){
                    return Json(new { success = true, item = model });
                }
            }
            return Json(new { success = false, msg = "提交失败" });
        }

        /// <summary>
        /// 删除奖项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeletePrize(int id)
        {
            var error = string.Empty;
            using (var _db = new LuckyDrawEntities())
            {
                var prize = _db.Prizes.Find(id);

                if (prize != null)
                {
                    prize.State = false;
                    var result= _db.SaveChanges();
                    if (result > 0)
                    {
                        return Json(new { success=true });
                    }
                    error = "删除失败";
                }
                else{
                    error = "找不到该奖项";
                }
            }
            return Json(new { success = false, msg = error });
        }
        #endregion
        #endregion


        #region RewardList
        #region Views
        public ActionResult RewardList(RewardViewModel viewmodel)
        {
            viewmodel.RewardPageModel = viewmodel.RewardPageModel ?? new RewardPagerModel() { 
                Page = new PageModel(){ PageNum=1} 
            };
            viewmodel.Filter = viewmodel.Filter ?? new RewardFilter();
            

            using (var _db = new LuckyDrawEntities())
            {

                var mpList = Filter(viewmodel.Filter, _db);

                viewmodel.RewardPageModel.Page.PageCount = (int)Math.Ceiling((decimal)mpList.Count() / (decimal)30);

                mpList = mpList.OrderBy(x => x.Id).Skip((viewmodel.RewardPageModel.Page.PageNum - 1) * 30).Take(30);
                viewmodel.RewardPageModel.ResultModel = mpList.ToList();
            }

            return View(viewmodel);
        }

        public FileResult Export(RewardViewModel viewmodel)
        {
            viewmodel.RewardPageModel = viewmodel.RewardPageModel ?? new RewardPagerModel()
            {
                Page = new PageModel() { PageNum = 1 }
            };
            viewmodel.Filter = viewmodel.Filter ?? new RewardFilter();

            using (var _db = new LuckyDrawEntities())
            {
                var mpList = Filter(viewmodel.Filter, _db);

                var stream = WriteFile(mpList.ToArray());
                return File(stream, "text/comma-separated-values", "韩熙获奖列表.csv");
            }
        }
        #endregion
        #region Method

        protected IQueryable<DrawResultModel> Filter(RewardFilter filter, LuckyDrawEntities _db)
        {
            var fResult = (from mp in _db.Member_Prize
                           join member in _db.Members on mp.MemberID equals member.Id into Members
                           from m in Members.DefaultIfEmpty()
                           join prize in _db.Prizes on mp.PrizeID equals prize.Id into Prizes
                           from p in Prizes.DefaultIfEmpty()
                           where mp.HasReceived
                           && (!string.IsNullOrEmpty(filter.MemberMobile) ? m.Mobile.Contains(filter.MemberMobile) : true)
                           && (!string.IsNullOrEmpty(filter.MemberName) ? m.Name.Contains(filter.MemberName) : true)
                           && (!string.IsNullOrEmpty(filter.PrizeName) ? p.Name.Contains(filter.PrizeName) : true)
                           select new DrawResultModel()
                           {
                               Member = m,
                               Prize = p,
                               Id = mp.Id,
                               AddTime = mp.AddTime
                           });

            return fResult;
        }

        protected Stream WriteFile(IList<DrawResultModel> model)
        {
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, System.Text.Encoding.UTF8);

            writer.WriteLine("#,手机号码,获奖者姓名,包裹地址,奖项,奖品,抽奖时间");

            foreach (var m in model)
            {
                writer.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6}", m.Id, m.Member.Mobile, m.Member.Name, m.Member.Address.Replace(",", "，"), m.Prize.Name, m.Prize.Detail, m.AddTime));
            }
            writer.Flush();
            ms.Position = 0;

            return ms;
        }

        #endregion

        #endregion
    }
}