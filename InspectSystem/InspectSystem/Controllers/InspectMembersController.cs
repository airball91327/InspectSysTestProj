using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InspectSystem.Models;

namespace InspectSystem.Controllers
{
    public class InspectMembersController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectMembers
        public ActionResult Index()
        {
            var inspectMemberAreas = db.InspectMemberAreas.Include(i => i.InspectAreas)
                                                          .Include(i => i.InspectMembers)
                                                          .OrderBy(i => i.AreaId);
            var inspectMembers = db.InspectMembers.OrderBy(i => i.Department);
            InspectMembersViewModel inspectMembersViewModel = new InspectMembersViewModel()
            {
                InspectMemberAreas = inspectMemberAreas.ToList(),
                InspectMembers = inspectMembers.ToList()
            };
            return View(inspectMembersViewModel);
        }

        // GET: InspectMembers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectMemberAreas inspectMemberAreas = db.InspectMemberAreas.Find(id);
            if (inspectMemberAreas == null)
            {
                return HttpNotFound();
            }
            return View(inspectMemberAreas);
        }

        // GET: InspectMembers/Create
        public ActionResult Create()
        {
            ViewBag.AreaId = new SelectList(db.InspectAreas, "AreaID", "AreaName");
            ViewBag.MemberId = new SelectList(db.InspectMembers, "MemberId", "MemberName");
            return View();
        }

        // POST: InspectMembers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MemberId,AreaId")] InspectMemberAreas inspectMemberAreas)
        {
            if (ModelState.IsValid)
            {
                db.InspectMemberAreas.Add(inspectMemberAreas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AreaId = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectMemberAreas.AreaId);
            ViewBag.MemberId = new SelectList(db.InspectMembers, "MemberId", "MemberName", inspectMemberAreas.MemberId);
            return View(inspectMemberAreas);
        }

        // GET: InspectMembers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectMemberAreas inspectMemberAreas = db.InspectMemberAreas.Find(id);
            if (inspectMemberAreas == null)
            {
                return HttpNotFound();
            }
            ViewBag.AreaId = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectMemberAreas.AreaId);
            ViewBag.MemberId = new SelectList(db.InspectMembers, "MemberId", "MemberName", inspectMemberAreas.MemberId);
            return View(inspectMemberAreas);
        }

        // POST: InspectMembers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MemberId,AreaId")] InspectMemberAreas inspectMemberAreas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectMemberAreas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectMemberAreas.AreaId);
            ViewBag.MemberId = new SelectList(db.InspectMembers, "MemberId", "MemberName", inspectMemberAreas.MemberId);
            return View(inspectMemberAreas);
        }

        // GET: InspectMembers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectMemberAreas inspectMemberAreas = db.InspectMemberAreas.Find(id);
            if (inspectMemberAreas == null)
            {
                return HttpNotFound();
            }
            return View(inspectMemberAreas);
        }

        // POST: InspectMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InspectMemberAreas inspectMemberAreas = db.InspectMemberAreas.Find(id);
            db.InspectMemberAreas.Remove(inspectMemberAreas);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
