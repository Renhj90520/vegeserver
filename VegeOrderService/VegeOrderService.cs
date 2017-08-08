using cn.jpush.api;
using cn.jpush.api.common;
using cn.jpush.api.common.resp;
using cn.jpush.api.push.mode;
using cn.jpush.api.push.notification;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using VegeOrderService.Models;

namespace VegeOrderService
{
    public partial class VegeOrderService : ServiceBase
    {
        private Timer orderTimer;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string devkey = ConfigurationManager.AppSettings.Get("devkey");
        private string devsecret = ConfigurationManager.AppSettings.Get("devsecret");
        private string mipushUrl = "https://api.xmpush.xiaomi.com/v3/message/all";
        private string appPkgName = "com.xjconvenience.vege.vege";
        public VegeOrderService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Debugger.Launch();
            try
            {
                log.Info("服务启动开始");
                this.orderTimer = new Timer();
                var interval = Convert.ToInt32(ConfigurationManager.AppSettings.Get("pushTimer"));
                this.orderTimer.Interval = interval * 1000;
                this.orderTimer.Elapsed += OrderTimer_Elapsed;
                this.orderTimer.Start();
                log.Info("服务启动完成");
            }
            catch (Exception ex)
            {
                log.Error("服务启动失败", ex);
            }
        }

        private async void OrderTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            log.Info("推送开始");
            try
            {
                using (VegeContext context = new VegeContext())
                {
                    var ordersUnNotified = await context.Orders.Where(o => o.NotifyState == "0" || o.NotifyState == "2").ToListAsync();
                    if (ordersUnNotified.Count() > 0)
                    {
                        var ids = ordersUnNotified.Select(o => o.Id).ToArray();
                        log.Debug("存在新订单，订单ids:" + string.Join(",", ids));

                        try
                        {
                            log.Debug("开始小米推送");
                            using (HttpClient client = new HttpClient())
                            {
                                HttpRequestMessage req = new HttpRequestMessage();
                                req.Method = HttpMethod.Post;
                                req.Headers.TryAddWithoutValidation("Authorization", "key=" + ConfigurationManager.AppSettings.Get("mipushsecret"));
                                req.RequestUri = new Uri(mipushUrl);
                                List<string> content = new List<string>();
                                content.Add("description=" + "订单ids:" + string.Join(",", ids));
                                content.Add("payload=订单ids:" + string.Join(",", ids));
                                content.Add("restricted_package_name=" + appPkgName);
                                content.Add("title=方便生活有新订单");
                                content.Add("pass_through=0");//通知栏消息
                                content.Add("extra.notify_effect=2");
                                content.Add("extra.intent_uri=intent:#Intent;action=com.xjconvenience.vege.mipush;end");
                                content.Add("notify_type=-1");
                                string contentStr = string.Join("&", content);
                                log.Debug("小米推送发送内容：" + contentStr);
                                
                                req.Content = new StringContent(contentStr, Encoding.UTF8, "application/x-www-form-urlencoded");
                                var mipushResp = await client.SendAsync(req);
                                var mipushResult = await mipushResp.Content.ReadAsStringAsync();
                                log.Debug("小米推送返回结果：" + mipushResult);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("小米推送错误", ex);
                        }

                        JPushClient jClient = new JPushClient(devkey, devsecret);
                        PushPayload payload = PushObject_Droid_Alert(string.Join(",", ids));
                        try
                        {
                            log.Debug("开始极光推送");
                            var result = jClient.SendPush(payload);

                            System.Threading.Thread.Sleep(10000);

                            var apiResultv3 = jClient.getReceivedApi_v3(result.msg_id.ToString());
                            if (apiResultv3.isResultOK())
                            {
                                log.Debug("开始更新推送状态");
                                ordersUnNotified.ForEach(o =>
                                {
                                    if (o.IsPaid == "0")
                                    {
                                        o.NotifyState = "1";
                                    }
                                    else if (o.IsPaid == "1")
                                    {
                                        o.NotifyState = "3";
                                    }
                                });

                                await context.SaveChangesAsync();
                                log.Debug("更新推送状态结束");
                            }
                        }
                        catch (APIRequestException ex)
                        {
                            log.Error("Error response from JPush server. Should review and fix it. ");
                            log.Error("HTTP Status: " + ex.Status);
                            log.Error("Error Code: " + ex.ErrorCode);
                            log.Error("Error Message: " + ex.ErrorMessage);
                        }
                        catch (APIConnectionException ex)
                        {
                            log.Error("极光推送连接错误", ex);
                        }
                        catch (Exception ex)
                        {
                            log.Error("极光推送错误", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("推送发生错误", ex);
            }
            log.Info("推送结束");
        }

        private PushPayload PushObject_Droid_Alert(string ids)
        {
            PushPayload pushPayLoad = new PushPayload();
            pushPayLoad.platform = Platform.android();
            var audience = Audience.all();
            pushPayLoad.audience = audience;
            var notification = new Notification().setAlert("方便生活有新的订单");
            AndroidNotification droidNotify = new AndroidNotification();
            droidNotify.setAlert("订单Id：" + ids);
            droidNotify.setBuilderID(3);
            droidNotify.setStyle(1);
            droidNotify.setBig_text("新订单");
            droidNotify.setInbox("订单Id：" + ids);
            droidNotify.setPriority(0);
            droidNotify.setCategory("Category str");
            notification.AndroidNotification = droidNotify;
            pushPayLoad.notification = notification.Check();

            return pushPayLoad;
        }

        protected override void OnStop()
        {
            try
            {
                log.Info("服务停止开始");
                this.orderTimer.Stop();
                this.orderTimer.Close();
                log.Info("服务停止完成");
            }
            catch (Exception ex)
            {
                log.Error("服务停止失败", ex);
            }
        }
    }
}
