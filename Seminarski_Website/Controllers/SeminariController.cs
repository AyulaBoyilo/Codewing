using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Seminarski_Website.Models;

namespace Seminarski_Website.Controllers
{
    public class SeminariController : Controller
    {
        private WebStranicaEntities db = new WebStranicaEntities();

        // GET: Seminari/Pretraga
        [Authorize]
        public ActionResult Pretraga(string searchString)
        {
            var seminari = from s in db.Seminar select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                seminari = seminari.Where(x => x.Naziv.Contains(searchString) || x.Opis.Contains(searchString));
            }

            List<int> BrojPotvrda = new List<int>();

            foreach (Seminar sem in db.Seminar)
            {
                IEnumerable<PregledPredbiljezbi> potvrde = (from p in db.Predbiljezba
                                                            join s in db.Seminar on p.IdSeminar equals s.IdSeminar
                                                            join k in db.Korisnik on p.IdKorisnik equals k.IdKorisnik
                                                            where p.Stanje == true && s.IdSeminar == sem.IdSeminar
                                                            select new PregledPredbiljezbi
                                                            {

                                                            }).ToList();

                int brojPotvrda = 0;

                foreach (PregledPredbiljezbi pred in potvrde)
                {
                    brojPotvrda++;
                }

                BrojPotvrda.Add(brojPotvrda);
            }

            ViewBag.BrojPotvrda = BrojPotvrda;

            return View(seminari);
        }

        // GET: Seminari/Stvori
        [Authorize]
        public ActionResult Stvori()
        {
            return View();
        }

        // POST: Seminari/Stvori
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Stvori([Bind(Include = "IdSeminar, Naziv, Datum, Popunjen, Opis")] Seminar seminar)
        {
            if (ModelState.IsValid)
            {
                db.Seminar.Add(seminar);
                db.SaveChanges();
                return RedirectToAction("Pretraga", "Seminari");
            }
            return View(seminar);
        }

        // GET: Seminari/Uredi/5
        [Authorize]
        public ActionResult Uredi(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var seminar = db.Seminar.Find(id);

            IEnumerable<PregledPredbiljezbi> potvrde = (from p in db.Predbiljezba
                                                        join s in db.Seminar on p.IdSeminar equals s.IdSeminar
                                                        join k in db.Korisnik on p.IdKorisnik equals k.IdKorisnik
                                                        where p.Stanje == true && s.IdSeminar == seminar.IdSeminar
                                                        select new PregledPredbiljezbi { }).ToList();

            int brojPotvrda = 0;

            foreach (PregledPredbiljezbi pred in potvrde)
            {
                brojPotvrda++;
            }

            ViewBag.BrojPotvrda = brojPotvrda;

            if (seminar == null)
            {
                return HttpNotFound();
            }

            return View(seminar);
        }

        // POST: Seminari/Uredi/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Uredi([Bind(Include = "IdSeminar, Naziv, Datum, Popunjen, Opis")] Seminar seminar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(seminar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Pretraga", "Seminari");
            }
            return View(seminar);
        }

        // GET: Seminari/Obrisi/5
        [Authorize]
        public ActionResult Obrisi(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Seminar seminar = db.Seminar.Find(id);


            if (seminar == null)
            {
                return HttpNotFound();
            }
            return View(seminar);
        }

        // POST: Seminari/Obrisi/5
        [Authorize]
        [HttpPost, ActionName("Obrisi")]
        [ValidateAntiForgeryToken]
        public ActionResult ObrisiConfirmed(int id)
        {
            Seminar seminar = db.Seminar.Find(id);

            foreach (Predbiljezba pred in db.Predbiljezba)
            {
                if (seminar.IdSeminar == pred.IdSeminar)
                {
                    db.Predbiljezba.Remove(pred);
                }
            }

            db.Seminar.Remove(seminar);
            db.SaveChanges();

            return RedirectToAction("Pretraga");
        }

        [AllowAnonymous]
        public ActionResult Nedostupna()
        {
            return View();
        }
    }
}