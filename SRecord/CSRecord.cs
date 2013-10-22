﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WelchAllyn.SRecord
{
    public class CSRecord
    {
        #region Data Members

        string startCode = "";
        int recordType = -1;
        int byteCount = 0;
        int dataByteCount = 0;
        int memoryAddr = 0;
        string data = "";
        int checkSum = 0;
        string inputData = "";
        int inputFileIndex = 0;

        #endregion

        #region Properties

        public string StartCode
        {
            get { return startCode; }
            set { startCode = value; }
        }

        public int RecordType
        {
            get { return recordType; }
            set { recordType = value; }
        }

        public int ByteCount
        {
            get { return byteCount; }
            set { byteCount = value; }
        }

        public int MemoryAddress
        {
            get { return memoryAddr; }
            set { memoryAddr = value; }
        }

        public string Data
        {
            get { return data; }
            set { data = value; }
        }

        public byte[] DataBytes
        {
            get
            {
                return Encoding.ASCII.GetBytes(Data);
            }
        }

        public int DataByteCount
        {
            get { return dataByteCount; }
            set { dataByteCount = value; }
        }

        public int CheckSum
        {
            get { return checkSum; }
            set { checkSum = value; }
        }

        public string InputData
        {
            get { return inputData; }
            set { inputData = value; }
        }

        public int InputFileIndex
        {
            get { return inputFileIndex; }
            set { inputFileIndex = value; }
        }

        #endregion

        #region Constructors

        public CSRecord()
        {
        }

        public CSRecord(string line)
        {
            string address = "";
            bool crcCheck = true;

            inputData = line;
            startCode = line.Substring(0, 1);
            recordType = int.Parse(line.Substring(1, 1), System.Globalization.NumberStyles.HexNumber);
            byteCount = int.Parse(line.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);

            switch (recordType)
            {
                case 0:
                    int length = line.Length;
                    data = line.Substring(8, line.Length - 10);  // Length = Length - recordtype - byte count - address - checksum
                    string crc = line.Substring(line.Length - 2, 2);
                    checkSum = int.Parse(line.Substring(inputData.Length - 2, 2), System.Globalization.NumberStyles.HexNumber);

                    break;
                case 1:
                    address = line.Substring(4, 4);
                    memoryAddr = int.Parse(address, System.Globalization.NumberStyles.HexNumber);
                    data = line.Substring(8, (byteCount - 3) * 2);
                    checkSum = int.Parse(line.Substring(((byteCount - 3) * 2) + 8, 2), System.Globalization.NumberStyles.HexNumber);
                    crcCheck = CheckCRC(line, checkSum);
                    dataByteCount = byteCount - 3;  // byteCount - address bytes - checksum
                    break;
                case 2:
                    address = line.Substring(4, 6);
                    memoryAddr = int.Parse(address, System.Globalization.NumberStyles.HexNumber);
                    data = line.Substring(10, (byteCount - 4) * 2);
                    checkSum = int.Parse(line.Substring(((byteCount - 4) * 2) + 10, 2), System.Globalization.NumberStyles.HexNumber);
                    crcCheck = CheckCRC(line, checkSum);
                    dataByteCount = byteCount - 4;  // byteCount - address bytes - checksum
                    //totalBytes = totalBytes + data.Length;
                    break;
                case 3:
                    address = line.Substring(4, 8);
                    memoryAddr = int.Parse(address, System.Globalization.NumberStyles.HexNumber);
                    data = line.Substring(12, (byteCount - 5) * 2);
                    checkSum = int.Parse(line.Substring(((byteCount - 5) * 2) + 12, 2), System.Globalization.NumberStyles.HexNumber);
                    crcCheck = CheckCRC(line, checkSum);
                    dataByteCount = byteCount - 5;  // byteCount - address bytes - checksum
                    break;
                case 5:
                    // RECORD COUNT TYPE.  WOULD INDICATE THE NUMBER OF RECORDS PREVIOUSLY SENT BUT THIS IS NOT
                    // USED FOR PRE-COMPILED CODE.

                    break;

                case 7:

                    address = line.Substring(4, 8);
                    memoryAddr = int.Parse(address, System.Globalization.NumberStyles.HexNumber);
                    checkSum = int.Parse(line.Substring(line.Length - 2, 2), System.Globalization.NumberStyles.HexNumber);

                    break;

                case 8:

                    address = line.Substring(4, 6);
                    memoryAddr = int.Parse(address, System.Globalization.NumberStyles.HexNumber);
                    checkSum = int.Parse(line.Substring(line.Length - 2, 2), System.Globalization.NumberStyles.HexNumber);

                    break;

                case 9:

                    address = line.Substring(4, 4);
                    memoryAddr = int.Parse(address, System.Globalization.NumberStyles.HexNumber);
                    checkSum = int.Parse(line.Substring(line.Length - 2, 2), System.Globalization.NumberStyles.HexNumber);

                    break;

            }

            if (crcCheck == false)
            {
                throw new Exception("something");
            }
        }
        #endregion

        #region Private Methods

        private bool CheckCRC(string line, int checkSum)
        {
            string pair = "";
            int counter = 0;
            int temp = 0;
            int total = 0;

            line = line.Substring(2);

            while (counter < line.Length - 2)
            {
                pair = line.Substring(counter, 2);
                temp = int.Parse(pair, System.Globalization.NumberStyles.HexNumber);
                total = total + temp;
                counter = counter + 2;
            }

            byte valueToComplement = (byte)total;
            byte complement = (byte)~valueToComplement;

            if (checkSum == (int)complement)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        #endregion
    }
}