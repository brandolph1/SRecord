using System;
using System.Globalization;
using System.Text;

namespace WelchAllyn.SRecord
{
    public class CSRecord
    {
        #region Data Members

        private object _sync = new object();
        private string startCode = "";
        private int recordType = -1;
        private int byteCount = 0;
        private int dataByteCount = 0;
        private uint memoryAddr = 0;
        private string data = "";
        private int checkSum = 0;
        private string inputData = "";
        private int inputFileIndex = 0;

        #endregion

        #region Public Methods
        ////////////////////
        //
        public void Clear()
        {
            lock (_sync)
            {
                startCode = "";
                recordType = -1;
                byteCount = 0;
                dataByteCount = 0;
                memoryAddr = 0;
                data = "";
                checkSum = 0;
                inputData = "";
                inputFileIndex = 0;
            }
        }

        public void Load(string line)
        {
            _parseString(line);
        }
        //
        ////////////////////
        #endregion

        #region Properties

        public string StartCode
        {
            get { lock (_sync) { return startCode; } }
            set { lock (_sync) { startCode = value; } }
        }

        public int RecordType
        {
            get { lock (_sync) { return recordType; } }
            set { lock (_sync) { recordType = value; } }
        }

        public int ByteCount
        {
            get { lock (_sync) { return byteCount; } }
            set { lock (_sync) { byteCount = value; } }
        }

        public uint MemoryAddress
        {
            get { lock (_sync) { return memoryAddr; } }
            set { lock (_sync) { memoryAddr = value; } }
        }

        public string Data
        {
            get { lock (_sync) { return data; } }
            set { lock (_sync) { data = value; } }
        }

        public byte[] DataBytes
        {
            get
            {
                lock (_sync)
                {
                    byte[] HexAsBytes = null;

                    if (data.Length % 2 != 0)
                    {
                        throw new ArgumentException("The binary key cannot have an odd number of digits.");
                    }
                    else
                    {
                        HexAsBytes = new byte[data.Length / 2];

                        for (int index = 0; index < HexAsBytes.Length; index++)
                        {
                            string byteValue = data.Substring(index * 2, 2);
                            HexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        }
                    }
                    
                    return HexAsBytes;
                }
            }
        }

        public int DataByteCount
        {
            get { lock (_sync) { return dataByteCount; } }
            set { lock (_sync) { dataByteCount = value; } }
        }

        public int CheckSum
        {
            get { lock (_sync) { return checkSum; } }
            set { lock (_sync) { checkSum = value; } }
        }

        public string InputData
        {
            get { lock (_sync) { return inputData; } }
            set { lock (_sync) { inputData = value; } }
        }

        public int InputFileIndex
        {
            get { lock (_sync) { return inputFileIndex; } }
            set { lock (_sync) { inputFileIndex = value; } }
        }

        #endregion

        #region Constructors

        public enum SRecordType
        {
            S0,
            S1,
            S2,
            S3,
            S4,
            S5,
            S6,
            S7,
            S8,
            S9
        }

        public CSRecord()
        {
        }

        public CSRecord(string line)
        {
            _parseString(line);
        }

        #endregion

        #region Private Methods

        private void _parseString(string line)
        {
            lock (_sync)
            {
                string address = "";
                bool crcCheck = true;

                inputData = line;
                startCode = line.Substring(0, 1);
                recordType = int.Parse(line.Substring(1, 1), NumberStyles.HexNumber);
                byteCount = int.Parse(line.Substring(2, 2), NumberStyles.HexNumber);

                switch (recordType)
                {
                    case 0:
                        int length = line.Length;
                        data = line.Substring(8, line.Length - 10);  // Length = Length - recordtype - byte count - address - checksum
                        string crc = line.Substring(line.Length - 2, 2);
                        checkSum = int.Parse(line.Substring(inputData.Length - 2, 2), NumberStyles.HexNumber);

                        break;
                    case 1:
                        address = line.Substring(4, 4);
                        memoryAddr = uint.Parse(address, System.Globalization.NumberStyles.HexNumber);
                        data = line.Substring(8, (byteCount - 3) * 2);
                        checkSum = int.Parse(line.Substring(((byteCount - 3) * 2) + 8, 2), NumberStyles.HexNumber);
                        crcCheck = _checkCRC(line, checkSum);
                        dataByteCount = byteCount - 3;  // byteCount - address bytes - checksum
                        break;
                    case 2:
                        address = line.Substring(4, 6);
                        memoryAddr = uint.Parse(address, System.Globalization.NumberStyles.HexNumber);
                        data = line.Substring(10, (byteCount - 4) * 2);
                        checkSum = int.Parse(line.Substring(((byteCount - 4) * 2) + 10, 2), NumberStyles.HexNumber);
                        crcCheck = _checkCRC(line, checkSum);
                        dataByteCount = byteCount - 4;  // byteCount - address bytes - checksum
                        //totalBytes = totalBytes + data.Length;
                        break;
                    case 3:
                        address = line.Substring(4, 8);
                        memoryAddr = uint.Parse(address, System.Globalization.NumberStyles.HexNumber);
                        data = line.Substring(12, (byteCount - 5) * 2);
                        checkSum = int.Parse(line.Substring(((byteCount - 5) * 2) + 12, 2), NumberStyles.HexNumber);
                        crcCheck = _checkCRC(line, checkSum);
                        dataByteCount = byteCount - 5;  // byteCount - address bytes - checksum
                        break;
                    case 5:
                        // RECORD COUNT TYPE.  WOULD INDICATE THE NUMBER OF RECORDS PREVIOUSLY SENT BUT THIS IS NOT
                        // USED FOR PRE-COMPILED CODE.

                        break;

                    case 7:

                        address = line.Substring(4, 8);
                        memoryAddr = uint.Parse(address, System.Globalization.NumberStyles.HexNumber);
                        checkSum = int.Parse(line.Substring(line.Length - 2, 2), NumberStyles.HexNumber);

                        break;

                    case 8:

                        address = line.Substring(4, 6);
                        memoryAddr = uint.Parse(address, System.Globalization.NumberStyles.HexNumber);
                        checkSum = int.Parse(line.Substring(line.Length - 2, 2), NumberStyles.HexNumber);

                        break;

                    case 9:

                        address = line.Substring(4, 4);
                        memoryAddr = uint.Parse(address, System.Globalization.NumberStyles.HexNumber);
                        checkSum = int.Parse(line.Substring(line.Length - 2, 2), NumberStyles.HexNumber);

                        break;

                }

                if (crcCheck == false)
                {
                    throw new Exception("Checksum failed");
                }
            }
        }

        private bool _checkCRC(string line, int checkSum)
        {
            string pair = "";
            int counter = 0;
            int temp = 0;
            int total = 0;

            line = line.Substring(2);

            while (counter < line.Length - 2)
            {
                pair = line.Substring(counter, 2);
                temp = int.Parse(pair, NumberStyles.HexNumber);
                total = total + temp;
                counter = counter + 2;
            }

            byte valueToComplement = (byte)total;
            byte complement = (byte)~valueToComplement;

            return (checkSum == (int)complement) ? true : false;
        }

        #endregion
    }
}
