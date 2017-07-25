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
        private VegeContext context = new VegeContext();
        public VegeOrderService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
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


                var ordersUnNotified = await this.context.Orders.Where(o => o.NotifyState == "0" || o.NotifyState == "2").ToListAsync();
                if (ordersUnNotified.Count() > 0)
                {
                    var ids = ordersUnNotified.Select(o => o.Id).ToArray();
                    JPushClient jClient = new JPushClient(devkey, devsecret);
                    PushPayload payload = PushObject_Droid_Alert(string.Join(",", ids));
                    try
                    {
                        var result = jClient.SendPush(payload);

                        System.Threading.Thread.Sleep(10000);

                        var apiResultv3 = jClient.getReceivedApi_v3(result.msg_id.ToString());
                        if (apiResultv3.isResultOK())
                        {
                            ordersUnNotified.ForEach(o =>
                            {
                                if (o.State == 1)
                                {
                                    o.NotifyState = "1";
                                }
                                else if (o.State == 2)
                                {
                                    o.NotifyState = "3";
                                }
                            });

                            await this.context.SaveChangesAsync();
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
