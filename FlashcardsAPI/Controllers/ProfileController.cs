using FlashcardsAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IFlashcardsAppDbService _flashcardsAppDbService;

        public ProfileController(IFlashcardsAppDbService service)
        {
            _flashcardsAppDbService = service;
        }
         
      
    }
}
