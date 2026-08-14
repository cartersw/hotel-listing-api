using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HotelListing.Api.Tests.Integration.Config
{
    [CollectionDefinition("Integration Tests")]
    public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
    {
    }
}
