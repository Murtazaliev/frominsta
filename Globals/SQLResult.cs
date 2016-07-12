using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InstagramMVC.Globals
{
    public class SQLResult
    {
        public AppEnums.SQLExecResult SQLStatus;
        public object Result;
        public Type ResultType;
    }
}