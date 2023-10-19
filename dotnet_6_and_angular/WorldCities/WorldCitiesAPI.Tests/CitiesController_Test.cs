using Microsoft.EntityFrameworkCore;
using WorldCitiesAPI.Controllers;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Tests
{
    public class CitiesController_Test
    {

        ///<summary>
        ///Test the GetCity() method
        /// </summary>
        [Fact]
        public async Task GetCity()
        {
            //Arrange
            //todo: define the required assets
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "WorldCities").Options;
            using var context = new ApplicationDbContext(options);

            context.Add(new City
            {
                Id = 1,
                CountryId = 1,
                Lat = 1,
                Lon = 1,
                Name = "TestCiteyy"
            });
            context.SaveChanges();

            var controller = new CitiesController(context);
            City? city_existing = null;
            City? city_notExisting = null;

            //Act
            //todo: invoke the test
            city_existing = (await controller.GetCity(1)).Value;
            city_notExisting = (await controller.GetCity(2)).Value;

            //Assert
            //todo: verify that conditions are met.
            Assert.NotNull(city_existing);
            Assert.Null(city_notExisting);

        }
    }
}
