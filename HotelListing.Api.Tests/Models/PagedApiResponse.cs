using System;
using System.Collections.Generic;
using System.Text;

namespace HotelListing.Api.Tests.Models
{
    internal record PagedApiResponse<T>
    {
        public T Data { get; set; } = default!;
    }
}
