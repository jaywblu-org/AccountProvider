using AccountProvider.Models;
using Azure.Core;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AccountProvider.Functions;

public class SignIn(ILogger<SignIn> logger, SignInManager<UserEntity> signInManager)
{
    private readonly ILogger<SignIn> _logger = logger;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;

    [Function("SignIn")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            if (body != null)
            {
                var signInRequest = JsonConvert.DeserializeObject<UserSignInRequest>(body);
                if (signInRequest != null && ValidateUserSignInRequest(signInRequest))
                {
                    var result = await _signInManager.PasswordSignInAsync(signInRequest.Email, signInRequest.Password, signInRequest.RememberMe, false);
                    if (result.Succeeded)
                    {
                        // Get token from TokenProvider

                        return new OkObjectResult("accesstoken");
                    }
                    return new UnauthorizedResult();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: SignIn.Run :: {ex.Message}");
        }

        return new BadRequestResult();
    }

    private bool ValidateUserSignInRequest(UserSignInRequest request)
    {
        return
            string.IsNullOrEmpty(request.Email) &&
            string.IsNullOrEmpty(request.Password);
    }
}
