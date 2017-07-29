using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vege.Models;
using Vege.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UnitsController : Controller
    {
        private IVegeRepository vegeRepository;
        private ILogger<UnitsController> log;

        public UnitsController(IVegeRepository vegeRepository, ILogger<UnitsController> log)
        {
            this.vegeRepository = vegeRepository;
            this.log = log;
        }
        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Unit unit)
        {
            Result<Unit> result = new Result<Unit>();
            try
            {
                var un = await this.vegeRepository.AddUnit(unit);
                if (un != null)
                {
                    result.body = un;
                    result.state = 1;
                }
                else
                {
                    result.state = 0;
                }
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }

            return Ok(result);
        }

        public async Task<IActionResult> GetAllUnits()
        {
            Result<IEnumerable<Unit>> result = new Result<IEnumerable<Unit>>();
            try
            {
                result.body = await this.vegeRepository.GetAllUnits();
                result.state = 1;
            }
            catch (Exception ex)
            {
                result.body = null;
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUnit([FromBody] Unit unit)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                if (await this.vegeRepository.UpdateUnit(unit))
                {
                    result.state = 1;
                }
                else
                {
                    result.state = 0;
                }
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Ok(result);
        }
    }
}
