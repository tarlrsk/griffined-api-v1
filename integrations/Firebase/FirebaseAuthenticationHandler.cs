using System.Text.Encodings.Web;
using FirebaseAdmin;
using FirebaseAdmin.Auth;

namespace griffined_api.Integrations.Firebase
{
    public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly FirebaseApp _firebaseApp;
        private readonly DataContext _context;

        //private readonly FirebaseAuthenticationFunctionHandler _authenticationFunctionHandler;

        public FirebaseAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            FirebaseApp firebaseApp,
            DataContext context)
            : base(options, logger, encoder, clock)
        {
            _firebaseApp = firebaseApp;
            _context = context;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Context.Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("No Authorization");
            }

            string? bearerToken = Context.Request.Headers["Authorization"];

            if (bearerToken == null || !bearerToken.StartsWith("Bearer "))
            {
                return AuthenticateResult.Fail("Invalid scheme");
            }

            string token = bearerToken["Bearer ".Length..];

            try
            {
                FirebaseToken firebaseToken = await FirebaseAuth.GetAuth(_firebaseApp).VerifyIdTokenAsync(token);

                return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new List<ClaimsIdentity>()
                {
                    new ClaimsIdentity(await ToClaimsAsync(firebaseToken.Claims), nameof(FirebaseAuthenticationHandler))
                }), JwtBearerDefaults.AuthenticationScheme));

            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }

        }


        private async Task<IEnumerable<Claim>?> ToClaimsAsync(IReadOnlyDictionary<string, object> claims)
        {
            if (claims == null)
            {
                throw new Exception("No Claims in Token");
            }

            var firebaseId = claims["user_id"].ToString();
            var email = claims["email"].ToString();

            if (firebaseId is null || email is null)
            {
                throw new Exception("No firebaseId and Email in Token");
            }
            string customRole = string.Empty;
            string azure_id = string.Empty;

            // TODO FIX SWITCH CASE
            var master = await _context.Staff.FirstOrDefaultAsync(u => u.FirebaseId == firebaseId && u.Role == "master");
            if (master != null)
            {
                azure_id = master.Id.ToString();
                customRole = "master";
            }
            else
            {
                var allstaff = await _context.Staff.FirstOrDefaultAsync(u => u.FirebaseId == firebaseId && u.Role == "allstaff");
                if (allstaff != null)
                {
                    azure_id = allstaff.Id.ToString();
                    customRole = "master";
                }
                else
                {

                    var oa = await _context.Staff.FirstOrDefaultAsync(u => u.FirebaseId == firebaseId && u.Role == "oa");
                    if (oa != null)
                    {
                        azure_id = oa.Id.ToString();
                        customRole = "oa";
                    }
                    else
                    {
                        var ea = await _context.Staff.FirstOrDefaultAsync(u => u.FirebaseId == firebaseId && u.Role == "ea");
                        if (ea != null)
                        {
                            azure_id = ea.Id.ToString();
                            customRole = "ea";
                        }
                        else
                        {
                            var ec = await _context.Staff.FirstOrDefaultAsync(u => u.FirebaseId == firebaseId && u.Role == "ec");
                            if (ec != null)
                            {
                                azure_id = ec.Id.ToString();
                                customRole = "ec";
                            }
                            else
                            {
                                var teacher = await _context.Teachers.Include(x => x.Mandays)
                                                                        .ThenInclude(x => x.WorkTimes)
                                                                     .FirstOrDefaultAsync(u => u.FirebaseId == firebaseId);
                                if (teacher != null)
                                {
                                    azure_id = teacher.Id.ToString();
                                    customRole = "teacher";
                                }
                                else
                                {
                                    var student = await _context.Students.FirstOrDefaultAsync(u => u.FirebaseId == firebaseId);
                                    if (student != null)
                                    {
                                        azure_id = student.Id.ToString();
                                        customRole = "student";
                                    }
                                    else
                                    {
                                        throw new NotFoundException("User not found");
                                    }
                                }
                            }
                        }
                    }
                }

            }

            return new List<Claim>
            {
                new Claim("azure_id", azure_id),
                new Claim("firebaseId", firebaseId),
                new Claim("email", email),
                new Claim(ClaimTypes.Role, customRole)
            };
        }
    }
}