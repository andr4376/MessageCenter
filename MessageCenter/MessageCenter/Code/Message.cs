using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageCenter.Code
{
    public abstract class Message
    {

        public virtual StatusCode Send()
        {

            return StatusCode.ERROR;
        }

        public virtual void Reset()
        {
            
        }
    }
}