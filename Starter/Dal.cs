using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Xml.Linq;
using ESTool;
using System.IO;

namespace Starter
{
    /// <summary>
    /// 数据层
    /// </summary>
    static class Dal
    {
        private static string DataPath =Path.Combine(MyWork.StartDir,"data.xml");
        //private static string DataPath = System.IO.Path.Combine(MyWork.StartDir, @"data.xml");
        #region 配置文件操作
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="setPath">配置文件路径</param>
        /// <param name="info">配置信息的结构</param>
        public static void ReadSetting(this string setPath, out SettingInfo info)
        {
            if (File.Exists(setPath))
            {
                var data = XElement.Load(setPath);
                
                info.Boot = data.Element("Boot").Value.ToLower() == "true" ? true : false;
                info.SysRightMenu = data.Element("RightMenu").Value.ToLower() == "true" ? true : false;
                info.HotKey = data.Element("HotKey").Value.ToLower();
                info.BackImg = data.Element("BackImg").Value;
                info.SendBug = data.Element("SendBug").Value.ToLower() == "true" ? true : false;
            }
            else
            {
                info = MyWork.DefaultSettingInfo;
                MyWork.DefaultSettingInfo.SaveToConfig(MyWork.ConfigPath);
            }
        }

        /// <summary>
        /// 将配置保存到文件
        /// </summary>
        /// <param name="setPath">配置文件路径</param>
        /// <param name="info">配置信息</param>
        public static void SaveToConfig(this SettingInfo info, string setPath)
        {
            if (!File.Exists(setPath))
            {
                XElement temp = new XElement("Config",
                    new XElement("Boot", info.Boot),
                    new XElement("RightMenu", info.SysRightMenu),
                    new XElement("HotKey", info.HotKey),
                    new XElement("BackImg", info.BackImg),
                    new XElement("SendBug", info.SendBug));

                temp.Save(setPath);
            }
            else
            {
                var config = XElement.Load(setPath);
                config.Element("Boot").SetValue(info.Boot);
                config.Element("RightMenu").SetValue(info.SysRightMenu);
                config.Element("HotKey").SetValue(info.HotKey);
                config.Element("BackImg").SetValue(info.BackImg);
                config.Element("SendBug").SetValue(info.SendBug);
                config.Save(setPath);
            }
        }
        
        /// <summary>
        /// 保存背景配置
        /// </summary>
        /// <param name="value">要设置的值</param>
        /// <param name="configpath">配置文件路径</param>
        public static void SetBackConfig(this string value,string configPath)
        {
            if (File.Exists(configPath))
            {
                var data = XElement.Load(configPath);
                data.Element("BackImg").SetValue(value);
                data.Save(configPath);
            }
        }
        /// <summary>
        /// 检测是否自动发送错误报告
        /// </summary>
        /// <param name="configPath">配置文件路径</param>
        /// <returns></returns>
        public static bool CheckAutoBug(this string configPath)
        {
            if (File.Exists(configPath))
            {
                var data = XElement.Load(configPath);
                return data.Element("SendBug").Value == "true" ? true : false;
            }
            else return true;
        }
        #endregion

        public static void Add(string path, string name, int page, int index, int type)
        {
            var data = XElement.Load(DataPath);
            XElement temp = new XElement("Item",
                new XElement("Name", name),
                new XElement("Path", path),
                new XElement("Type", type),
                new XElement("Page", page),
                new XElement("Index", index));
            data.Add(temp);
            data.Save(DataPath);
        }
        /// <summary>
        /// 修复数据
        /// </summary>
        public static void RepairData()
        {
            var data = XElement.Load(DataPath);
            var pages = (from page in data.Descendants("Page")
                         select page).Distinct(new ComparerPageIndexInXml());
            foreach (var p in pages)
            {
                var items = (from item in data.Descendants("Item")
                             where item.Element("Page").Value == p.Value
                             select item).OrderBy(q => int.Parse(q.Element("Index").Value));
                
                for (int i = 0; i < items.Count(); i++)
                {
                    items.ElementAt(i).Element("Index").Value = i.ToString();
                }
            }
            data.Save(DataPath);
        }
        
