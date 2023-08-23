using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FirebaseAdmin;
using Firebase.Auth;

namespace griffined_api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]

    public class AuthController : ControllerBase
    {
        readonly FirebaseAuthProvider firebaseAuthProvider = new(new FirebaseConfig(Environment.GetEnvironmentVariable("FIREBASE_API_KEY")));


        [HttpPost("login")]
        public async Task<ActionResult> Login(UserDto request)
        {
            try
            {
                FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.SignInWithEmailAndPasswordAsync(request.Email, request.Password);
                var response = new TokenResponseDto
                {
                    AccessToken = firebaseAuthLink.FirebaseToken
                };
                return Ok(response);

            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reset-password"), Authorize(Roles = "oa, ea, ec, master")]
        public async Task<ActionResult> ResetPassword(string email)
        {
            await firebaseAuthProvider.SendPasswordResetEmailAsync(email);
            return Ok("Success");
        }


    }
}