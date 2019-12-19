using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GJ.Utility.NPOI;

namespace BrokerageGather
{
    public class POIExport
    {
        public List<POIEntity> ConvertAndGroupPOI(List<POI> pois)
        {
            if (pois != null && pois.Count > 0)
            {
                var query = from q in pois
                            group q by q.UID into g
                            let p = g.FirstOrDefault()
                            orderby p.Name
                            select new POIEntity
                            {
                                UID = p.UID,
                                Name = p.Name,
                                Address = p.Address,
                                Telephone = p.Telephone,
                                Lat = p.Location.Lat,
                                Lng = p.Location.Lng
                            };
                return query.ToList();
            }
            return null;
        }

        public void Export(List<POIEntity> pois, string path, string query)
        {
            var dt = NPOIHelper.ListToDataTable(pois);
            var workbook = NPOIHelper.GenerateData(dt, "POI数据:" + query, "POI数据", new string[] { "UID", "Name", "Address", "Telephone", "Lng", "Lat" }, new string[] { "UID", "店名", "地址", "联系电话", "经度", "纬度" });
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                workbook.Write(fs);
            }
        }
    }

    public class POIEntity
    {
        public string UID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
    }
}
