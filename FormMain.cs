using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace BrokerageGather
{
    public partial class FormMain : Form
    {
        private List<Block> _blocks = null;
        private List<POI> _pois = null;
        private BackgroundWorker bw = new BackgroundWorker();
        private Stopwatch sw = new Stopwatch();

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            txtCityName.Text = "武汉";
            txtCoordinateOffset.Text = "0.03";
            txtQueryWord.Text = "房地产中介";

            bw.DoWork += spliteBlock_RunWork;
            bw.RunWorkerCompleted += spliteBlock_RunWorkerCompleted;
            bw.ProgressChanged += spliteBlock_ProgressChanged;
            bw.WorkerReportsProgress = true;
        }

        private void btnGetCityBlocks_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtCityName.Text) && !string.IsNullOrEmpty(txtCoordinateOffset.Text))
                {
                    decimal offset;
                    if (decimal.TryParse(txtCoordinateOffset.Text, out offset))
                    {
                        ChangeAllCtrlStatus(false);
                        sw.Start();
                        //txtMessage.Text = string.Empty;
                        txtMessage.SelectionStart = txtMessage.Text.Length;
                        txtMessage.ScrollToCaret();
                        bw.RunWorkerAsync();

                        //BlockMaker bm = new BlockMaker(txtCityName.Text, offset);
                        //var blocks = bm.SplitBlock();
                        //MessageBox.Show("分块完成，分块数量：" + blocks.Count);
                    }
                    else
                    {
                        MessageBox.Show("坐标偏移量格式错误");
                    }
                }
                else
                {
                    MessageBox.Show("请输入城市名和坐标偏移量");
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void spliteBlock_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (newRoundEventArgs.Count > 0)
            {
                txtMessage.Text += string.Format("返回数据，总数量{0}\r\n", newRoundEventArgs.Count);
                txtMessage.SelectionStart = txtMessage.Text.Length;
                txtMessage.ScrollToCaret();
            }
            if (newRoundEventArgs.Map != null)
            {
                picMap.Image = newRoundEventArgs.Map;
            }
        }

        private void spliteBlock_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            sw.Stop();
            _blocks = e.Result as List<Block>;
            var distance = 0d;
            if (_blocks.Count > 1)
            {
                distance = BlockMaker.GetTwoBlockDistance(_blocks[0].Center, _blocks[1].Center);
            }
            var msg = string.Format("分块完成，分块间距{4}米（2000米以内最佳），总数量:{0},耗时:{1}小时{2}分钟{3}秒\r\n", _blocks.Count, sw.Elapsed.Hours, sw.Elapsed.Minutes, sw.Elapsed.Seconds, distance);
            MessageBox.Show(msg);
            txtMessage.Text += msg;
            txtMessage.SelectionStart = txtMessage.Text.Length;
            txtMessage.ScrollToCaret();
            ChangeAllCtrlStatus(true);
        }

        private void spliteBlock_RunWork(object sender, DoWorkEventArgs e)
        {
            var bw = sender as BackgroundWorker;
            BlockMaker bm = new BlockMaker(txtCityName.Text, decimal.Parse(txtCoordinateOffset.Text));
            bm.NewRound += Bm_NewRound;
            var blocks = bm.SplitBlock();
            e.Result = blocks;
        }

        private void Bm_NewRound(object sender, BlockMaker.NewRoundEventArgs e)
        {
            newRoundEventArgs = e;
            bw.ReportProgress(1);
        }

        private BlockMaker.NewRoundEventArgs newRoundEventArgs;
        private MapPainter.MapPainterEventArgs painterEventArgs;

        private void Gather()
        {
            //page_size=20&page_num=0

            //http://api.map.baidu.com/place/v2/search?query=美食&page_size=10&page_num=0&scope=1&bounds=39.915,116.404,39.975,116.414&output=json&ak={您的密钥}
            //http://api.map.baidu.com/place/v2/search?query=银行&location=39.915,116.404&radius=2000&output=xml&ak=E4805d16520de693a3fe707cdc962045
            //String MAP_WEBAPI_URI = "http://api.map.baidu.com/place/v2/search?page_size={0}&page_num={1}&query={2}&location={3},{4}&radius={5}&output=json&ak={6}";
            String MAP_WEBAPI_URI = "http://api.map.baidu.com/place/v2/search?page_size={0}&page_num={1}&scope=1&query={2}&bounds={3},{4},{5},{6}&output=json&ak={7}";
            int PAGE_SIZE = 20;

            List<string> ak = ConfigurationManager.AppSettings["BaiduMapAK"].Split(',').ToList();
            int akIndex = 0;
            Dictionary<string, DateTime> akDict = new Dictionary<string, DateTime>();
            //给每个AK赋初值为当天时间
            ak.ForEach(delegate(string item)
            {
                akDict.Add(item, DateTime.Today);
            });

            int pageNum = 0;
            int total = 0;

            string[] bus = new string[] { };
            JavaScriptSerializer json = new JavaScriptSerializer();

            WebClient wc = new WebClient();

            int redo = 0;//重试次数
            #region 循环读取此项因素百度API返回的每页数据，添加POI点及小区因素距离详细信息
            do
            {
                #region 请求百度POI数据
                StringBuilder result = new StringBuilder();
                string uri = String.Format(MAP_WEBAPI_URI,
                    PAGE_SIZE,
                    pageNum,
                    "房地产中介",
                    //listEstate[indexEstate].Lat,
                    //listEstate[indexEstate].Lng,
                    2000,
                    ak[akIndex]);

                Stream stream = Stream.Null;
                try
                {
                    stream = wc.OpenRead(uri);
                    if (stream == null)
                    {
                        if (redo == 3)//重试3次获取不到则切换到下一页
                        {
                            pageNum++;
                        }
                        continue;
                    }
                }
                catch (WebException ex)
                {
                    redo++;//增加重试次数

                    continue;
                    //throw;
                }

                StreamReader sr = new StreamReader(stream);
                string strLine = "";

                while ((strLine = sr.ReadLine()) != null)
                {
                    result.Append(strLine);
                }
                sr.Close();
                #endregion

                BaiduMapJSON mapPois = json.Deserialize<BaiduMapJSON>(result.ToString());


                //成功返回
                if (mapPois != null && mapPois.Status == 0 && mapPois.Message.ToLower() == "ok")
                {
                    total = mapPois.Total;
                    #region 添加距离点记录到EstateFactorDistance
                    //List<EstateFactorDistance> listAdd = new List<EstateFactorDistance>();
                    foreach (var poi in mapPois.Results)
                    {

                        Guid poiId = Guid.Empty;//此值必须根据查询到的结果重新设置值为：当前缓存的或者从数据库查询到的POI的Id值

                        #region 判断POI点是否已存在,不存在则添加
                        //var pois = listPOIAdd.Where(
                        //        item => item.UID == poi.UID 
                        //        && item.FactorCode == listEstateFactor[indexFactor].Code).FirstOrDefault();

                        //if (pois == null)
                        //{ 
                        //    poiId = Guid.NewGuid();
                        //    #region  新增POI点数据到数据库

                        //    //listPOIAdd.Add(itempoi);


                        //    #endregion
                        //}
                        //else
                        //{
                        //    poiId = pois.Id;
                        //}
                        #endregion
                    }

                    #endregion
                }
                else
                {
                    //2  请求参数非法  Parameter Invalid  
                    //3  权限校验失败  Verify Failure  
                    //4  配额校验失败  Quota Failure  
                    //5  ak不存在或者非法  AK Failure  
                    //2xx  无权限   
                    //3xx  配额错误  

                    //判断是否配额校验失败，则更换AK
                    if (mapPois.Status >= 300)
                    {
                        //更改AK时间为下一天
                        akDict[ak[akIndex]] = DateTime.Today.AddDays(1);

                        //查找当前所有AK的时间是否都大于当天，如果都大于则需等待，否则继续读取
                        bool isOver = true;
                        int indexFind = -1;
                        for (int i = 0; i < ak.Count; i++)
                        {
                            if (akDict[ak[i]] <= DateTime.Today)
                            {
                                isOver = false;
                                indexFind = i;
                                break;
                            }
                        }
                        if (indexFind > -1)
                        {
                            akIndex = indexFind;
                        }
                        else
                        {
                            akIndex = akIndex == ak.Count - 1 ? 0 : akIndex++;
                        }

                        //如果全部AK都无效，则需等待，否则继续找下一个AK
                        if (isOver)
                        {
                            //保存所有缓存中数据
                            //var t = BasLogger.LogErrorAsync(taskId, String.Format("{0}获取因素{1}失败。错误原因：{2}", listEstate[indexEstate].Name, listEstateFactor[indexFactor].Name, mapPois.Message));
                            DateTime dtTomorrow = DateTime.Today.AddDays(1).AddHours(1);

                            //超过每天限额则等待,需等待到次天才能再次进行采集                                            
                            //Thread.Sleep(dtTomorrow -DateTime.Now);       
                            Task.Delay(dtTomorrow - DateTime.Now);

                        }
                    }

                }

                //增加页数，读取下一页数据
                pageNum++;
            } while (PAGE_SIZE * pageNum < total);
            #endregion
        }

        private void btnSerialize_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.RestoreDirectory = true;
            sfd.Filter = "json文件(*.json)|*.json";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ChangeAllCtrlStatus(false);
                var serializeBackgroundWorker = new BackgroundWorker();
                serializeBackgroundWorker.DoWork += SerializeBackgroundWorker_DoWork;
                serializeBackgroundWorker.RunWorkerCompleted += SerializeBackgroundWorker_RunWorkerCompleted;
                serializeBackgroundWorker.RunWorkerAsync(sfd.FileName);
            }
        }

        private void SerializeBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result)
            {
                MessageBox.Show("序列化完成");
            }
            else
            {
                MessageBox.Show("块数据为空");
            }
            ChangeAllCtrlStatus(true);
        }

        private void ChangeAllCtrlStatus(bool enable)
        {
            txtCityName.Enabled =
            txtCoordinateOffset.Enabled =
            txtQueryWord.Enabled =
            btnDserialize.Enabled =
            btnGetCityBlocks.Enabled =
            btnQueryPOI.Enabled =
            btnSaveResult.Enabled =
            btnSerialize.Enabled = enable;
        }

        private void SerializeBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var path = e.Argument as string;
            if (!string.IsNullOrEmpty(path))
            {
                if (_blocks != null && _blocks.Count > 0)
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    jss.MaxJsonLength = Int32.MaxValue;
                    var str = jss.Serialize(_blocks);
                    File.WriteAllText(path, str);
                    e.Result = true;
                }
                else
                {
                    e.Result = false;
                }
            }
        }

        private void btnDserialize_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.RestoreDirectory = true;
            sfd.Filter = "json文件(*.json)|*.json";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ChangeAllCtrlStatus(false);
                var deserializeBackgroundWorker = new BackgroundWorker();
                deserializeBackgroundWorker.DoWork += DeserializeBackgroundWorker_DoWork; ;
                deserializeBackgroundWorker.RunWorkerCompleted += DeserializeBackgroundWorker_RunWorkerCompleted;
                deserializeBackgroundWorker.RunWorkerAsync(sfd.FileName);
            }
        }

        private void DeserializeBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                picMap.Image = e.Result as Bitmap;
                var distance = 0d;
                if (_blocks.Count > 1)
                {
                    distance = BlockMaker.GetTwoBlockDistance(_blocks[0].Center, _blocks[1].Center);
                }
                var msg = string.Format("反序列化成功，分块间距{1}米（2000米以内最佳），总数量:{0}\r\n", _blocks.Count, distance);

                MessageBox.Show(msg);
                txtMessage.Text += msg;
                txtMessage.SelectionStart = txtMessage.Text.Length;
                txtMessage.ScrollToCaret();
            }
            else
            {
                MessageBox.Show("反序列化失败");
            }

            ChangeAllCtrlStatus(true);
        }

        private void DeserializeBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var path = e.Argument as string;
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                jss.MaxJsonLength = Int32.MaxValue;
                var str = File.ReadAllText(path);
                _blocks = jss.Deserialize<List<Block>>(str);
                if (_blocks != null && _blocks.Count > 0)
                {
                    var left = (from q in _blocks
                                orderby q.Center.Lng
                                select q.Center.Lng).FirstOrDefault();
                    var top = (from q in _blocks
                               orderby q.Center.Lat descending
                               select q.Center.Lat).FirstOrDefault();
                    var bottom = (from q in _blocks
                                  orderby q.Center.Lat
                                  select q.Center.Lat).FirstOrDefault();
                    var right = (from q in _blocks
                                 orderby q.Center.Lng descending
                                 select q.Center.Lng).FirstOrDefault();

                    MapPainter mp = new MapPainter(left, top, bottom, right);

                    foreach (var block in _blocks)
                    {
                        mp.Draw(block.Center.Lat, block.Center.Lng, block.DistrictColor);
                    }

                    e.Result = mp.Map;
                }
                else
                {
                    e.Result = null;
                }
            }
        }

        private void btnQueryPOI_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtQueryWord.Text))
            {
                MessageBox.Show("请输入查询关键字，比如'房地产中介'");
                return;
            }
            if (_blocks != null && _blocks.Count > 0)
            {
                ChangeAllCtrlStatus(false);
                var queryPOIBackgroundWorker = new BackgroundWorker();
                queryPOIBackgroundWorker.WorkerReportsProgress = true;
                queryPOIBackgroundWorker.DoWork += QueryPOIBackgroundWorker_DoWork;
                queryPOIBackgroundWorker.ProgressChanged += QueryPOIBackgroundWorker_ProgressChanged;
                queryPOIBackgroundWorker.RunWorkerCompleted += QueryPOIBackgroundWorker_RunWorkerCompleted;
                queryPOIBackgroundWorker.RunWorkerAsync(txtQueryWord.Text);
            }
            else
            {
                MessageBox.Show("分块数据为空");
            }
        }

        private void QueryPOIBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var pois = e.Result as List<POI>;
            if (pois != null && pois.Count > 0)
            {
                var msg = string.Format("POI数据查询完毕,{0}数据总数{1}\r\n", txtQueryWord.Text, pois.Count);
                MessageBox.Show(msg);
                txtMessage.Text += msg;
                txtMessage.SelectionStart = txtMessage.Text.Length;
                txtMessage.ScrollToCaret();
            }
            else
            {
                MessageBox.Show("POI数据为空");
            }
            ChangeAllCtrlStatus(true);
        }

        private void QueryPOIBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var msg = string.Format("POI数据总数{0}\r\n", _pois.Count);
            txtMessage.Text += msg;
            txtMessage.SelectionStart = txtMessage.Text.Length;
            txtMessage.ScrollToCaret();
            picMap.Image = painterEventArgs.Map.Clone() as Bitmap;
        }

        private void QueryPOIBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _pois = new List<POI>();
            var query = e.Argument as string;

            var left = (from q in _blocks
                        orderby q.Center.Lng
                        select q.Center.Lng).FirstOrDefault();
            var top = (from q in _blocks
                       orderby q.Center.Lat descending
                       select q.Center.Lat).FirstOrDefault();
            var bottom = (from q in _blocks
                          orderby q.Center.Lat
                          select q.Center.Lat).FirstOrDefault();
            var right = (from q in _blocks
                         orderby q.Center.Lng descending
                         select q.Center.Lng).FirstOrDefault();

            MapPainter mp = new MapPainter(left, top, bottom, right);
            mp.Map = picMap.Image.Clone() as Bitmap;
            mp.PaintEvent += Mp_PaintEvent;
            foreach (var block in _blocks)
            {
                _pois.AddRange(block.ReadBlockPOI(query, 20));
                mp.Draw(block.Center.Lat, block.Center.Lng, Color.DarkGreen);
                var bwk = sender as BackgroundWorker;
                bwk.ReportProgress(1);
            }
            e.Result = _pois;
        }

        private void Mp_PaintEvent(object sender, MapPainter.MapPainterEventArgs e)
        {
            painterEventArgs = e;
        }

        private void btnSaveResult_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.RestoreDirectory = true;
            sfd.Filter = "excel文件(*.xls)|*.xls";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                POIExport pe = new POIExport();
                var pois = pe.ConvertAndGroupPOI(_pois);
                pe.Export(pois, sfd.FileName, txtQueryWord.Text);
            }
        }
    }
}
