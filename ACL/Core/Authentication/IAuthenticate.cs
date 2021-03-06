﻿using System.Threading.Tasks;
using DataLayer.Models.Entities.ACL;
using Microsoft.AspNetCore.Http;

namespace ACL.Core.Authentication
{
    /// <summary>
    ///     Defines the methods for the communication between the
    ///     client and the UH's authentication api.
    /// </summary>
    public interface IAuthenticate
    {
        /// <summary>
        ///     Take the request from the client and
        ///     create a WebRequest to take the data from the
        ///     Universith authentication api.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns>
        ///     Returns a dict with the username, email, group, student or professor.
        ///     This data is taken from the response of the authentication api.
        /// </returns>
        Task<Principal> AuthenticateRequestAsync(HttpContext httpContext);

        Principal AuthenticateRequest(HttpContext httpContext);
    }
}