using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.Controllers
{
    [Route("api/citiesIM")]
    public class PointsOfInterestControllerInMemory : Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;

        public PointsOfInterestControllerInMemory(ILogger<PointsOfInterestController> logger, IMailService mailService)
        {
            _logger = logger;
            _mailService = mailService;
        }

        [HttpGet("{cityId}/pointsofinterest")] // We need this route to get the points of interest for a specific city
        public IActionResult GetPointsOfInterestIM(int cityId)
        {
            try
            {
                //throw new Exception("Exception sample");

                var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound();
                }

                return Ok(city.PointsOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);
                return StatusCode(500, "A problem happened while handling your request.");      // WARNING - this will go to consumer of API so be sure not to return any stack trace or implementation information
            }
        }

        [HttpGet("{cityId}/pointofinterest/{id}", Name =
            "GetPointOfInterestIM")] // We need this route to get the points of interest for a specific city. Named so it can be used in create below
        public IActionResult GetPointOfInterestIM(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterestIM(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            // If we can't deserialise the point of interest from the body, this is the consumer's responsibility, so we just return BadRequest.
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            // This is adding rules to an additional place (model and controller), which is not ideal. It breaks the separation of concerns concept. It is the default approach, however.
            // Possibly check out FluentValidation which is a validation library for .NET that uses a fluent interface and lambda expressions for building validation rules
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            // ModelState.IsValid checks the attributes on the model (required, maxlength, etc) for validity.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // If we can't find the city, return NotFound, as before.
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            // We need to map the PointOfInterestForCreationDto to the PointOfInterestDto, which will be stored.

            // demo level hack...to be improved
            var maxPointOfInterestId =
                CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            // "GetPointOfInterest" is named in the attribute for GetPointOfInterest action.
            // Use an anonymous type to pass the parameters for the action.
            return CreatedAtRoute("GetPointOfInterest", new {cityId = cityId, id = finalPointOfInterest.Id},
                finalPointOfInterest);
        }

        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterestIM(int cityId, int id, [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            // If we can't deserialise the point of interest from the body, this is the consumer's responsibility, so we just return BadRequest.
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            // This is adding rules to an additional place (model and controller), which is not ideal. It breaks the separation of concerns concept. It is the default approach, however.
            // Possibly check out FluentValidation which is a validation library for .NET that uses a fluent interface and lambda expressions for building validation rules
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            // ModelState.IsValid checks the attributes on the model (required, maxlength, etc) for validity.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            // If we can't find the city, return NotFound, as before.
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            // The problem here is that we have to provide ALL the fields, instead of just the ones we want to update.
            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

            // We aren't returning anything new. Possibly return a 200 with the updated resource, but the consumer already knows this.
            return NoContent();
        }

        // This is using a JsonPatch document to send through a list of changes to be applied.
        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterestIM(int cityId, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            // If we can't deserialise the patch document from the body, just return BadRequest.
            if (patchDoc == null)
            {
                return BadRequest();
            }

            // If we can't find the city, return NotFound, as before.
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            // Need to transform the pointOfInterestDto we got from the store to a pointOfInterestForUpdateDto that we got from the PATCH request
            var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };

            // We can pass in the ModelState to get the validation that has been set up on the model.
            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            // And then check it's valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            TryValidateModel(pointOfInterestToPatch);

            // And then check it's valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterestIM(int cityId, int id)
        {
            // If we can't find the city, return NotFound, as before.
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            
            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfInterestFromStore);

            _mailService.Send("Point of interest deleted.",
                $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");

            return NoContent();
        }

    }
}
