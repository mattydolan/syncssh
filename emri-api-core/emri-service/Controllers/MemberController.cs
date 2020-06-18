using System.Collections.Generic;
using DddsUtils.Logging.NetStandard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace emri_service.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class MemberController : ControllerBase
	{
		private readonly ILogger _logger;
		private const string loggerName = "MemberApi";

		public MemberController(ILogFactory logFactory)
		{
			_logger = logFactory.GetLogger(loggerName);
		}

		// GET: api/<controller>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public IEnumerable<string> Get()
		{
			_logger.Debug("Retrieve Member data");
			return new string[] { "TODO Return Member Data" };
		}

		// GET api/<controller>/5
		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public string Get(int id)
		{
			return "value";
		}

		// POST api/<controller>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult Post([FromBody] string value)
		{
			return CreatedAtAction(nameof(Post), value);
		}

		// PUT api/<controller>/5
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult Put(int id, [FromBody] string value)
		{
			return NoContent();
		}

		// DELETE api/<controller>/5
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult Delete(int id)
		{
			return NoContent();
		}
	}
}
