using Common;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IPublishEndpoint publishEndpoint;
        private readonly ILogger<EmployeeController> logger;

        public EmployeeController(IPublishEndpoint publishEndpoint,ILogger<EmployeeController> logger)
        {
            this.publishEndpoint = publishEndpoint;
            this.logger = logger;
        }

        [HttpPost]
        public ActionResult<Employee> InsertEmployee(Employee employee)
        {
            logger.LogInformation("Employee Inserted:{employee}",employee);

           
            publishEndpoint.Publish<Employee>(employee);
            var activity = System.Diagnostics.Activity.Current;
            if (activity != null)
            {
                activity.AddTag("message.body", System.Text.Json.JsonSerializer.Serialize(employee));
            }

            return Ok(employee);
        }
    }

    
}
