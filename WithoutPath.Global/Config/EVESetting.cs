using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace WithoutPath.Global.Config
{
    public class EVESetting : ConfigurationSection
    {
        [ConfigurationProperty("AppID", IsRequired = true)]
        public string AppID
        {
            get
            {
                return this["AppID"] as string;
            }
            set
            {
                this["AppID"] = value;
            }
        }

        [ConfigurationProperty("AppSecret", IsRequired = true)]
        public string AppSecret
        {
            get
            {
                return this["AppSecret"] as string;
            }
            set
            {
                this["AppSecret"] = value;
            }
        }
    }
}