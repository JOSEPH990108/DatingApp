﻿using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class ErrorController : BaseApiController
    {
    private readonly ApplicationDbContext _db;
    public ErrorController(ApplicationDbContext db)
    {
        _db = db;
    }

    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetSecret()
    {
        return "secret text";
    }

    [HttpGet("not-found")]
    public ActionResult<ApplicationUser> GetNotFound()
    {
        var thing = _db.Users.Find(-1);

        if(thing == null)
            return NotFound();
        
        return thing;
    }

    [HttpGet("server-error")]
    public ActionResult<string> GetServerError()
    {
        var thing = _db.Users.Find(-1);

        var thingToReturn = thing.ToString();

        return thingToReturn;
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("This was not a good request");
    }
}
}


