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
        FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(Environment.GetEnvironmentVariable("FIREBASE_API_KEY")));


        [HttpPost("login")]
        public async Task<ActionResult> Login(UserDto request)
        {
            try
            {
                FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.SignInWithEmailAndPasswordAsync(request.email, request.password);
                var response = new TokenResponseDto();
                response.accessToken = firebaseAuthLink.FirebaseToken;
                return Ok(response);

            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reset-password"), Authorize(Roles = "oa, ea, ep, master")]
        public async Task<ActionResult> ResetPassword(string email)
        {
            await firebaseAuthProvider.SendPasswordResetEmailAsync(email);
            return Ok("Success");
        }


    }
}