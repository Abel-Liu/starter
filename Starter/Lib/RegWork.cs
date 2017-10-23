using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace ESTool
{
    public static class RegWork
    {
        /// <summary>
        /// 检测软件在此计算机是否第一次运行并在第一次运行时设置键值
        /// </summary>
        /// <returns></returns>
        public static bool CheckFirstRun()
        {
            try
            {
                RegistryKey soft = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
                RegistryKey my = soft.OpenSubKey("EasyStarter", true);
                if (my == null)
                {
                    my = soft.CreateSubKey("EasyStarter");
                    my.SetValue("FirstRun", 0);
                    my.Close();
                    soft.Close();
                    return true;
                }
                else
                {
                    if (my.GetValue("FirstRun", 1).ToString() == "1")////值为1表示还未运行过
                    {
                        my.SetValue("FirstRun", 0);
                        my.Close();
                        soft.Close();
                        return true;
                    }
                    else
                    {
                        my.Close();
                        soft.Close();
                        return false;
                    }
                }
            }
            catch { return false; }
        }
        
        #region 注册表开机启动

        /// <summary>
        /// 检查启动键值
        /// </summary>
        /// <param name="ExePath">应用程序路径</param>
        /// <param name="RegName">键名</param>
        /// <param name="RunPath">注册表中表示启动项的路径</param>
        public static void CheckRegRun(this string ExePath, string RegName = "EasyStarter", string RunPath = @"Software\Microsoft\Windows\CurrentVersion\Run")
        {
            if (!ExistRegRun(RunPath, RegName))  //////不存在则创建
            {
                AddRegRun(ExePath, RegName, RunPath);
            }
            else if (GetRunValue(RunPath, RegName).ToString().ToLower() != ExePath.ToLower())////值不对则更新
            {
                AddRegRun(ExePath, RegName, RunPath);
            }
        }
        /// <summary>
        /// 从注册表中删除键值.成功返回true,否则返回false
        /// </summary>
        /// <param name="keyPath">位于注册表的路径</param>
        /// <param name="keyName">要删除的键名</param>
        public static bool DelRegRun(string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Run", string keyName = "EasyStarter")
        {
            try
            {
                RegistryKey hklm = Registry.LocalMachine;
                RegistryKey runs;
                runs = hklm.OpenSubKey(keyPath, true);
                string[] names = runs.GetValueNames();
                foreach (string str in names)
                {
                    if (str.ToUpper() == keyName.ToUpper())
                    {
                        runs.DeleteValue(keyName, false);
                        break;
                    }
                }
                runs.Close();
                hklm.Close();
                return true;
            }
            catch { return false; }
        }

        #region 私有方法
        /// <summary>
        /// 向注册表添加键值.成功返回true,否则返回false
        /// </summary>
        /// <param name="keyValue">键值(软件路径)</param>
        /// <param name="keyName">键名(显示名称)</param>
        /// <param name="keyPath">注册表中表示启动项的路径</param>
        /// <returns></returns>
        private static bool AddRegRun(string keyValue, string keyName, string keyPath)
        {
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey runs;
            try
            {
                runs = hklm.CreateSubKey(keyPath);
                runs.SetValue(keyName, keyValue);
                runs.Close();
                hklm.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 在注册表中检索与指定名称关联的值
        /// </summary>
        /// <param name="keyPath">位于注册表的路径</param>
        /// <param name="keyName">键名</param>
        /// <returns></returns>
        private static object GetRunValue(string keyPath, string keyName)
        {
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey runs;
            runs = hklm.OpenSubKey(keyPath, true);
            object result = runs.GetValue(keyName);
            runs.Close();
            hklm.Close();
            return result;
        }
        /// <summary>
        /// 判断注册表中指定键值是否存在
        /// </summary>
        /// <param name="keyPath">位于注册表的路径</param>
        /// <param name="keyName">要判断的键名</param>
        /// <returns></returns>
        private static bool ExistRegRun(string keyPath, string keyName)
        {
            bool result = false;
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey runs;
            runs = hklm.OpenSubKey(keyPath, true);
            string[] names = runs.GetValueNames();
            foreach (string str in names)
            {
                if (str.ToUpper() == keyName.ToUpper())
                {
                    result = true;
                    break;
                }
            }
            runs.Close();
            hklm.Close();
            return result;
        }
        #endregion
        #endregion

        #region 注册表右键菜单
        /// <summary>
        /// 添加右键菜单
        /// </summary>
        /// <param name="programPath">应用程序的路径，包括可执行文件的名称。</param>
        /// <param name="displayName">右键菜单上显示的名称</param>
        public static void AddRegMenu(this string programPath, string displayName = "添加到EasyStarter")
        {
            try
            {
                RegistryKey shell = Registry.ClassesRoot.OpenSubKey("*", true).OpenSubKey("shell", true);
                if (shell == null) shell = Registry.ClassesRoot.OpenSubKey("*", true).CreateSubKey("shell");

                RegistryKey custome = shell.OpenSubKey(displayName, true);
                if (custome == null) custome = shell.CreateSubKey(displayName);
                custome.SetValue("icon", programPath);

                RegistryKey cmd = custome.OpenSubKey("command", true);
                if (cmd == null) cmd = custome.CreateSubKey("command");
                cmd.SetValue("", programPath + " \"%1\"");
                cmd.Close();
                custome.Close();
                shell.Close();
            }
            catch (Exception e) { throw e; }
            try
            {
                RegistryKey shell = Registry.ClassesRoot.OpenSubKey("directory", true).OpenSubKey("shell", true);
                if (shell == null) shell = Registry.ClassesRoot.OpenSubKey("directory", true).CreateSubKey("shell");

                RegistryKey custome = shell.OpenSubKey(displayName, true);
                if (custome == null) custome = shell.CreateSubKey(displayName);
                custome.SetValue("icon", programPath);

                RegistryKey cmd = custome.OpenSubKey("command", true);
                if (cmd == null) cmd = custome.CreateSubKey("command");
                cmd.SetValue("", programPath + " \"%1\"");
                cmd.Close();
                custome.Close();
                shell.Close();
            }
            catch (Exception e)
            { throw e; }
        }
        /// <summary>
        /// 检测右键菜单
        /// </summary>
        /// <param name="programPath">应用程序的路径，包括可执行文件的名称。</param>
        /// <param name="displayName">右键菜单上显示的名称</param>
        public static bool CheckRegMenu(string programPath, string displayName = "添加到EasyStarter")
        {
            try
            {
                RegistryKey shell = Registry.ClassesRoot.OpenSubKey("*", true).OpenSubKey("shell", true);
                if (shell == null)
                    return false;
                RegistryKey custome = shell.OpenSubKey(displayName, true);
                if (custome == null)
                    return false;
                if (custome.GetValue("icon", "0").ToString() != programPath)
                    return false;
                RegistryKey cmd = custome.OpenSubKey("command", true);
                if (cmd == null)
                    return false;
                if (cmd.GetValue("", "0").ToString() != programPath + " \"%1\"")
                    return false;

                RegistryKey shell2 = Registry.ClassesRoot.OpenSubKey("directory", true).OpenSubKey("shell", true);
                if (shell == null)
                    return false;
                RegistryKey custome2 = shell.OpenSubKey(displayName, true);
                if (custome == null)
                    return false;
                if (custome.GetValue("icon", "0").ToString() != programPath)
                    return false;
                RegistryKey cmd2 = custome.OpenSubKey("command", true);
                if (cmd == null)
                    return false;
                string result2 = cmd.GetValue("", 0).ToString();
                cmd.Close();
                custome.Close();
                shell.Close();
                cmd2.Close();
                custome2.Close();
                shell2.Close();
                if (result2 != programPath + " \"%1\"")
                    return false;
                else return true;
            }
            catch
            { return false; }

        }
        /// <summary>
        /// 删除右键菜单
        /// </summary>
        /// <param name="displayName">右键菜单上显示的名称</param>
        public static void DelRegMenu(string displayName = "添加到EasyStarter")
        {
            try
            {
                RegistryKey shell = Registry.ClassesRoot.OpenSubKey("*", true).OpenSubKey("shell", true);
                if (shell != null) shell.DeleteSubKeyTree(displayName);

                shell = Registry.ClassesRoot.OpenSubKey("directory", true).OpenSubKey("shell", true);
                if (shell != null) shell.DeleteSubKeyTree(displayName);
                shell.Close();
            }
            catch (Exception e)
            { throw e; }
        }
        #endregion
    }
}
