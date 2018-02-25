using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]               // Add this here so we don't need to do it at each action
    //[Route("api/[controller]")]         // Can use [controller] to automatically use the name of the controller. Debatable if this is better or not for APIs
    public class CitiesController : Controller
    {
        private readonly ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet]
        //public JsonResult GetCities()
        // Change the signatures to return an ActionResult instead of a JsonResult to properly package the response which might not be Json.
        public IActionResult GetCities()
        {
            // The actions are working on the Dto models, not what is returned from the repository.

            var cityEntities = _cityInfoRepository.GetCities();

            var results = Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);

            // This is a very long-winded and error prone method of mapping. We are going to use Automapper instead.
            //foreach (var cityEntity in cityEntities)
            //{
            //    results.Add(new CityWithoutPointsOfInterestDto
            //    {
            //        Id = cityEntity.Id,
            //        Name = cityEntity.Name,
            //        Description = cityEntity.Description
            //    } );
            //}

            return Ok(results);
        }

        //[HttpGet("api/cities/{id}")]        // As api/cities is part of the routing template, we can just use {id}
        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false)
        {
            // Find city

            var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);
            if (city == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                // Map the results over.
                var cityResult = Mapper.Map<CityDto>(city);



                //var cityResult = new CityDto
                //{
                //    Id = city.Id,
                //    Name = city.Name,
                //    Description = city.Description
                //};

                //foreach (var poi in city.PointsOfInterest)
                //{
                //    cityResult.PointsOfInterest.Add(
                //        new PointOfInterestDto
                //        {
                //            Id = poi.Id,
                //            Name = poi.Name,
                //            Description = poi.Description
                //        });
                //}

                return Ok(cityResult);

            }

            // No points of interest are needed

            var cityWithoutPointsOfInterestResult = Mapper.Map<CityWithoutPointsOfInterestDto>(city);

            //var cityWithoutPointsOfInterestResult =
            //    new CityWithoutPointsOfInterestDto
            //    {
            //        Id = city.Id,
            //        Name = city.Name,
            //        Description = city.Description
            //    };

            return Ok(cityWithoutPointsOfInterestResult);
        }
    }
}
