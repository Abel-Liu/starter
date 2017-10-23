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
        
    }
}
