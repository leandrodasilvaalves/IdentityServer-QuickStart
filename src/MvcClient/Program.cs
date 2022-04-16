using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// IdentityServer Config - Begin
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
builder.Services.AddAuthentication(options => //adds the authentication services to DI.
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc"; //we need the user to login, we will be using the OpenID Connect protocol.
})
.AddCookie("Cookies") //add the handler that can process cookies.
.AddOpenIdConnect("oidc", options => //is used to configure the handler that performs the OpenID Connect protocol
{
    options.BackchannelHttpHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = delegate { return true; } //only dev environment
    };
    options.Authority = "https://localhost:5001"; // indicates where the trusted token service is located.
    options.ClientId = "mvc";
    options.ClientSecret = "secret";
    options.ResponseType = "code";
    options.SaveTokens = true; //is used to persist the tokens from IdentityServer in the cookie
    options.GetClaimsFromUserInfoEndpoint = true;
    options.Scope.Add("api1");
    options.Scope.Add("offline_access");
});
// IdentityServer Config - End

builder.Services.AddControllersWithViews();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
).RequireAuthorization(); //all controllers are protected. 
//You can also use the [Authorize] attribute, if you want to specify authorization on a per controller or action method basis.

app.Run();