        /// <summary>
        /// 更新指定页及之后的页码
        /// </summary>
        /// <param name="pageIndex">需要更新的首个页码</param>
        public static void UpdatePage(this int pageIndex)
        {
            var data = XElement.Load(DataPath);
            var items = from item in data.Descendants("Item")
                        where int.Parse(item.Element("Page").Value) >= pageIndex
                        select item;
            foreach (var i in items)
                i.Element("Page").SetValue(int.Parse(i.Element("Page").Value) - 1);
            data.Save(DataPath);
        }
        /// <summary>
        /// 更新索引
        /// </summary>
        /// <param name="page">需要更新索引的页面</param>
        /// <param name="index">被删除的项目索引</param>
        public static void UpdateIndex(this int page, int index)
        {
            var data = XElement.Load(DataPath);
            var items = from item in data.Descendants("Item")
                        where int.Parse(item.Element("Index").Value) > index &&
                        item.Element("Page").Value == page.ToString()
                        select item;
            foreach (var i in items)
                i.Element("Index").SetValue(int.Parse(i.Element("Index").Value) - 1);
            data.Save(DataPath);
        }
        /// <summary>
        /// 修改显示名称
        /// </summary>
        /// <param name="path">需要修改的项目的路径</param>
        /// <param name="newname">新名称</param>
        public static void ChangeName(this string path, string newname)
        {
            var data = XElement.Load(DataPath);
            var result = from item in data.Descendants("Item")
                         where item.Element("Path").Value == path
                         select item;
            result.First().Element("Name").Value = newname;
            data.Save(DataPath);
        }
        /// <summary>
        /// 修改索引
        /// </summary>
        /// <param name="path">需要修改的项目的路径</param>
        /// <param name="newindex">新索引</param>
        public static void ChangeIndex(this string path, int newindex)
        {
            var data = XElement.Load(DataPath);
            var result = from item in data.Descendants("Item")
                         where item.Element("Path").Value == path
                         select item;
            result.First().Element("Index").Value = newindex.ToString();
            data.Save(DataPath);
        }
        /// <summary>
        /// 修改索引
        /// </summary>
        /// <param name="path">需要修改的项目的路径</param>
        /// <param name="newpage">新页码</param>
        /// <param name="newindex">新索引</param>
        public static void ChangeIndex(this string path, int newpage, int newindex)
        {
            var data = XElement.Load(DataPath);
            var result = from item in data.Descendants("Item")
                         where item.Element("Path").Value == path
                         select item;
            result.First().Element("Page").Value = newpage.ToString();
            result.First().Element("Index").Value = newindex.ToString();
            data.Save(DataPath);
        }
        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="path">需要检查的路径</param>
        /// <returns></returns>
        public static bool CheckExists(this string path)
        {
            var data = XElement.Load(DataPath);
            var result = from item in data.Descendants("Item")
                         where item.Element("Path").Value == path
                         select item;
            return result.Count() > 0 ? true : false;
        }
        /// <summary>
        /// 获取页面数目
        /// </summary>
        /// <returns></returns>
        public static int GetPageCount()
        {
            var data = XElement.Load(DataPath);
            var pages = (from page in data.Descendants("Page")
                         select page).Distinct(new ComparerPageIndexInXml());
            return pages.Count() == 0 ? 0 : pages.Max(q => int.Parse(q.Value));
        }
        /// <summary>
        /// 删除指定项目
        /// </summary>
        /// <param name="path">项目的路径</param>
        public static void Del(this string path)
        {
            var data = XElement.Load(DataPath);
            var result = from item in data.Descendants("Item")
                         where item.Element("Path").Value == path
                         select item;
            result.Remove();
            data.Save(DataPath);
        }
        /// <summary>
        /// 删除指定页的所有项目
        /// </summary>
        /// <param name="page">页面索引</param>
        public static void Del(this int page)
        {
            var data = XElement.Load(DataPath);
            var result = from item in data.Descendants("Item")
                         where item.Element("Page").Value == page.ToString()
                         select item;

            result.Remove();
            data.Save(DataPath);
        }
        
