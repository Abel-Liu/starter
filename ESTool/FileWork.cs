using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace ESTool
{
    /// <summary>
    /// 提供IO操作
    /// </summary>
    public static class FileWork
    {
        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="lnkPath">快捷方式完整路径</param>
        /// <param name="sourcePath">源文件路径</param>
        public static void CreatShortCut(this string lnkPath,string sourcePath)
        {
            if (!File.Exists(lnkPath))
            {
                IWshRuntimeLibrary.WshShellClass desk = new IWshRuntimeLibrary.WshShellClass();
                var temp = desk.CreateShortcut(lnkPath) as IWshRuntimeLibrary.IWshShortcut;
                temp.TargetPath = sourcePath;
                temp.WorkingDirectory = Directory.GetParent(sourcePath).FullName;
                temp.WindowStyle = 1;
                temp.IconLocation = sourcePath;
                temp.Save();
            }
        }
        /// <summary>
        /// 判断是文件还是文件夹
        /// </summary>
        /// <param name="path">需要判断的路径</param>
        /// <returns></returns>
        public static FileType JudgeFileType(this string path)
        {
            if (path.Length <= 3)
                return FileType.Driver;
            else
            {
                if (File.Exists(path))
                    return FileType.File;
                else if (Directory.Exists(path))
                    return FileType.Directory;
                else return FileType.None;
            }
        }

        /// <summary>
        /// 根据路径获取默认文件或文件夹名
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string GetNameByPath(this string path)
        {
            try
            {
                FileType type = JudgeFileType(path);
                if (type == FileType.File)
                {
                    string str = path.Substring(path.LastIndexOf('\\') + 1);
                    if (str.LastIndexOf('.') == -1)
                        return str;
                    else
                        return str.Substring(0, str.LastIndexOf('.'));
                }
                else if (type == FileType.Directory)
                {
                    if (path.LastIndexOf('\\') != path.Length)
                    {
                        return path.Substring(path.LastIndexOf('\\') + 1);
                    }
                    else
                    {
                        return path.Substring(0, path.Length - 1).GetNameByPath();
                    }
                }
                else if (type == FileType.Driver)
                {
                    return path.Substring(0, 1).ToUpper() + "盘";
                }
                else return null;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 计算快捷方式所指向的文件地址
        /// </summary>
        /// <param name="path">快捷方式</param>
        /// <returns></returns>
        public static string getPathByLnk(this string path)
        {
            try
            {
                List<byte> bos = new List<byte>();
                FileStream r = new FileStream(path, System.IO.FileMode.Open);
                byte[] bys = new byte[4];
                r.Seek(0x4C, SeekOrigin.Begin);
                r.Read(bys, 0, 2);
                int offset = Bytes2Int(bys, 0, 2);
                int fileLocationInfoSagement = offset + 0x4E;
                int filePathInfoSagement = fileLocationInfoSagement + 0x10;
                r.Seek(filePathInfoSagement, SeekOrigin.Begin);
                r.Read(bys, 0, 4);
                int filePathInfoOffset = fileLocationInfoSagement + Bytes2Int(bys, 0, 4);
                if (filePathInfoOffset < r.Length)
                {
                    r.Seek(filePathInfoOffset, SeekOrigin.Begin);
                    for (byte b = 0; (b = (byte)r.ReadByte()) != 0; )
                        bos.Add(b);
                    Encoding enc = Encoding.Default;
                    return enc.GetString(bos.ToArray());
                }
                else return "";
            }
            catch { return ""; }
        }
        public static int Bytes2Int(byte[] bys, int start, int len)
        {
            int n = 0;
            for (int i = start, k = start + len % 5; i < k; i++)
            {
                n += (bys[i] & 0xff) << (i * 8);
            }
            return n;
        }
    }
}
