using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace BrokerageGather
{
    public class BlockColorProvider
    {
        public readonly static Dictionary<string, Color> _disColor = new Dictionary<string, Color>();
        public static Color GetDistrictColor(string district)
        {
            if (!string.IsNullOrEmpty(district))
            {
                if (_disColor.ContainsKey(district))
                {
                    return _disColor[district];
                }
                else
                {
                    //Random r = new Random((int)DateTime.Now.Ticks);
                    //int int_Red = r.Next(256);
                    //int int_Green = r.Next(256);
                    //int int_Blue = (int_Red + int_Green > 400) ? 0 : 400 - int_Red - int_Green;
                    //int_Blue = (int_Blue > 255) ? 255 : int_Blue;
                    //Color c = Color.FromArgb(int_Red, int_Green, int_Blue);
                    //_disColor.Add(district, c);
                    //return c;

                    Random randomNum_1 = new Random(Guid.NewGuid().GetHashCode());
                    System.Threading.Thread.Sleep(randomNum_1.Next(1));
                    int int_Red = randomNum_1.Next(255);

                    Random randomNum_2 = new Random((int)DateTime.Now.Ticks);
                    int int_Green = randomNum_2.Next(255);

                    Random randomNum_3 = new Random(Guid.NewGuid().GetHashCode());

                    int int_Blue = randomNum_3.Next(255);
                    int_Blue = (int_Red + int_Green > 380) ? int_Red + int_Green - 380 : int_Blue;
                    int_Blue = (int_Blue > 255) ? 255 : int_Blue;

                    Color c = Color.FromArgb(int_Red, int_Green, int_Blue);
                    _disColor.Add(district, c);
                    return c;
                }
            }
            return Color.White;
        }

        //获取加深颜色
        public static Color GetDarkerColor(Color color)
        {
            const int max = 255;
            int increase = new Random(Guid.NewGuid().GetHashCode()).Next(30, 255); //还可以根据需要调整此处的值


            int r = Math.Abs(Math.Min(color.R - increase, max));
            int g = Math.Abs(Math.Min(color.G - increase, max));
            int b = Math.Abs(Math.Min(color.B - increase, max));


            return Color.FromArgb(r, g, b);
        }
    }

    [Serializable]
    public class Block
    {
        private const string _apiGetLocationCity = "http://api.map.baidu.com/geocoder/v2/?output=json&ak={0}&location={1},{2}&pois=0";
        private const string _apiGetPOI = "http://api.map.baidu.com/place/v2/search?query={0}&bounds={1},{2},{3},{4}&output=json&ak={5}&page_size={6}&page_num={7}";
        public Location Center { get; set; }
        public Location LeftTop { get; set; }
        public Location LeftBottom { get; set; }
        public Location RightTop { get; set; }
        public Location RightBottom { get; set; }
        public string CityName { get; set; }
        public string District { get; set; }
        public Color DistrictColor { get { return BlockColorProvider.GetDistrictColor(District); } }
        public string JsonContent { get; set; }
        public bool InCity = false;
        public Block()
        {

        }
        public Block(string cityName)
        {
            CityName = cityName;
        }
        public bool VerifyCity()
        {
            var url = string.Format(_apiGetLocationCity, AKProvider.Peek(), Center.Lat, Center.Lng);

            string content = null;

            WebClient wc = new WebClient();
            var stream = wc.OpenRead(url);
            if (stream != null)
            {
                StreamReader sr = new StreamReader(stream);
                content = sr.ReadToEnd();
                sr.Close();
                stream.Close();
            }

            if (!string.IsNullOrEmpty(content))
            {
                JsonContent = content;
                JavaScriptSerializer jss = new JavaScriptSerializer();
                jss.MaxJsonLength = Int32.MaxValue;
                var location = jss.Deserialize<LocationInfoDefinition>(content);
                if (location.Status == 0)
                {
                    if (location.Result.AddressComponent.City.Contains(CityName))
                    {
                        District = location.Result.AddressComponent.District;
                        //Debug.WriteLine(url);
                        InCity = true;
                        return true;
                    }
                }
                else
                {
                    AKProvider.Pop(AKProvider.Peek());
                    return VerifyCity();
                }
            }
            wc.Dispose();
            return false;
        }

        public List<POI> ReadBlockPOI(string keyword, int pageSize)
        {
            int pageNum = 0;
            var list = new List<POI>();
            while (ReadPOIPage(keyword, pageSize, list, pageNum))
            {
                pageNum++;
            }
            return list;
        }

        public bool ReadPOIPage(string keyword, int pageSize, List<POI> list, int pageNum = 0)
        {
            var url = string.Format(_apiGetPOI, keyword, LeftBottom.Lat, LeftBottom.Lng, RightTop.Lat, RightTop.Lng, AKProvider.Peek(), pageSize, pageNum);
            string content = null;

            WebClient wc = new WebClient();
            var stream = wc.OpenRead(url);
            if (stream != null)
            {
                StreamReader sr = new StreamReader(stream);
                content = sr.ReadToEnd();
                sr.Close();
                stream.Close();
            }
            wc.Dispose();
            if (!string.IsNullOrEmpty(content))
            {
                JsonContent = content;
                JavaScriptSerializer jss = new JavaScriptSerializer();
                jss.MaxJsonLength = Int32.MaxValue;
                var poi = jss.Deserialize<BaiduMapJSON>(content);
                if (poi.Status == 0)
                {
                    if (poi.Results.Count > 0)
                    {
                        list.AddRange(poi.Results);

                        if (pageSize * ++pageNum < poi.Total)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //AK失效
                    AKProvider.Pop(AKProvider.Peek());
                    ReadPOIPage(keyword, pageSize, list, pageNum);
                    return ReadPOIPage(keyword, pageSize, list, pageNum);
                }
            }
            return false;
        }
    }
}