        /// <summary>
        /// 获取指定页的所有项目
        /// </summary>
        /// <param name="page">页面索引</param>
        /// <returns></returns>
        public static List<ItemInfo> GetData(this int page)
        {
            List<ItemInfo> infolist = new List<ItemInfo>();
            var data = XElement.Load(DataPath);
            var items = (from item in data.Descendants("Item")
                         where item.Element("Page").Value == page.ToString()
                         select item).OrderBy(q => int.Parse(q.Element("Index").Value));
            
            foreach (var i in items)
                infolist.Add(new ItemInfo(
                    i.Descendants("Path").Single().Value,
                    i.Descendants("Name").Single().Value,
                    int.Parse(i.Descendants("Page").Single().Value),
                    int.Parse(i.Descendants("Index").Single().Value),
                    int.Parse(i.Descendants("Type").Single().Value)
                    ));
            return infolist;
        }
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public static List<ItemInfo> GetData()
        {
            List<ItemInfo> infolist = new List<ItemInfo>();
            var data = XElement.Load(DataPath);
            var items = from item in data.Descendants("Item") select item;
            foreach (var i in items)
                infolist.Add(new ItemInfo(
                    i.Element("Path").Value,
                    i.Element("Name").Value,
                    int.Parse(i.Element("Page").Value),
                    int.Parse(i.Element("Index").Value),
                    int.Parse(i.Element("Type").Value)
                    ));
            return infolist;
        }

        #region
        /// <summary>
        /// 连接字符串
        /// </summary>
        //private static string connstr = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=" + MyWork.StartDir + "Data.mdb";

        ///// <summary>
        ///// 修复数据库中的错误索引
        ///// </summary>
        //public static void RepairData()
        //{
        //    string selectpagestr = string.Format("select distinct mPage from item");
        //    OleDbConnection conn = new OleDbConnection(connstr);
        //    OleDbDataAdapter selectpageda = new OleDbDataAdapter(selectpagestr, conn);
        //    DataSet allpage = new DataSet();
        //    conn.Open();
        //    selectpageda.Fill(allpage);
        //    int page;
        //    int oldindex;
            
        //    foreach (DataRow dr in allpage.Tables[0].Rows)
        //    {
        //        page = int.Parse(dr.ItemArray[0].ToString());
        //        OleDbDataAdapter da = new OleDbDataAdapter(string.Format("select mIndex from item where mPage={0} order by mIndex", page), conn);
        //        DataSet allIndexofPage = new DataSet();
        //        da.Fill(allIndexofPage);
        //        OleDbCommand cmd = new OleDbCommand("", conn);
        //        for (int i = 0; i < allIndexofPage.Tables[0].Rows.Count; i++)
        //        {
        //            oldindex = int.Parse(allIndexofPage.Tables[0].Rows[i].ItemArray[0].ToString());
        //            cmd.CommandText = string.Format("update item set mIndex={0} where mPage={1} and mIndex={2}", i, page, oldindex);
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //    conn.Close();
        //}

        ///// <summary>
        ///// 向数据库添加一条记录。
        ///// </summary>
        //public static int Add(ItemInfo info)
        //{
        //    try
        //    {
        //        string str = string.Format("insert into item values('{0}','{1}',{2},{3},{4})", info.ItemPath, info.ItemName, info.ItemIndex, info.ItemPage, info.ItemType);
        //        OleDbConnection conn = new OleDbConnection(connstr);
        //        OleDbCommand cmd = new OleDbCommand(str, conn);
        //        conn.Open();
        //        cmd.ExecuteNonQuery();
        //        conn.Close();
        //        return 1;
        //    }
        //    catch (Exception e)
        //    {
        //        MyWork.NoteLog(e);
        //        return 0;
        //    }
        //}

        //public static int Add(ItemInfo[] info)
        //{
        //    return 0;
        //}

