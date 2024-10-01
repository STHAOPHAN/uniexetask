﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using uniexetask.web.Models;

namespace uniexetask.web.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;



        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _next = next;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null || !endpoint.Metadata.Any(meta => meta is AuthorizeAttribute))
            {
                await _next(context);
                return;
            }

            var at = context.Request.Cookies["AccessToken"];
            var rt = context.Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(at))
            {
                at = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            }

            if (at != null)
            {
                var userPrincipal = ValidateToken(at);
                if (userPrincipal != null)
                {
                    context.User = userPrincipal;
                }
                else
                {
                    var newAccessToken = await RefreshAccessToken(rt);
                    if (newAccessToken != null)
                    {
                        context.Response.Cookies.Append("AccessToken", newAccessToken);
                        context.User = ValidateToken(newAccessToken);
                    }
                    else
                    {
                        await HandleUnauthorized(context);
                        return;
                    }
                }
            }
            else
            {
                await HandleUnauthorized(context);
                return;
            }

            await _next(context);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key must be configured"));

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        private async Task<string?> RefreshAccessToken(string refreshToken)
        {
            var client = _httpClientFactory.CreateClient("UniApiClient");

            var response = await client.PostAsJsonAsync("auth/refresh-token", new { RefreshToken = refreshToken });

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }

        private async Task HandleUnauthorized(HttpContext context)
        {
            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // For AJAX requests, return 401 status
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
            }
            else
            {
                // For regular requests, redirect to login page
                context.Response.Redirect("/Auth/Login");
            }
        }
    }
}