using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace POSPrinterLibrary
{
    public class JungsanData
    {
        // 일자 (ex 2021년 8월)
        public DateTime Date { get; set; }
        public string ShortDate => Date.ToShortDateString();
        // 총매출액
        public int SalesPrice { get; set; } = 0;
        // 할인금액
        public int DiscountPrice { get; set; } = 0;
        // 취소
        public int CanceledPrice { get; set; } = 0;
        // 순매출액
        public int ProfitPrice { get; set; } = 0;
    }

    public class CustomerPrintData
    {
        public (int orderNum, int price) Order { get; set; }
        public List<(string name, int price, int quantity)> MenuList { get; set; }
        public (string cardCompany, string cardNumber, string tid, string franchiseCode) Card { get; set; }
        public (string licenseNumber, string ownerName, string storeName, string address, string phoneNumber) Store { get; set; }
    }

    public class MenuPrintData
    {
        // 이름
        public string Name { get; set; }
        // 갯수
        public int Quantity { get; set; }
        // 옵션
        public string OptionString { get; set; }
    }
}