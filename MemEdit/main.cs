using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MemEdit
{
    // Token: 0x02000005 RID: 5
    
    public class memEdit
    {
        // Token: 0x06000005 RID: 5
        [DllImport("Kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);

        // Token: 0x06000006 RID: 6
        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int size, IntPtr lpNumberOfBytesWritten);

        // Token: 0x06000007 RID: 7
        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        // Token: 0x06000008 RID: 8
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        // Token: 0x06000009 RID: 9 RVA: 0x0000208A File Offset: 0x0000028A
        public memEdit(string procName)
        {
            this.proc = this.SetProcess(procName);
        }

        // Token: 0x0600000A RID: 10 RVA: 0x000020A4 File Offset: 0x000002A4
        public Process GetProcess()
        {
            return this.proc;
        }

        // Token: 0x0600000B RID: 11 RVA: 0x000020BC File Offset: 0x000002BC
        public Process SetProcess(string procName)
        {
            this.proc = Process.GetProcessesByName(procName)[0];
            bool flag = this.proc == null;
            if (flag)
            {
                throw new InvalidOperationException("Process was not found, are you using the correct bit version and have no miss-spellings?");
            }
            return this.proc;
        }

        public int GetProcessID(string name)
        {
            Process[] processes = Process.GetProcesses();
            Process[] array = processes;
            int result;
            for (int i = 0; i < array.Length; i++)
            {
                Process process = array[i];
                bool flag = process.ProcessName == name;
                if (flag)
                {
                    result = process.Id;
                    return result;
                }
            }
            result = 0;
            return result;
        }

        // Token: 0x0600000C RID: 12 RVA: 0x000020FC File Offset: 0x000002FC
        public IntPtr GetModuleBase(string moduleName)
        {
            bool flag = string.IsNullOrEmpty(moduleName);
            if (flag)
            {
                throw new InvalidOperationException("moduleName was either null or empty.");
            }
            bool flag2 = this.proc == null;
            if (flag2)
            {
                throw new InvalidOperationException("process is invalid, check your init.");
            }
            try
            {
                bool flag3 = moduleName.Contains(".exe");
                if (flag3)
                {
                    bool flag4 = this.proc.MainModule != null;
                    if (flag4)
                    {
                        return this.proc.MainModule.BaseAddress;
                    }
                }
                foreach (object obj in this.proc.Modules)
                {
                    ProcessModule processModule = (ProcessModule)obj;
                    bool flag5 = processModule.ModuleName == moduleName;
                    if (flag5)
                    {
                        return processModule.BaseAddress;
                    }
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Failed to find the specified module, search string might have miss-spellings.");
            }
            return IntPtr.Zero;
        }

        // Token: 0x0600000D RID: 13 RVA: 0x00002210 File Offset: 0x00000410
        public IntPtr ReadPointer(IntPtr addy)
        {
            byte[] array = new byte[8];
            memEdit.ReadProcessMemory(this.proc.Handle, addy, array, array.Length, IntPtr.Zero);
            return (IntPtr)BitConverter.ToInt64(array);
        }

        // Token: 0x0600000E RID: 14 RVA: 0x00002254 File Offset: 0x00000454
        public IntPtr ReadPointer(IntPtr addy, int offset)
        {
            byte[] array = new byte[8];
            memEdit.ReadProcessMemory(this.proc.Handle, addy + offset, array, array.Length, IntPtr.Zero);
            return (IntPtr)BitConverter.ToInt64(array);
        }

        // Token: 0x0600000F RID: 15 RVA: 0x000022A0 File Offset: 0x000004A0
        public IntPtr ReadPointer(long addy, long offset)
        {
            byte[] array = new byte[8];
            memEdit.ReadProcessMemory(this.proc.Handle, (IntPtr)(addy + offset), array, array.Length, IntPtr.Zero);
            return (IntPtr)BitConverter.ToInt64(array);
        }

        // Token: 0x06000010 RID: 16 RVA: 0x000022EC File Offset: 0x000004EC
        public IntPtr ReadPointer(IntPtr addy, long offset)
        {
            return this.ReadPointer(addy, offset);
        }

        // Token: 0x06000011 RID: 17 RVA: 0x00002308 File Offset: 0x00000508
        public IntPtr ReadPointer(IntPtr addy, int[] offsets)
        {
            IntPtr intPtr = addy;
            foreach (int offset in offsets)
            {
                intPtr = this.ReadPointer(intPtr, offset);
            }
            return intPtr;
        }

        // Token: 0x06000012 RID: 18 RVA: 0x00002340 File Offset: 0x00000540
        public IntPtr ReadPointer(IntPtr addy, long[] offsets)
        {
            IntPtr intPtr = addy;
            foreach (long offset in offsets)
            {
                intPtr = this.ReadPointer(intPtr, offset);
            }
            return intPtr;
        }

        // Token: 0x06000013 RID: 19 RVA: 0x00002378 File Offset: 0x00000578
        public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2)
        {
            return this.ReadPointer(addy, new int[]
            {
                offset1,
                offset2
            });
        }

        // Token: 0x06000014 RID: 20 RVA: 0x000023A0 File Offset: 0x000005A0
        public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2, int offset3)
        {
            return this.ReadPointer(addy, new int[]
            {
                offset1,
                offset2,
                offset3
            });
        }

        // Token: 0x06000015 RID: 21 RVA: 0x000023CC File Offset: 0x000005CC
        public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2, int offset3, int offset4)
        {
            return this.ReadPointer(addy, new int[]
            {
                offset1,
                offset2,
                offset3,
                offset4
            });
        }

        // Token: 0x06000016 RID: 22 RVA: 0x00002400 File Offset: 0x00000600
        public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2, int offset3, int offset4, int offset5)
        {
            return this.ReadPointer(addy, new int[]
            {
                offset1,
                offset2,
                offset3,
                offset4,
                offset5
            });
        }

        // Token: 0x06000017 RID: 23 RVA: 0x00002438 File Offset: 0x00000638
        public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2, int offset3, int offset4, int offset5, int offset6)
        {
            return this.ReadPointer(addy, new int[]
            {
                offset1,
                offset2,
                offset3,
                offset4,
                offset5,
                offset6
            });
        }

        // Token: 0x06000018 RID: 24 RVA: 0x00002474 File Offset: 0x00000674
        public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2, int offset3, int offset4, int offset5, int offset6, int offset7)
        {
            return this.ReadPointer(addy, new int[]
            {
                offset1,
                offset2,
                offset3,
                offset4,
                offset5,
                offset6,
                offset7
            });
        }

        // Token: 0x06000019 RID: 25 RVA: 0x000024B4 File Offset: 0x000006B4
        public IntPtr ReadPointer(IntPtr addy, long offset1, long offset2)
        {
            return this.ReadPointer(addy, new long[]
            {
                offset1,
                offset2
            });
        }

        // Token: 0x0600001A RID: 26 RVA: 0x000024DC File Offset: 0x000006DC
        public IntPtr ReadPointer(IntPtr addy, long offset1, long offset2, long offset3)
        {
            return this.ReadPointer(addy, new long[]
            {
                offset1,
                offset2,
                offset3
            });
        }

        // Token: 0x0600001B RID: 27 RVA: 0x00002508 File Offset: 0x00000708
        public IntPtr ReadPointer(IntPtr addy, long offset1, long offset2, long offset3, long offset4)
        {
            return this.ReadPointer(addy, new long[]
            {
                offset1,
                offset2,
                offset3,
                offset4
            });
        }

        // Token: 0x0600001C RID: 28 RVA: 0x0000253C File Offset: 0x0000073C
        public IntPtr ReadPointer(IntPtr addy, long offset1, long offset2, long offset3, long offset4, long offset5)
        {
            return this.ReadPointer(addy, new long[]
            {
                offset1,
                offset2,
                offset3,
                offset4,
                offset5
            });
        }

        // Token: 0x0600001D RID: 29 RVA: 0x00002574 File Offset: 0x00000774
        public IntPtr ReadPointer(IntPtr addy, long offset1, long offset2, long offset3, long offset4, long offset5, long offset6)
        {
            return this.ReadPointer(addy, new long[]
            {
                offset1,
                offset2,
                offset3,
                offset4,
                offset5,
                offset6
            });
        }

        // Token: 0x0600001E RID: 30 RVA: 0x000025B0 File Offset: 0x000007B0
        public IntPtr ReadPointer(IntPtr addy, long offset1, long offset2, long offset3, long offset4, long offset5, long offset6, long offset7)
        {
            return this.ReadPointer(addy, new long[]
            {
                offset1,
                offset2,
                offset3,
                offset4,
                offset5,
                offset6,
                offset7
            });
        }

        // Token: 0x0600001F RID: 31 RVA: 0x000025F0 File Offset: 0x000007F0
        public IntPtr ReadPointer(IntPtr addy, IntPtr offset1, int offset2)
        {
            IntPtr addy2 = this.ReadPointer((IntPtr)((long)addy + (long)offset1));
            return this.ReadPointer(addy2, offset2);
        }

        // Token: 0x06000020 RID: 32 RVA: 0x00002624 File Offset: 0x00000824
        public byte[] ReadBytes(IntPtr addy, int bytes)
        {
            byte[] array = new byte[bytes];
            memEdit.ReadProcessMemory(this.proc.Handle, addy, array, array.Length, IntPtr.Zero);
            return array;
        }

        // Token: 0x06000021 RID: 33 RVA: 0x0000265C File Offset: 0x0000085C
        public byte[] ReadBytes(IntPtr addy, int offset, int bytes)
        {
            byte[] array = new byte[bytes];
            memEdit.ReadProcessMemory(this.proc.Handle, addy + offset, array, array.Length, IntPtr.Zero);
            return array;
        }

        // Token: 0x06000022 RID: 34 RVA: 0x00002698 File Offset: 0x00000898
        public int ReadInt(IntPtr address)
        {
            return BitConverter.ToInt32(this.ReadBytes(address, 4));
        }

        // Token: 0x06000023 RID: 35 RVA: 0x000026BC File Offset: 0x000008BC
        public int ReadInt(IntPtr address, int offset)
        {
            return BitConverter.ToInt32(this.ReadBytes(address + offset, 4));
        }

        // Token: 0x06000024 RID: 36 RVA: 0x000026E8 File Offset: 0x000008E8
        public IntPtr ReadLong(IntPtr address)
        {
            return (IntPtr)BitConverter.ToInt64(this.ReadBytes(address, 8));
        }

        // Token: 0x06000025 RID: 37 RVA: 0x00002714 File Offset: 0x00000914
        public IntPtr ReadLong(IntPtr address, int offset)
        {
            return (IntPtr)BitConverter.ToInt64(this.ReadBytes(address + offset, 8));
        }

        // Token: 0x06000026 RID: 38 RVA: 0x00002744 File Offset: 0x00000944
        public float ReadFloat(IntPtr address)
        {
            return BitConverter.ToSingle(this.ReadBytes(address, 4));
        }

        // Token: 0x06000027 RID: 39 RVA: 0x00002768 File Offset: 0x00000968
        public float ReadFloat(IntPtr address, int offset)
        {
            return BitConverter.ToSingle(this.ReadBytes(address + offset, 4));
        }

        // Token: 0x06000028 RID: 40 RVA: 0x00002794 File Offset: 0x00000994
        public double ReadDouble(IntPtr address)
        {
            return BitConverter.ToDouble(this.ReadBytes(address, 8));
        }

        // Token: 0x06000029 RID: 41 RVA: 0x000027B8 File Offset: 0x000009B8
        public double ReadDouble(IntPtr address, int offset)
        {
            return BitConverter.ToDouble(this.ReadBytes(address + offset, 4));
        }

        // Token: 0x0600002A RID: 42 RVA: 0x000027E4 File Offset: 0x000009E4
        public Vector3 ReadVec(IntPtr address)
        {
            byte[] value = this.ReadBytes(address, 12);
            return new Vector3
            {
                X = BitConverter.ToSingle(value, 0),
                Y = BitConverter.ToSingle(value, 4),
                Z = BitConverter.ToSingle(value, 8)
            };
        }

        // Token: 0x0600002B RID: 43 RVA: 0x00002834 File Offset: 0x00000A34
        public Vector3 ReadVec(IntPtr address, int offset)
        {
            byte[] value = this.ReadBytes(address + offset, 12);
            return new Vector3
            {
                X = BitConverter.ToSingle(value, 0),
                Y = BitConverter.ToSingle(value, 4),
                Z = BitConverter.ToSingle(value, 8)
            };
        }

        // Token: 0x0600002C RID: 44 RVA: 0x0000288C File Offset: 0x00000A8C
        public double[] ReadDoubleVec(IntPtr address)
        {
            byte[] value = this.ReadBytes(address, 24);
            return new double[]
            {
                BitConverter.ToDouble(value, 0),
                BitConverter.ToDouble(value, 8),
                BitConverter.ToDouble(value, 16)
            };
        }

        // Token: 0x0600002D RID: 45 RVA: 0x000028D0 File Offset: 0x00000AD0
        public double[] ReadDoubleVec(IntPtr address, int offset)
        {
            byte[] value = this.ReadBytes(address + offset, 24);
            return new double[]
            {
                BitConverter.ToDouble(value, 0),
                BitConverter.ToDouble(value, 8),
                BitConverter.ToDouble(value, 16)
            };
        }

        // Token: 0x0600002E RID: 46 RVA: 0x00002918 File Offset: 0x00000B18
        public short ReadShort(IntPtr address)
        {
            return BitConverter.ToInt16(this.ReadBytes(address, 2));
        }

        // Token: 0x0600002F RID: 47 RVA: 0x0000293C File Offset: 0x00000B3C
        public short ReadShort(IntPtr address, int offset)
        {
            return BitConverter.ToInt16(this.ReadBytes(address + offset, 2));
        }

        // Token: 0x06000030 RID: 48 RVA: 0x00002968 File Offset: 0x00000B68
        public ushort ReadUShort(IntPtr address)
        {
            return BitConverter.ToUInt16(this.ReadBytes(address, 2));
        }

        // Token: 0x06000031 RID: 49 RVA: 0x0000298C File Offset: 0x00000B8C
        public ushort ReadUShort(IntPtr address, int offset)
        {
            return BitConverter.ToUInt16(this.ReadBytes(address + offset, 2));
        }

        // Token: 0x06000032 RID: 50 RVA: 0x000029B8 File Offset: 0x00000BB8
        public uint ReadUInt(IntPtr address)
        {
            return BitConverter.ToUInt32(this.ReadBytes(address, 4));
        }

        // Token: 0x06000033 RID: 51 RVA: 0x000029DC File Offset: 0x00000BDC
        public uint ReadUInt(IntPtr address, int offset)
        {
            return BitConverter.ToUInt32(this.ReadBytes(address + offset, 4));
        }

        // Token: 0x06000034 RID: 52 RVA: 0x00002A08 File Offset: 0x00000C08
        public ulong ReadULong(IntPtr address)
        {
            return BitConverter.ToUInt64(this.ReadBytes(address, 8));
        }

        // Token: 0x06000035 RID: 53 RVA: 0x00002A2C File Offset: 0x00000C2C
        public ulong ReadULong(IntPtr address, int offset)
        {
            return BitConverter.ToUInt64(this.ReadBytes(address + offset, 8));
        }

        // Token: 0x06000036 RID: 54 RVA: 0x00002A58 File Offset: 0x00000C58
        public bool ReadBool(IntPtr address)
        {
            return BitConverter.ToBoolean(this.ReadBytes(address, 1));
        }

        // Token: 0x06000037 RID: 55 RVA: 0x00002A7C File Offset: 0x00000C7C
        public bool ReadBool(IntPtr address, int offset)
        {
            return BitConverter.ToBoolean(this.ReadBytes(address + offset, 1));
        }

        // Token: 0x06000038 RID: 56 RVA: 0x00002AA8 File Offset: 0x00000CA8
        public string ReadString(IntPtr address, int length)
        {
            return Encoding.UTF8.GetString(this.ReadBytes(address, length));
        }

        // Token: 0x06000039 RID: 57 RVA: 0x00002ACC File Offset: 0x00000CCC
        public string ReadString(IntPtr address, int offset, int length)
        {
            return Encoding.UTF8.GetString(this.ReadBytes(address + offset, length));
        }

        // Token: 0x0600003A RID: 58 RVA: 0x00002AF8 File Offset: 0x00000CF8
        public char ReadChar(IntPtr address)
        {
            return BitConverter.ToChar(this.ReadBytes(address, 2));
        }

        // Token: 0x0600003B RID: 59 RVA: 0x00002B1C File Offset: 0x00000D1C
        public char ReadChar(IntPtr address, int offset)
        {
            return BitConverter.ToChar(this.ReadBytes(address + offset, 2));
        }

        // Token: 0x0600003C RID: 60 RVA: 0x00002B48 File Offset: 0x00000D48
        public float[] ReadMatrix(IntPtr address)
        {
            byte[] array = this.ReadBytes(address, 64);
            float[] array2 = new float[array.Length];
            array2[0] = BitConverter.ToSingle(array, 0);
            array2[1] = BitConverter.ToSingle(array, 4);
            array2[2] = BitConverter.ToSingle(array, 8);
            array2[3] = BitConverter.ToSingle(array, 12);
            array2[4] = BitConverter.ToSingle(array, 16);
            array2[5] = BitConverter.ToSingle(array, 20);
            array2[6] = BitConverter.ToSingle(array, 24);
            array2[7] = BitConverter.ToSingle(array, 28);
            array2[8] = BitConverter.ToSingle(array, 32);
            array2[9] = BitConverter.ToSingle(array, 36);
            array2[10] = BitConverter.ToSingle(array, 40);
            array2[11] = BitConverter.ToSingle(array, 44);
            array2[12] = BitConverter.ToSingle(array, 48);
            array2[13] = BitConverter.ToSingle(array, 52);
            array2[14] = BitConverter.ToSingle(array, 56);
            array2[15] = BitConverter.ToSingle(array, 60);
            return array2;
        }

        // Token: 0x0600003D RID: 61 RVA: 0x00002C24 File Offset: 0x00000E24
        public bool WriteBytes(IntPtr address, byte[] newbytes)
        {
            return memEdit.WriteProcessMemory(this.proc.Handle, address, newbytes, newbytes.Length, IntPtr.Zero);
        }

        // Token: 0x0600003E RID: 62 RVA: 0x00002C50 File Offset: 0x00000E50
        public bool WriteBytes(IntPtr address, int offset, byte[] newbytes)
        {
            return memEdit.WriteProcessMemory(this.proc.Handle, address + offset, newbytes, newbytes.Length, IntPtr.Zero);
        }

        // Token: 0x0600003F RID: 63 RVA: 0x00002C84 File Offset: 0x00000E84
        public bool WriteBytes(IntPtr address, string newbytes)
        {
            byte[] array = (from b in newbytes.Split(' ', StringSplitOptions.None)
                            select Convert.ToByte(b, 16)).ToArray<byte>();
            return memEdit.WriteProcessMemory(this.proc.Handle, address, array, array.Length, IntPtr.Zero);
        }

        // Token: 0x06000040 RID: 64 RVA: 0x00002CE4 File Offset: 0x00000EE4
        public bool WriteBytes(IntPtr address, int offset, string newbytes)
        {
            byte[] array = (from b in newbytes.Split(' ', StringSplitOptions.None)
                            select Convert.ToByte(b, 16)).ToArray<byte>();
            return memEdit.WriteProcessMemory(this.proc.Handle, address + offset, array, array.Length, IntPtr.Zero);
        }

        // Token: 0x06000041 RID: 65 RVA: 0x00002D4C File Offset: 0x00000F4C
        public bool WriteDoubleVec(IntPtr address, double[] value)
        {
            byte[] array = new byte[24];
            byte[] bytes = BitConverter.GetBytes(value[0]);
            byte[] bytes2 = BitConverter.GetBytes(value[1]);
            byte[] bytes3 = BitConverter.GetBytes(value[2]);
            bytes.CopyTo(array, 0);
            bytes2.CopyTo(array, 8);
            bytes3.CopyTo(array, 16);
            return this.WriteBytes(address, array);
        }

        // Token: 0x06000042 RID: 66 RVA: 0x00002DA8 File Offset: 0x00000FA8
        public bool WriteDoubleVec(IntPtr address, int offset, double[] value)
        {
            byte[] array = new byte[24];
            byte[] bytes = BitConverter.GetBytes(value[0]);
            byte[] bytes2 = BitConverter.GetBytes(value[1]);
            byte[] bytes3 = BitConverter.GetBytes(value[2]);
            bytes.CopyTo(array, 0);
            bytes2.CopyTo(array, 8);
            bytes3.CopyTo(array, 16);
            return this.WriteBytes(address + offset, array);
        }

        // Token: 0x06000043 RID: 67 RVA: 0x00002E0C File Offset: 0x0000100C
        public bool WriteInt(IntPtr address, int value)
        {
            return this.WriteBytes(address, BitConverter.GetBytes(value));
        }

        // Token: 0x06000044 RID: 68 RVA: 0x00002E2C File Offset: 0x0000102C
        public bool WriteInt(IntPtr address, int offset, int value)
        {
            return this.WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        // Token: 0x06000045 RID: 69 RVA: 0x00002E54 File Offset: 0x00001054
        public bool WriteShort(IntPtr address, short value)
        {
            return this.WriteBytes(address, BitConverter.GetBytes(value));
        }

        // Token: 0x06000046 RID: 70 RVA: 0x00002E74 File Offset: 0x00001074
        public bool WriteShort(IntPtr address, int offset, short value)
        {
            return this.WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        // Token: 0x06000047 RID: 71 RVA: 0x00002E9C File Offset: 0x0000109C
        public bool WriteUShort(IntPtr address, ushort value)
        {
            return this.WriteBytes(address, BitConverter.GetBytes(value));
        }

        // Token: 0x06000048 RID: 72 RVA: 0x00002EBC File Offset: 0x000010BC
        public bool WriteUShort(IntPtr address, int offset, ushort value)
        {
            return this.WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        // Token: 0x06000049 RID: 73 RVA: 0x00002EE4 File Offset: 0x000010E4
        public bool WriteUInt(IntPtr address, uint value)
        {
            return this.WriteBytes(address, BitConverter.GetBytes(value));
        }

        // Token: 0x0600004A RID: 74 RVA: 0x00002F04 File Offset: 0x00001104
        public bool WriteUInt(IntPtr address, int offset, uint value)
        {
            return this.WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        // Token: 0x0600004B RID: 75 RVA: 0x00002F2C File Offset: 0x0000112C
        public bool WriteLong(IntPtr address, long value)
        {
            return this.WriteBytes(address, BitConverter.GetBytes(value));
        }

        // Token: 0x0600004C RID: 76 RVA: 0x00002F4C File Offset: 0x0000114C
        public bool WriteLong(IntPtr address, int offset, long value)
        {
            return this.WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        // Token: 0x0600004D RID: 77 RVA: 0x00002F74 File Offset: 0x00001174
        public bool WriteULong(IntPtr address, ulong value)
        {
            return this.WriteBytes(address, BitConverter.GetBytes(value));
        }

        // Token: 0x0600004E RID: 78 RVA: 0x00002F94 File Offset: 0x00001194
        public bool WriteULong(IntPtr address, int offset, ulong value)
        {
            return this.WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        // Token: 0x0600004F RID: 79 RVA: 0x00002FBC File Offset: 0x000011BC
        public bool WriteFloat(IntPtr address, float value)
        {
            return this.WriteBytes(address, BitConverter.GetBytes(value));
        }

        // Token: 0x06000050 RID: 80 RVA: 0x00002FDC File Offset: 0x000011DC
        public bool WriteFloat(IntPtr address, int offset, float value)
        {
            return this.WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        // Token: 0x06000051 RID: 81 RVA: 0x00003004 File Offset: 0x00001204
        public bool WriteDouble(IntPtr address, double value)
        {
            return this.WriteBytes(address, BitConverter.GetBytes(value));
        }

        // Token: 0x06000052 RID: 82 RVA: 0x00003024 File Offset: 0x00001224
        public bool WriteDouble(IntPtr address, int offset, double value)
        {
            return this.WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        // Token: 0x06000053 RID: 83 RVA: 0x0000304C File Offset: 0x0000124C
        public bool WriteBool(IntPtr address, bool value)
        {
            return this.WriteBytes(address, BitConverter.GetBytes(value));
        }

        // Token: 0x06000054 RID: 84 RVA: 0x0000306C File Offset: 0x0000126C
        public bool WriteBool(IntPtr address, int offset, bool value)
        {
            return this.WriteBytes(address + offset, BitConverter.GetBytes(value));
        }

        // Token: 0x06000055 RID: 85 RVA: 0x00003094 File Offset: 0x00001294
        public bool WriteString(IntPtr address, string value)
        {
            return this.WriteBytes(address, Encoding.UTF8.GetBytes(value));
        }

        // Token: 0x06000056 RID: 86 RVA: 0x000030B8 File Offset: 0x000012B8
        public bool WriteVec(IntPtr address, Vector3 value)
        {
            byte[] array = new byte[12];
            byte[] bytes = BitConverter.GetBytes(value.X);
            byte[] bytes2 = BitConverter.GetBytes(value.Y);
            byte[] bytes3 = BitConverter.GetBytes(value.Z);
            bytes.CopyTo(array, 0);
            bytes2.CopyTo(array, 4);
            bytes3.CopyTo(array, 8);
            return this.WriteBytes(address, array);
        }

        // Token: 0x06000057 RID: 87 RVA: 0x0000311C File Offset: 0x0000131C
        public bool WriteVec(IntPtr address, int offset, Vector3 value)
        {
            byte[] array = new byte[12];
            byte[] bytes = BitConverter.GetBytes(value.X);
            byte[] bytes2 = BitConverter.GetBytes(value.Y);
            byte[] bytes3 = BitConverter.GetBytes(value.Z);
            bytes.CopyTo(array, 0);
            bytes2.CopyTo(array, 4);
            bytes3.CopyTo(array, 8);
            return this.WriteBytes(address + offset, array);
        }

        // Token: 0x06000058 RID: 88 RVA: 0x00003188 File Offset: 0x00001388
        public bool Nop(IntPtr address, int length)
        {
            byte[] array = new byte[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = 144;
            }
            return this.WriteBytes(address, array);
        }

        // Token: 0x06000059 RID: 89 RVA: 0x000031C4 File Offset: 0x000013C4
        public bool Nop(IntPtr address, int offset, int length)
        {
            byte[] array = new byte[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = 144;
            }
            return this.WriteBytes(address + offset, array);
        }

        // Token: 0x0600005A RID: 90 RVA: 0x00003208 File Offset: 0x00001408
        public IntPtr ScanForBytes32(string moduleName, string needle)
        {
            ProcessModule processModule = this.proc.Modules.OfType<ProcessModule>().FirstOrDefault((ProcessModule x) => x.ModuleName == moduleName);
            bool flag = processModule == null;
            if (flag)
            {
                throw new InvalidOperationException("module was not found. Check your module name.");
            }
            byte[] needle2 = (from b in needle.Split(' ', StringSplitOptions.None)
                              select Convert.ToByte(b, 16)).ToArray<byte>();
            bool flag2 = processModule.FileName == null;
            if (flag2)
            {
                throw new InvalidOperationException("module filename was null.");
            }
            byte[] array;
            using (FileStream fileStream = new FileStream(processModule.FileName, FileMode.Open, FileAccess.Read))
            {
                array = new byte[fileStream.Length];
                fileStream.Read(array, 0, (int)fileStream.Length);
            }
            return (IntPtr)this.ScanForBytes32(array, needle2);
        }

        // Token: 0x0600005B RID: 91 RVA: 0x0000330C File Offset: 0x0000150C
        public long ScanForBytes32(byte[] haystack, byte[] needle)
        {
            for (int i = 0; i < haystack.Length - needle.Length; i++)
            {
                bool flag = true;
                for (int j = 0; j < needle.Length; j++)
                {
                    bool flag2 = needle[j] != byte.MaxValue && haystack[i + j] != needle[j];
                    if (flag2)
                    {
                        flag = false;
                        break;
                    }
                }
                bool flag3 = flag;
                if (flag3)
                {
                    return (long)i;
                }
            }
            return -1L;
        }

        // Token: 0x04000003 RID: 3
        private Process proc;
    }
}
