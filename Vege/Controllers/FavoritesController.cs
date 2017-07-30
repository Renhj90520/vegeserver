using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vege.Models;
using Vege.Repositories;
using Vege.DTO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FavoritesController : Controller
    {
        private IVegeRepository vegeRepository;
        private ILogger<FavoritesController> log;

        public FavoritesController(IVegeRepository vegeRepository, ILogger<FavoritesController> log)
        {
            this.vegeRepository = vegeRepository;
            this.log = log;
        }
        // GET: api/values
        [HttpGet("{openid}/{productid?}")]
        public async Task<IActionResult> Get(string openid, int? productid)
        {
            Result<IEnumerable<FavoriteDTO>> result = new Result<IEnumerable<FavoriteDTO>>();
            try
            {
                result.body = await this.vegeRepository.getFavorites(openid, productid);
                result.state = 1;
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Ok(result);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Favorite fav)
        {
            Result<Favorite> result = new Result<Favorite>();
            try
            {
                result.body = await this.vegeRepository.AddFavorite(fav);
                result.state = 1;
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Ok(result);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                if (await this.vegeRepository.DeleteFavorite(id))
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
