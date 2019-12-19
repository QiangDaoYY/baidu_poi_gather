using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Drawing;

namespace BrokerageGather
{
    public class BlockMaker
    {
        public class NewRoundEventArgs : EventArgs
        {
            public int Count { get; set; }
            public Bitmap Map { get; set; }
        }

        private const string _apiFindCityLocation = "http://api.map.baidu.com/geocoder/v2/?output=json&ak={0}&address={1}";
        public Decimal Offset { get; set; }
        public Location CityCenter { get; set; }
        public string JsonContent { get; set; }
        public string CityName { get; set; }
        public event EventHandler<NewRoundEventArgs> NewRound;
        public BlockMaker(string cityName, decimal offset)
        {
            Offset = offset;
            CityName = cityName;
        }

        public List<Block> SplitBlock()
        {
            CityCenter = GetCityCenter();

            var blocks = new List<Block>();

            int round = 1;
            while (Spread(round++, blocks))
            {
                blocks = blocks.Where(i => i.InCity).ToList();

                var left = (from q in blocks
                            orderby q.Center.Lng
                            select q.Center.Lng).FirstOrDefault();
                var top = (from q in blocks
                           orderby q.Center.Lat descending
                           select q.Center.Lat).FirstOrDefault();
                var bottom = (from q in blocks
                              orderby q.Center.Lat
                              select q.Center.Lat).FirstOrDefault();
                var right = (from q in blocks
                             orderby q.Center.Lng descending
                             select q.Center.Lng).FirstOrDefault();

                MapPainter mp = new MapPainter(left, top, bottom, right);

                foreach (var block in blocks)
                {
                    mp.Draw(block.Center.Lat, block.Center.Lng, block.DistrictColor);
                }
                if(NewRound!=null)
                NewRound.Invoke(this, new NewRoundEventArgs() { Count = blocks.Count, Map = mp.Map });
            }

            return blocks;
        }

        public static double GetTwoBlockDistance(Location p1, Location p2)
        {
            return MapHelper.GetDistanceGoogle(p1, p2);
            //return MapHelper.GetDistance(p1, p2);
        }

        /// <summary>
        /// 扩散
        /// </summary>
        /// <param name="round">圈数</param>
        /// <param name="blocks">块集合</param>
        /// <returns></returns>
        private bool Spread(int round, List<Block> blocks)
        {
            var flag = false;
            if (round > 0)
            {
                //var count = Math.Pow((2 * round), 2) - Math.Pow(2 * (round - 1), 2);
                var count = 2 * round - 1;
                for (int i = 0; i < count; i++)
                {
                    var lng = CityCenter.Lng - Offset * (round - i);
                    var lat = CityCenter.Lat + Offset * round;
                    var block = NewBlock(lng, lat);

                    if (block.VerifyCity())
                    {
                        blocks.Add(block);
                        flag = true;
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    var lng = CityCenter.Lng + Offset * (round - 1);
                    var lat = CityCenter.Lat + Offset * (round - i);
                    var block = NewBlock(lng, lat);

                    if (block.VerifyCity())
                    {
                        blocks.Add(block);
                        flag = true;
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    var lng = CityCenter.Lng + Offset * (round - i - 1);
                    var lat = CityCenter.Lat - Offset * (round - 1);
                    var block = NewBlock(lng, lat);

                    if (block.VerifyCity())
                    {
                        blocks.Add(block);
                        flag = true;
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    var lng = CityCenter.Lng - Offset * round;
                    var lat = CityCenter.Lat - Offset * (round - i - 1);
                    var block = NewBlock(lng, lat);

                    if (block.VerifyCity())
                    {
                        blocks.Add(block);
                        flag = true;
                    }
                }
            }
            return flag;
        }

        private Block NewBlock(decimal lng, decimal lat)
        {
            return new Block(CityName)
            {
                LeftTop = new Location(lat, lng),
                RightTop = new Location(lat, lng + Offset),
                LeftBottom = new Location(lat - Offset, lng),
                RightBottom = new Location(lat - Offset, lng + Offset),
                Center = new Location(lat - Offset / 2, lng + Offset / 2)
            };
        }

        private Location GetCityCenter()
        {
            var url = string.Format(_apiFindCityLocation, AKProvider.Peek(), CityName);

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
                var city = jss.Deserialize<CityLocationDefinition>(content);
                if (city.Result.Level == "城市")
                {
                    var location = city.Result.Location;
                    return location;
                }
                else
                {
                    throw new ApplicationException("您输入的地名不是城市级别，而是" + city.Result.Level);
                }
            }
            wc.Dispose();
            return null;
        }
    }
}
