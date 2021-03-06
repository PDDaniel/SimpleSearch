﻿using SimpleSearch.Factories;
using SimpleSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleSearch.Controllers
{
    public class HomeController : Controller
    {
        ItemFactory itemFac = new ItemFactory();
        ArticleFactory articleFac = new ArticleFactory();

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        /* Et view der kan vise alle items, så længe det er en liste.
         * Denne bliver brugt både til at liste alle items i databasen,
         * men også for at vise søgeresultat. -> Dette kan ses via if-sætningen
         * TempData["SimpleSearchResult"] == null etc... Hvis der findes et søge
         * resultat, så viser vi en liste med søgeresultatet, ellers viser vi alle items */
        public ActionResult ShowAllItems()
        {
            if (TempData["SimpleSearchResult"] == null)
            {
                return View(itemFac.GetAll());
            }
            else
            {
                List<Item> simpleSearchResult = TempData["SimpleSearchResult"] as List<Item>;
                return View(simpleSearchResult);
            }
        }

        /* Simpel Detail Side */
        public ActionResult ShowItem(int id = 0)
        {
            return View(itemFac.Get(id));
        }

        /* Når vi via søgefunktionen på Layout'et søger bliver denne metode kørt.
         * Igennem QueryStrings henter vi brugerens søgeord og filtrere en liste ny liste med søgeresultatet */
        public ActionResult SearchSubmit()
        {
            // Henter brugers søgeord
            string nameQuery = Request.QueryString["name"].ToString();
            int categoryQuery = int.Parse(Request.QueryString["categoryid"].ToString());

            if (categoryQuery > 0)
            {
                TempData["SimpleSearchResult"] = itemFac.GetAll()
                    .Where(x =>
                    x.Name.ToLower().Contains(nameQuery.ToLower())
                    &&
                    x.CategoryID == categoryQuery
                    ).ToList();
            }
            else
            {
                TempData["SimpleSearchResult"] = itemFac.GetAll()
                    .Where(x =>
                    x.Name.ToLower().Contains(nameQuery.ToLower())
                    ).ToList();
            }

            // Gemmer en liste, der indeholder mængden af svar på søgningen


            return RedirectToAction("ShowAllItems");
        }

        public ActionResult ShowArticle(int id)
        {
            return View(articleFac.Get(id));
        }

        public ActionResult AdvancedSearch()
        {
            if (TempData["AdvancedSearchResult"] == null)
            {
                List<object> allSearchableItems = new List<object>();
                allSearchableItems.AddRange(itemFac.GetAll());
                allSearchableItems.AddRange(articleFac.GetAll());
                return View(allSearchableItems);
            }
            else
            {
                return View((TempData["AdvancedSearchResult"] as List<object>));
            }
        }

        public ActionResult AdvancedSearchSubmit()
        {
            int contentID = int.Parse(Request.QueryString["contentID"].ToString());
            string searchQuery = Request.QueryString["searchQuery"].ToString();

            if (searchQuery != null)
            {
                searchQuery = searchQuery.Trim();
            }

            List<object> advancedSearchResult = new List<object>();

            // Hvis contentID < end 1, så skal vi søge i alle tabeller.
            if (contentID < 1)
            {
                List<Item> itemResults = itemFac.GetAll()
                    .Where(x => x.Name.ToLower().Contains(searchQuery.ToLower()))
                    .ToList();
                advancedSearchResult.AddRange(itemResults);
                List<Article> articlesResults = articleFac.GetAll()
                    .Where(x => x.Title.ToLower().Contains(searchQuery.ToLower()))
                    .ToList();
                advancedSearchResult.AddRange(articlesResults);
            }
            else if (contentID == 1) // Hvis contentID er 1, vil det sige at brugeren har valgt at søge på Item
            {
                List<Item> itemResults = itemFac.GetAll()
                    .Where(x => x.Name.ToLower().Contains(searchQuery.ToLower()))
                    .ToList();
                advancedSearchResult.AddRange(itemResults);
            }
            else if(contentID == 2) // Hvis contentID er 2, vil det sige at brugeren har valgt at søge på Article
            {
                List<Article> articlesResults = articleFac.GetAll()
                    .Where(x => x.Title.ToLower().Contains(searchQuery.ToLower()))
                    .ToList();
                advancedSearchResult.AddRange(articlesResults);
            }


            TempData["AdvancedSearchResult"] = advancedSearchResult;

            return RedirectToAction("AdvancedSearch");
        }
    }
}