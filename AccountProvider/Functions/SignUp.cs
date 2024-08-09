using AccountProvider.Models;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AccountProvider.Functions;

public class SignUp(ILogger<SignUp> logger, UserManager<UserEntity> userManager)
{
    private readonly ILogger<SignUp> _logger = logger;
    private readonly UserManager<UserEntity> _userManager = userManager;

    [Function("SignUp")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            if (body != null)
            {
                var registrationRequest = JsonConvert.DeserializeObject<UserRegistrationRequest>(body);
                if (registrationRequest != null && ValidateUserRegistrationRequest(registrationRequest))
                {
                    if (!await _userManager.Users.AnyAsync(x => x.Email == registrationRequest.Email))
                    {
                        var userEntity = new UserEntity
                        {
                            FirstName = registrationRequest.FirstName,
                            LastName = registrationRequest.LastName,
                            Email = registrationRequest.Email,
                            UserName = registrationRequest.Email
                        };

                        var result = await _userManager.CreateAsync(userEntity, registrationRequest.Password);
                        if (result.Succeeded)
                        {
                            // send verification code

                            return new OkResult();
                        }
                    }
                    else
                    {
                        return new ConflictResult();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: SignUp.Run :: {ex.Message}");
        }

        return new BadRequestResult();
    }

    private bool ValidateUserRegistrationRequest(UserRegistrationRequest request)
    {
        return 
            !string.IsNullOrEmpty(request.Email) && 
            !string.IsNullOrEmpty(request.Password) && 
            !string.IsNullOrEmpty(request.FirstName) && 
            !string.IsNullOrEmpty(request.LastName);
    }
}
