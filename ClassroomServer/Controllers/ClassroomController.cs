using ClassLibrary;
using ClassroomServer.Logic;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomServer.Controllers
{
    /// <summary>
    /// Controller for handling classroom-related API requests, such as voting, session status, and student generation.
    /// </summary>
    [ApiController]
    [Route("api/classroom")]
    public class ClassroomController : ControllerBase
    {
        private readonly ClassroomLogic _classroomLogic;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassroomController"/> class.
        /// </summary>
        /// <param name="classroomLogic">The logic instance used for managing classroom operations.</param>
        public ClassroomController(ClassroomLogic classroomLogic)
        {
            _classroomLogic = classroomLogic;
        }

        /// <summary>
        /// Gets the current class session status.
        /// </summary>
        /// <returns>A boolean indicating whether the class is in session.</returns>
        [HttpGet("status")]
        public ActionResult<bool> GetClassStatus()
        {
            return Ok(_classroomLogic.IsClassInSession());
        }

        /// <summary>
        /// Retrieves the next unique ID from the server for identifying doors or teachers.
        /// </summary>
        /// <returns>An integer representing a unique ID.</returns>
        [HttpGet("getUniqueId")]
        public ActionResult<int> GetUniqueId()
        {
            int uniqueId = _classroomLogic.GetUniqueId();
            return Ok(uniqueId);
        }

        /// <summary>
        /// Checks if there are enough students to start the voting process.
        /// </summary>
        /// <returns>A boolean indicating if voting to start the class can begin.</returns>
        [HttpGet("enoughstudents")]
        public ActionResult<bool> EnoughStudents()
        {
            bool result = _classroomLogic.canvotingstart();
            return Ok(result);
        }

        /// <summary>
        /// Allows a teacher to cast a vote to start the class.
        /// </summary>
        /// <param name="teacher">The teacher casting the vote.</param>
        /// <returns>A boolean indicating if the vote to start the class was successful.</returns>
        [HttpPost("start")]
        public ActionResult<bool> StartClass([FromBody] Teacher teacher)
        {
            if (teacher == null)
            {
                return BadRequest("Teacher data is required.");
            }

            bool result = _classroomLogic.VoteToStartClass(teacher);
            return Ok(result);
        }

        /// <summary>
        /// Allows a teacher to cast a vote to end the class.
        /// </summary>
        /// <param name="teacher">The teacher casting the vote.</param>
        /// <returns>A boolean indicating if the vote to end the class was successful.</returns>
        [HttpPost("end")]
        public ActionResult<bool> EndClass([FromBody] Teacher teacher)
        {
            if (teacher == null)
            {
                return BadRequest("Teacher data is required.");
            }

            bool result = _classroomLogic.VoteToEndClass(teacher);
            return Ok(result);
        }

        /// <summary>
        /// Increments the number of students generated at the door.
        /// </summary>
        /// <param name="door">The door object containing the number of generated students.</param>
        /// <returns>A success message if students are generated, or a bad request if door data is missing.</returns>
        [HttpPost("generate")]
        public ActionResult GenerateStudents([FromBody] Door door)
        {
            if (door == null)
            {
                return BadRequest("Door data is required.");
            }

            _classroomLogic.studnetnsnumber(door.AmountOfStudents);
            return Ok();
        }

        /// <summary>
        /// Allows a teacher to vote to start the class session.
        /// </summary>
        /// <param name="teacher">The teacher casting the vote to start.</param>
        /// <returns>A boolean indicating if the vote to start the class was counted.</returns>
        [HttpPost("vote/start")]
        public ActionResult<bool> VoteStartClass([FromBody] Teacher teacher)
        {
            if (teacher == null)
            {
                return BadRequest("Teacher data is required.");
            }

            bool result = _classroomLogic.VoteToStartClass(teacher);
            return Ok(result);
        }

        /// <summary>
        /// Allows a teacher to vote to end the class session.
        /// </summary>
        /// <param name="teacher">The teacher casting the vote to end.</param>
        /// <returns>A boolean indicating if the vote to end the class was counted.</returns>
        [HttpPost("vote/end")]
        public ActionResult<bool> VoteEndClass([FromBody] Teacher teacher)
        {
            if (teacher == null)
            {
                return BadRequest("Teacher data is required.");
            }

            bool result = _classroomLogic.VoteToEndClass(teacher);
            return Ok(result);
        }

    }
}
