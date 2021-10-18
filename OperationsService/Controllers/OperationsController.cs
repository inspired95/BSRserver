using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OperationsService.Data;
using OperationsService.Data.Models;


namespace OperationsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly ILogger<OperationsController> _logger;
        private readonly DatabaseContext _context;


        public OperationsController(ILogger<OperationsController> logger, DatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Operation> Get()
        {
            _logger.LogInformation("Endpoint: Get.");
            return _context.Operations.ToList();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Operation))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(int id)
        {
            _logger.LogInformation($"Endpoint: Get/{id}.");

            var operation = _context.Operations.SingleOrDefault(operation => operation.Id == id);

            if (operation == null)
            {
                _logger.LogInformation($"Operation with id={id} not found.");
                return NotFound();
            }
            _logger.LogInformation($"Operation with id={id} found. {operation}");
            return Ok(operation);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Operation))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Post([FromBody] Operation operation)
        {
            _logger.LogInformation($"Endpoint: Post. {operation}");
            _context.Operations.Add(operation);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception caught. Post of operation {operation} failed. Reason:{e.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return CreatedAtAction(nameof(Post), new { id = operation.Id }, operation);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Operation))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Put(int id, [FromBody] Operation updatedOperation)
        {
            _logger.LogInformation($"Endpoint: Put/{id}. new");

            var operationToUpdate = _context.Operations.SingleOrDefault(operation => operation.Id == id);

            if (operationToUpdate == null)
            {
                _logger.LogInformation($"Operation with id={id} not found.");
                return NotFound();
            }

            operationToUpdate.Date = updatedOperation.Date;
            operationToUpdate.Amount = updatedOperation.Amount;
            operationToUpdate.Bank = updatedOperation.Bank;
            operationToUpdate.Description = updatedOperation.Description;
            operationToUpdate.Type = updatedOperation.Type;
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception caught. Put of operation with id={id}, oldOperation={operationToUpdate}, newOperation={updatedOperation} failed. Reason: {e.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(updatedOperation);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(int id)
        {
            Operation customer = new Operation() { Id = id };
            _context.Operations.Attach(customer);
            _context.Operations.Remove(customer);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception caught. Delete of operation with id={id} failed. Reason: {e.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok();
        }
    }
}
