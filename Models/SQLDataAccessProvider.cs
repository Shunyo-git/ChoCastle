using System;
using System.Configuration;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;



namespace ChoCastle.Models
{
    public class SQLDataAccessProvider
    {

        private static DateTime SqlDateTime_MinValue = Convert.ToDateTime("1900/1/1");

        /*** PROPERTIES ***/
        /// <summary>
        /// 取得連線字串
        /// </summary>
        /// <returns>連線字串</returns>   
        protected string ConnectionString
        {
            get
            {
                if (ConfigurationManager.ConnectionStrings["DefaultConnection"] == null)
                    throw (new NullReferenceException("ConnectionString configuration is missing from you web.config. It should contain  <connectionStrings> <add key=\"DefaultConnectionString\" value=\"Server=(local);Integrated Security=True;Database=(DatabaseName)\" </connectionStrings>"));

                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                if (System.Web.HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToLower() == "www.SMIT14.com.tw")
                {
                    connectionString = connectionString.ToLower().Replace("|DataDirectory|", System.Web.HttpContext.Current.Server.MapPath("/") + "DB\\");
                }
                if (String.IsNullOrEmpty(connectionString))
                    throw (new NullReferenceException("ConnectionString configuration is missing from you web.config. It should contain  <connectionStrings> <add key=\"DefaultConnectionString\" value=\"Server=(local);Integrated Security=True;Database=(DatabaseName)\" </connectionStrings>"));
                else
                    return (connectionString);
            }
        }

        /*** DELEGATE ***/
        private delegate void TGenerateListFromReader<T>(SqlDataReader returnData, ref List<T> tempList);

        /*****************************  BASE CLASS IMPLEMENTATION *****************************/

        #region ShoppingCart
        private const string SP_ShoppingCart_GetCartIdByMemberID= "SP_ShoppingCart_GetCartIdByMemberID";
        private const string SP_ShoppingCart_RemovePreviousCartByMember = "SP_ShoppingCart_RemovePreviousCartByMember";
        private const string SP_ShoppingDetail_GetShoppingDetailByCartID = "SP_ShoppingDetail_GetShoppingDetailByCartID";
        private const string SP_ShoppingDetail_GetShoppingDetailByCartProduct = "SP_ShoppingDetail_GetShoppingDetailByCartProduct";
        private const string SP_ShoppingDetail_UpdateItem = "SP_ShoppingDetail_UpdateItem";
        private const string SP_ShoppingDetail_RemoveItem = "SP_ShoppingDetail_RemoveItem";

        public int GetCartID(int MemberID)
        {

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);

