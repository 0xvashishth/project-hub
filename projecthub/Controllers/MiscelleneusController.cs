using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projecthub.Models;

namespace projecthub.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MiscelleneusController : ControllerBase
    {
        private readonly ProjectContext _context;
        private readonly IConfiguration _configuration;

        public MiscelleneusController(ProjectContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration=configuration;
        }

        // GET: api/Miscelleneus/number
        [HttpGet("{number}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetRecentProjects(long number)
        {
            if (_context.Projects == null)
            {
                return NotFound();
            }

            return await _context.Projects.Select(x => projectToDto(x)).Take(Convert.ToInt32(number)).ToListAsync();
        }

        private static ProjectDTO projectToDto(Projects proj)
        {
            Console.WriteLine(proj.CreaterId);
            return new ProjectDTO
            {
                Id = proj.Id,
                Name = proj.Name,
                Description = proj.Description,
                Imagesurls = proj.Imagesurls,
                Creator = (proj.CreaterId).ToString(),
                Ytlink = proj.Ytlink,
                Likes = proj.Likes,
                Reports = proj.Reports
            };
        }


        private bool ProjectsExists(long id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