        ///// <summary>
        ///// 检查是否存在
        ///// </summary>
        ///// <param name="path">路径</param>
        ///// <returns></returns>
        //public static bool CheckExist(string path)
        //{
        //    try
        //    {
        //        string strcheck = string.Format("select count(*) from item where mPath='{0}'", path);
        //        OleDbConnection conn = new OleDbConnection(connstr);
        //        OleDbDataAdapter da = new OleDbDataAdapter(strcheck, conn);
        //        DataSet ds = new DataSet();
        //        conn.Open();
        //        da.Fill(ds);
        //        conn.Close();
        //        int c = int.Parse(ds.Tables[0].Rows[0].ItemArray[0].ToString());
        //        return c > 0 ? true : false;
        //    }
        //    catch (Exception e)
        //    {
        //        MyWork.NoteLog(e);
        //        return false;
        //    }
        //}
        ///// <summary>
        ///// 更改名称
        ///// </summary>
        ///// <param name="path">路径</param>
        ///// <param name="newname">新名称</param>
        ///// <returns></returns>
        //public static int Change(string path, string newname)
        //{
        //    try
        //    {
        //        string str = string.Format("update item set mName='{0}' where mPath='{1}'", newname, path);
        //        OleDbConnection conn = new OleDbConnection(connstr);
        //        OleDbCommand cmd = new OleDbCommand(str, conn);
        //        conn.Open();
        //        cmd.ExecuteNonQuery();
        //        conn.Close();
        //        return 1;
        //    }
        //    catch (Exception e)
        //    {
        //        MyWork.NoteLog(e);
        //        return 0;
        //    }
        //}
        ///// <summary>
        ///// 修改项目索引
        ///// </summary>
        ///// <param name="path">要修该的项目路径</param>
        ///// <param name="newIndex">新索引</param>
        //public static void ChangeIndex(string path, int newIndex)
        //{
        //    try
        //    {
        //        string str = string.Format("update item set mIndex={0} where mPath='{1}'", newIndex, path);
        //        OleDbConnection conn = new OleDbConnection(connstr);
        //        OleDbCommand cmd = new OleDbCommand(str, conn);
        //        conn.Open();
        //        cmd.ExecuteNonQuery();
        //        conn.Close();
        //    }
        //    catch (Exception e)
        //    {
        //        MyWork.NoteLog(e);
        //    }
        //}
        ///// <summary>
        ///// 修改项目索引
        ///// </summary>
        ///// <param name="path">要修该的项目路径</param>
        ///// <param name="newPage">新页面</param>
        ///// <param name="newIndex">新索引</param>
        //public static void ChangeIndex(string path, int newPage, int newIndex)
        //{
        //    try
        //    {
        //        string str = string.Format("update item set mIndex={0},mPage={1} where mPath='{2}'", newIndex, newPage, path);
        //        OleDbConnection conn = new OleDbConnection(connstr);
        //        OleDbCommand cmd = new OleDbCommand(str, conn);
        //        conn.Open();
        //        cmd.ExecuteNonQuery();
        //        conn.Close();
        //    }
        //    catch (Exception e)
        //    {
        //        MyWork.NoteLog(e);
        //    }
        //}
        ///// <summary>
        ///// 删除一条数据
        ///// </summary>
        ///// <param name="path">路径</param>
        ///// <returns></returns>
        //public static int Del(string path)
        //{
        //    try
        //    {
        //        string str = string.Format("delete from item where mPath='{0}'", path);
        //        OleDbConnection conn = new OleDbConnection(connstr);
        //        OleDbCommand cmd = new OleDbCommand(str, conn);
        //        conn.Open();
        //        cmd.ExecuteNonQuery();
        //        conn.Close();
        //        return 1;
        //    }
        //    catch (Exception e)
        //    {
        //        MyWork.NoteLog(e);
        //        return 0;
        //    }
        //}
        ///// <summary>
        ///// 删除指定页的所有数据
        ///// </summary>
        ///// <param name="page">指定页</param>
        ///// <returns></returns>
        //public static int Del(int page)
        //{
        //    try
        //    {
        //        string str = string.Format("delete from item where mPage={0}", page);
        //        OleDbConnection conn = new OleDbConnection(connstr);
        //        OleDbCommand cmd = new OleDbCommand(str, conn);
        //        conn.Open();
        //        cmd.ExecuteNonQuery();
        //        conn.Close();
        //        return 1;
        //    }
        //    catch (Exception e)
        //    {
        //        MyWork.NoteLog(e);
        //        return 0;
        //    }

