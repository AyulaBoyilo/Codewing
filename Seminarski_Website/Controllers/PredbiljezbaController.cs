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
    public class PredbiljezbaController : Controller
    {
        private WebStranicaEntities db = new WebStranicaEntities();

        // GET: Predbiljezba
        public ActionResult Pretraga(string searchString)
        {
            var seminari = from s in db.Seminar select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                seminari = seminari.Where(x => x.Naziv.Contains(searchString) || x.Opis.Contains(searchString));
            }

            return View(seminari);
        }

        // GET: Predbiljezba
        public ActionResult CreateUser(int id)
        {
            Seminar seminar = db.Seminar.Find(id);
            Session["IdSeminar"] = id;
            Session["Seminar"] = seminar.Naziv;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser([Bind(Include = "IdKorisnik, Ime, Prezime, Adresa, Email, Telefon")] Korisnik korisnik)
        {
            bool duplikat = false;
            bool postoji = false;
            int postojeci = 0;
            Predbiljezba predbiljezba = new Predbiljezba();

            foreach (Korisnik kor in db.Korisnik)
            {
                if (kor.Ime == korisnik.Ime && kor.Prezime == korisnik.Prezime && kor.Adresa == korisnik.Adresa && kor.Email == korisnik.Email && kor.Telefon == korisnik.Telefon)
                {
                    postojeci = kor.IdKorisnik;
                    duplikat = true;
                    break;
                }
            }
            if (!duplikat)
            {
                if (ModelState.IsValid)
                {
                    db.Korisnik.Add(korisnik);
                    db.SaveChanges();
                }
                predbiljezba.IdKorisnik = korisnik.IdKorisnik;
            }
            else
            {
                predbiljezba.IdKorisnik = postojeci;
            }
            predbiljezba.IdSeminar = (int)Session["IdSeminar"];
            Session["IdSeminar"] = null;
            predbiljezba.Datum = DateTime.Now;
            predbiljezba.Stanje = null;


            foreach (Predbiljezba pred in db.Predbiljezba)
            {
                if (pred.IdKorisnik == predbiljezba.IdKorisnik && pred.IdSeminar == predbiljezba.IdSeminar)
                {
                    postoji = true;
                    break;
                }
            }
            if (postoji)
            {
                return RedirectToAction("Postoji", "Predbiljezba");
            }
            else
            {
                try
                {
                    db.Predbiljezba.Add(predbiljezba);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return RedirectToAction("Greska", "Predbiljezba");
                }
            }

            return RedirectToAction("Potvrda", "Predbiljezba");
        }

        // GET: Predbiljezba
        public ActionResult Potvrda()
        {
            return View();
        }

        // GET: Predbiljezba
        public ActionResult Postoji()
        {
            return View();
        }

        // GET: Predbiljezba
        public ActionResult Greska()
        {
            return View();
        }

    }
}