using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokerageGather
{
    public class MapHelper
    {
        /** 地球半径 */
        private static double EARTH_RADIUS = 6378137.0;

        private MapHelper()
        {
        }

        /// <summary>
        /// 弧度转角度
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double degrees(double d)
        {
            return d * (180 / Math.PI);
        }

        ///// <summary>
        ///// 计算两个经纬度之间的直接距离
        ///// </summary> 
        //public static double GetDistance(Location lt1, Location lt2)
        //{
        //    double radLat1 = radians((double)lt1.Lat);
        //    double radLat2 = radians((double)lt2.Lat);
        //    double a = radLat1 - radLat2;
        //    double b = radians((double)lt1.Lng) - radians((double)lt2.Lng);
        //    double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
        //    s = s * EARTH_RADIUS;
        //    s = Math.Round(s * 10000) / 10000;
        //    return s;
        //}

        ///  <summary>
        ///  计算两个经纬度之间的直接距离(google 算法)
        ///  </summary>
        public static double GetDistanceGoogle(Location lt1, Location lt2)
        {
            double radLat1 = radians((double)lt1.Lat);
            double radLng1 = radians((double)lt1.Lng);
            double radLat2 = radians((double)lt2.Lat);
            double radLng2 = radians((double)lt2.Lng);
            double s = Math.Acos(Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Cos(radLng1 - radLng2) + Math.Sin(radLat1) * Math.Sin(radLat2));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }

        public static double hav(double theta)
        {
            double s = Math.Sin(theta / 2);
            return s * s;
        }

        /// <summary>
        /// 经纬度转换成弧度
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double radians(double d)
        {
            return d * Math.PI / 180.0;
        }

        //public static double getDistance(double lat0, double lng0, double lat1,
        //        double lng1)
        //{
        //    // from math import sin, asin, cos, radians, fabs, sqrt  

        //    // def hav(theta):  
        //    // s = sin(theta / 2)  
        //    // return s * s  

        //    // def get_distance_hav(lat0, lng0, lat1, lng1):  
        //    // "用haversine公式计算球面两点间的距离。"  
        //    // # 经纬度转换成弧度  
        //    // lat0 = radians(lat0)  
        //    // lat1 = radians(lat1)  
        //    // lng0 = radians(lng0)  
        //    // lng1 = radians(lng1)  

        //    // dlng = fabs(lng0 - lng1)  
        //    // dlat = fabs(lat0 - lat1)  
        //    // h = hav(dlat) + cos(lat0) * cos(lat1) * hav(dlng)  
        //    // distance = 2 * EARTH_RADIUS * asin(sqrt(h))  

        //    // return distance  

        //    lat0 = radians(lat0);
        //    lat1 = radians(lat1);
        //    lng0 = radians(lng0);
        //    lng1 = radians(lng1);

        //    double dlng = Math.Abs(lng0 - lng1);
        //    double dlat = Math.Abs(lat0 - lat1);
        //    double h = hav(dlat) + Math.Cos(lat0) * Math.Cos(lat1) * hav(dlng);
        //    double distance = 2 * EARTH_RADIUS * Math.Asin(Math.Sqrt(h));

        //    return distance;
        //}

        public static Location[] getDegreeCoordinates(double lat, double lng,
                double distance)
        {
            /** 左上角 */
            Location left_top = null;
            /** 右上角 */
            Location right_top = null;
            /** 左下角 */
            Location left_bottom = null;
            /** 右下角 */
            Location right_bottom = null;

            // float dlng = 2 * asin(sin(distance / (2 * EARTH_RADIUS)) / cos(lat));  
            // float dlng = degrees(dlng) // 弧度转换成角度  
            double dlng = 2 * Math.Asin(Math.Sin(distance / (2 * EARTH_RADIUS)) / Math.Cos(radians(lat)));
            dlng = degrees(dlng);

            // dlat = distance / EARTH_RADIUS  
            // dlng = degrees(dlat) # 弧度转换成角度  
            double dlat = distance / EARTH_RADIUS;
            dlat = degrees(dlat); // # 弧度转换成角度  

            // left-top : (lat + dlat, lng - dlng)  
            // right-top : (lat + dlat, lng + dlng)  
            // left-bottom : (lat - dlat, lng - dlng)  
            // right-bottom: (lat - dlat, lng + dlng)  
            left_top = new Location((decimal)(lat + dlat), (decimal)(lng - dlng));
            right_top = new Location((decimal)(lat + dlat), (decimal)(lng + dlng));
            left_bottom = new Location((decimal)(lat - dlat), (decimal)(lng - dlng));
            right_bottom = new Location((decimal)(lat - dlat), (decimal)(lng + dlng));

            Location[] locations = new Location[4];
            locations[0] = left_top;
            locations[1] = right_top;
            locations[2] = left_bottom;
            locations[3] = right_bottom;
            return locations;

        }
    }
}