        //}
        ///// <summary>
        ///// 更新指定页及之后的页码
        ///// </summary>
        ///// <param name="pageIndex">需要更新的页码</param>
        ///// <returns></returns>
        //public static int UpdatePage(int pageIndex)
        //{
        //    try
        //    {
        //        string str = string.Format("update item set mPage=mPage-1 where mPage>={0}", pageIndex);
        //        OleDbConnection conn = new OleDbConnection(connstr);
        //        OleDbCommand cmd = new OleDbCommand(str, conn);
        //        conn.Open();
        //        cmd.ExecuteNonQuery();
        //        conn.Close();
        //        return 1;
        //    }
        //    catch (Exception e)
        //    {
        //        MyWork.NoteLog(e);
        //        return 0;
        //    }
        //}
        ///// <summary>
        ///// 更新索引
        ///// </summary>
        ///// <param name="index">被删除的项目索引</param>
        ///// <param name="page">所在页面</param>
        ///// <returns></returns>
        //public static int UpdateIndex(int index, int page)
        //{
        //    try
        //    {
        //        string str = string.Format("update item set mIndex=mIndex-1 where mIndex>{0} and mpage={1}", index, page);
        //        OleDbConnection conn = new OleDbConnection(connstr);
        //        OleDbCommand cmd = new OleDbCommand(str, conn);
        //        conn.Open();
        //        cmd.ExecuteNonQuery();
        //        conn.Close();
        //        return 1;
        //    }
        //    catch (Exception e)
        //    {
        //        MyWork.NoteLog(e);
        //        return 0;
        //    }
        //}

        ///// <summary>
        ///// 读取指定页面的所有数据
        ///// </summary>
        ///// <param name="page">页面</param>
        ///// <returns></returns>
        //public static DataTable GetData(int page)
        //{
        //    try
        //    {
        //        string str = string.Format("select * from item where mPage={0} order by mIndex", page);
        //        OleDbConnection conn = new OleDbConnection(connstr);
        //        OleDbDataAdapter da = new OleDbDataAdapter(str, conn);
        //        DataSet ds = new DataSet();
        //        conn.Open();
        //        da.Fill(ds);
        //        conn.Close();
        //        return ds.Tables[0];
        //    }
        //    catch (Exception e)
        //    {
        //        MyWork.NoteLog(e);
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// 读取所有数据
        ///// </summary>
        ///// <returns></returns>
        //public static DataTable GetData()
        //{
        //    try
        //    {
        //        string str = "select * from item order by mPage,mIndex";
        //        OleDbConnection conn = new OleDbConnection(connstr);
        //        OleDbDataAdapter da = new OleDbDataAdapter(str, conn);
        //        DataSet ds = new DataSet();
        //        conn.Open();
        //        da.Fill(ds);
        //        conn.Close();
        //        return ds.Tables[0];
        //    }
        //    catch (Exception e)
        //    {
        //        MyWork.NoteLog(e);
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// 获取页面数目
        ///// </summary>
        ///// <returns></returns>
        //public static int GetPageCount()
        //{
        //    try
        //    {
        //        string str = "select mPage from item order by mPage desc";
        //        OleDbConnection conn = new OleDbConnection(connstr);
        //        OleDbDataAdapter da = new OleDbDataAdapter(str, conn);
        //        DataSet ds = new DataSet();
        //        conn.Open();
        //        da.Fill(ds);
        //        conn.Close();
        //        if (ds.Tables[0].Rows.Count == 0)
        //            return 0;
        //        else
        //            return int.Parse(ds.Tables[0].Rows[0].ItemArray[0].ToString());
        //    }
        //    catch (Exception e)
        //    {
        //        MyWork.NoteLog(e);
        //        return 0;
        //    }
        //}
        #endregion
    }
}
