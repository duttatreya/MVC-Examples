using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingCartMVCProject.Models;

namespace ShoppingCartMVCProject.Controllers
{
    public class GatewayController : Controller
    {
        MVCDBEntities db = new MVCDBEntities();
        List<ShoppingCartList> lst = new List<ShoppingCartList>();
        // GET: Gateway
        public ActionResult CardInfo()
        {
            
            decimal finalamt = 0.0M;
            if (Session["templst"] == null) { }
            else
            {
                string data = Session["templst"].ToString();
                string[] rows = data.Split(':');
                foreach (var r in rows)
                {
                    if (!string.IsNullOrEmpty(r))
                    {
                        string[] cols = r.Split(',');
                        ShoppingCartList sc = new ShoppingCartList();
                        sc.PID = Convert.ToInt32(cols[0]);
                        sc.Qty = Convert.ToInt32(cols[1]);
                        sc.TotAmt = Convert.ToDecimal(cols[2]);
                        lst.Add(sc);
                        finalamt += sc.TotAmt;

                    }
                }
            }
            Session["finalamt"] = finalamt;
            Session["lst"] = lst;
            return View(lst);
        }

        [HttpPost]
        public ActionResult Cardinfo(decimal finalamt = 0.0M, long cardno = 0, int cvv = 0, string expdate = "")
        {
            finalamt = Convert.ToDecimal(Session["finalamt"]);
            cardno = Convert.ToInt64(Request.Form["txtcardno"]);
            cvv = Convert.ToInt32(Request.Form["txtcvv"]);
            expdate = Request.Form["ddlmonth"].ToString() + "/" + Request.Form["ddlyear"].ToString();
            try {
                localhost.CardInfoService svc = new localhost.CardInfoService();
                var res = svc.ProcessPayment(cardno, cvv, expdate, finalamt);
                if (string.IsNullOrEmpty(res))
                {
                    ModelState.AddModelError("", "Error Adding Service");
                }
                else
                {
                    ModelState.AddModelError("", res);
                    OrderInfo info = new OrderInfo();
                    info.OrderDate = DateTime.Now;
                    info.PersonID = Convert.ToInt32(Session["pid"]);
                    info.Status = "Payment Done";
                    info.TotAmount = Convert.ToDecimal(Session["finalamt"]);
                    db.OrderInfoes.Add(info);
                    var r = db.SaveChanges();
                    if(r > 0)
                    {
                       
                        int orderid = db.OrderInfoes.Max(x => x.OrderId);
                        lst = (List<ShoppingCartList>)Session["lst"];
                        foreach(var d in lst)
                        {
                            OrderTransaction ot = new OrderTransaction();
                            ot.OrderID = orderid;
                            ot.pid = d.PID;
                            ot.qty = d.Qty;
                            db.OrderTransactions.Add(ot);
                        }
                        var op = db.SaveChanges();
                        if(op > 0)
                        {
                            ModelState.AddModelError("", "Order Placed Successfully");
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(lst);
        }
    }
}