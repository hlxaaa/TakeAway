using System.ComponentModel.DataAnnotations;
using takeAwayWebApi.Attribute;

namespace takeAwayWebApi.Models.Request
{
    public class AllReq
    {
    }

    public class GetTagsReq
    {
        [IdNotZeroValidate(ErrorMessage = "区域id错误", AllowEmptyStrings = false)]
        public string areaId { get; set; }

    }

    public class GetFoodsReq
    {
        public string tagId { get; set; }

        public string userId { get; set; }

        [IdNotZeroValidate(ErrorMessage = "区域id错误", AllowEmptyStrings = false)]
        public string areaId { get; set; }

    }

    public class ToPayPageReq : UserToken
    {
        [IdNotZeroValidate(ErrorMessage = "区域id错误", AllowEmptyStrings = false)]
        public string areaId { get; set; }
    }

    public class CreateOrderReq : UserToken
    {
        [IdNotZeroValidate(ErrorMessage = "地址id错误", AllowEmptyStrings = false)]
        public string addressId { get; set; }
        [IdNotZeroValidate(ErrorMessage = "区域id错误", AllowEmptyStrings = false)]
        public string areaId { get; set; }

        public string isUseBalance { get; set; }

        public string isUseCoupon { get; set; }
        [Required]
        public string listFoods { get; set; }

        public string remarks { get; set; }

        public string payType { get; set; }
        public string payMoney { get; set; }
    }

    public class PayOrder2Req : UserToken
    {
        [IdNotZeroValidate(ErrorMessage = "订单id错误", AllowEmptyStrings = false)]
        public string orderId { get; set; }
        public string payType { get; set; }
        public string payMoney { get; set; }
        public string spbill_create_ip { get; set; }
    }

    public class GetHistoryOrderReq : UserToken
    {

        [IdNotZeroValidate(ErrorMessage = "页码错误", AllowEmptyStrings = false)]
        public string pageIndex { get; set; }
    }

    public class PlaceOrderReq : UserToken
    {
        [Required]
        public string isUseBalance { get; set; }
        [Required]
        public string isUseCoupon { get; set; }
        [Required]
        public string listFoods { get; set; }
        [Required]
        public string areaId { get; set; }
        public string remarks { get; set; }
        public string addressId { get; set; }
    }

    public class UserByOrderIdReq : UserToken
    {

        [IdNotZeroValidate(ErrorMessage = "订单id错误", AllowEmptyStrings = false)]
        public string orderId { get; set; }
    }

    public class RiderByOrderIdReq : RiderToken
    {

        [IdNotZeroValidate(ErrorMessage = "订单id错误", AllowEmptyStrings = false)]
        public string orderId { get; set; }
    }

    public class RiderByPageReq : RiderToken
    {
        [IdNotZeroValidate(ErrorMessage = "页码错误", AllowEmptyStrings = false)]
        public string pageIndex { get; set; }
    }

    public class AddCommentReq : UserToken
    {
        [IdNotZeroValidate(ErrorMessage = "订单id错误", AllowEmptyStrings = false)]
        public string orderId { get; set; }
        public string listFoodComments { get; set; }
        [StarsValidate(ErrorMessage = "骑手星级错误", AllowEmptyStrings = false)]
        public string riderStars { get; set; }
        public string riderComment { get; set; }
    }

    public class PlaceReserveOrderReq : UserToken
    {
        [Required]
        public string isUseBalance { get; set; }
        [Required]
        public string isUseCoupon { get; set; }
        public string remarks { get; set; }
   
        public string timeArea { get; set; }
        [Required]
        public string listFoods { get; set; }
        //[IdNotZeroValidate(ErrorMessage = "地址id错误", AllowEmptyStrings = false)]
        public string addressId { get; set; }

    }

    public class CreateReserveOrderReq : UserToken
    {
        public string addressId { get; set; }
        [Required]
        public string deposit { get; set; }
        [Required]
        public string listFoods { get; set; }
        [Required]
        public string isUseBalance { get; set; }
        [Required]
        public string isUseCoupon { get; set; }
        public string remarks { get; set; }
        public string payType { get; set; }
        [Required]
        public string payMoney { get; set; }
        [Required]
        public string timeArea { get; set; }
    }

    public class PayReserveOrderReq : UserToken
    {
        [IdNotZeroValidate(ErrorMessage = "订单id错误", AllowEmptyStrings = false)]
        public string orderId { get; set; }
        public string payType { get; set; }
        [Required]
        public string payMoney { get; set; }
        public string spbill_create_ip { get; set; }
    }

    public class SetTempFoodsReq : UserToken
    {
        [IdNotZeroValidate(ErrorMessage = "菜品id错误", AllowEmptyStrings = false)]
        public string foodId { get; set; }
        public string math { get; set; }
        [IdNotZeroValidate(ErrorMessage = "区域id错误", AllowEmptyStrings = false)]
        public string areaId { get; set; }
    }

    public class GetReserveFoodsReq
    {
        //[IdNotZeroValidate(ErrorMessage = "标签id错误", AllowEmptyStrings = false)]
        public string tagId { get; set; }
        [IdNotZeroValidate(ErrorMessage = "页码错误", AllowEmptyStrings = false)]
        public string pageIndex { get; set; }
    }

    public class ToReservePayPage : UserToken
    {
        [IdNotZeroValidate(ErrorMessage = "菜品id错误", AllowEmptyStrings = false)]
        public string foodId { get; set; }
        [IdNotZeroValidate(ErrorMessage = "数量错误", AllowEmptyStrings = false)]
        public string amount { get; set; }
    }

    public class GetArticleListReq
    {
        [IdNotZeroValidate(ErrorMessage = "页码错误", AllowEmptyStrings = false)]
        public string pageIndex { get; set; }
        public string userId { get; set; }
    }

    public class GetShopList : UserToken
    {
        [IdNotZeroValidate(ErrorMessage = "区域id错误", AllowEmptyStrings = false)]
        public string areaId { get; set; }
    }

    public class GetShopListReq2 : UserToken
    {
        [LatValidate]
        public string lat { get; set; }
        [LngValidate]
        public string lng { get; set; }
    }

    public class SetAreaReq : RiderToken
    {
        [IdNotZeroValidate(ErrorMessage = "区域id错误", AllowEmptyStrings = false)]
        public string areaId { get; set; }
    }

    public class SetPosition : RiderToken
    {
        [LatValidate]
        public string lat { get; set; }
        [LngValidate]
        public string lng { get; set; }
    }

    public class GiveToOtherRider : RiderToken
    {
        [Required]
        public string listFoods { set; get; }
        [IdNotZeroValidate(ErrorMessage = "目标骑手id错误", AllowEmptyStrings = false)]
        public string targetRiderId { get; set; }
    }

    public class AgreeApplyReq : RiderToken
    {
        [IdNotZeroValidate(ErrorMessage = "目标骑手id错误", AllowEmptyStrings = false)]
        public string targetRiderId { get; set; }
    }

    public class GetSendingOrderReq : UserToken
    {
        //[LatValidate]
        public string lat { get; set; }
        //[LngValidate]
        public string lng { get; set; }
    }

    public class Money {
        public string money { get; set; }
    }
}