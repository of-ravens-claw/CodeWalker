﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CodeWalker.GameFiles
{
    public class YdrFile : GameFile, PackedFile
    {
        public Drawable Drawable { get; set; }

        public YdrFile() : base(null, GameFileType.Ydr)
        {
        }
        public YdrFile(RpfFileEntry entry) : base(entry, GameFileType.Ydr)
        {
        }

        public void Load(byte[] data)
        {
            //direct load from a raw, compressed ydr file

            RpfFile.LoadResourceFile(this, data, (uint)GetVersion(RpfManager.IsGen9));

            Loaded = true;
        }
        public void Load(byte[] data, RpfFileEntry entry)
        {
            Name = entry.Name;
            RpfFileEntry = entry;


            RpfResourceFileEntry resentry = entry as RpfResourceFileEntry;
            if (resentry == null)
            {
                throw new Exception("File entry wasn't a resource! (is it binary data?)");
            }

            ResourceDataReader rd = new ResourceDataReader(resentry, data);

            if (rd.IsGen9)
            {
                switch (resentry.Version)
                {
                    case 159:
                    case 154:
                        break;
                    case 165:
                        rd.IsGen9 = false;
                        break;
                    default:
                        break;
                }
            }

            //MemoryUsage = 0;

#if !DEBUG
            try
#endif
            {
                Drawable = rd.ReadBlock<Drawable>();
                Drawable.Owner = this;
                //MemoryUsage += Drawable.MemoryUsage; //uses decompressed filesize now...
            }
#if !DEBUG
            catch (Exception ex)
            {
                string err = ex.ToString();
            }
#endif

            Loaded = true;

        }

        public byte[] Save()
        {
            var gen9 = RpfManager.IsGen9;
            if (gen9)
            {
                Drawable?.EnsureGen9();
            }

            byte[] data = ResourceBuilder.Build(Drawable, GetVersion(gen9), true, gen9);

            return data;
        }


        public int GetVersion(bool gen9)
        {
            return gen9 ? 159 : 165;
        }

    }




    public class YdrXml : MetaXmlBase
    {

        public static string GetXml(YdrFile ydr, string outputFolder = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(XmlHeader);

            if (ydr?.Drawable != null)
            {
                Drawable.WriteXmlNode(ydr.Drawable, sb, 0, outputFolder);
            }

            return sb.ToString();
        }

    }

    public class XmlYdr
    {

        public static YdrFile GetYdr(string xml, string inputFolder = "")
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return GetYdr(doc, inputFolder);
        }

        public static YdrFile GetYdr(XmlDocument doc, string inputFolder = "")
        {
            YdrFile r = new YdrFile();

            var ddsfolder = inputFolder;

            var node = doc.DocumentElement;
            if (node != null)
            {
                r.Drawable = Drawable.ReadXmlNode(node, ddsfolder);
            }

            r.Name = Path.GetFileName(inputFolder);

            return r;
        }

    }



}
