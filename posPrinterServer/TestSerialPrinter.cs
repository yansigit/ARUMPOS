using System;
using System.Diagnostics;
using System.Text;

namespace POSPrinterLibrary
{
    class TestSerialPrinter
    {
        public TestSerialPrinter(string portName, int baudRate)
        {

        }

        public void Write(byte[] bytes)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var encoding = System.Text.Encoding.GetEncoding("euc-kr");
            Debug.WriteLine(encoding.GetString(bytes));
        }

        public void Dispose()
        {

        }
    }
}