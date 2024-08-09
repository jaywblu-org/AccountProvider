using AccountProvider.Models;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AccountProvider.Functions;

public class VerifyUser(ILogger<VerifyUser> logger, UserManager<UserEntity> userManager)
{
    private readonly ILogger<VerifyUser> _logger = logger;
    private readonly UserManager<UserEntity> _userManager = userManager;

    [Function("VerifyUser")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            if (body != null)
            {
                var verificationRequest = JsonConvert.DeserializeObject<VerificationRequest>(body);
                if (verificationRequest != null && ValidateVerificationRequest(verificationRequest))
                {
                    // verify code
                    var isVerified = true;

                    if (isVerified)
                    {
                        var user = await _userManager.FindByEmailAsync(verificationRequest.Email);
                        if (user != null)
                        {
                            user.EmailConfirmed = true;
                            var result = await _userManager.UpdateAsync(user);
                            
                            if (result.Succeeded)
                            {
                                return new OkResult();
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: VerifyUser.Run :: {ex.Message}");
        }

        return new UnauthorizedResult();
    }

    private bool ValidateVerificationRequest(VerificationRequest request)
    {
        return
            !string.IsNullOrEmpty(request.Email) &&
            !string.IsNullOrEmpty(request.VerificationCode);
    }
}
