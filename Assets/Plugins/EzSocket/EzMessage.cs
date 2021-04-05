using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace EazyEngine.Networking
{
    //public class EzObject
    //{
    //    byte[] _bytes;
    //    public EzObject(byte[] pBytes)
    //    {
    //        _bytes = pBytes;
    //    }
    //}
    [Serializable]
    public class EzTable
    {
        public int row,col;
        public Type[] typeCol;
        public string[] _Cols;
        public object[,] _datas;
        List<string> listString = new List<string>();
        List<byte> buffer = new List<byte>();
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public void endCode()
        {
            buffer.Add((byte)col);
            buffer.AddRange(BitConverter.GetBytes( (ushort)row));
            string strCol = "";
            for(int i = 0; i < _Cols.Length; ++i)
            {
                strCol += _Cols[i] + (i < _Cols.Length -1 ? "}" : "");
            }
            var pString = Base64Encode(strCol);
           
            buffer.AddRange(BitConverter.GetBytes( (ushort)pString.Length));
            buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pString));
            for(int i = 0; i < col; ++i)
            {
                if(typeCol[i] == typeof(string))
                {
                    buffer.Add((byte)TypeData.STRING);
                }
                if (typeCol[i] == typeof(int)|| typeCol[i] == typeof(long))
                {
                    buffer.Add((byte)TypeData.INTERGER);
                    if(typeCol[i] == typeof(long))
                    {
                        typeCol[i] = typeof(int);
                    }
                }
            }
            byte[] pByteStrings = ArrayListString(listString);
            buffer.AddRange(BitConverter.GetBytes(pByteStrings.Length));
            buffer.AddRange(pByteStrings);
            for(int i = 0; i < col; ++i)
            {
                for(int j = 0; j < row; j++)
                {
                    Type pType = typeCol[i];
            
                    if (pType == typeof(int) || pType == typeof(string))
                    {
                        if (_datas[i, j] == null ||  (pType == typeof(string) && _datas[i, j].ToString().Length == 0))
                        {
                            _datas[i, j] = 0;
                            buffer.AddRange(BitConverter.GetBytes(Convert.ToInt32(_datas[i, j])));
                        }
                        else
                        {
                            int index = Convert.ToInt32(_datas[i, j]);
                            if (index >= 0)
                            {
                                index++;
                            }
                            buffer.AddRange(BitConverter.GetBytes(index));
                        }
                    }
                }
            }
        }

        public void deCode()
        {
            byte[] readBuf = buffer.ToArray();
            int pos = 0;
            col = readBuf[pos];
            pos++;
            row = BitConverter.ToUInt16(readBuf, pos);
            pos += 2;
            int lengthColName = BitConverter.ToUInt16(readBuf,pos);
            pos+=2;
            string strCol = System.Text.ASCIIEncoding.ASCII.GetString(readBuf, pos, lengthColName);
            strCol = Base64Decode(strCol);
            pos += lengthColName;
            _Cols = strCol.Split('}');
            typeCol = new Type[col];
            for (int i = 0; i < col; ++i)
            {
                int pType = readBuf[pos];
                pos++;
                if(pType == (int)TypeData.INTERGER)
                {
                    typeCol[i] = typeof(int);
                }
                if(pType == (int)TypeData.STRING)
                {
                    typeCol[i] = typeof(string);
                }
            }
            int lengthString = BitConverter.ToInt32(readBuf, pos);
            pos += 4;
            string mainStr = System.Text.ASCIIEncoding.ASCII.GetString(readBuf, pos, lengthString);
            mainStr = Base64Decode(mainStr);
            listString.AddRange(mainStr.Split('}'));
            pos += lengthString;
            _datas = new object[col, row];
            for (int i = 0; i < col; ++i)
            {
                for (int j = 0; j < row; j++)
                {
                    _datas[i, j] = BitConverter.ToInt32(readBuf, pos);             
                    if(typeCol[i] == typeof(string))
                    {
                        if ((int)_datas[i, j] == 0)
                        {
                            _datas[i, j] = "";
                            pos += 4;
                            continue;
                        }
                        _datas[i, j] = listString[Convert.ToInt32(_datas[i, j])-1];
                    }
                    else
                    {
                        _datas[i, j] = (int)_datas[i, j]  > 0 ? ((int)_datas[i, j] - 1) : (int)_datas[i, j];
                    }
                    pos += 4;
                }
            }

        }

        public byte[] ToArray()
        {
            return buffer.ToArray();
        }


        public byte[] ArrayListString(List<string> pList)
        {
            string pMainStr = "";
            for(int i = 0; i < pList.Count; ++i)
            {
                pMainStr += pList[i] + (i < pList.Count - 1 ? "}" : "");
            }
            pMainStr = Base64Encode(pMainStr);
           return System.Text.ASCIIEncoding.ASCII.GetBytes(pMainStr);
        }

        public EzTable(byte[] pBuff)
        {
            buffer.AddRange(pBuff);
            deCode();
        }

        public EzTable(DataTable pTable)
        {
            row = pTable.Rows.Count;
            col = pTable.Columns.Count;
            _Cols = new string[pTable.Columns.Count];
            typeCol = new Type[pTable.Columns.Count];
            _datas = new object[pTable.Columns.Count, pTable.Rows.Count];
            for (int i = 0; i < _Cols.Length; ++i)
            {
                _Cols[i] = pTable.Columns[i].ColumnName;
                typeCol[i] = pTable.Columns[i].DataType;
                for (int j = 0; j < pTable.Rows.Count; j++)
                {
                    if((typeCol[i] == typeof(string)))
                    {
                       string pValue = pTable.Rows[j][_Cols[i]].ToString();
                        bool isExist = false;
                        int index = 0;
                        if (pValue.Length > 0)
                        {
                            int indexClear = 0;
                            int countSpace = 0;
                            for (int k = pValue.Length - 1; k >= 0; --k)
                            {
                                if (pValue[k] != ' ')
                                {
                                    indexClear = k;
                                    break;
                                }
                                else
                                {
                                    countSpace++;
                                }
                            }
                            if (countSpace > 0)
                            {
                                pValue = pValue.Remove(indexClear, countSpace);
                            }
                        }
                        for (int g = 0; g < listString.Count; ++g)
                        {
                      
                            if (listString[g] == pValue)
                            {
                                isExist = true;
                                index = g;
                                _datas[i, j] = index;
                                break;
                            }
                        }
                        if (!isExist && pValue.Length > 0)
                        {
                            index = listString.Count;
                            listString.Add(pValue);
                            _datas[i, j] = index;
                        }
                        else if(pValue.Length == 0)
                        {
                            _datas[i, j] = null;
                        }
                   
                    }
                    else
                    {
                        if (pTable.Rows[j][_Cols[i]] == null || pTable.Rows[j][_Cols[i]].ToString().Length == 0)
                        {
                            _datas[i, j] = 0;
                        }
                        else
                        {
                            _datas[i, j] = pTable.Rows[j][_Cols[i]];
                        }
                    }
                }
            }
            endCode();
        }
    }
    public enum TypeData { STRING, INTERGER, ARRAY, TABLE, STREAM }
    public struct EzRawData
    {
        public byte _byte;
        public object _value;
        public EzRawData(byte pByte, object pValue)
        {
            _byte = pByte;
            _value = pValue;
        }
    }
    public class EzMessage
    {
        public static Encoding encoding =  ASCIIEncoding.Unicode;
        List<byte> Buff;
        byte[] readBuff;
        ushort _header;
        List<EzRawData> _dict;
        int readpos;

        public ushort Header { get => _header; set => _header = value; }

        public EzMessage(ushort pHeader)
        {
            Buff = new List<byte>();
            Header = pHeader;
            _dict = new List<EzRawData>();
        }

        public EzMessage(byte[] pBytes)
        {
            Buff = new List<byte>();
            readpos = 0;
            Buff.AddRange(pBytes);
            _dict = new List<EzRawData>();
        }

        public bool containData(byte pIndex)
        {
            for (int i = 0; i < _dict.Count; ++i)
            {
                if (_dict[i]._byte == pIndex)
                {
                    return true;
                }
            }
            return false;
        }


        public void addValue(byte pIndex, object pValue)
        {
            for (int i = 0; i < _dict.Count; ++i)
            {
                if (_dict[i]._byte == pIndex)
                {
                    _dict[i] = new EzRawData(pIndex, pValue);
                    return;
                }
            }
            _dict.Add(new EzRawData(pIndex, pValue));
        }

        public void endCode()
        {
            Buff.AddRange(BitConverter.GetBytes(Header));
            Buff.AddRange(BitConverter.GetBytes((ushort)_dict.Count));
            for (int i = 0; i < _dict.Count; ++i)
            {
                Buff.Add(_dict[i]._byte);

                if (_dict[i]._value.GetType() == typeof(string))
                {
                    Buff.Add((byte)TypeData.STRING);
                    byte[] pArrayByte = encoding.GetBytes((string)_dict[i]._value);
                    Buff.AddRange(BitConverter.GetBytes(pArrayByte.Length));
                    Buff.AddRange(pArrayByte);
                }
                else if (_dict[i]._value.GetType() == typeof(int))
                {
                    Buff.Add((byte)TypeData.INTERGER);
                    Buff.AddRange(BitConverter.GetBytes((int)_dict[i]._value));
                }
                else if (_dict[i]._value.GetType() == typeof(EzTable))
                {
                    EzTable pTable = (EzTable)_dict[i]._value;
                    byte[] pBuff = pTable.ToArray();
                    Buff.Add((byte)TypeData.TABLE);
                    Buff.AddRange(BitConverter.GetBytes(pBuff.Length));
                    Buff.AddRange(pBuff);
                }
                else
                {
                    Buff.Add((byte)TypeData.STREAM);
                    MemoryStream ms = new MemoryStream();
                    BinaryFormatter bf1 = new BinaryFormatter();
                    bf1.Serialize(ms, _dict[i]._value);
                    byte[] datas = ms.ToArray();
                    Buff.AddRange(BitConverter.GetBytes(datas.Length));
                    Buff.AddRange(datas);
                }
            }
        }

        public void deCode()
        {
            readBuff = ToArray();
            if (readBuff == null  || readBuff.Length == 0) return;
            Header = BitConverter.ToUInt16(readBuff, readpos);
            readpos += 2;
            int lengthElement = BitConverter.ToUInt16(readBuff, readpos);
            readpos += 2;

            for (int i = 0; i < lengthElement; ++i)
            {
                object pValue = null;
                byte pIndex = readBuff[readpos];
                readpos++;
                byte pType = readBuff[readpos];
                readpos++;
                if (pType == (byte)TypeData.STRING)
                {
                    int len = BitConverter.ToInt32(readBuff, readpos);
                    readpos += 4;
                    pValue = encoding.GetString(readBuff, readpos, len);
                    readpos += len;
                }
                else if (pType == (byte)TypeData.INTERGER)
                {
                    pValue = BitConverter.ToInt32(readBuff, readpos);
                    readpos += 4;
                }
                else if (pType == (byte)TypeData.TABLE)
                {
                    int len = BitConverter.ToInt32(readBuff, readpos);
                    readpos += 4;
                    byte[] pBuff = new byte[len];
                    for(int j = 0; j < len; j++)
                    {
                        pBuff[j] = readBuff[readpos + j];
                    }
                    pValue = new EzTable(pBuff);
                    readpos += len;
                }
                else
                {
                    int len = BitConverter.ToInt32(readBuff, readpos);
                    readpos += 4;
                    MemoryStream ms = new MemoryStream(readBuff, readpos, len);
                    BinaryFormatter bf1 = new BinaryFormatter();
                   // ms.Position = 0;
                    pValue = bf1.Deserialize(ms);
                    readpos += len;
                }
                _dict.Add(new EzRawData(pIndex, pValue));
            }
        }

        public byte[] ToArray()
        {
            return Buff.ToArray();
        }

        public object getValue(byte pTag)
        {
            for (int i = 0; i < _dict.Count; ++i)
            {
                if (_dict[i]._byte == pTag)
                {
                    return _dict[i]._value;
                }
            }
            return null;
        }

        public string getString(byte pTag)
        {
            object pObject = getValue(pTag);
            return (pObject != null) ? (string)pObject : "";
        }

        public int getInt(byte pTag)
        {
            object pObject = getValue(pTag);
            return (pObject != null) ? (int)pObject : 0;
        }
    }
}
