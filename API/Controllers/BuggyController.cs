using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController(DataContext context): BaseAPIController
{
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuth()
    {
        return "screen text";
    }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
    {
        var thing = context.Users.Find(-1);

        if (thing == null) return NotFound();

        return thing;
    }
    
    [HttpGet("server-error")]
        public ActionResult<AppUser> GetServerError()
    {
        try{
        var thing = context.Users.Find(-1) ?? throw new Exception("Uh oh - somthing bad has happened");
        
        return thing;
    }
    catch{
        return StatusCode(500, "Computer says no");
    }
    }
[HttpGet("bad-request")]
        public ActionResult<AppUser> GetBadRequest()
    {
        return BadRequest ("Not a good request");
    }

}
