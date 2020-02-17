using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ddacAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ddacAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private UserManager<Customer> _userManager;
        private SignInManager<Customer> _signInManager;

        public CustomerController(UserManager<Customer> userManager, SignInManager<Customer> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

        }

        [HttpPost]
        [Route("signup")]
        //POST : /api/Customer/signup
        public async Task<Object> PostCustomer(CustomerSignUpModel model)
        {
            var customerToSignup = new Customer()
            {
                UserName = model.Username,
                Email = model.Email,

            };

            try
            {
                var result = await _userManager.CreateAsync(customerToSignup, model.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}