using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CityInfo.API.Controllers
{
    [Route("api/citiesIM")]               // Add this here so we don't need to do it at each action
    //[Route("api/[controller]")]         // Can use [controller] to automatically use the name of the controller. Debatable if this is better or not for APIs
    public class CitiesControllerInMemory : Controller
    {
        [HttpGet]
        //public JsonResult GetCities()
        // Change the signatures to return an ActionResult instead of a JsonResult to properly package the response which might not be Json.
        public IActionResult GetCities()
        {
            return Ok(CitiesDataStore.Current.Cities);
        }

        //[HttpGet("api/cities/{id}")]        // As api/cities is part of the routing template, we can just use {id}
        [HttpGet("{id}")]
        public IActionResult GetCity(int id)
        {
            // Find city

            var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
            if (cityToReturn == null)
            {
                return NotFound();              // NotFound and OK are helpers
            }

            return Ok(cityToReturn);
        }
    }
}
