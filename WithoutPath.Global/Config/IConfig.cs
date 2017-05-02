using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WithoutPath.Global.Config
{
    public interface IConfig
    {
        string Lang { get; }

        bool EnableMail { get; }

        MailSetting MailSetting { get; }

        EVESetting EVESetting { get; }
    }
}