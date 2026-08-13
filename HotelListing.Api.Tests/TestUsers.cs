using System;
using System.Collections.Generic;
using System.Text;

namespace HotelListing.Api.Tests
{
    public static class TestUsers
    {
        public const string AdminId = "11111111-1111-1111-1111-111111111111";
        public const string ManagerOneId = "22222222-2222-2222-2222-222222222222";
        public const string ManagerTwoId = "33333333-3333-3333-3333-333333333333";
        public const string UserId = "44444444-4444-4444-4444-444444444444";

        public const string AdminEmail = "admin@test.com";
        public const string ManagerOneEmail = "manager1@test.com";
        public const string ManagerTwoEmail = "manager2@test.com";
        public const string UserEmail = "user@test.com";

        public const string AdminUserName = "admin";
        public const string ManagerOneUserName = "manager1";
        public const string ManagerTwoUserName = "manager2";
        public const string UserUserName = "user";

        public const string Password = "Test_PASSWORD1!";
    }


    public static class TestHotels
    {
        public const int HotelOneId = 1;
        public const string HotelOneName = "Hotel One";
        public const string HotelOneAddress = "Hotel One Ave";
        public const double HotelOneRating = 5.0;
        public const decimal HotelOneNightlyRate = 125;
        public const int HotelOneCountryId = 1001;


        public const int HotelTwoId = 1002;
    }


    public static class TestCountries
    {
        public const int CountryOneId = 1001;
        public const string CountryOneName = "Testlandia";
        public const string CountryOneShortName = "TL";
    }

    public static class RoleNames
    {
        public const string Admin = "Administrator";
        public const string User = "User";
    }
}
