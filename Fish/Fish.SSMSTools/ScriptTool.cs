using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.IO;
using Fish.CommLib;

namespace Fish.SSMSTools
{
    public class ScriptTool
    {
        ScriptingOptions scriptOption = new ScriptingOptions();
        string RootPath = @"C:\yy-store\GitSrc\D_S_C_Y";

        public string ServerAddr { get; set; }
        public string DBName { get; set; }

        private TableCollection Tables;
        private StoredProcedureCollection StoredProcedures;
        private UserDefinedFunctionCollection UserDefinedFunctions;
        private ViewCollection Views;
        private UserDefinedAggregateCollection UserDefinedAggregates;
        private UserDefinedDataTypeCollection UserDefinedDataTypes;
        private UserDefinedTypeCollection UserDefinedTypes;
        //private UserCollection Users;
        private SchemaCollection Schemas;

        public ScriptTool(string serIp, string uid, string pwd, string dbName)
        {
            ServerAddr = serIp;
            DBName = dbName;
 
            var db = new Server(new ServerConnection(serIp, uid, pwd)).Databases[dbName];

            Tables = db.Tables;
            StoredProcedures = db.StoredProcedures;
            Views = db.Views;
            UserDefinedFunctions = db.UserDefinedFunctions;
            UserDefinedAggregates = db.UserDefinedAggregates;
            UserDefinedDataTypes = db.UserDefinedDataTypes;
            UserDefinedTypes = db.UserDefinedTypes;
            Schemas = db.Schemas;

            IniScriptOption();
        }
        private void IniScriptOption()
        {

            scriptOption.ContinueScriptingOnError = true;
            scriptOption.IncludeIfNotExists = true;
            scriptOption.NoCollation = true;
            scriptOption.ScriptDrops = false;
            scriptOption.ContinueScriptingOnError = true;
            //scriptOption.DriAllConstraints = true;
            scriptOption.WithDependencies = false;
            scriptOption.DriForeignKeys = true;
            scriptOption.DriPrimaryKey = true;
            scriptOption.DriDefaults = true;
            scriptOption.DriChecks = true;
            scriptOption.DriUniqueKeys = true;
            scriptOption.Triggers = true;
            scriptOption.ExtendedProperties = true;
            scriptOption.NoIdentities = false;
            //scriptOption.AnsiPadding = true;
            scriptOption.ToFileOnly = true;
        }

        private ScriptingOptions GetOpt(string sch, string nam, string typ)
        {
            scriptOption.FileName = GetPath(sch, nam, typ);
            //Comm.SaveLog("gening: [" + scriptOption.FileName+"] ");
            return scriptOption;
        }

        public void GenAll()
        {

            GentTable();
            GentFunc();
            GentProc();
            GentView();
            GentUserDef();
            GentUserInfo();
        }

        public void GentTable()
        {
            foreach (Table item in Tables)
            {
                if (item.IsSystemObject)
                {
                    continue;
                }
                var sCollection = item.Script(GetOpt(item.Schema, item.Name, "Table"));
            }
            Console.WriteLine("table over");
        }

        public void GentProc()
        {
            foreach (StoredProcedure item in StoredProcedures)
            {
                if (item.IsSystemObject)
                {
                    continue;
                }
                var sCollection = item.Script(GetOpt(item.Schema, item.Name, "StoredProcedure"));
            }
            Console.WriteLine("proc over");
        }

        public void GentView()
        {
            foreach (View item in Views)
            {
                if (item.IsSystemObject)
                {
                    continue;
                }
                var sCollection = item.Script(GetOpt(item.Schema, item.Name, "View"));
            }
            Console.WriteLine("view over");
        }

        public void GentFunc()
        {
            foreach (UserDefinedFunction item in UserDefinedFunctions)
            {
                if (item.IsSystemObject)
                {
                    continue;
                }
                var sCollection = item.Script(GetOpt(item.Schema, item.Name, "UserDefinedFunction"));
            }
            Console.WriteLine("func over");
        }

        public void GentUserDef()
        {
            foreach (UserDefinedAggregate item in UserDefinedAggregates)
            {
                item.Script(GetOpt(item.Schema, item.Name, "UserDefinedAggregates"));
            }

            foreach (UserDefinedDataType item in UserDefinedDataTypes)
            {
                item.Script(GetOpt(item.Schema, item.Name, "UserDefinedDataTypes"));
            }

            foreach (UserDefinedType item in UserDefinedTypes)
            {
                item.Script(GetOpt(item.Schema, item.Name, "UserDefinedTypes"));
            }
        }

        public void GentUserInfo()
        {
            foreach (Schema item in Schemas)
            {
                if (item.IsSystemObject)
                {
                    continue;
                }
                item.Script(GetOpt("", item.Name, "Schema"));
            }
        }

        private string GetPath(string sch, string name, string objtype)
        {
            var filename = "[{0}].[{1}]";
            if (string.IsNullOrEmpty(sch))
            {
                filename = "{0}{1}";
            }
            var serName = "{0}.{1}";

            var objname = string.Format(filename, sch, name);
            var server = string.Format(serName, ServerAddr, DBName);

            server = CryptManager.SHA1(server);

            if (objname.Contains("\\"))
            {
                objname = objname.Replace("\\", "@");
            }
            if (server.Contains("\\"))
            {
                server = server.Replace("\\", "@");
            }

            var path = Path.Combine(RootPath, server, objtype);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = path + "\\" + objname + ".js";
            return path;
        }

    }
}
