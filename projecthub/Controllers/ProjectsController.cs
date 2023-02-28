using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> PutProject(long id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Projects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
          if (_context.Projects == null)
          {
              return Problem("Entity set 'ProjectContext.Projects'  is null.");
          }
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.Id }, project);
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

        private ProjectDTO projectToDto(Project proj)
        {
            return new ProjectDTO
            {
                Id = proj.Id,
                Name = proj.Name,
                Description = proj.Description,
                Imagesurls = proj.Imagesurls,
                Ytlink = proj.Ytlink,
                Creator = proj.Creater!.Name,
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
