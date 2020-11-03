using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/v{version:apiVersion}/camps")]
    [ApiVersion("2.0")]
    [ApiController]
    public class Camps2Controller : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public Camps2Controller(ICampRepository campRepository,
                               IMapper mapper,
                               LinkGenerator linkGenerator)
        {
            _campRepository = campRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> Get(bool includeTalks = false)
        {
            try
            {
                var results = await _campRepository.GetAllCampsAsync(includeTalks);
                var result = new
                {
                    Count = results.Count(),
                    Results = _mapper.Map<CampModel[]>(results)
                };

            return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                var result = await _campRepository.GetCampAsync(moniker);
                if (result == null) return NotFound();

                return _mapper.Map<CampModel>(result);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

       

        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var results = await _campRepository.GetAllCampsByEventDate(theDate, includeTalks);

                if (!results.Any()) return NotFound();

                return _mapper.Map<CampModel[]>(results);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                var existing = await _campRepository.GetCampAsync(model.Moniker);
                if(existing != null)
                {
                    return BadRequest("Moniker in Use!");
                }

                var location = _linkGenerator.GetPathByAction("Get", "Camps", new { moniker = model.Moniker });

                if(string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use current moniker");
                }
                 
                // Create new Camp
                var camp = _mapper.Map<Camp>(model);
                _campRepository.Add(camp);

                if (await _campRepository.SaveChangesAsync())
                {
                    //return Created($"/api/camps/{camp.Moniker}", _mapper.Map<CampModel>(camp));
                    return Created(location, _mapper.Map<CampModel>(camp));
                }

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                var oldCamp = await _campRepository.GetCampAsync(moniker);
                if (oldCamp == null) return NotFound($"Could not find camp with moniker of {moniker}");

                _mapper.Map(model, oldCamp);

                if(await _campRepository.SaveChangesAsync())
                {
                    return _mapper.Map<CampModel>(oldCamp);
                }

                return BadRequest("Failed to update the camp");

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var oldCamp = await _campRepository.GetCampAsync(moniker);
                if (oldCamp == null) return NotFound($"Could not find camp with moniker of {moniker}");

                _campRepository.Delete(oldCamp);

                if (await _campRepository.SaveChangesAsync())
                {
                    return Ok();
                }

                return BadRequest("Failed to delete the camp");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }
    }
}
