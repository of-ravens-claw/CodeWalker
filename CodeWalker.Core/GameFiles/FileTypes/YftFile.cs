﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CodeWalker.GameFiles
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class YftFile : GameFile, PackedFile
    {
        public FragType Fragment { get; set; }

        public YftFile() : base(null, GameFileType.Yft)
        {
        }
        public YftFile(RpfFileEntry entry) : base(entry, GameFileType.Yft)
        {
        }

        public void Load(byte[] data)
        {
            //direct load from a raw, compressed yft file

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
                    case 171:
                        break;
                    case 162:
                        rd.IsGen9 = false;
                        break;
                    default:
                        break;
                }
            }


            Fragment = rd.ReadBlock<FragType>();

            if (Fragment != null)
            {
                Fragment.Yft = this;

                if (Fragment.Drawable != null)
                {
                    Fragment.Drawable.Owner = this;
                }
                if (Fragment.DrawableCloth != null)
                {
                    Fragment.DrawableCloth.Owner = this;
                }
            }

            Loaded = true;
        }

        public byte[] Save()
        {
            var gen9 = RpfManager.IsGen9;
            if (gen9)
            {
                Fragment?.EnsureGen9();
            }

            byte[] data = ResourceBuilder.Build(Fragment, GetVersion(gen9), true, gen9);

            return data;
        }

        public int GetVersion(bool gen9)
        {
            return gen9 ? 171 : 162;
        }

    }





    public class YftXml : MetaXmlBase
    {

        public static string GetXml(YftFile yft, string outputFolder = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(XmlHeader);

            if (yft?.Fragment != null)
            {
                FragType.WriteXmlNode(yft.Fragment, sb, 0, outputFolder);
            }

            return sb.ToString();
        }

    }

    public class XmlYft
    {

        public static YftFile GetYft(string xml, string inputFolder = "")
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return GetYft(doc, inputFolder);
        }

        public static YftFile GetYft(XmlDocument doc, string inputFolder = "")
        {
            YftFile r = new YftFile();

            var ddsfolder = inputFolder;

            var node = doc.DocumentElement;
            if (node != null)
            {
                r.Fragment = FragType.ReadXmlNode(node, ddsfolder);
            }

            r.Name = Path.GetFileName(inputFolder);

            return r;
        }

    }




}
