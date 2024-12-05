using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseAPIController
{
    [HttpPost("register")] //account/register
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {
        if (await UserExists(registerDTO.Username)) return BadRequest("Username is taken");

        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            UserName = registerDTO.Username.ToLower(),
            PasswaordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDTO
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)

        };
    }

    [HttpPost("login")] //account/login
    public async Task<ActionResult<UserDTO>> Login(loginDTO loginDTO)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == loginDTO.Username.ToLower());
        if (user == null) return Unauthorized("invalid username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswaordHash[i]) return Unauthorized("invalid password");
        }
        return new UserDTO
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }

    private async Task<bool> UserExists(string Username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == Username.ToLower()); //Bob != bob
    }

    

}
