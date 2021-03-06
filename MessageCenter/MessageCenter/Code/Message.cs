﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageCenter.Code
{
    public abstract class Message
    {
        /// <summary>
        /// Overridable Send method for messages
        /// </summary>
        /// <returns>A report containing send status and a description</returns>
        public abstract KeyValuePair<StatusCode, string> Send();


        public abstract void Reset();
    }
}