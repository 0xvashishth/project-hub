using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using projecthub.Models;

namespace projecthub.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectContext _context;
        private readonly IConfiguration _configuration;

        public ProjectsController(ProjectContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration=configuration;
        }

        // GET: api/Projects
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjects()
        {
          if (_context.Projects == null)
          {
              return NotFound();
          }
            return await _context.Projects.Select(x => projectToDto(x)).ToListAsync();
            /*return await _context.Users.Select(x => UserToDto(x)).ToListAsync();*/
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProjectDTO>> GetProject(long id)
        {
          if (_context.Projects == null)
          {
              return NotFound();
          }
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return projectToDto(project);
        }


        // PUT: api/Projects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(long id, ProjectCreateDTO project)
        {
            if (id != project.Id)
            {
                return Ok();
            }

            var proj = await _context.Projects.FindAsync(id);
            if(proj == null)
            {
                return NotFound("Project Not Found");
            }

            proj.Ytlink = project.Ytlink;
            proj.Visibility = project.visibility;
            proj.Description = project.Description;
            proj.Name = project.Name;
            proj.Imagesurls = project.Imagesurls;

            _context.Entry(proj).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok("Project Updated Sucessfully");
        }

        // POST: api/Projects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProjectDTO>> PostProject(ProjectCreateDTO project, long creatorId)
        {
          if (_context.Projects == null)
          {
              return Problem("Entity set 'ProjectContext.Projects'  is null.");
          }

          if(creatorId != int.Parse(project.creator))
            {
                return BadRequest();
            }

            Projects proj = new Projects();
            proj.Name = project.Name;
            proj.Imagesurls = project.Imagesurls;
            proj.Description = project.Description;
            proj.Ytlink = project.Ytlink;
            proj.Visibility = project.visibility;
            User user = await _context.Users.FindAsync(creatorId);
            if(user == null)
            {
                return NotFound("Creator not found");
            }
            proj.Creater = user;
            proj.Likes = 0;
            proj.Reports=0;
            _context.Projects.Add(proj);
            await _context.SaveChangesAsync();

            return projectToDto(proj);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(long id)
        {
            if (_context.Projects == null)
            {
                return NotFound();
            }
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
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

        private bool ProjectExists(long id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
