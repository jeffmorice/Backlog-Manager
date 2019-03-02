﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BacklogManager.Common;
using BacklogManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BacklogManager.Controllers
{   
    public class OMDbController : Controller
    {
        //Class containing API key
        OMDbClient omdbClient = new OMDbClient();
        //Hosted web API REST Service base url
        string baseUrl = "https://www.omdbapi.com/";

        //ToDo: add parameters title, year, and type
        //ToDo: add request string contruction logic
        //ToDo: Pass List of OMDbTitle objects to Search view.
        [HttpGet]     //needs to be a get
        public async Task<ActionResult> Index(string title, string year, string type, bool search)
        {
            //use try, except here to catch network errors and user input errors
            List<OMDbTitle> omdbTitles = new List<OMDbTitle>();

            string requestString = baseUrl + "?apikey=" + omdbClient.ApiKey;

            //construct request string
            if (search == true)
            {
                if(title != null)
                {
                    requestString = requestString + "&s=" + title;
                    if (year != null)
                    {
                        requestString = requestString + "&y=" + year;
                    }
                    if (type != null)
                    {
                        requestString = requestString + "&type=" + type;
                    }
                }
            }


            using (var client = new HttpClient())
            {
                //Passing service base url
                client.BaseAddress = new Uri(baseUrl);

                client.DefaultRequestHeaders.Clear();

                //Define request data format (default for omdb is JSON, may be extraneous)
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource
                //I predict I will need to tweak how the request string is constructed.
                HttpResponseMessage response = await client.GetAsync(requestString);

                //Checking if the response, sent using HttpClient, is successful or not
                if (response.IsSuccessStatusCode)
                {
                    omdbTitles = JsonToListOmdbTitles(response);
                }
                //returning OMDbTitle list to view
                return View("Search", omdbTitles);
                //return View("Index", responseString);
            }
        }

        private List<OMDbTitle> JsonToListOmdbTitles(HttpResponseMessage response)
        {
            List<OMDbTitle> omdbTitles = new List<OMDbTitle>();

            //Storing the response details received from web api
            string responseString = response.Content.ReadAsStringAsync().Result;

            //if the string begins with '{"Search":', extract just the search results
            if (responseString.Substring(0,10) == "{\"Search\":")
                {
                string[] searchArray = responseString.Split(new char[] { '[', ']' });

                responseString = searchArray[1];
                }
            
            //add brackets to turn the response string into an array
            responseString = "[" + responseString + "]";

            //Deserializing the response received from web api and storing in the OMDbTitle list
            omdbTitles = JsonConvert.DeserializeObject<List<OMDbTitle>>(responseString);

            return omdbTitles;
        }
    }
}