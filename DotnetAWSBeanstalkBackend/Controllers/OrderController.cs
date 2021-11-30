using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using ArumModels.Models;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Validations.Rules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DotnetAWSBeanstalkBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        // POST api/Order
        [HttpPost]
        public ReturnValues PostOrder([FromBody] OrderParams order, [FromHeader] int test)
        {
            var shop = Shop.GetById(order.ShopId);
            if (!shop.IsOpened)
            {
                return new ErrorReturnValues {Error = "영업 종료되었습니다", Status = "error"};
            }

            User user;
            if (Request.Headers.TryGetValue("kakao_token", out var token))
            {
                // 카카오 토큰 검증 성공여부 체크
                if (UserController.CheckKakaoTokenValid(token.ToString()).Item1)
                {
                    var kakaoId = UserController.CheckKakaoTokenValid(token.ToString()).Item2["id"];
                    user = ArumModels.Models.User.GetUserByKakaoId(int.Parse(kakaoId));
                }
                else
                {
                    return new ErrorReturnValues { Error = "카카오 토큰 검증에 실패했습니다", Status = "error" };
                }
            }
            else if (Request.Headers.TryGetValue("token", out token))
            {
                user = ArumModels.Models.User.GetUserByToken(token);
                if (user == null)
                {
                    return new ErrorReturnValues { Error = "토큰 정보가 올바르지 않습니다", Status = "error" };
                }
            }
            else
            {
                return new ErrorReturnValues { Error = "토큰 정보가 없습니다", Status = "error" };
            }

            // 쿠폰, 스탬프 사용 여부 체크
            if (order.IsUsingStamp)
            {
                if (!ArumModels.Models.User.IsStampsValid(user.Id, order.ShopId))
                {
                    return new ErrorReturnValues { Error = "스탬프 수량이 부족합니다", Status = "error" };
                }
            } else if (order.CouponCode is { Length: > 0 })
            {
                // 사용자가 이미 쿠폰 사용했으면
                // if (user.IsAbleToUseCoupon == false)
                // {
                //     return new ErrorReturnValues { Error = "이미 쿠폰을 사용하셨습니다", Status = "error" };
                // }

                if (!ArumModels.Models.User.CheckCouponValid(order.CouponCode))
                {
                    return new ErrorReturnValues { Error = "존재하지 않는 쿠폰번호이거나 이미 소진된 쿠폰입니다.", Status = "error" };
                }
            }

            // 금액상한설정
            if (order.TotalPrice > 70000)
            {
                return new ErrorReturnValues { Error = "7만원 이상 결제는 불가능합니다.", Status = "error" };
            }

            // API 카드 결제
            // http://pay.itnj.co.kr:19000/payment/v1.0/request/card
            var httpClient = Program.SetHttpClientItnjSecretKey(shop.ItnjSecretKey);
            var purchaseResult = httpClient.PostAsync("http://pay.itnj.co.kr:19000/payment/v1.0/request/card",
                new StringContent($@"
                {{
                    ""goodsCnt"": 1,
                    ""goodsName"": ""{order.MenusName}"",
                    ""amt"": {order.TotalPrice},
                    ""buyerName"": ""{order.BuyerName}"",
                    ""cardNum"": ""{order.CardNumber}"",
                    ""cardExpire"": ""{order.CardExpire}"",
                    ""buyerEmail"": ""{user.Email}"",
                    ""buyerTel"": ""{user.PhoneNumber}""
                }}
                ",Encoding.UTF8,"application/json")).Result;
            
            var purchaseResultJson = JObject.Parse(purchaseResult.Content.ReadAsStringAsync().Result);

            if (purchaseResultJson["success"].ToObject<bool>() != true)
            {
                return new ErrorReturnValues { Error = purchaseResultJson["error"].ToString(),
                    Status = "error" };
            }

            try
            {
                order.CardName = purchaseResultJson["data"]["appCardName"].ToString();
                order.CardNumber = order.CardNumber[..6];
                order.Message = purchaseResultJson["data"]["errorMSG"].ToString();
                order.Tid = purchaseResultJson["data"]["tid"].ToString();
                order.Type = purchaseResultJson["data"]["type"].ToString();
                order.PaymentCode = purchaseResultJson["data"]["paymentCode"].ToString();
                order.Moid = purchaseResultJson["data"]["moid"].ToString();

                // 테스트 결제 취소
                if (test == 1)
                {
                    var result = httpClient.DeleteAsync($"http://pay.itnj.co.kr:19000/payment/v1.0/cancel/card/{order.Moid}").Result;
                    Debug.WriteLine(result.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("결제 카드정보 관련 입력 오류: 리턴값에 해당정보 없음");
                Debug.WriteLine(exception);

                order.CardName = "DEBUG카드";
                order.CardNumber = "DEBUG111122223333";
                order.Message = "DEBUG1234";
                order.Tid = "DEBUG123455";
            }

            // Order 내용 DB 저장
            order.User = user;
            Order.SaveOrderInformation(order);

            // 스탬프 혹은 쿠폰 차감
            if (order.IsUsingStamp)
            {
                ArumModels.Models.User.RedeemStamps(user.Id, order.ShopId);
            }
            else if (order.CouponCode is { Length: > 0 })
            {
                ArumModels.Models.User.LockUsingCoupon(user.Id);
                ArumModels.Models.User.UseCoupon(order.CouponCode);
            }
            else
            {
                ArumModels.Models.User.IncreaseStamp(user.Id, order.ShopId);
            }

            //결과 리턴
            return new OrderReturnValues()
            {
                Status = "succeed",
                OrderId = order.Id,
                Message = "정상적으로 주문되었습니다",
                PurchaseInfo = purchaseResultJson.ToString()
            };
        }

        [HttpPost("CardNumberTest")]
        public Dictionary<string, string> CardNumberTest(CardTestParams @params)
        {
            var secretKey = Shop.GetById(@params.ShopId).ItnjSecretKey;
            var httpClient = Program.SetHttpClientItnjSecretKey(secretKey);
            var checkingQuery = httpClient.PostAsync("http://pay.itnj.co.kr:19000/payment/v1.0/valid/card",
                new StringContent($@"
{{
    ""cardNum"": ""{@params.CardNumber}"",
    ""cardExpire"": ""{@params.CardExpire}""
}}
", Encoding.UTF8, "application/json")).Result;

            var checkingQueryJson = JObject.Parse(checkingQuery.Content.ReadAsStringAsync().Result);
            return checkingQueryJson.ToObject<Dictionary<string, string>>();
        }

        [HttpGet("{orderId:int}")]
        public Order GetOrder(int orderId)
        {
            return Order.GetOrder(orderId);
        }

        [HttpGet("Recent/{userId:int}/{N:int}")]
        public ICollection<Order> GetRecentOrders(int userId, int N)
        {

            return Order.GetRecentOrders(userId, N);
        }

        [HttpGet("{orderId:int}/Status")]
        public Dictionary<string, int> GetOrderStatus(int orderId)
        {
            var json = new Dictionary<string, int> {["status"] = Order.GetOrder(orderId).Status};
            return json;
        }

        [HttpGet("User/{userId:int}")]
        [HttpGet("User/{userId:int}/{page:int}")]
        public IEnumerable<Order> GetOrderList(int userId, int page)
        {
            if (page == 0)
            {
                page = 1;
            }
            return Order.GetOrdersByUserId(userId, page);
        }


        [HttpPost("UpdatePurchaseStatus")]
        public ReturnValues UpdatePurchaseStatus(OrderResultJson orderResultJson)
        {
            var moid = orderResultJson.Moid;
            var type = orderResultJson.Type;

            var result = Order.ChangeOrderTypeByMoid(moid, type);
            return result ? new ReturnValues() { Message = "주문 타입이 반영되었습니다",Status = "OK" } : new ErrorReturnValues() { Error = "주문 타입 변경에 실패했습니다" , Status = "error" };
        }

        [HttpDelete("Delete/{orderId:int}")]
        public ReturnValues DeleteOrder([FromHeader] string token, int orderId)
        {
            var user = ArumModels.Models.User.GetUserByToken(token);
            var order = Order.GetOrder(orderId);

            if (order.User.Id != user.Id)
            {
                return new ErrorReturnValues()
                {
                    Status = "error",
                    Error = "해당 주문을 요청한 유저가 아닙니다"
                };
            }

            Order.Delete(orderId);
            return new ReturnValues()
            {
                Status = "succeed",
                Message = "주문 삭제처리 완료하였습니다"
            };
        }
    }

    public class OrderParams : Order
    {
        public string CardExpire { get; set; }
        public bool IsUsingStamp { get; set; }
        public string CouponCode { get; set; }
        public string BuyerName { get; set; }
    }

    public class CardTestParams
    {
        public string CardNumber { get; set; }
        public string CardExpire { get; set; }
        public int ShopId { get; set; }
    }

    // { "status", "succeed" },
    // { "orderId", order.Id.ToString() },
    // { "message", "정상적으로 주문되었습니다"},
    // { "purchase_info", purchaseResultJson.ToString()}
    public class OrderReturnValues : ReturnValues
    {
        public int OrderId { get; set; }
        public string PurchaseInfo { get; set; }
    }

    public class ErrorReturnValues : ReturnValues
    {
        public string Error { get; set; }
    }

    public class ReturnValues
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class OrderResultJson
    {
        public string Type { get; set; }
        public string Moid { get; set; }
        public string PaymentCode { get; set; }
    }
}
