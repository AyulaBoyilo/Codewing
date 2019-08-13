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
    public class PredbiljezbeController : Controller
    {

        private WebStranicaEntities db = new WebStranicaEntities();

        [Authorize]
        // GET: Predbiljezbe
        public ActionResult Pregled(string searchString)
        {

            IEnumerable<PregledPredbiljezbi> predbiljezbe = (from p in db.Predbiljezba
                               join s in db.Seminar on p.IdSeminar equals s.IdSeminar
                               join k in db.Korisnik on p.IdKorisnik equals k.IdKorisnik
                               select new PregledPredbiljezbi{
                                   IdPredbiljezbe = p.IdPredbiljezbe,
                                   Naziv = s.Naziv,
                                   Datum = s.Datum,
                                   Ime = k.Ime,
                                   Prezime = k.Prezime,
                                   Adresa = k.Adresa,
                                   Email = k.Email,
                                   Telefon = k.Telefon,
                                   Stanje = p.Stanje,
                                   Popunjen = s.Popunjen
                               }).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                predbiljezbe = predbiljezbe.Where(x => x.Naziv.Contains(searchString) || x.Ime.Contains(searchString) || x.Prezime.Contains(searchString));
            }

            return View(predbiljezbe);
        }

        [Authorize]
        public ActionResult Uredi(int id)
        {

            PregledPredbiljezbi predbiljezba = (from p in db.Predbiljezba
                               join s in db.Seminar on p.IdSeminar equals s.IdSeminar
                               join k in db.Korisnik on p.IdKorisnik equals k.IdKorisnik
                               where p.IdPredbiljezbe == id
                               select new PregledPredbiljezbi
                               {
                                   IdPredbiljezbe = p.IdPredbiljezbe,
                                   Naziv = s.Naziv,
                                   Datum = s.Datum,
                                   Ime = k.Ime,
                                   Prezime = k.Prezime,
                                   Adresa = k.Adresa,
                                   Email = k.Email,
                                   Telefon = k.Telefon,
                                   Stanje = p.Stanje,
                                   Popunjen = s.Popunjen
                               }).SingleOrDefault();

            var seminar = db.Predbiljezba.Find(id);

            IEnumerable<PregledPredbiljezbi> potvrde = (from p in db.Predbiljezba
                                join s in db.Seminar on p.IdSeminar equals s.IdSeminar
                                join k in db.Korisnik on p.IdKorisnik equals k.IdKorisnik
                                where p.Stanje == true && s.IdSeminar == seminar.IdSeminar
                                select new PregledPredbiljezbi
                                {

                                }).ToList();

            int brojPotvrda = 0;

            foreach (PregledPredbiljezbi item in potvrde)
            {
                brojPotvrda++;
            }

            ViewBag.BrojPotvrda = brojPotvrda;
            Session["IdPredbiljezbe"] = predbiljezba.IdPredbiljezbe;

            return View(predbiljezba);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Uredi(string stanje)
        {

            var predbiljezba = db.Predbiljezba.Find(Session["IdPredbiljezbe"]);
            Session["IdPredbiljezbe"] = null;

            if (stanje == "prihvacena")
            {
                predbiljezba.Stanje = true;
                db.Entry(predbiljezba).State = EntityState.Modified;
                db.SaveChanges();
            }
            else if (stanje == "odbijena")
            {
                predbiljezba.Stanje = false;
                db.Entry(predbiljezba).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Pregled", "Predbiljezbe");
        }


        [AllowAnonymous]
        public ActionResult Nedostupna()
        {
            return View();
        }
    }

}