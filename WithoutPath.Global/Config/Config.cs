using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WithoutPath.Global.Config
{
    public class Config : IConfig
    {
        public string Lang
        {
            get
            {
                return ConfigurationManager.AppSettings["Culture"] as string;
            }
        }

        public bool EnableMail
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["EnableMail"]);
            }
        }

        public MailSetting MailSetting
        {
            get
            {
                return (MailSetting)ConfigurationManager.GetSection("mailConfig");
            }
        }

        public EVESetting EVESetting
        {
            get
            {
                return (EVESetting)ConfigurationManager.GetSection("EVEConfig");
            }
        }
    }
}