using LuckyDraw.Helper;
using LuckyDraw.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LuckyDraw.Controllers
{
    public class BackWorkController : Controller
    {
        LuckyDrawEntities _db = new LuckyDrawEntities();
        //
        // GET: /BackWork/
        #region Views
        public ActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();
            model.HeadImg = _db.Manus.Single(x => x.Key.Equals("HeadImg")).Value;
            model.VideoSrc = _db.Manus.Single(x => x.Key.Equals("VideoSrc")).Value;
            model.DrawRulesBack = _db.Manus.Where(x => x.Key.Equals("DrawRules")).OrderBy(x => x.Order).ToList();
            model.Prizes = _db.Prizes.Where(x => x.State).ToList();
            model.Roulette = _db.Manus.Single(x => x.Key.Equals("Roulette")).Value;

            return View(model);
        }

        #endregion

        #region ParticalViews
        public PartialViewResult EditPrizeView()
        {
            return PartialView();
        }
        #endregion

        #region Actions

        #region HeadImg
        public async Task<JsonResult> UploadHeadImg()
        {
            var file = Request.Files["HeadImg"];
            if (file != null && file.ContentLength > 0)
            {
                var extend = Path.GetExtension(file.FileName);
                var path = await FileHelper.Upload(file, Guid.NewGuid().ToString("N") + extend);

                var headImg = _db.Manus.SingleOrDefault(x => x.Key.Equals("HeadImg"));
                headImg.Value = path;
                _db.SaveChanges();

                return Json(new { success = true, result = path });
            }

            return Json(new { success = false, msg = "上传图片失败" });
        }
        #endregion

        #region Video
        public JsonResult UploadVideo(string VideoSrc)
        {
            var src = _db.Manus.SingleOrDefault(x => x.Key.Equals("VideoSrc"));
            src.Value = VideoSrc;
            _db.SaveChanges();

            return Json(new { success = true, result = VideoSrc });
        }

        #endregion

        #region Prize
        public ActionResult EditPrize(Prize model)
        {
            if (model.Id != 0)
            {
                var prize = _db.Prizes.Find(model.Id);

                if (prize != null)
                {
                    prize.Name = model.Name;
                    prize.Detail = model.Detail;
                    prize.Count = model.Count;
                    prize.Percent = model.Percent;
                    prize.Angle = model.Angle;
                }
            }
            else
            {
                model.State = true;
                
                _db.Prizes.Add(model);
            }
            _db.SaveChanges();

            var pList = _db.Prizes.Where(x => x.State).ToList();

            return PartialView("_DrawRoulette", pList);
        }

        public JsonResult DeletePrize(int id)
        {
            var prize = _db.Prizes.Find(id);
            if (prize != null)
            {
                prize.State = false;
                var result = _db.SaveChanges();

                if (result > 0)
                {
                    return Json(new { success = true, msg="删除成功" });
                }
            }

            return Json(new { success = false, msg = "删除成功" });
        }
        #endregion

        #region Rules

        public PartialViewResult EditRules(Manu model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    model.Key = "DrawRules";

                    _db.Entry(model).State = System.Data.EntityState.Added;
                }
                else
                {
                    var manu = _db.Manus.SingleOrDefault(x => x.Id.Equals(model.Id));
                    if (manu != null)
                    {
                        manu.Order = model.Order;
                        manu.Value = model.Value;
                        _db.Entry(manu).State = System.Data.EntityState.Modified;
                    }
                }

                _db.SaveChanges();
            }

            var rules = _db.Manus.Where(x => x.Key.Equals("DrawRules")).OrderBy(x => x.Order).ToList();

            return PartialView("_DrawRules", rules);
        }

        public JsonResult DeleteRules(int id)
        {
            var rule = _db.Manus.SingleOrDefault(x => x.Id.Equals(id));

            if (rule != null)
            {
                _db.Manus.Remove(rule);
                _db.SaveChanges();

                return Json(new { success = true, msg = "删除成功" });
            }

            return Json(new { success = false, msg="删除出错" });
        }
        #endregion

        #region Roulette
        public async Task<JsonResult> UploadRoulette()
        {
            var file = Request.Files["Roulette"];
            if (file != null && file.ContentLength > 0)
            {
                FileHelper.Rename(Server.MapPath("~/Content/Images/"), "ly-plate.png", Guid.NewGuid().ToString("N") + ".png");
                var path = await FileHelper.Upload(file, "ly-plate.png");

                var roulette = _db.Manus.SingleOrDefault(x => x.Key.Equals("Roulette"));
                roulette.Value = path;
                _db.SaveChanges();

                return Json(new { success = true, result = path });
            }

            return Json(new { success = false, msg = "上传图片失败" });
        }
        #endregion

        #endregion

        #region IDispose
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();

            base.Dispose(disposing);
        }
        #endregion
    }
}