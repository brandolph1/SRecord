using System;
using WelchAllyn.SRecord;

namespace WelchAllyn.SRecord.Test
{
    class Program
    {
        static CSRecord srec;

        static void Main(string[] args)
        {
            String[] arrTestSRecords = { "S31500000000070000EA74080200FFE1552A680800A00C",
                                         "S315000000108C0800A0B40800A0D80800A018000000B2",
                                         "S315000000201C000000D3F021E3100F11EEEC159FE544"
                                       };
            int nn = 1;
            byte[] thedata;

            Console.WriteLine("Testing SRecords--");

            foreach (String s in arrTestSRecords)
            {
                Console.WriteLine("{0}: {1}", nn, s);
                srec = new CSRecord(s);
                Console.WriteLine(" start code={0} rec type={1} byte cnt={2}", srec.StartCode, srec.RecordType, srec.ByteCount);
                Console.WriteLine(" data cnt={0}   checksum={1}", srec.DataByteCount, srec.CheckSum);
                Console.WriteLine(" address={0:X8}", srec.MemoryAddress);

                Console.Write(" data=");
                thedata = srec.DataBytes;

                for (int ii = 0; ii < thedata.Length; ++ii)
                {
                    Console.Write("{0:X2} ", thedata[ii]);
                }

                Console.WriteLine("\n");

                ++nn;
            }
        }
    }
}
