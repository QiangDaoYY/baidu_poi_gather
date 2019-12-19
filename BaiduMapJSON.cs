using System;
using System.Collections.Generic;

namespace BrokerageGather
{
    [Serializable]
    public class BaiduMapJSON
    {
        public int Status { get; set; }
        public String Message { get; set; }
        public int Total { get; set; }
        public List<POI> Results { get; set; }
    }


    [Serializable]
    public class POI
    {
        public String UID { get; set; }
        public String Name { get; set; }
        public String Address { get; set; }
        public String Telephone { get; set; }
        public Location Location { get; set; }
    }

    [Serializable]
    public class Location
    {
        public Location() { }
        public Location(decimal lat, decimal lng) { Lat = lat; Lng = lng; }
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }

    [Serializable]
    public class LocationInfoDefinition
    {
        public int? Status { get; set; }
        public string Message { get; set; }
        public LocationInfoDefinitionResult Result { get; set; }
    }

    public class LocationInfoDefinitionResult
    {
        public AddressComponent AddressComponent { get; set; }
        public string Formatted_Address { get; set; }
        public Location Location { get; set; }
        public string Business { get; set; }
    }

    [Serializable]
    public class CityLocationDefinition
    {
        public int? Status { get; set; }
        public string Message { get; set; }
        public CityLocationDefinitionResult Result { get; set; }
    }

    [Serializable]
    public class CityLocationDefinitionResult
    {
        public Location Location { get; set; }
        public int? Precise { get; set; }
        public int? Confidence { get; set; }
        public string Level { get; set; }
    }

    [Serializable]
    public class AddressComponent
    {
        public string Country { get; set; }
        public string Country_Code { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Adcode { get; set; }
        public string Street { get; set; }
        public string Street_Number { get; set; }
        public string Direction { get; set; }
        public int? Distance { get; set; }
    }
}