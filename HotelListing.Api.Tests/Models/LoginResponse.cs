using System;
using System.Collections.Generic;
using System.Text;

namespace HotelListing.Api.Tests.DTOs
{
    internal record LoginResponse
    {
        public string Token { get; init; } = string.Empty;
    }
    
}