            AddParamToSQLCmd(sqlCmd, "@MemberID", SqlDbType.Int, 0, ParameterDirection.Input, MemberID);
            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_ShoppingCart_GetCartIdByMemberID);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue);

        }
        public bool RemovePreviousCart(int MemberID,int CartID)
        {

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);

            AddParamToSQLCmd(sqlCmd, "@MemberID", SqlDbType.Int, 0, ParameterDirection.Input, MemberID);
            AddParamToSQLCmd(sqlCmd, "@CartID", SqlDbType.Int, 0, ParameterDirection.Input, CartID);
            
            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_ShoppingCart_RemovePreviousCartByMember);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);

        }
        public List<ShoppingDetail> GetShoppingDetailsByCart(int CartID)
        {
            //if (CartID <= 0)
                //throw (new ArgumentOutOfRangeException("CartID"));

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@CartID", SqlDbType.Int, 0, ParameterDirection.Input, CartID);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_ShoppingDetail_GetShoppingDetailByCartID);

            List<ShoppingDetail> dataList = new List<ShoppingDetail>();
             
            TExecuteReaderCmd<ShoppingDetail>(sqlCmd, ShoppingDetail_TGenerateListFromReader<ShoppingDetail>, ref dataList);

            return dataList;
        }
        public ShoppingDetail GetCartShoppingDetailByProductID(int ProductID, int CartID)
        {


            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ProductID", SqlDbType.Int, 0, ParameterDirection.Input, ProductID);
            AddParamToSQLCmd(sqlCmd, "@CartID", SqlDbType.Int, 0, ParameterDirection.Input, CartID);


            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_ShoppingDetail_GetShoppingDetailByCartProduct);

            List<ShoppingDetail> dataList = new List<ShoppingDetail>();

            TExecuteReaderCmd<ShoppingDetail>(sqlCmd, ShoppingDetail_TGenerateListFromReader<ShoppingDetail>, ref dataList);

            if (dataList.Count > 0)
                return dataList[0];

            else
                return null;
        }
        public bool UpdateShoppingDetail(int CarID, int ProductID, string ProductName, int UnitPrice, int OrderQuantity, int Subtotal, System.DateTime AddedDate)
        {

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);

            AddParamToSQLCmd(sqlCmd, "@CarID", SqlDbType.Int, 0, ParameterDirection.Input, CarID);

            AddParamToSQLCmd(sqlCmd, "@ProductID", SqlDbType.Int, 0, ParameterDirection.Input, ProductID);
            AddParamToSQLCmd(sqlCmd, "@ProductName", SqlDbType.NVarChar, 100, ParameterDirection.Input, ProductName);
            AddParamToSQLCmd(sqlCmd, "@UnitPrice", SqlDbType.Int, 0, ParameterDirection.Input, UnitPrice);
            AddParamToSQLCmd(sqlCmd, "@OrderQuantity", SqlDbType.Int, 0, ParameterDirection.Input, OrderQuantity);
            AddParamToSQLCmd(sqlCmd, "@Subtotal", SqlDbType.Int, 0, ParameterDirection.Input, Subtotal);
            AddParamToSQLCmd(sqlCmd, "@AddedDate", SqlDbType.DateTime, 0, ParameterDirection.Input, AddedDate);
            
            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_ShoppingDetail_UpdateItem);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);

        }

        public bool RemoveShoppingDetail(int CartID, int ProductID)
        {

            if (CartID <= 0)
                throw (new ArgumentOutOfRangeException("CartID"));
            if (ProductID <= 0)
                throw (new ArgumentOutOfRangeException("ProductID"));

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);
            AddParamToSQLCmd(sqlCmd, "@CartID", SqlDbType.Int, 0, ParameterDirection.Input, CartID);
            AddParamToSQLCmd(sqlCmd, "@ProductID", SqlDbType.Int, 0, ParameterDirection.Input, ProductID);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_ShoppingDetail_RemoveItem);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);

        }
        #endregion

        #region Product
        private const string SP_Product_GetProductByID = "SP_Product_GetProductByID";
        private const string SP_Product_ListProduct = "SP_Product_ListProduct";
        private const string SP_Product_ListProductByApproveType = "SP_Product_ListProductByApproveType";
        private const string SP_Product_ListProductByCategory = "SP_Product_ListProductByCategory";

        private const string SP_Product_UpdateProduct = "SP_Product_UpdateProduct";
        private const string SP_Product_AddProduct = "SP_Product_AddProduct";
        private const string SP_Product_RemoveProduct = "SP_Product_RemoveProduct";

        private const string SP_Product_AddProductColor = "SP_Product_AddProductColor";
        private const string SP_Product_RemoveProductColor = "SP_Product_RemoveProductColor";
        private const string SP_Product_SetProductStock = "SP_Product_SetProductStock";
        private const string SP_Product_GetProductStock = "SP_Product_GetProductStock";
        private const string SP_Product_ResetProductStock = "SP_Product_ResetProductStock";

        private const string SP_Product_AddProductCapability = "SP_Product_AddProductCapability";
        private const string SP_Product_RemoveProductCapability = "SP_Product_RemoveProductCapability";

        public Product GetProduct(int ID)
        {
            if (ID <= 0)
                throw (new ArgumentOutOfRangeException("ID"));

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ID", SqlDbType.Int, 0, ParameterDirection.Input, ID);


            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Product_GetProductByID);

            List<Product> dataList = new List<Product>();

            TExecuteReaderCmd<Product>(sqlCmd, Product_TGenerateListFromReader<Product>, ref dataList);

            if (dataList.Count > 0)
                return dataList[0];

            else
                return null;
        }
        public List<Product> GetProduct()
        {

            SqlCommand sqlCmd = new SqlCommand();


            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Product_ListProduct);

            List<Product> dataList = new List<Product>();

            TExecuteReaderCmd<Product>(sqlCmd, Product_TGenerateListFromReader<Product>, ref dataList);
            return dataList;

        }
        
        public bool UpdateProduct(int ID, int CategoryID, string Name, string SKU, string Content, bool IsApproved, bool IsNewArrival, int SortID, int ListPrice, int SellingPrice, int SizeCollectionID)
        {

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);

            AddParamToSQLCmd(sqlCmd, "@ID", SqlDbType.Int, 0, ParameterDirection.Input, ID);

            AddParamToSQLCmd(sqlCmd, "@CategoryID", SqlDbType.Int, 0, ParameterDirection.Input, CategoryID);
            AddParamToSQLCmd(sqlCmd, "@Name", SqlDbType.NVarChar, 100, ParameterDirection.Input, Name);
            AddParamToSQLCmd(sqlCmd, "@SKU", SqlDbType.NVarChar, 50, ParameterDirection.Input, SKU);
            AddParamToSQLCmd(sqlCmd, "@Content", SqlDbType.NText, 0, ParameterDirection.Input, Content);
            AddParamToSQLCmd(sqlCmd, "@IsApproved", SqlDbType.Bit, 1, ParameterDirection.Input, IsApproved);
            AddParamToSQLCmd(sqlCmd, "@IsNewArrival", SqlDbType.Bit, 1, ParameterDirection.Input, IsNewArrival);
            AddParamToSQLCmd(sqlCmd, "@SortID", SqlDbType.Int, 0, ParameterDirection.Input, SortID);
            AddParamToSQLCmd(sqlCmd, "@ListPrice", SqlDbType.Int, 0, ParameterDirection.Input, ListPrice);
            AddParamToSQLCmd(sqlCmd, "@SellingPrice", SqlDbType.Int, 0, ParameterDirection.Input, SellingPrice);
            AddParamToSQLCmd(sqlCmd, "@SizeCollectionID", SqlDbType.Int, 0, ParameterDirection.Input, SizeCollectionID);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Product_UpdateProduct);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);

        }
        public int AddProduct(int CategoryID, string Name, string SKU, string Content, bool IsApproved, bool IsNewArrival, int SortID, int ListPrice, int SellingPrice, int SizeCollectionID)
        {

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);

            AddParamToSQLCmd(sqlCmd, "@CategoryID", SqlDbType.Int, 0, ParameterDirection.Input, CategoryID);
            AddParamToSQLCmd(sqlCmd, "@Name", SqlDbType.NVarChar, 100, ParameterDirection.Input, Name);
            AddParamToSQLCmd(sqlCmd, "@SKU", SqlDbType.NVarChar, 50, ParameterDirection.Input, SKU);
            AddParamToSQLCmd(sqlCmd, "@Content", SqlDbType.NText, 0, ParameterDirection.Input, Content);
            AddParamToSQLCmd(sqlCmd, "@IsApproved", SqlDbType.Bit, 1, ParameterDirection.Input, IsApproved);
            AddParamToSQLCmd(sqlCmd, "@IsNewArrival", SqlDbType.Bit, 1, ParameterDirection.Input, IsNewArrival);
            AddParamToSQLCmd(sqlCmd, "@SortID", SqlDbType.Int, 0, ParameterDirection.Input, SortID);
            AddParamToSQLCmd(sqlCmd, "@ListPrice", SqlDbType.Int, 0, ParameterDirection.Input, ListPrice);
            AddParamToSQLCmd(sqlCmd, "@SellingPrice", SqlDbType.Int, 0, ParameterDirection.Input, SellingPrice);
            AddParamToSQLCmd(sqlCmd, "@SizeCollectionID", SqlDbType.Int, 0, ParameterDirection.Input, SizeCollectionID);
            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Product_AddProduct);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue);

        }
        public bool RemoveProduct(int ID)
        {

            if (ID <= 0)
                throw (new ArgumentOutOfRangeException("ID"));

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);
            AddParamToSQLCmd(sqlCmd, "@ID", SqlDbType.Int, 0, ParameterDirection.Input, ID);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Product_RemoveProduct);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);

        }

        public bool AddProductColor(int ProductID, int ColorID)
        {

            if (ProductID <= 0)
                throw (new ArgumentOutOfRangeException("ProductID"));

            if (ColorID <= 0)
                throw (new ArgumentOutOfRangeException("ColorID"));


            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);

            AddParamToSQLCmd(sqlCmd, "@ProductID", SqlDbType.Int, 0, ParameterDirection.Input, ProductID);
            AddParamToSQLCmd(sqlCmd, "@ColorID", SqlDbType.Int, 0, ParameterDirection.Input, ColorID);


            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Product_AddProductColor);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);
        }
        public bool RemoveProductColor(int ProductID, int ColorID)
        {

            if (ProductID <= 0)
                throw (new ArgumentOutOfRangeException("ProductID"));
            if (ColorID <= 0)
                throw (new ArgumentOutOfRangeException("ColorID"));

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);

            AddParamToSQLCmd(sqlCmd, "@ProductID", SqlDbType.Int, 0, ParameterDirection.Input, ProductID);
            AddParamToSQLCmd(sqlCmd, "@ColorID", SqlDbType.Int, 0, ParameterDirection.Input, ColorID);


            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Product_RemoveProductColor);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);


        }

        public bool SetProductStock(int ProductID, int ColorID, string Size, bool IsInStock)
        {

            if (ProductID <= 0)
                throw (new ArgumentOutOfRangeException("ProductID"));
            if (ColorID <= 0)
                throw (new ArgumentOutOfRangeException("ColorID"));

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);

            AddParamToSQLCmd(sqlCmd, "@ProductID", SqlDbType.Int, 0, ParameterDirection.Input, ProductID);
            AddParamToSQLCmd(sqlCmd, "@ColorID", SqlDbType.Int, 0, ParameterDirection.Input, ColorID);
            AddParamToSQLCmd(sqlCmd, "@Size", SqlDbType.NVarChar, 10, ParameterDirection.Input, Size);
            AddParamToSQLCmd(sqlCmd, "@IsInStock", SqlDbType.Bit, 10, ParameterDirection.Input, IsInStock);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Product_SetProductStock);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);


        }
        public bool GetProductStock(int ProductID, int ColorID, string SizeValue)
        {

            if (ProductID <= 0)
                throw (new ArgumentOutOfRangeException("ProductID"));
            if (ColorID <= 0)
                throw (new ArgumentOutOfRangeException("ColorID"));

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Bit, 0, ParameterDirection.ReturnValue, null);

            AddParamToSQLCmd(sqlCmd, "@ProductID", SqlDbType.Int, 0, ParameterDirection.Input, ProductID);
            AddParamToSQLCmd(sqlCmd, "@ColorID", SqlDbType.Int, 0, ParameterDirection.Input, ColorID);
            AddParamToSQLCmd(sqlCmd, "@SizeValue", SqlDbType.NVarChar, 10, ParameterDirection.Input, SizeValue);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Product_GetProductStock);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? false : true);



        }
        public bool ResetProductStock(int ProductID)
        {

            if (ProductID <= 0)
                throw (new ArgumentOutOfRangeException("ProductID"));


            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);

            AddParamToSQLCmd(sqlCmd, "@ProductID", SqlDbType.Int, 0, ParameterDirection.Input, ProductID);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Product_ResetProductStock);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);


        }



        public bool AddProductCapability(int ProductID, int CapabilityID)
        {

            if (ProductID <= 0)
                throw (new ArgumentOutOfRangeException("ProductID"));

            if (CapabilityID <= 0)
                throw (new ArgumentOutOfRangeException("CapabilityID"));


            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);

            AddParamToSQLCmd(sqlCmd, "@ProductID", SqlDbType.Int, 0, ParameterDirection.Input, ProductID);
            AddParamToSQLCmd(sqlCmd, "@CapabilityID", SqlDbType.Int, 0, ParameterDirection.Input, CapabilityID);


            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Product_AddProductCapability);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);
        }
        public bool RemoveProductCapability(int ProductID, int CapabilityID)
        {

            if (ProductID <= 0)
                throw (new ArgumentOutOfRangeException("ProductID"));
            if (CapabilityID <= 0)
                throw (new ArgumentOutOfRangeException("CapabilityID"));

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);

            AddParamToSQLCmd(sqlCmd, "@ProductID", SqlDbType.Int, 0, ParameterDirection.Input, ProductID);
            AddParamToSQLCmd(sqlCmd, "@CapabilityID", SqlDbType.Int, 0, ParameterDirection.Input, CapabilityID);


            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Product_RemoveProductCapability);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);


        }
        #endregion

        #region Order
        private const string SP_Order_GetOrderByOrderID = "SP_Order_GetOrderByOrderID";
        private const string SP_Order_GetOrderBySessionID = "SP_Order_GetOrderBySessionID";
        private const string SP_Order_ListOrder = "SP_Order_ListOrder";
        private const string SP_Order_ListOrderByStatus = "SP_Order_ListOrderByStatus";

        private const string SP_Order_UpdateOrder = "SP_Order_UpdateOrder";
        private const string SP_Order_AddOrder = "SP_Order_AddOrder";
        private const string SP_Order_RemoveOrder = "SP_Order_RemoveOrder";

        private const string SP_Order_GetTodayLastOrderNo = "SP_Order_GetTodayLastOrderNo";
        private const string SP_Order_UpdateOrderStatus = "SP_Order_UpdateOrderStatus";

        private const string SP_Order_FindOrder = "SP_Order_FindOrder";


        public Order GetOrderByOrderID(int OrderID)
        {
            if (OrderID <= 0)
                throw (new ArgumentOutOfRangeException("OrderID"));

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@OrderID", SqlDbType.Int, 0, ParameterDirection.Input, OrderID);


            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Order_GetOrderByOrderID);

            List<Order> dataList = new List<Order>();

            TExecuteReaderCmd<Order>(sqlCmd, Order_TGenerateListFromReader<Order>, ref dataList);

            if (dataList.Count > 0)
                return dataList[0];

            else
                return null;
        }
        public Order GetOrderBySessionID(string SessionID)
        {


            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@SessionID", SqlDbType.VarChar, 255, ParameterDirection.Input, SessionID);


            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Order_GetOrderBySessionID);

            List<Order> dataList = new List<Order>();

            TExecuteReaderCmd<Order>(sqlCmd, Order_TGenerateListFromReader<Order>, ref dataList);

            if (dataList.Count > 0)
                return dataList[0];

            else
                return null;
        }

        public List<Order> GetOrder()
        {
            SqlCommand sqlCmd = new SqlCommand();


            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Order_ListOrder);

            List<Order> dataList = new List<Order>();

            TExecuteReaderCmd<Order>(sqlCmd, Order_TGenerateListFromReader<Order>, ref dataList);

            return dataList;
        }
        public enum OrderStatus { InCart, NewOrder, ErrorOrder, OutOfStock, Confirmed, Delivered, Completed, CancelOrder }
        public List<Order> GetOrderByStatus(OrderStatus Status)
        {
            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@Status", SqlDbType.Int, 0, ParameterDirection.Input, (int)Status);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Order_ListOrderByStatus);

            List<Order> dataList = new List<Order>();

            TExecuteReaderCmd<Order>(sqlCmd, Order_TGenerateListFromReader<Order>, ref dataList);

            return dataList;
        }

        public bool UpdateOrder(int OrderID, string SessionID, string OrderNo, string OrderName, string OrderTel, string OrderMobile, string OrderZipcode,
                                string OrderAddress, string OrderEmail, int ShippingCost, DateTime OrderDate, DateTime DeliverDate, DateTime PaidDate, OrderStatus Status,
                                string TypeOfPayment, bool IsPaid, bool IsShipped, string OrderMemo, string Description)
        {

            //if (OrderDate==DateTime.MinValue) { }
            //if (DeliverDate == DateTime.MinValue) { }
            //if (PaidDate == DateTime.MinValue) { }

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);

            AddParamToSQLCmd(sqlCmd, "@OrderID", SqlDbType.Int, 0, ParameterDirection.Input, OrderID);


            AddParamToSQLCmd(sqlCmd, "@SessionID", SqlDbType.NVarChar, 255, ParameterDirection.Input, SessionID);
            AddParamToSQLCmd(sqlCmd, "@OrderNo", SqlDbType.NVarChar, 50, ParameterDirection.Input, OrderNo);
            AddParamToSQLCmd(sqlCmd, "@OrderName", SqlDbType.NVarChar, 50, ParameterDirection.Input, OrderName);
            AddParamToSQLCmd(sqlCmd, "@OrderTel", SqlDbType.NVarChar, 50, ParameterDirection.Input, OrderTel);
            AddParamToSQLCmd(sqlCmd, "@OrderMobile", SqlDbType.NVarChar, 50, ParameterDirection.Input, OrderMobile);
            AddParamToSQLCmd(sqlCmd, "@OrderZipcode", SqlDbType.NVarChar, 10, ParameterDirection.Input, OrderZipcode);
            AddParamToSQLCmd(sqlCmd, "@OrderAddress", SqlDbType.NVarChar, 255, ParameterDirection.Input, OrderAddress);
            AddParamToSQLCmd(sqlCmd, "@OrderEmail", SqlDbType.NVarChar, 255, ParameterDirection.Input, OrderEmail);

            AddParamToSQLCmd(sqlCmd, "@OrderDate", SqlDbType.DateTime, 0, ParameterDirection.Input, OrderDate);
            AddParamToSQLCmd(sqlCmd, "@DeliverDate", SqlDbType.DateTime, 0, ParameterDirection.Input, DeliverDate);
            AddParamToSQLCmd(sqlCmd, "@PaidDate", SqlDbType.DateTime, 0, ParameterDirection.Input, PaidDate);

            AddParamToSQLCmd(sqlCmd, "@Status", SqlDbType.Int, 0, ParameterDirection.Input, (int)Status);
            AddParamToSQLCmd(sqlCmd, "@TypeOfPayment", SqlDbType.NVarChar, 50, ParameterDirection.Input, TypeOfPayment);

            AddParamToSQLCmd(sqlCmd, "@IsPaid", SqlDbType.Bit, 1, ParameterDirection.Input, IsPaid);
            AddParamToSQLCmd(sqlCmd, "@IsShipped", SqlDbType.Bit, 1, ParameterDirection.Input, IsShipped);

            AddParamToSQLCmd(sqlCmd, "@OrderMemo", SqlDbType.NVarChar, 0, ParameterDirection.Input, OrderMemo);
            AddParamToSQLCmd(sqlCmd, "@Description", SqlDbType.NVarChar, 0, ParameterDirection.Input, Description);
            AddParamToSQLCmd(sqlCmd, "@ShippingCost", SqlDbType.Int, 0, ParameterDirection.Input, ShippingCost);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Order_UpdateOrder);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);

        }
        public int AddOrder(int CartID)
        {

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);


            AddParamToSQLCmd(sqlCmd, "@CartID", SqlDbType.Int, 0, ParameterDirection.Input, CartID);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Order_AddOrder);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue);

        }
        public bool RemoveOrder(int OrderID)
        {

            if (OrderID <= 0)
                throw (new ArgumentOutOfRangeException("OrderID"));

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);
            AddParamToSQLCmd(sqlCmd, "@OrderID", SqlDbType.Int, 0, ParameterDirection.Input, OrderID);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Order_RemoveOrder);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);

        }

        public bool UpdateOrderStatus(int OrderID, OrderStatus Status)
        {

            if (OrderID <= 0)
                throw (new ArgumentOutOfRangeException("OrderID"));

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);
            AddParamToSQLCmd(sqlCmd, "@OrderID", SqlDbType.Int, 0, ParameterDirection.Input, OrderID);
            AddParamToSQLCmd(sqlCmd, "@Status", SqlDbType.Int, 0, ParameterDirection.Input, (int)Status);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Order_UpdateOrderStatus);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);

        }

        public string GetTodayLastOrderNo()
        {


            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.NVarChar, 50, ParameterDirection.Output, null);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Order_GetTodayLastOrderNo);
            ExecuteScalarCmd(sqlCmd);

            if (sqlCmd.Parameters["@ReturnValue"].Value != null)
            {
                return Convert.ToString(sqlCmd.Parameters["@ReturnValue"].Value);
            }
            else
            {
                return string.Empty;
            }


        }

        public List<Order> FindOrder(string Keyword)
        {
            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@Keyword", SqlDbType.NVarChar, 50, ParameterDirection.Input, Keyword);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_Order_FindOrder);

            List<Order> dataList = new List<Order>();

            TExecuteReaderCmd<Order>(sqlCmd, Order_TGenerateListFromReader<Order>, ref dataList);

            return dataList;
        }

        #endregion

        #region OrderDetail
        private const string SP_OrderItem_GetOrderItemID = "SP_OrderItem_GetOrderItemID";
        private const string SP_OrderItem_ListOrderItemByOrderID = "SP_OrderItem_ListOrderItemByOrderID";
        private const string SP_OrderItem_UpdateOrderItem = "SP_OrderItem_UpdateItem";
        private const string SP_OrderItem_AddOrderItem = "SP_OrderItem_AddItem";
        private const string SP_OrderItem_RemoveOrderItem = "SP_OrderItem_RemoveItem";
        private const string SP_OrderItem_UpdateQuantity = "SP_OrderItem_UpdateQuantity";


        public OrderDetail GetOrderItem(int ItemID)
        {


            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ItemID", SqlDbType.Int, 0, ParameterDirection.Input, ItemID);


            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_OrderItem_GetOrderItemID);

            List<OrderDetail> dataList = new List<OrderDetail>();

            TExecuteReaderCmd<OrderDetail>(sqlCmd, OrderItem_TGenerateListFromReader<OrderDetail>, ref dataList);

            if (dataList.Count > 0)
                return dataList[0];

            else
                return null;
        }
        public List<OrderDetail> GetOrderItemByOrderID(int OrderID)
        {
            if (OrderID <= 0)
                throw (new ArgumentOutOfRangeException("OrderID"));

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@OrderID", SqlDbType.Int, 0, ParameterDirection.Input, OrderID);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_OrderItem_ListOrderItemByOrderID);

            List<OrderDetail> dataList = new List<OrderDetail>();

            TExecuteReaderCmd<OrderDetail>(sqlCmd, OrderItem_TGenerateListFromReader<OrderDetail>, ref dataList);

            return dataList;
        }

        public bool UpdateOrderItem(int ItemID, int OrderID, string SKU, string Size, int ColorID, int Quantity, int UnitPrice)
        {

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);

            AddParamToSQLCmd(sqlCmd, "@ItemID", SqlDbType.Int, 0, ParameterDirection.Input, ItemID);
            AddParamToSQLCmd(sqlCmd, "@OrderID", SqlDbType.Int, 0, ParameterDirection.Input, OrderID);
            AddParamToSQLCmd(sqlCmd, "@SKU", SqlDbType.NVarChar, 50, ParameterDirection.Input, SKU);
            AddParamToSQLCmd(sqlCmd, "@Size", SqlDbType.NVarChar, 50, ParameterDirection.Input, Size);
            AddParamToSQLCmd(sqlCmd, "@ColorID", SqlDbType.Int, 0, ParameterDirection.Input, ColorID);

            AddParamToSQLCmd(sqlCmd, "@Quantity", SqlDbType.Int, 0, ParameterDirection.Input, Quantity);
            AddParamToSQLCmd(sqlCmd, "@UnitPrice", SqlDbType.Int, 0, ParameterDirection.Input, UnitPrice);


            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_OrderItem_UpdateOrderItem);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);

        }
        public int AddOrderItem(int OrderID, string SKU, string Size, int ColorID, int Quantity, int UnitPrice)
        {

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);

            AddParamToSQLCmd(sqlCmd, "@OrderID", SqlDbType.Int, 0, ParameterDirection.Input, OrderID);
            AddParamToSQLCmd(sqlCmd, "@SKU", SqlDbType.NVarChar, 50, ParameterDirection.Input, SKU);
            AddParamToSQLCmd(sqlCmd, "@Size", SqlDbType.NVarChar, 50, ParameterDirection.Input, Size);
            AddParamToSQLCmd(sqlCmd, "@ColorID", SqlDbType.Int, 0, ParameterDirection.Input, ColorID);

            AddParamToSQLCmd(sqlCmd, "@Quantity", SqlDbType.Int, 0, ParameterDirection.Input, Quantity);
            AddParamToSQLCmd(sqlCmd, "@UnitPrice", SqlDbType.Int, 0, ParameterDirection.Input, UnitPrice);


            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_OrderItem_AddOrderItem);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue);

        }
        public bool RemoveOrderItem(int ItemID)
        {

            if (ItemID <= 0)
                throw (new ArgumentOutOfRangeException("ItemID"));

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);
            AddParamToSQLCmd(sqlCmd, "@ItemID", SqlDbType.Int, 0, ParameterDirection.Input, ItemID);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_OrderItem_RemoveOrderItem);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);

        }
        public bool UpdateOrderItemQuantity(int itemID, int quantity)
        {

            if (itemID <= 0)
                throw (new ArgumentOutOfRangeException("itemID"));


            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);
            AddParamToSQLCmd(sqlCmd, "@ItemID", SqlDbType.Int, 0, ParameterDirection.Input, itemID);
            AddParamToSQLCmd(sqlCmd, "@Quantity", SqlDbType.Int, 0, ParameterDirection.Input, quantity);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_OrderItem_UpdateQuantity);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);

        }
        #endregion

        #region PhotoImage

        private const string SP_PhotoImage_GetAllPhotos = "sp_get_all_files";
        private const string SP_PhotoImage_GetPhotoByID = "sp_get_file_details";
        private const string SP_PhotoImage_AddPhoto = "sp_insert_file";
        private const string SP_PhotoImage_SetMainPhoto = "sp_set_main_file";

        public List<ProductImage> GetAllProductImage()
        {

            SqlCommand sqlCmd = new SqlCommand();


            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_PhotoImage_GetAllPhotos);

            List<ProductImage> dataList = new List<ProductImage>();

            TExecuteReaderCmd<ProductImage>(sqlCmd, ProductImage_TGenerateListFromReader<ProductImage>, ref dataList);
            return dataList;

        }

        public ProductImage GetPhotoImageByID(int PhotID)
        {
            if (PhotID <= 0)
                throw (new ArgumentOutOfRangeException("PhotID"));

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@PhotID", SqlDbType.Int, 0, ParameterDirection.Input, PhotID);


            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_PhotoImage_GetPhotoByID);

            List<ProductImage> dataList = new List<ProductImage>();

            TExecuteReaderCmd<ProductImage>(sqlCmd, ProductImage_TGenerateListFromReader<ProductImage>, ref dataList);

            if (dataList.Count > 0)
                return dataList[0];

            else
                return null;
        }

        public int AddProductImage(string file_name, string file_ext, string file_base64, Nullable<int> productID, Nullable<int> isMain, Nullable<int> sortID)
        {

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);

            AddParamToSQLCmd(sqlCmd, "@file_name", SqlDbType.NVarChar, 50, ParameterDirection.Input, file_name);
            AddParamToSQLCmd(sqlCmd, "@file_ext", SqlDbType.NVarChar, 50, ParameterDirection.Input, file_ext);
            AddParamToSQLCmd(sqlCmd, "@file_base64", SqlDbType.NVarChar,0 , ParameterDirection.Input, file_base64);
            AddParamToSQLCmd(sqlCmd, "@productID", SqlDbType.Int, 0, ParameterDirection.Input, productID);

            AddParamToSQLCmd(sqlCmd, "@isMain", SqlDbType.Int, 0, ParameterDirection.Input, isMain);
            AddParamToSQLCmd(sqlCmd, "@sortID", SqlDbType.Int, 0, ParameterDirection.Input, sortID);
            //AddParamToSQLCmd(sqlCmd, "@ProductName", SqlDbType.NChar, 50, ParameterDirection.Input, "");


            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_PhotoImage_AddPhoto);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue);

        }

        public bool SetMainPhoto(int PhotoID)
        {

            if (PhotoID <= 0)
                throw (new ArgumentOutOfRangeException("PhotoID"));

            SqlCommand sqlCmd = new SqlCommand();

            AddParamToSQLCmd(sqlCmd, "@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, null);
            AddParamToSQLCmd(sqlCmd, "@PhotoID", SqlDbType.Int, 0, ParameterDirection.Input, PhotoID);

            SetCommandType(sqlCmd, CommandType.StoredProcedure, SP_PhotoImage_SetMainPhoto);
            ExecuteScalarCmd(sqlCmd);

            int returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

            return (returnValue == 0 ? true : false);

        }

        #endregion

        /*****************************  SQL HELPER METHODS *****************************/
        #region SQL HELPER METHODS
        private void AddParamToSQLCmd(SqlCommand sqlCmd,
                                      string paramId,
                                      SqlDbType sqlType,
                                      int paramSize,
                                      ParameterDirection paramDirection,
                                      object paramvalue)
        {

            if (sqlCmd == null)
                throw (new ArgumentNullException("sqlCmd"));
            if (paramId == string.Empty)
                throw (new ArgumentOutOfRangeException("paramId"));

            SqlParameter newSqlParam = new SqlParameter();
            newSqlParam.ParameterName = paramId;
            newSqlParam.SqlDbType = sqlType;
            newSqlParam.Direction = paramDirection;

            if (paramSize > 0)
                newSqlParam.Size = paramSize;

            if (paramvalue != null)
                newSqlParam.Value = paramvalue;

            sqlCmd.Parameters.Add(newSqlParam);
        }

        private void ExecuteScalarCmd(SqlCommand sqlCmd)
        {
            if (ConnectionString == string.Empty)
                throw (new ArgumentOutOfRangeException("ConnectionString"));

            if (sqlCmd == null)
                throw (new ArgumentNullException("sqlCmd"));

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                sqlCmd.Connection = cn;
                cn.Open();
                sqlCmd.ExecuteScalar();
            }
        }

        private void SetCommandType(SqlCommand sqlCmd, CommandType cmdType, string cmdText)
        {
            sqlCmd.CommandType = cmdType;
            sqlCmd.CommandText = cmdText;
        }

        private void TExecuteReaderCmd<T>(SqlCommand sqlCmd, TGenerateListFromReader<T> gcfr, ref List<T> List)
        {
            if (ConnectionString == string.Empty)
                throw (new ArgumentOutOfRangeException("ConnectionString"));

            if (sqlCmd == null)
                throw (new ArgumentNullException("sqlCmd"));

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                sqlCmd.Connection = cn;

                cn.Open();

                gcfr(sqlCmd.ExecuteReader(), ref List);
            }
        }
        #endregion

        /*****************************  GENARATE List HELPER METHODS  *****************************/



        private void Product_TGenerateListFromReader<T>(SqlDataReader returnData, ref List<Product> dataList)
        {
            while (returnData.Read())
            {
                //Product int ID, string Name, string Content, int SortID, bool IsApproved, DateTime CreationDate
                int ID = 0;
                if (returnData["ID"] != DBNull.Value) { ID = Convert.ToInt32(returnData["ID"]); }

                int CategoryID = 0;
                if (returnData["CategoryID"] != DBNull.Value) { CategoryID = Convert.ToInt32(returnData["CategoryID"]); }

                string Name = string.Empty;
                if (returnData["Name"] != DBNull.Value) { Name = Convert.ToString(returnData["Name"]); }

                string SKU = string.Empty;
                if (returnData["SKU"] != DBNull.Value) { SKU = Convert.ToString(returnData["SKU"]); }

                string Content = string.Empty;
                if (returnData["Content"] != DBNull.Value) { Content = Convert.ToString(returnData["Content"]); }

                bool IsApproved = false;
                if (returnData["IsApproved"] != DBNull.Value) { IsApproved = Convert.ToBoolean(returnData["IsApproved"]); }

                bool IsNewArrival = false;
                if (returnData["IsNewArrival"] != DBNull.Value) { IsNewArrival = Convert.ToBoolean(returnData["IsNewArrival"]); }

                int SortID = 0;
                if (returnData["SortID"] != DBNull.Value) { SortID = Convert.ToInt32(returnData["SortID"]); }

                int ListPrice = 0;
                if (returnData["ListPrice"] != DBNull.Value) { ListPrice = Convert.ToInt32(returnData["ListPrice"]); }

                int SellingPrice = 0;
                if (returnData["SellingPrice"] != DBNull.Value) { SellingPrice = Convert.ToInt32(returnData["SellingPrice"]); }


                DateTime CreationDate = DateTime.MaxValue;
                if (returnData["CreationDate"] != DBNull.Value) { CreationDate = Convert.ToDateTime(returnData["CreationDate"]); }

                int SizeCollectionID = 0;
                if (returnData["SizeCollectionID"] != DBNull.Value) { SizeCollectionID = Convert.ToInt32(returnData["SizeCollectionID"]); }


                Product currentItem = new Product(); // ID, CategoryID, Name, SKU, Content, IsApproved, IsNewArrival, SortID, ListPrice, SellingPrice, SizeCollectionID, CreationDate);

                dataList.Add(currentItem);
            }
        }

 
        //Order
        private void Order_TGenerateListFromReader<T>(SqlDataReader returnData, ref List<Order> dataList)
        {
            while (returnData.Read())
            {
                //  int OrderID string SessionID string OrderNo string OrderName string OrderTel string OrderMobile string OrderZipcode string OrderAddress 
                //string OrderEmail int OrderAmount DateTime CreationDate DateTime OrderDate DateTime DeliverDate DateTime PaidDate OrderStatus Status        
                //string TypeOfPayment bool IsPaid bool IsShipped string OrderMemo string Description         
                int OrderID = 0;
                if (returnData["OrderID"] != DBNull.Value) { OrderID = Convert.ToInt32(returnData["OrderID"]); }

                int ShippingCost = 0;
                if (returnData["ShippingCost"] != DBNull.Value) { ShippingCost = Convert.ToInt32(returnData["ShippingCost"]); }

                string SessionID = string.Empty;
                if (returnData["SessionID"] != DBNull.Value) { SessionID = Convert.ToString(returnData["SessionID"]); }

                string OrderNo = string.Empty;
                if (returnData["OrderNo"] != DBNull.Value) { OrderNo = Convert.ToString(returnData["OrderNo"]); }

                string OrderName = string.Empty;
                if (returnData["OrderName"] != DBNull.Value) { OrderName = Convert.ToString(returnData["OrderName"]); }

                string OrderTel = string.Empty;
                if (returnData["OrderTel"] != DBNull.Value) { OrderTel = Convert.ToString(returnData["OrderTel"]); }

                string OrderMobile = string.Empty;
                if (returnData["OrderMobile"] != DBNull.Value) { OrderMobile = Convert.ToString(returnData["OrderMobile"]); }

                string OrderZipcode = string.Empty;
                if (returnData["OrderZipcode"] != DBNull.Value) { OrderZipcode = Convert.ToString(returnData["OrderZipcode"]); }

                string OrderAddress = string.Empty;
                if (returnData["OrderAddress"] != DBNull.Value) { OrderAddress = Convert.ToString(returnData["OrderAddress"]); }

                string OrderEmail = string.Empty;
                if (returnData["OrderEmail"] != DBNull.Value) { OrderEmail = Convert.ToString(returnData["OrderEmail"]); }

                int OrderAmount = 0;
                if (returnData["OrderAmount"] != DBNull.Value) { OrderAmount = Convert.ToInt32(returnData["OrderAmount"]); }

                DateTime CreationDate = SqlDateTime_MinValue;
                if (returnData["CreationDate"] != DBNull.Value) { CreationDate = Convert.ToDateTime(returnData["CreationDate"]); }

                DateTime OrderDate = SqlDateTime_MinValue;
                if (returnData["OrderDate"] != DBNull.Value) { OrderDate = Convert.ToDateTime(returnData["OrderDate"]); }

                DateTime DeliverDate = SqlDateTime_MinValue;
                if (returnData["DeliverDate"] != DBNull.Value) { DeliverDate = Convert.ToDateTime(returnData["DeliverDate"]); }

                DateTime PaidDate = SqlDateTime_MinValue;
                if (returnData["PaidDate"] != DBNull.Value) { PaidDate = Convert.ToDateTime(returnData["PaidDate"]); }

                int intOrderStatus = 0;
                if (returnData["Status"] != DBNull.Value) { intOrderStatus = Convert.ToInt32(returnData["Status"]); }
                OrderStatus Status = (OrderStatus)intOrderStatus;

                string TypeOfPayment = string.Empty;
                if (returnData["TypeOfPayment"] != DBNull.Value) { TypeOfPayment = Convert.ToString(returnData["TypeOfPayment"]); }

                bool IsPaid = false;
                if (returnData["IsPaid"] != DBNull.Value) { IsPaid = Convert.ToBoolean(returnData["IsPaid"]); }

                bool IsShipped = false;
                if (returnData["IsShipped"] != DBNull.Value) { IsShipped = Convert.ToBoolean(returnData["IsShipped"]); }

                string OrderMemo = string.Empty;
                if (returnData["OrderMemo"] != DBNull.Value) { OrderMemo = Convert.ToString(returnData["OrderMemo"]); }

                string Description = string.Empty;
                if (returnData["Description"] != DBNull.Value) { Description = Convert.ToString(returnData["Description"]); }


                Order currentItem = new Order(); // OrderID, SessionID, OrderNo, OrderName, OrderTel, OrderMobile, OrderZipcode, OrderAddress, OrderEmail, OrderAmount, ShippingCost, CreationDate, OrderDate, DeliverDate, PaidDate, Status, TypeOfPayment, IsPaid, IsShipped, OrderMemo, Description);

                dataList.Add(currentItem);
            }
        }

        //OrderItem
        private void OrderItem_TGenerateListFromReader<T>(SqlDataReader returnData, ref List<OrderDetail> dataList)
        {
            while (returnData.Read())
            {
                //this.ID, this.OrderID, this.SKU, this.Size, this.Color, this.Name, this.Quantity, this.UnitPrice
                int ItemID = 0;
                if (returnData["ItemID"] != DBNull.Value) { ItemID = Convert.ToInt32(returnData["ItemID"]); }

                int OrderID = 0;
                if (returnData["OrderID"] != DBNull.Value) { OrderID = Convert.ToInt32(returnData["OrderID"]); }

                string SKU = string.Empty;
                if (returnData["SKU"] != DBNull.Value) { SKU = Convert.ToString(returnData["SKU"]); }

                string Size = string.Empty;
                if (returnData["Size"] != DBNull.Value) { Size = Convert.ToString(returnData["Size"]); }

                int ColorID = 0;
                if (returnData["ColorID"] != DBNull.Value) { ColorID = Convert.ToInt32(returnData["ColorID"]); }

                string Name = string.Empty;
                if (returnData["Name"] != DBNull.Value) { Name = Convert.ToString(returnData["Name"]); }

                int Quantity = 0;
                if (returnData["Quantity"] != DBNull.Value) { Quantity = Convert.ToInt32(returnData["Quantity"]); }

                int UnitPrice = 0;
                if (returnData["UnitPrice"] != DBNull.Value) { UnitPrice = Convert.ToInt32(returnData["UnitPrice"]); }


                OrderDetail currentItem = new OrderDetail(); // ItemID, OrderID, SKU, Size, ColorID, Name, Quantity, UnitPrice);

                dataList.Add(currentItem);
            }
        }

        // ShoppinDetail
        private void ShoppingDetail_TGenerateListFromReader<T>(SqlDataReader returnData, ref List<ShoppingDetail> dataList)
        {
            while (returnData.Read())
            {
                /* int CarID,int ProductID,string ProductName,int UnitPrice,int OrderQuantity,int Subtotal,System.DateTime AddedDate
                    public int CarID { get; set; }
                    public Nullable<int> ProductID { get; set; }
                    public string ProductName { get; set; }
                    public Nullable<int> UnitPrice { get; set; }
                    public Nullable<int> OrderQuantity { get; set; }
                    public Nullable<int> Subtotal { get; set; }
                    public Nullable<System.DateTime> AddedDate { get; set; }
                    public string ModifiedDate { get; set; }
                 */
                int CarID = 0;
                if (returnData["CarID"] != DBNull.Value) { CarID = Convert.ToInt32(returnData["CarID"]); }

                int ProductID = 0;
                if (returnData["ProductID"] != DBNull.Value) { ProductID = Convert.ToInt32(returnData["ProductID"]); }

                string ProductName = string.Empty;
                if (returnData["ProductName"] != DBNull.Value) { ProductName = Convert.ToString(returnData["ProductName"]); }

                int UnitPrice = 0;
                if (returnData["UnitPrice"] != DBNull.Value) { UnitPrice = Convert.ToInt32(returnData["UnitPrice"]); }
 
                int OrderQuantity = 0;
                if (returnData["OrderQuantity"] != DBNull.Value) { OrderQuantity = Convert.ToInt32(returnData["OrderQuantity"]); }

                int Subtotal = 0;
                if (returnData["Subtotal"] != DBNull.Value) { Subtotal = Convert.ToInt32(returnData["Subtotal"]); }

                DateTime AddedDate = SqlDateTime_MinValue;
                if (returnData["AddedDate"] != DBNull.Value) { AddedDate = Convert.ToDateTime(returnData["AddedDate"]); }


                DateTime ModifiedDate = SqlDateTime_MinValue;
                if (returnData["ModifiedDate"] != DBNull.Value) { ModifiedDate = Convert.ToDateTime(returnData["ModifiedDate"]); }

               
       
                ShoppingDetail currentItem = new ShoppingDetail();
                currentItem.CarID = CarID;
                currentItem.ProductID = ProductID;
                currentItem.ProductName = ProductName;
                currentItem.UnitPrice = UnitPrice;
                currentItem.OrderQuantity = OrderQuantity;
                currentItem.Subtotal = Subtotal;
                currentItem.AddedDate = AddedDate;
                //currentItem.ModifiedDate = ModifiedDate;

                dataList.Add(currentItem);
            }
        }

        //ProductImage_TGenerateListFromReader
        private void ProductImage_TGenerateListFromReader<T>(SqlDataReader returnData, ref List<ProductImage> dataList)
        {
            while (returnData.Read())
            {
                
                int file_id = 0;
                if (returnData["file_id"] != DBNull.Value) { file_id = Convert.ToInt32(returnData["file_id"]); }

                int ProductID = 0;
                if (returnData["ProductID"] != DBNull.Value) { ProductID = Convert.ToInt32(returnData["ProductID"]); }

                string file_name = string.Empty;
                if (returnData["file_name"] != DBNull.Value) { file_name = Convert.ToString(returnData["file_name"]); }

                string file_ext = string.Empty;
                if (returnData["file_ext"] != DBNull.Value) { file_ext = Convert.ToString(returnData["file_ext"]); }

                string file_base6 = string.Empty;
                if (returnData["file_base6"] != DBNull.Value) { file_base6 = Convert.ToString(returnData["file_base6"]); }
                
                int SortID = 0;
                if (returnData["SortID"] != DBNull.Value) { SortID = Convert.ToInt32(returnData["SortID"]); }

                int isMain = 0;
                if (returnData["isMain"] != DBNull.Value) { isMain = Convert.ToInt32(returnData["isMain"]); }

                string ProductName = string.Empty;
                if (returnData["ProductName"] != DBNull.Value) { ProductName = Convert.ToString(returnData["ProductName"]); }


                ProductImage currentItem = new ProductImage();
                currentItem.file_id = file_id;
                currentItem.file_name = file_name;
                currentItem.file_ext = file_ext;
                currentItem.file_base6 = file_base6;
                currentItem.ProductID = ProductID;
                currentItem.SortID = SortID;
                currentItem.isMain = isMain;
                currentItem.ProductName = ProductName;

                dataList.Add(currentItem);
            }
        }

    }//Clsss
}//Namespace
