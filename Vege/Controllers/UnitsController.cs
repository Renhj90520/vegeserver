using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vege.Models;
using Vege.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Route("api/[controller]")]
    public class UnitsController : Controller
    {
        private IVegeRepository vegeRepository;

        public UnitsController(IVegeRepository vegeRepository)
        {
            this.vegeRepository = vegeRepository;
        }
        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Unit unit)
        {
            Result<Unit> result = new Result<Unit>();
            try
            {
                if (await this.vegeRepository.AddUnit(unit))
                {
                    result.State = 1;
                }
                else
                {
                    result.State = 0;
                }
            }
            catch (Exception ex)
            {
                result.State = 0;
                result.Message = ex.Message;
            }

            return Ok(result);
        }

        public async Task<IActionResult> GetAllUnits()
        {
            Result<IEnumerable<Unit>> result = new Result<IEnumerable<Unit>>();
            try
            {
                result.Body = await this.vegeRepository.GetAllUnits();
                result.State = 1;
            }
            catch (Exception ex)
            {
                result.Body = null;
                result.State = 0;
                result.Message = ex.Message;
            }
            return Ok(result);
        }
    }
}
