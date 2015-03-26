using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Metaseed.Data;

namespace Metaseed.MetaStudioTest.Data
{
    /*
     * <TextBox Text="{Binding UserName, ValidatesOnNotifyDataErrors=True}"/>
<TextBox Text="{Binding Password, ValidatesOnNotifyDataErrors=True}" />
<TextBox Text="{Binding RepeatPassword, ValidatesOnNotifyDataErrors=True}" />
     * http://www.jonathanantoine.com/2011/09/18/wpf-4-5-asynchronous-data-validation/
     * https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifydataerrorinfo(v=vs.95).aspx
     * The binding will then register itself for the ErrorsChanged event of the binded UserInput. Eeach time this event is raised for the binded property, the controls will dress itself to display an error. As pointed out before, this is done only if the HasErrors is set to true.
    */
    public class UserInput : ValidatableModel
    {
        private string _userName;
        private string _email;
        private string _repeatEmail;

        [Required]
        [StringLength(20)]
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; RaisePropertyChanged("UserName"); }
        }

        [Required]
        [EmailAddress]
        [StringLength(60)]
        public string Email
        {
            get { return _email; }
            set { _email = value; RaisePropertyChanged("Email"); }
        }

        [Required]
        [EmailAddress]
        [StringLength(60)]
        [CustomValidation(typeof(UserInput), "SameEmailValidate")]
        public string RepeatEmail
        {
            get { return _repeatEmail; }
            set { _repeatEmail = value; RaisePropertyChanged("RepeatEmail"); }
        }

        public static ValidationResult SameEmailValidate(object obj, ValidationContext context)
        {
            var user = (UserInput)context.ObjectInstance;
            if (user.Email != user.RepeatEmail)
            {
                return new ValidationResult("The emails are not equal", new List<string> { "Email", "RepeatEmail" });
            }
            return ValidationResult.Success;
        }
    }
}
