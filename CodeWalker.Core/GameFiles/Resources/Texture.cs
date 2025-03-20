using CodeWalker.Utils;
using CodeWalker.World;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CodeWalker.GameFiles
{


    [TypeConverter(typeof(ExpandableObjectConverter))] public class TextureDictionary : ResourceFileBase
    {
        public override long BlockLength => 64;

        // structure data
        public uint Unknown_10h { get; set; } // 0x00000000
        public uint Unknown_14h { get; set; } // 0x00000000
        public uint Unknown_18h { get; set; } = 1; // 0x00000001
        public uint Unknown_1Ch { get; set; } // 0x00000000
        public ResourceSimpleList64_uint TextureNameHashes { get; set; }
        public ResourcePointerList64<Texture> Textures { get; set; }

        public Dictionary<uint, Texture> Dict { get; set; }

        public long MemoryUsage
        {
            get
            {
                long val = 0;
                if ((Textures != null) && (Textures.data_items != null))
                {
                    foreach (var tex in Textures.data_items)
                    {
                        if (tex != null)
                        {
                            val += tex.MemoryUsage;
                        }
                    }
                }
                return val;
            }
        }


        public TextureDictionary()
        { }

        public override void Read(ResourceDataReader reader, params object[] parameters)
        {
            base.Read(reader, parameters);

            // read structure data
            this.Unknown_10h = reader.ReadUInt32();
            this.Unknown_14h = reader.ReadUInt32();
            this.Unknown_18h = reader.ReadUInt32();
            this.Unknown_1Ch = reader.ReadUInt32();
            this.TextureNameHashes = reader.ReadBlock<ResourceSimpleList64_uint>();
            this.Textures = reader.ReadBlock<ResourcePointerList64<Texture>>();

            BuildDict();
        }
        public override void Write(ResourceDataWriter writer, params object[] parameters)
        {
            base.Write(writer, parameters);


            // write structure data
            writer.Write(this.Unknown_10h);
            writer.Write(this.Unknown_14h);
            writer.Write(this.Unknown_18h);
            writer.Write(this.Unknown_1Ch);
            writer.WriteBlock(this.TextureNameHashes);
            writer.WriteBlock(this.Textures);
        }
        public void WriteXml(StringBuilder sb, int indent, string ddsfolder)
        {

            if (Textures?.data_items != null)
            {
                foreach (var tex in Textures.data_items)
                {
                    YtdXml.OpenTag(sb, indent, "Item");
                    tex.WriteXml(sb, indent + 1, ddsfolder);
                    YtdXml.CloseTag(sb, indent, "Item");
                }
            }

        }
        public void ReadXml(XmlNode node, string ddsfolder)
        {
            var textures = new List<Texture>();

            var inodes = node.SelectNodes("Item");
            if (inodes != null)
            {
                foreach (XmlNode inode in inodes)
                {
                    var tex = new Texture();
                    tex.ReadXml(inode, ddsfolder);
                    textures.Add(tex);
                }
            }

            BuildFromTextureList(textures);
        }
        public static void WriteXmlNode(TextureDictionary d, StringBuilder sb, int indent, string ddsfolder, string name = "TextureDictionary")
        {
            if (d == null) return;
            if ((d.Textures?.data_items == null) || (d.Textures.data_items.Length == 0))
            {
                YtdXml.SelfClosingTag(sb, indent, name);
            }
            else
            {
                YtdXml.OpenTag(sb, indent, name);
                d.WriteXml(sb, indent + 1, ddsfolder);
                YtdXml.CloseTag(sb, indent, name);
            }
        }
        public static TextureDictionary ReadXmlNode(XmlNode node, string ddsfolder)
        {
            if (node == null) return null;
            var td = new TextureDictionary();
            td.ReadXml(node, ddsfolder);
            return td;
        }


        public override Tuple<long, IResourceBlock>[] GetParts()
        {
            return new Tuple<long, IResourceBlock>[] {
                new Tuple<long, IResourceBlock>(0x20, TextureNameHashes),
                new Tuple<long, IResourceBlock>(0x30, Textures)
            };
        }

        public Texture Lookup(uint hash)
        {
            Texture tex = null;
            if (Dict != null)
            {
                Dict.TryGetValue(hash, out tex);
            }
            return tex;
        }

        private void BuildDict()
        {
            var dict = new Dictionary<uint, Texture>();
            if ((Textures?.data_items != null) && (TextureNameHashes?.data_items != null))
            {
                for (int i = 0; (i < Textures.data_items.Length) && (i < TextureNameHashes.data_items.Length); i++)
                {
                    var tex = Textures.data_items[i];
                    var hash = TextureNameHashes.data_items[i];
                    dict[hash] = tex;
                }
            }
            Dict = dict;
        }

        public void BuildFromTextureList(List<Texture> textures)
        {
            textures.Sort((a, b) => a.NameHash.CompareTo(b.NameHash));
            
            var texturehashes = new List<uint>();
            foreach (var tex in textures)
            {
                texturehashes.Add(tex.NameHash);
            }

            TextureNameHashes = new ResourceSimpleList64_uint();
            TextureNameHashes.data_items = texturehashes.ToArray();
            Textures = new ResourcePointerList64<Texture>();
            Textures.data_items = textures.ToArray();
            BuildDict();
        }


        public void EnsureGen9()
        {
            // do nothing. we don't support gen9.
        }

    }

    [TypeConverter(typeof(ExpandableObjectConverter))] public class TextureBase : ResourceSystemBlock
    {
        public override long BlockLength => 72; // sizeof(rage::grcTexture)

        // structure data
        public ulong VFT { get; set; }
        public ulong Unknown_8h { get; set; } // pgBase

        // begin grcCellGcmTextureWrapper
        public TextureFormatOrbis Format { get; set; }
        public byte Levels { get; set; } // Number of mip maps
        public byte Dimension { get; set; }
        public byte ImageType { get; set; }

        public uint OwnsMemory { get; set; } // Always 0 on Resources

        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public ushort Depth { get; set; }
        public GrcTileMode TileMode { get; set; }
        public GrcBindFlags BindFlag { get; set; }

        public uint UsesPreAllocatedMem { get; set; } // Always 0 on Resources
        public uint Offset { get; set; } // Seems to be unused?
        // end grcCellGcmTextureWrapper

        public ulong NamePointer { get; set; }
        public ushort RefCount { get; set; } = 1;
        public ushort Unknown_32h { get; set; }
        public uint Unknown_34h { get; set; } // 0x00000000
        public uint Unknown_38h { get; set; } // 0x00000000
        public uint Unknown_3Ch { get; set; } // 0x00000000
        public uint UsageData { get; set; }
        public uint Unknown_44h { get; set; } // 0x00000000


        // reference data
        public string Name { get; set; }
        public uint NameHash { get; set; }

        private string_r NameBlock = null;

        public TextureData Data { get; set; }

        public TextureUsage Usage
        {
            get => (TextureUsage)(UsageData & 0x1F);
            set => UsageData = (UsageData & 0xFFFFFFE0) + (((uint)value) & 0x1F);
        }
        public TextureUsageFlags UsageFlags
        {
            get => (TextureUsageFlags)(UsageData >> 5);
            set => UsageData = (UsageData & 0x1F) + (((uint)value) << 5);
        }

        public override void Read(ResourceDataReader reader, params object[] parameters)
        {
            var initPos = reader.Position;

            // read structure data
            this.VFT = reader.ReadUInt64();

            this.Unknown_8h = reader.ReadUInt64();

            // grcCellGcmTextureWrapper
            Format = (TextureFormatOrbis)reader.ReadByte();
            Levels = reader.ReadByte();
            Dimension = reader.ReadByte();
            ImageType = reader.ReadByte();

            OwnsMemory = reader.ReadUInt32();

            Width = reader.ReadUInt16();
            Height = reader.ReadUInt16();
            Depth = reader.ReadUInt16();
            TileMode = (GrcTileMode)reader.ReadByte();
            BindFlag = (GrcBindFlags)reader.ReadByte();

            UsesPreAllocatedMem = reader.ReadUInt32();
            Offset = reader.ReadUInt32();

            this.NamePointer = reader.ReadUInt64();
            this.RefCount = reader.ReadUInt16();
            this.Unknown_32h = reader.ReadUInt16();
            this.Unknown_34h = reader.ReadUInt32();
            this.Unknown_38h = reader.ReadUInt32();
            this.Unknown_3Ch = reader.ReadUInt32();
            this.UsageData = reader.ReadUInt32();
            this.Unknown_44h = reader.ReadUInt32();

            // read reference data
            this.Name = reader.ReadStringAt( //BlockAt<string_r>(
                this.NamePointer // offset
            );

            if (!string.IsNullOrEmpty(Name))
            {
                NameHash = JenkHash.GenHash(Name.ToLowerInvariant());
            }

            var endPos = reader.Position;

            if ((endPos - initPos) != 72) // sizeof(rage::grcTexture))
                throw new Exception("Failed to read the correct amount of bytes");

            // tests
            if (OwnsMemory != 0)
            { }
            if (UsesPreAllocatedMem != 0)
            { }
            if (Offset != 0)
            { }
            if (BindFlag != GrcBindFlags.None)
            { }

            if (ImageType != 0)
            {}

            switch (TileMode)
            {
                case GrcTileMode.Thin_1dThin: break; // depth > 1
                case GrcTileMode.Display_LinearAligned: Debugger.Break(); break; // depth == 0

                case GrcTileMode.Depth_2dThin_64:
                    if (NameHash == 0x5C74C225) // "givemechecker"
                        break;

                    goto default; // fallthrough

                default: break;
            }
        }
        public override void Write(ResourceDataWriter writer, params object[] parameters)
        {
            var initPos = writer.Position;

            // update structure data
            this.NamePointer = (ulong)(this.NameBlock != null ? this.NameBlock.FilePosition : 0);

            // write structure data
            writer.Write(this.VFT);
            writer.Write(this.Unknown_8h);

            // grcCellGcmTextureWrapper
            writer.Write((byte)this.Format);
            writer.Write(this.Levels);
            writer.Write(this.Dimension);
            writer.Write(this.ImageType);
            writer.Write(this.OwnsMemory);
            writer.Write(this.Width);
            writer.Write(this.Height);
            writer.Write(this.Depth);
            writer.Write((byte)this.TileMode);
            writer.Write((byte)this.BindFlag);
            writer.Write(this.UsesPreAllocatedMem);
            writer.Write(this.Offset);

            writer.Write(this.NamePointer);
            writer.Write(this.RefCount);
            writer.Write(this.Unknown_32h);
            writer.Write(this.Unknown_34h);
            writer.Write(this.Unknown_38h);
            writer.Write(this.Unknown_3Ch);
            writer.Write(this.UsageData);
            writer.Write(this.Unknown_44h);

            var endPos = writer.Position;

            if ((endPos - initPos) != 72) // sizeof(rage::grcTexture))
                throw new Exception("Failed to write the correct amount of bytes");
        }
        public virtual void WriteXml(StringBuilder sb, int indent, string ddsfolder)
        {
            YtdXml.StringTag(sb, indent, "Name", YtdXml.XmlEscape(Name));
            YtdXml.ValueTag(sb, indent, "Unk32", Unknown_32h.ToString());
            YtdXml.StringTag(sb, indent, "Usage", Usage.ToString());
            YtdXml.StringTag(sb, indent, "UsageFlags", UsageFlags.ToString());
        }
        public virtual void ReadXml(XmlNode node, string ddsfolder)
        {
            Name = Xml.GetChildInnerText(node, "Name");
            NameHash = JenkHash.GenHash(Name?.ToLowerInvariant());
            Unknown_32h = (ushort)Xml.GetChildUIntAttribute(node, "Unk32", "value");
            Usage = Xml.GetChildEnumInnerText<TextureUsage>(node, "Usage");
            UsageFlags = Xml.GetChildEnumInnerText<TextureUsageFlags>(node, "UsageFlags");
        }
        public override IResourceBlock[] GetReferences()
        {
            var list = new List<IResourceBlock>();
            if (!string.IsNullOrEmpty(Name))
            {
                NameBlock = (string_r)Name;
                list.Add(NameBlock);
            }
            return list.ToArray();
        }
        public override string ToString()
        {
            return $"TextureBase: {Name}";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))] public class Texture : TextureBase
    {
        public override long BlockLength => 144; // sizeof(rage::grcOrbisDurangoTextureBase)

        // Texture subclass structure data
        public ulong DataPointer { get; set; } // m_pGraphicsMem
        public ulong DataSize { get; set; } // m_GraphicsMemorySize
        public ulong LockPointer { get; set; } // m_pLockInfoPtr
        // Pointer to the Offset and Pitch

        public long MemoryUsage
        {
            get
            {
                long val = 0;
                if (Data != null)
                {
                    val += Data.FullData.LongLength;
                }
                return val;
            }
        }

        public override void Read(ResourceDataReader reader, params object[] parameters)
        {
            var initPos = reader.Position;

            base.Read(reader, parameters);
            
            // read structure data
            this.DataPointer = reader.ReadUInt64();
            this.DataSize = reader.ReadUInt64();
            this.LockPointer = reader.ReadUInt64();

            // User Memory, we don't need to know the contents.
            // ...because they're always zero in the file.
            var um1 = reader.ReadUInt64();
            var um2 = reader.ReadUInt64();
            var um3 = reader.ReadUInt64();
            var um4 = reader.ReadUInt64();
            var um5 = reader.ReadUInt64();
            var um6 = reader.ReadUInt64();

            // read reference data
            //this.Data = reader.ReadBlockAt<TextureData>(this.DataPointer, this.DataSize);
            this.Data = reader.ReadBlockAt<TextureData>(this.DataPointer, (int)this.DataSize, (int)this.Width, (int)this.Height, (int)this.Format, (int)this.TileMode);

            // change `dump` with the debugger to use this
            bool dump = false;
            if (dump) // <-- put a breakpoint here
            {
                File.WriteAllBytes($"X:\\dumps\\{Name}.bin", Data.FullData);
            }

            var endPos = reader.Position;

            if ((endPos - initPos) != 144) // sizeof(rage::grcOrbisDurangoTextureBase))
                throw new Exception("Failed to read the correct amount of bytes");
        }
        public override void Write(ResourceDataWriter writer, params object[] parameters)
        {
            var initPos = writer.Position;

            base.Write(writer, parameters);
            this.DataPointer = (ulong)this.Data.FilePosition;
            this.DataSize = (ulong)this.Data.FullData.Length;

            // write structure data
            writer.Write(this.DataPointer);
            writer.Write(this.DataSize);
            writer.Write(this.LockPointer); // TODO: We need to handle this!!

            // User Memory, just fill it with zeroes.
            const ulong UINT64_ZERO = 0;
            writer.Write(UINT64_ZERO); // 1
            writer.Write(UINT64_ZERO); // 2
            writer.Write(UINT64_ZERO); // 3
            writer.Write(UINT64_ZERO); // 4
            writer.Write(UINT64_ZERO); // 5
            writer.Write(UINT64_ZERO); // 6
            var endPos = writer.Position;

            if ((endPos - initPos) != 144) // sizeof(rage::grcOrbisDurangoTextureBase))
                throw new Exception("Failed to write the correct amount of bytes");
        }
        public override void WriteXml(StringBuilder sb, int indent, string ddsfolder)
        {
            base.WriteXml(sb, indent, ddsfolder);
            YtdXml.ValueTag(sb, indent, "Width", Width.ToString());
            YtdXml.ValueTag(sb, indent, "Height", Height.ToString());
            YtdXml.ValueTag(sb, indent, "Levels", Levels.ToString());
            YtdXml.StringTag(sb, indent, "Format", DDSIO.GetTextureFormat((DDSIO.DXGI_FORMAT)Format).ToString());
            YtdXml.StringTag(sb, indent, "FileName", YtdXml.XmlEscape((Name ?? "null") + ".dds"));

            try
            {
                if (!string.IsNullOrEmpty(ddsfolder))
                {
                    if (!Directory.Exists(ddsfolder))
                    {
                        Directory.CreateDirectory(ddsfolder);
                    }
                    var filepath = Path.Combine(ddsfolder, (Name ?? "null") + ".dds");
                    var dds = DDSIO.GetDDSFile(this);
                    File.WriteAllBytes(filepath, dds);
                }
            }
            catch { }
        }
        public override void ReadXml(XmlNode node, string ddsfolder)
        {
            base.ReadXml(node, ddsfolder);
            Width = (ushort)Xml.GetChildUIntAttribute(node, "Width");
            Height = (ushort)Xml.GetChildUIntAttribute(node, "Height");
            Levels = (byte)Xml.GetChildUIntAttribute(node, "Levels");
            Format = (TextureFormatOrbis)DDSIO.GetDXGIFormat(Xml.GetChildEnumInnerText<TextureFormat>(node, "Format"));
            var filename = Xml.GetChildInnerText(node, "FileName");


            if ((!string.IsNullOrEmpty(filename)) && (!string.IsNullOrEmpty(ddsfolder)))
            {
                var filepath = Path.Combine(ddsfolder, filename);
                if (File.Exists(filepath))
                {
                    try
                    {
                        var dds = File.ReadAllBytes(filepath);
                        var tex = DDSIO.GetTexture(dds);
                        if (tex != null)
                        {
                            // todo: DataSize?
                            Data = tex.Data;
                            Width = tex.Width;
                            Height = tex.Height;
                            Depth = tex.Depth;
                            Levels = tex.Levels;
                            Format = tex.Format;
                            TileMode = tex.TileMode;
                        }
                    }
                    catch
                    {
                        throw new Exception("Texture file format not supported:\n" + filepath);
                    }
                }
                else
                {
                    throw new Exception("Texture file not found:\n" + filepath);
                }
            }
        }

        public override IResourceBlock[] GetReferences()
        {
            var list = new List<IResourceBlock>(base.GetReferences());
            list.Add(Data);
            return list.ToArray();
        }

        public override string ToString()
        {
            return $"Texture: {Width}x{Height}: {Name}";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))] public class TextureData : ResourceGraphicsBlock
    {
        public override long BlockLength => FullData.Length;

        public byte[] FullData { get; set; }
        public bool Deswizzled { get; private set; } = false;

        /// <summary>
        /// Reads the data-block from a stream.
        /// </summary>
        public override void Read(ResourceDataReader reader, params object[] parameters)
        {
            if (Deswizzled)
                return;

            int length = Convert.ToInt32(parameters[0]);
            int width = Convert.ToInt32(parameters[1]);
            int height = Convert.ToInt32(parameters[2]);
            int format = Convert.ToInt32(parameters[3]);
            GrcTileMode tileMode = (GrcTileMode)Convert.ToInt32(parameters[4]);

            if (tileMode != GrcTileMode.Thin_1dThin
                || width < 32 || height < 32 /* This could be bad. */
                || !DDSIO.PS4Swizzle.IsSupportedFormat((DDSIO.DXGI_FORMAT)format))
            {
                FullData = reader.ReadBytes(length);
                return;
            }

            byte[] array = new byte[length];
            array = reader.ReadBytes(length);

            FullData = DDSIO.PS4Swizzle.Unswizzle(array, width, height, format);
            Deswizzled = true;
        }

        /// <summary>
        /// Writes the data-block to a stream.
        /// </summary>
        public override void Write(ResourceDataWriter writer, params object[] parameters)
        {
            writer.Write(FullData);
        }
    }

    // This does not match Orbis or Durango, it instead matches DXGI_FORMAT,
    // which in turns matches sga's texture format.
    // So, annoyingly, for compatibility reasons, we'll have to convert them.
    public enum TextureFormat : uint
    {
        D3DFMT_R8G8B8   = 20,
        D3DFMT_A8R8G8B8 = 21,
        D3DFMT_X8R8G8B8 = 22,
        D3DFMT_R5G6B5   = 23,
        D3DFMT_X1R5G5B5 = 24,
        D3DFMT_A1R5G5B5 = 25,
        D3DFMT_A8 = 28,
        D3DFMT_A8B8G8R8 = 32,
        D3DFMT_L8 = 50,

        // fourCC
        D3DFMT_DXT1 = 0x31545844,
        D3DFMT_DXT3 = 0x33545844,
        D3DFMT_DXT5 = 0x35545844,
        D3DFMT_ATI1 = 0x31495441,
        D3DFMT_ATI2 = 0x32495441,
        D3DFMT_BC7 = 0x20374342,

        //UNKNOWN
    }

    // GRC_TEMP_XG_FORMAT
    public enum TextureFormatOrbis : byte // This should be `uint`, but GcmFormat is a `byte`...
    {
        UNKNOWN                     = 0,
	    R32G32B32A32_TYPELESS       = 1,
	    R32G32B32A32_FLOAT          = 2,
	    R32G32B32A32_UINT           = 3,
	    R32G32B32A32_SINT           = 4,
	    R32G32B32_TYPELESS          = 5,
	    R32G32B32_FLOAT             = 6,
	    R32G32B32_UINT              = 7,
	    R32G32B32_SINT              = 8,
	    R16G16B16A16_TYPELESS       = 9,
	    R16G16B16A16_FLOAT          = 10,
	    R16G16B16A16_UNORM          = 11,
	    R16G16B16A16_UINT           = 12,
	    R16G16B16A16_SNORM          = 13,
	    R16G16B16A16_SINT           = 14,
	    R32G32_TYPELESS             = 15,
	    R32G32_FLOAT                = 16,
	    R32G32_UINT                 = 17,
	    R32G32_SINT                 = 18,
	    R32G8X24_TYPELESS           = 19,
	    D32_FLOAT_S8X24_UINT        = 20,
	    R32_FLOAT_X8X24_TYPELESS    = 21,
	    X32_TYPELESS_G8X24_UINT     = 22,
	    R10G10B10A2_TYPELESS        = 23,
	    R10G10B10A2_UNORM           = 24,
	    R10G10B10A2_UINT            = 25,
	    R11G11B10_FLOAT             = 26,
	    R8G8B8A8_TYPELESS           = 27,
	    R8G8B8A8_UNORM              = 28,
	    R8G8B8A8_UNORM_SRGB         = 29,
	    R8G8B8A8_UINT               = 30,
	    R8G8B8A8_SNORM              = 31,
	    R8G8B8A8_SINT               = 32,
	    R16G16_TYPELESS             = 33,
	    R16G16_FLOAT                = 34,
	    R16G16_UNORM                = 35,
	    R16G16_UINT                 = 36,
	    R16G16_SNORM                = 37,
	    R16G16_SINT                 = 38,
	    R32_TYPELESS                = 39,
	    D32_FLOAT                   = 40,
	    R32_FLOAT                   = 41,
	    R32_UINT                    = 42,
	    R32_SINT                    = 43,
	    R24G8_TYPELESS              = 44,
	    D24_UNORM_S8_UINT           = 45,
	    R24_UNORM_X8_TYPELESS       = 46,
	    X24_TYPELESS_G8_UINT        = 47,
	    R8G8_TYPELESS               = 48,
	    R8G8_UNORM                  = 49,
	    R8G8_UINT                   = 50,
	    R8G8_SNORM                  = 51,
	    R8G8_SINT                   = 52,
	    R16_TYPELESS                = 53,
	    R16_FLOAT                   = 54,
	    D16_UNORM                   = 55,
	    R16_UNORM                   = 56,
	    R16_UINT                    = 57,
	    R16_SNORM                   = 58,
	    R16_SINT                    = 59,
	    R8_TYPELESS                 = 60,
	    R8_UNORM                    = 61,
	    R8_UINT                     = 62,
	    R8_SNORM                    = 63,
	    R8_SINT                     = 64,
	    A8_UNORM                    = 65,
	    R1_UNORM                    = 66,
	    R9G9B9E5_SHAREDEXP          = 67,
	    R8G8_B8G8_UNORM             = 68,
	    G8R8_G8B8_UNORM             = 69,
	    BC1_TYPELESS                = 70,
	    BC1_UNORM                   = 71,
	    BC1_UNORM_SRGB              = 72,
	    BC2_TYPELESS                = 73,
	    BC2_UNORM                   = 74,
	    BC2_UNORM_SRGB              = 75,
	    BC3_TYPELESS                = 76,
	    BC3_UNORM                   = 77,
	    BC3_UNORM_SRGB              = 78,
	    BC4_TYPELESS                = 79,
	    BC4_UNORM                   = 80,
	    BC4_SNORM                   = 81,
	    BC5_TYPELESS                = 82,
	    BC5_UNORM                   = 83,
	    BC5_SNORM                   = 84,
	    B5G6R5_UNORM                = 85,
	    B5G5R5A1_UNORM              = 86,
	    B8G8R8A8_UNORM              = 87,
	    B8G8R8X8_UNORM              = 88,
	    R10G10B10_XR_BIAS_A2_UNORM  = 89,
	    B8G8R8A8_TYPELESS           = 90,
	    B8G8R8A8_UNORM_SRGB         = 91,
	    B8G8R8X8_TYPELESS           = 92,
	    B8G8R8X8_UNORM_SRGB         = 93,
	    BC6H_TYPELESS               = 94,
	    BC6H_UF16                   = 95,
	    BC6H_SF16                   = 96,
	    BC7_TYPELESS                = 97,
	    BC7_UNORM                   = 98,
	    BC7_UNORM_SRGB              = 99,
	    AYUV                        = 100,
	    Y410                        = 101,
	    Y416                        = 102,
	    NV12                        = 103,
	    P010                        = 104,
	    P016                        = 105,
	    _420_OPAQUE                 = 106, // NOTE: Had to add an underscore to the start.
	    YUY2                        = 107,
	    Y210                        = 108,
	    Y216                        = 109,
	    NV11                        = 110,
	    AI44                        = 111,
	    IA44                        = 112,
	    P8                          = 113,
	    A8P8                        = 114,
	    B4G4R4A4_UNORM              = 115,
	    R10G10B10_7E3_A2_FLOAT      = 116,
	    R10G10B10_6E4_A2_FLOAT      = 117,
    }

    // sce::Gnm::TileMode and XG_TILE_MODE are the same (see C:\Program Files (x86)\SCE\ORBIS SDKs\1.000\target\include_common\gnm\constants.h & C:\Program Files (x86)\Microsoft Durango XDK\xdk\include\um\xg.h).
    // Something that works in our favour: RageBuilder uses Xbox One code to generate textures
    // thus, we only need to support Xbox One-compatible tiling modes.
    public enum GrcTileMode : byte
    {
    	// Depth modes (for depth buffers)
    	Depth_2dThin_64                   = 0x00000000, // XG_TILE_MODE_COMP_DEPTH_0
    	Depth_2dThin_128                  = 0x00000001, // XG_TILE_MODE_COMP_DEPTH_1
    	Depth_2dThin_256                  = 0x00000002, // XG_TILE_MODE_COMP_DEPTH_2
    	Depth_2dThin_512                  = 0x00000003, // XG_TILE_MODE_COMP_DEPTH_3
    	Depth_2dThin_1K                   = 0x00000004, // XG_TILE_MODE_COMP_DEPTH_4
    	Depth_1dThin                      = 0x00000005, // XG_TILE_MODE_UNC_DEPTH_5
    	Depth_2dThinPrt_256               = 0x00000006, // XG_TILE_MODE_UNC_DEPTH_6
    	Depth_2dThinPrt_1K                = 0x00000007, // XG_TILE_MODE_UNC_DEPTH_7

    	// Display modes
    	Display_LinearAligned			  = 0x00000008, // XG_TILE_MODE_LINEAR
    	Display_1dThin                    = 0x00000009, // XG_TILE_MODE_DISPLAY
    	Display_2dThin_OrbisOnly          = 0x0000000A, // XG_TILE_MODE_RESERVED_10
    	Display_ThinPrt_OrbisOnly         = 0x0000000B, // XG_TILE_MODE_RESERVED_11
    	Display_2dThinPrt_OrbisOnly	      = 0x0000000C, // XG_TILE_MODE_RESERVED_12

    	// Thin modes (for non-displayable 1D/2D/3D surfaces)
    	Thin_1dThin                       = 0x0000000D, // XG_TILE_MODE_1D_THIN
    	Thin_2dThin                       = 0x0000000E, // XG_TILE_MODE_2D_THIN
    	Thin_3dThin_OrbisOnly             = 0x0000000F, // XG_TILE_MODE_RESERVED_15
    	Thin_ThinPrt_OrbisOnly            = 0x00000010, // XG_TILE_MODE_RESERVED_16
    	Thin_2dThinPrt_OrbisOnly          = 0x00000011, // XG_TILE_MODE_RESERVED_17
    	Thin_3dThinPrt_OrbisOnly          = 0x00000012, // XG_TILE_MODE_RESERVED_18

    	// Thick modes (for 3D textures)
    	Thick_1dThick_OrbisOnly			  = 0x00000013, // XG_TILE_MODE_RESERVED_19
    	Thick_2dThick_OrbisOnly			  = 0x00000014, // XG_TILE_MODE_RESERVED_20
    	Thick_3dThick_OrbisOnly			  = 0x00000015, // XG_TILE_MODE_RESERVED_21
    	Thick_ThickPrt_OrbisOnly		  = 0x00000016, // XG_TILE_MODE_RESERVED_22
    	Thick_2dThickPrt_OrbisOnly        = 0x00000017, // XG_TILE_MODE_RESERVED_23
    	Thick_3dThickPrt_OrbisOnly        = 0x00000018, // XG_TILE_MODE_RESERVED_24
    	Thick_2dXThick_OrbisOnly          = 0x00000019, // XG_TILE_MODE_RESERVED_25
    	Thick_3dXThick_OrbisOnly		  = 0x0000001A, // XG_TILE_MODE_RESERVED_26

    	// Rotated modes -- not used
    	Rotated_1dThin_OrbisOnly		  = 0x0000001B, // XG_TILE_MODE_RESERVED_27
    	Rotated_2dThin_OrbisOnly		  = 0x0000001C, // XG_TILE_MODE_RESERVED_28
    	Rotated_ThinPrt_OrbisOnly		  = 0x0000001D, // XG_TILE_MODE_RESERVED_29
    	Rotated_2dThinPrt_OrbisOnly		  = 0x0000001E, // XG_TILE_MODE_RESERVED_30

    	// Hugely inefficient linear display mode -- do not use!
    	Display_LinearGeneral             = 0x0000001F, // XG_TILE_MODE_LINEAR_GENERAL
    	Max								  = 0x00000020
    };

    public enum TextureUsage : byte
    {
        UNKNOWN = 0,
        DEFAULT = 1,
        TERRAIN = 2,
        CLOUDDENSITY = 3,
        CLOUDNORMAL = 4,
        CABLE = 5,
        FENCE = 6,
        ENVEFF = 7, //unused by V
        SCRIPT = 8,
        WATERFLOW = 9,
        WATERFOAM = 10,
        WATERFOG = 11,
        WATEROCEAN = 12,
        WATER = 13, //unused by V
        FOAMOPACITY = 14,
        FOAM = 15,  //unused by V
        DIFFUSEMIPSHARPEN = 16,
        DIFFUSEDETAIL = 17, //unused by V
        DIFFUSEDARK = 18,
        DIFFUSEALPHAOPAQUE = 19,
        DIFFUSE = 20,
        DETAIL = 21,
        NORMAL = 22,
        SPECULAR = 23,
        EMISSIVE = 24,
        TINTPALETTE = 25,
        SKIPPROCESSING = 26,
        DONOTOPTIMIZE = 27, //unused by V
        TEST = 28,  //unused by V
        COUNT = 29, //unused by V
    }
    
    [Flags]
    public enum TextureUsageFlags : uint
    {
        NOT_HALF = 1,
        HD_SPLIT = (1 << 1),
        X2 = (1 << 2),
        X4 = (1 << 3),
        Y4 = (1 << 4),
        X8 = (1 << 5),
        X16 = (1 << 6),
        X32 = (1 << 7),
        X64 = (1 << 8),
        Y64 = (1 << 9),
        X128 = (1 << 10),
        X256 = (1 << 11),
        X512 = (1 << 12),
        Y512 = (1 << 13),
        X1024 = (1 << 14),//wtf is all this?
        Y1024 = (1 << 15),
        X2048 = (1 << 16),
        Y2048 = (1 << 17),
        EMBEDDEDSCRIPTRT = (1 << 18),
        UNK19 = (1 << 19),  //unused by V
        UNK20 = (1 << 20),  //unused by V
        UNK21 = (1 << 21),  //unused by V
        FLAG_FULL = (1 << 22),
        MAPS_HALF = (1 << 23),
        UNK24 = (1 << 24),//used by almost everything...
    }

    // grcBindFlag
    [Flags]
    public enum GrcBindFlags
    {
        None             = 0x0,
        VertexBuffer     = 0x1,
        IndexBuffer		 = 0x2,
		ConstantBuffer	 = 0x4,
		ShaderResource	 = 0x8,
		StreamOutput	 = 0x10,
		RenderTarget	 = 0x20,
		DepthStencil	 = 0x40,
		UnorderedAccess	 = 0x80,
    }

    // Gen9 leftovers - please ignore.
    public enum ShaderResourceViewDimensionG9 : ushort //probably actually a uint
    {
        Texture2D = 0x41,//0x401
        Texture2DArray = 0x61,//0x601
        TextureCube = 0x82,//0x802
        Texture3D = 0xa3,//0xa03
        Buffer = 0x14,//0x104
    }
    public class ShaderResourceViewG9 : ResourceSystemBlock
    {
        public override long BlockLength => 32;//64
        public ulong VFT { get; set; } = 0x00000001406b77d8;
        public ulong Unknown_08h { get; set; }
        public ShaderResourceViewDimensionG9 Dimension { get; set; }
        public ushort Unknown_12h { get; set; } = 0xFFFF;
        public uint Unknown_14h { get; set; } = 0xFFFFFFFF;
        public ulong Unknown_18h { get; set; }

        public override void Read(ResourceDataReader reader, params object[] parameters)
        {
            VFT = reader.ReadUInt64();//runtime ptr?
            Unknown_08h = reader.ReadUInt64();
            Dimension = (ShaderResourceViewDimensionG9)reader.ReadUInt16();//0x41
            Unknown_12h = reader.ReadUInt16();
            Unknown_14h = reader.ReadUInt32();
            Unknown_18h = reader.ReadUInt64();

            switch (VFT)
            {
                case 0x00000001406b77d8:
                case 0x0000000140695e58:
                case 0x000000014070f830:
                case 0x00000001406b9308:
                case 0x0000000140729b58:
                case 0x0000000140703378:
                case 0x0000000140704670:
                case 0x00000001407096f0:
                case 0x00000001406900a8:
                case 0x00000001406b7358:
                    break;
                default:
                    break;
            }

            switch (Dimension)
            {
                case ShaderResourceViewDimensionG9.Texture2D:
                case ShaderResourceViewDimensionG9.Texture2DArray:
                case ShaderResourceViewDimensionG9.Texture3D:
                    break;
                case ShaderResourceViewDimensionG9.Buffer:
                    break;
                default:
                    break;
            }
            if (Unknown_08h != 0)
            { }
            if (Unknown_18h != 0)
            { }
        }
        public override void Write(ResourceDataWriter writer, params object[] parameters)
        {
            writer.Write(VFT);
            writer.Write(Unknown_08h);
            writer.Write((ushort)Dimension);
            writer.Write(Unknown_12h);
            writer.Write(Unknown_14h);
            writer.Write(Unknown_18h);
        }
    }

}
