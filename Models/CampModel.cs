using System;
using System.Collections.Generic;

namespace CoreCodeCamp.Models
{
    public class CampModel
    {
        public string Name { get; set; }
        public string Moniker { get; set; }
        public DateTime EventDate { get; set; } = DateTime.MinValue;
        public int Length { get; set; } = 1;

        public string Venue { get; set; }
        public string LocationVenueNameAddress1 { get; set; }
        public string LocationVenueNameAddress2 { get; set; }
        public string LocationVenueNameAddress3 { get; set; }
        public string LocationVenueNameCityTown { get; set; }
        public string LocationVenueNameStateProvince { get; set; }
        public string LocationVenueNamePostalCode { get; set; }
        public string LocationVenueNameCountry { get; set; }

        public ICollection<TalkModel> Talks { get; set; }
    }
}
