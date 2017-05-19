﻿using System;
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
    public class CategoriesController : Controller
    {
        private IVegeRepository vegeRepository;
        public CategoriesController(IVegeRepository vegeRepository)
        {
            this.vegeRepository = vegeRepository;
        }
        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Category category)
        {
            Result<Category> result = new Result<Category>();
            try
            {
                if (await this.vegeRepository.AddCategory(category))
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
    }
}