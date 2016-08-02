using System;
using System.Linq;
using System.Text;
using System.Windows.Forms.VisualStyles;
using BotCore.Types;

namespace BotCore
{
    public class Packet
    {
        public byte[] Data;
        public int Idx = 1;
        public bool Processed;
        public int Type;
        public DateTime Date;

        public Client Client { get; set; }

        public Packet()
        {
            Date = DateTime.Now;
            Data = new byte[0];
        }

        public Packet(byte OpCode) : this()
        {
            WriteByte(OpCode);
        }

        public Packet(string hex) : this()
        {
            try
            {
                hex = hex.Replace(" ", string.Empty).Trim();
                Data = Enumerable.Range(0, hex.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                    .ToArray();
            }
            catch
            {
                return;
            }
        }

        public Packet(byte[] data) : this()
        {
            Write(data);
        }

        public Packet(byte OpCode, byte[] bodyData) : this()
        {
            WriteByte(OpCode);
            Write(bodyData);
        }

        public byte this[int index]
        {
            get { return Data[index]; }
            set
            {
                if (Client._memory != null && Client._memory == null && !Client._memory.IsRunning) return;
                if (Type == 2 && Client.SendPointer != 0)
                    if (Client._memory != null)
                        Client._memory.Write((IntPtr)Client.SendPointer + index, value, false);
                if (Type == 1 && Client.RecvPointer != 0)
                    if (Client._memory != null)
                        Client._memory.Write((IntPtr)Client.RecvPointer + index, value, false);
            }
        }

        public byte[] Read(int length)
        {
            var bytes = Data;
            if (bytes != null && Idx + (length - 1) < bytes.Length)
            {
                var buffer = new byte[length];
                Buffer.BlockCopy(Data, Idx, buffer, 0, length);
                Idx += length;
                return buffer;
            }
            throw new IndexOutOfRangeException();
        }

        public sbyte ReadSByte()
        {
            if (Idx < Data.Length)
            {
                return (sbyte)Data[Idx++];
            }
            throw new IndexOutOfRangeException();
        }

        internal byte[] ToArray()
        {
            return Data;
        }

        public byte ReadByte()
        {
            if (Idx < Data.Length)
            {
                return Data[Idx++];
            }
            return 0;
        }

        public bool ReadBoolean()
        {
            if (Idx < Data.Length)
            {
                return Data[Idx++] != 0;
            }
            throw new IndexOutOfRangeException();
        }

        public short ReadInt16()
        {
            if (Idx + 1 < Data.Length)
            {
                return (short)(Data[Idx++] << 8 | Data[Idx++]);
            }
            return 0;
        }

        public ushort ReadUInt16()
        {
            if (Idx + 1 < Data.Length)
            {
                return (ushort)(Data[Idx++] << 8 | Data[Idx++]);
            }
            return 0;
        }

        public int ReadInt32()
        {
            if (Idx + 3 < Data.Length)
            {
                return Data[Idx++] << 24 | Data[Idx++] << 16 | Data[Idx++] << 8 |
                       Data[Idx++];
            }
            return 0;
        }

        public uint ReadUInt32()
        {
            if (Idx + 3 < Data.Length)
            {
                return
                    (uint)
                        (Data[Idx++] << 24 | Data[Idx++] << 16 | Data[Idx++] << 8 |
                         Data[Idx++]);
            }
            throw new IndexOutOfRangeException();
        }

        public long ReadInt64()
        {
            if (Idx + 7 < Data.Length)
            {
                return Data[Idx++] << 56 | Data[Idx++] << 48 | Data[Idx++] << 40 |
                       Data[Idx++] << 32 | Data[Idx++] << 24 | Data[Idx++] << 16 |
                       Data[Idx++] << 8 | Data[Idx++];
            }
            throw new IndexOutOfRangeException();
        }

        public ulong ReadUInt64()
        {
            if (Idx + 7 < Data.Length)
            {
                return
                    (ulong)
                        (Data[Idx++] << 56 | Data[Idx++] << 48 | Data[Idx++] << 40 |
                         Data[Idx++] << 32 | Data[Idx++] << 24 | Data[Idx++] << 16 |
                         Data[Idx++] << 8 | Data[Idx++]);
            }
            throw new IndexOutOfRangeException();
        }

        public string ReadString()
        {
            return ReadString(Data.Length - Idx).Trim('\0');
        }

        public string ReadString(int length)
        {
            if (Idx + (length - 1) < Data.Length)
            {
                var buffer = new byte[length];
                Buffer.BlockCopy(Data, Idx, buffer, 0, length);
                Idx += length;
                return Encoding.GetEncoding(949).GetString(buffer);
            }
            return "guild member";
        }

        public string ReadString8()
        {
            var length = ReadByte();
            return ReadString(length);
        }

        public string ReadString16()
        {
            var length = ReadUInt16();
            return ReadString(length);
        }

        public void Write(byte[] value)
        {
            foreach (var t in value)
            {
                WriteByte(t);
            }
        }

        public void WriteSByte(sbyte value)
        {
            var length = Data.Length;
            Array.Resize(ref Data, length + 1);
            Data[length] = (byte)value;
        }

        public void WriteByte(byte value)
        {
            var length = Data.Length;
            Array.Resize(ref Data, length + 1);
            Data[length] = value;
        }

        public void WriteByte(bool value)
        {
            var length = Data.Length;
            Array.Resize(ref Data, length + 1);
            Data[length] = (byte)(value ? 1 : 0);
        }

        public void WriteInt16(short value)
        {
            var data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }
            Write(data);
        }

        public void WriteAnimation(Animation value)
        {
            WriteUInt16((ushort)value);
        }
        public void WriteUInt16(ushort value)
        {
            var data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }
            Write(data);
        }

        public void WriteInt32(int value)
        {
            var data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }
            Write(data);
        }

        public void WriteUInt32(uint value)
        {
            var data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }
            Write(data);
        }

        public void WriteInt64(long value)
        {
            var data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }
            Write(data);
        }

        public void WriteUInt64(ulong value)
        {
            var data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }
            Write(data);
        }

        public void WriteString(string value)
        {
            var buffer = Encoding.GetEncoding(949).GetBytes(value);
            Write(buffer);
        }

        public void WriteString8(string value)
        {
            var buffer = Encoding.GetEncoding(949).GetBytes(value);
            WriteByte((byte)buffer.Length);
            Write(buffer);
        }

        public void WriteString8(string format, params object[] args)
        {
            WriteString8(string.Format(format, args));
        }

        public void WriteString16(string value)
        {
            var buffer = Encoding.GetEncoding(949).GetBytes(value);
            WriteUInt16((ushort)buffer.Length);
            Write(buffer);
        }

        public void WriteString16(string format, params object[] args)
        {
            WriteString16(string.Format(format, args));
        }

        public override string ToString()
        {
            return string.Format("{0:X2}", BitConverter.ToString(Data).Replace('-', ' '));
        }
    }
}
