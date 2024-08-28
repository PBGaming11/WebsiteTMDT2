using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebsiteTMDT.Models;
/*Fillter để lưu giá tiền khi chuyển trang thì nó không bị reset về 0*/
public class CartTotalFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.Controller as Controller;
        if (controller != null)
        {
            var cart = controller.HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            int totalAmount = cart?.Sum(c => c.Price * c.Quantity) ?? 0;
            controller.ViewBag.TotalAmount = totalAmount;
        }

        base.OnActionExecuting(context);
    }
}
