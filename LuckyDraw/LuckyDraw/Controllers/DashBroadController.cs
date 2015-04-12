using LuckyDraw.Models;
using System;
using System.Collections.Generic;
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
        public ActionResult RewardList()
        {
            return View();
        }
        #endregion
    }
}