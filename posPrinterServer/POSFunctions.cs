using System;
using System.Collections.Generic;
using System.Linq;
using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;

namespace POSPrinterLibrary
{
    public class POSFunctions
    {
#if DEBUG
        static TestSerialPrinter printer;
#else
        static SerialPrinter printer;
#endif
        public static void TestPrinting(string portName, int baudRate)
        {
            var e = new CustomEpson();
            try
            {
#if DEBUG
                printer = new TestSerialPrinter(portName: portName, baudRate: baudRate);
#else
                printer = new SerialPrinter(portName:portName, baudRate:baudRate);
#endif
                printer.Write(
                    ByteSplicer.Combine(
                        e.CenterAlign(),
                        e.SetStyles(PrintStyle.FontB | PrintStyle.DoubleHeight | PrintStyle.DoubleWidth | PrintStyle.Bold),
                        e.PrintLine("프린트가 정상작동합니다."),
                        e.PrintLine("즐거운 하루 되세요."),
                        e.FeedLines(3),
                        e.FullCut()
                        ));

                printer.Dispose();
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }
        public static void StartPrintJungsan(bool isMonth, JungsanData jungsanData, string portName, int baudRate)
        {
            Console.Write("접속했습니다.\n");
            var e = new CustomEpson();
            Console.Write(portName);
#if DEBUG
            printer = new TestSerialPrinter(portName: portName, baudRate: baudRate);
            
#else
            printer = new SerialPrinter(portName: portName, baudRate: baudRate);
#endif
            var dateString = isMonth
                ? $"{jungsanData.Date.Year}년 {jungsanData.Date.Month}월"
                : $"{jungsanData.Date.Year}년 {jungsanData.Date.Month}월 {jungsanData.Date.Day}일";

            printer.Write(
                ByteSplicer.Combine(
                    e.CenterAlign(),
                    e.SetStyles(PrintStyle.FontB | PrintStyle.DoubleHeight | PrintStyle.DoubleWidth | PrintStyle.Bold),
                    e.PrintLine("판매집계표"),
                    e.SetStyles(PrintStyle.None),
                    e.PrintLine("--------------------------"),
                    e.SetStyles(PrintStyle.FontB | PrintStyle.DoubleHeight),
                    e.PrintLine(dateString),
                    e.SetStyles(PrintStyle.None),
                    e.PrintLine("--------------------------")
                ));

                printer.Write(
                ByteSplicer.Combine(
                    e.SetStyles(PrintStyle.FontB | PrintStyle.DoubleHeight),
                    e.PrintLine("구분   \t\t금액"),
                    e.PrintLine("총매출액 \t\t" + jungsanData.SalesPrice),
                    e.PrintLine("할인    \t\t" + jungsanData.DiscountPrice),
                    e.PrintLine("취소    \t\t" + jungsanData.CanceledPrice),
                    e.PrintLine("순매출액 \t\t" + jungsanData.ProfitPrice),
                    e.FeedLines(3),
                    e.FullCut()
                ));

            printer.Dispose();
        }

        // 직원용프린트 함수
        public static void StartEmployeePrint(int orderId, List<MenuPrintData> menuData, string portName, int baudRate)
        {
            Console.Write("접속했습니다.\n");
            var e = new CustomEpson();
            Console.Write("portName:" + portName + " " + baudRate);
#if DEBUG
            printer = new TestSerialPrinter(portName: portName, baudRate: baudRate);
#else
            printer = new SerialPrinter(portName: portName, baudRate: baudRate);
#endif
            printer.Write(
                ByteSplicer.Combine(
                    e.CenterAlign(),
                    e.SetStyles(PrintStyle.FontB | PrintStyle.DoubleHeight | PrintStyle.DoubleWidth | PrintStyle.Bold),
                    e.PrintLine("주문표"),
                    e.SetStyles(PrintStyle.None),
                    e.PrintLine("----------------"),
                    e.SetStyles(PrintStyle.FontB | PrintStyle.DoubleHeight | PrintStyle.DoubleWidth | PrintStyle.Bold),
                    e.PrintLine($"주문번호: {orderId}"),
                    e.SetStyles(PrintStyle.None),
                    e.PrintLine("----------------")
                ));

            byte[] menuBytes = e.FeedLines(1);
            foreach (var menuPrintData in menuData)
            {
                var menuLine = e.PrintLine($"{menuPrintData.Name} \t\t 수량:{menuPrintData.Quantity}");
                var options = menuPrintData.OptionString.Split("\n");
                menuBytes = ByteSplicer.Combine(menuBytes, menuLine);
                menuBytes = options.Aggregate(menuBytes, (current, option) => ByteSplicer.Combine(current, e.PrintLine($"\t\t {option}")));
                menuBytes = ByteSplicer.Combine(menuBytes,
                    e.PrintLine("   "));
            }

            printer.Write(
                ByteSplicer.Combine(
                    e.SetStyles(PrintStyle.FontB | PrintStyle.DoubleHeight),
                    e.CenterAlign(),
                    menuBytes,
                    e.FeedLines(3),
                    e.FullCut()
                ));

            printer.Dispose();
        }

        // 고객용 영수증 출력
        public static void StartCustomerPrint(CustomerPrintData printData, string portName, int baudRate)
        {
            Console.Write("접속했습니다.\n");
            var e = new CustomEpson();
            Console.Write("portName:" + portName + " " + baudRate);
#if DEBUG
            printer = new TestSerialPrinter(portName: portName, baudRate: baudRate);
#else
            printer = new SerialPrinter(portName: portName, baudRate: baudRate);
#endif
            printer.Write(
                ByteSplicer.Combine(
                    e.FeedLines(1),
                    e.CenterAlign(),
                    e.SetStyles(PrintStyle.FontB | PrintStyle.DoubleHeight | PrintStyle.DoubleWidth | PrintStyle.Bold),
                    e.PrintLine("주문번호: " + printData.Order.orderNum),
                    e.SetStyles(PrintStyle.FontB | PrintStyle.Bold),
                    e.FeedLines(1),
                    e.PrintLine(printData.Store.storeName),
                    e.PrintLine(printData.Store.address),
                    e.PrintLine($"TEL.{printData.Store.phoneNumber}"),
                    e.SetStyles(PrintStyle.FontB | PrintStyle.Bold),
                    e.PrintLine("----------------"),
                    e.PrintLine("메뉴\t\t수량\t\t가격")
                ));

            byte[] bodyBytes = e.FeedLines(1);
            bodyBytes = printData.MenuList.Select(menu => ByteSplicer.Combine(
                e.PrintLine(menu.name + "\t\t" + menu.quantity + "\t\t" + menu.price)))
                .Aggregate(bodyBytes, (current, menuBytes) =>
                    ByteSplicer.Combine(current, menuBytes, e.FeedLines(1)));

            printer.Write(bodyBytes);

            int orderPrice = printData.Order.price;
            var supplyPrice = (int)(orderPrice / 1.1);
            var tax = orderPrice - supplyPrice;

            printer.Write(ByteSplicer.Combine(
                e.FeedLines(1),
                e.RightAlign(),
                e.SetStyles(PrintStyle.FontB | PrintStyle.DoubleHeight | PrintStyle.DoubleWidth | PrintStyle.Bold),
                e.PrintLine("합계금액 : " + orderPrice + " 원"),
                e.PrintLine("공급가액 : " + supplyPrice + " 원"),
                e.PrintLine("부가세 : " + tax + " 원"),
                e.CenterAlign(),
                e.SetStyles(PrintStyle.FontB | PrintStyle.Bold | PrintStyle.DoubleWidth),
                e.PrintLine("----------------")
            ));

            printer.Write(ByteSplicer.Combine(
                e.FeedLines(1),
                e.CenterAlign(),
                e.SetStyles(PrintStyle.FontB | PrintStyle.DoubleHeight | PrintStyle.Bold),
                e.PrintLine("신용카드 승인 정보"),
                e.SetStyles(PrintStyle.FontB | PrintStyle.Bold),
                e.PrintLine("카드명: " + printData.Card.cardCompany),
                e.PrintLine("카드번호: " + printData.Card.cardNumber[..6] + "**********"),
                e.FeedLines(1),
                e.PrintLine($"사업자:  {printData.Store.licenseNumber} {printData.Store.ownerName}"),
                e.PrintLine($"가맹점명:  {printData.Store.storeName}"),
                e.PrintLine($"가맹번호:  {printData.Card.franchiseCode}"),
                e.PrintLine("승인일시:  " + DateTime.Now),
                e.PrintLine("승인번호:  " + printData.Card.tid),
                e.PrintLine("승인금액:  " + orderPrice + "원"),
                e.FeedLines(1),
                e.PrintLine($"{printData.Store.storeName} 이용해주셔서 감사합니다.")
            ));

            printer.Write(
                ByteSplicer.Combine(e.FeedLines(3)));

            printer.Write(
                ByteSplicer.Combine(
                    e.FullCut()
                )
            );

            printer.Dispose();
        }
    }
}