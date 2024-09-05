using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebsiteTMDT.Models;

public class CartItemCountFilter : ActionFilterAttribute
{
    /*Fillter để lưu số lượng sản phẩm trong giỏ hàng khi chuyển trang thì nó không bị reset về 0*/
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.Controller as Controller;
        if (controller != null)
        {
            var cart = controller.HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            int cartItemCount = cart?.Sum(c => c.Quantity) ?? 0;
            controller.ViewBag.CartItemCount = cartItemCount;
        }

        base.OnActionExecuting(context);
    }
}
