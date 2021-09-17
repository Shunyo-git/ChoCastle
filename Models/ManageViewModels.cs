using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace ChoCastle.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }

        //2021/9/17 by sean
        //新增會員擴充屬性


        public string Email { get; set; }
        public string ChineseName { get; set; }
        public string Gender { get; set; }
        public string PostCode { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string LineID { get; set; }
        public System.DateTime Birthday { get; set; }

    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "{0} 的長度至少必須為 {2} 個字元。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認新密碼")]
        [Compare("NewPassword", ErrorMessage = "新密碼與確認密碼不相符。")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "目前密碼")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 的長度至少必須為 {2} 個字元。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認新密碼")]
        [Compare("NewPassword", ErrorMessage = "新密碼與確認密碼不相符。")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "電話號碼")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "代碼")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "電話號碼")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }


    //2021/9/16 by sean
    public class MemberViewModel
    {

        //public ApplicationUser GetUser { get; set; }

        [Display(Name = "帳號名稱")]
        public string UserName { get; set; }

        [Required]
        [StringLength(5, ErrorMessage = "{0} 的長度至少必須為 {2} - 10個字元。", MinimumLength = 2)]
        [Display(Name = "姓名")]
        public string ChineseName { get; set; }

  
        [Display(Name = "電子郵件")]
        public string Email { get; set; }


        [Required]
        [Display(Name = "性別")]
        public string Gender { get; set; }

        [RegularExpression(@"^[0-9]{3,6}$", ErrorMessage = "郵遞區號格式不正確")]
        [StringLength(6, ErrorMessage = "{0} 的長度至少必須為 {2} 個字元。", MinimumLength = 3)]
        [Display(Name = "郵遞區號")]
        public string PostCode { get; set; }

        [StringLength(50, ErrorMessage = "{0} 的長度至少必須為 {2} 個字元。", MinimumLength = 10)]
        [Display(Name = "地址")]
        public string Address { get; set; }

        [Required]
        [RegularExpression(@"^09[0-9]{8}$", ErrorMessage = "行動電話格式不正確")]
        [Display(Name = "行動電話號碼")]
        public string Mobile { get; set; }

       
        
        [RegularExpression(@"^[a-z0-9]*$", ErrorMessage = "Line ID格式不正確")]
        [Display(Name = "Line ID")]
        public string LineID { get; set; }

        //^(((?:19|20)[0-9]{2})[-/.](0?[1-9]|1[012])[-/.](0?[1-9]|[12][0-9]|3[01]))*$
        //^((19|20)\d{2}[/.-](0?[1-9]|1[0-2])[/.-](0?[1-9]|[12][0-9]|3[01]))$
        [RegularExpression(@"^((19|20)\d{2}[/.-](0?[1-9]|1[0-2])[/.-](0?[1-9]|[12][0-9]|3[01]))$", ErrorMessage = "生日格式不正確(例如:2001/1/1)")]
        [Display(Name = "西元生日")]
        public string Birthday { get; set; }
    }
}