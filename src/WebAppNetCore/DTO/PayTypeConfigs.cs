using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WebAppNetCore;

namespace WebAppNetCore.DTO
{        
   [ConfiguredOptions(nameof(PayTypeConfigs))]                                          
    public class PayTypeConfigs
    {
        public Dictionary<string, PayTypeInfo> Types { get; set; }

    }

    public class PayTypeInfo
    {
        public int SysNo { get; set; }
        /// <summary>
        /// 支付方式    
        /// 1=微信支付
        /// 2=智慧生活卡支付
        /// 3=线下支付
        /// </summary>
        public PaymentType PayTypeName { get; set; }

        public string ChannelId { get; set; }
        public string MerId { get; set; }
        public string AppId { get; set; }
        public string NotifyUrl { get; set; }
        public string ReturnUrl { get; set; }
        public string RequestUrl { get; set; }
        public string SignPath { get; set; }
        public string TermId { get; set; }
    }
    public enum PaymentType
    {
        /// <summary>
        /// 微信支付
        /// </summary>
        [Description("微信支付")]
        WeChatPay = 1,
        /// <summary>
        /// 智慧生活卡支付
        /// </summary>
        [Description("智慧生活卡支付")]
        SmartCardPay = 2,
        /// <summary>
        /// 线下支付
        /// </summary>
        [Description("线下支付")]
        OfflinePay = 3
    }
}

